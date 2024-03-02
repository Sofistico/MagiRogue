using Arquimedes.Enumerators;
using GoRogue.FOV;
using MagusEngine.Bus;
using MagusEngine.Bus.UiBus;
using MagusEngine.Commands;
using MagusEngine.Core.Entities.Base;
using MagusEngine.Core.MapStuff;
using MagusEngine.ECS.Components.ActorComponents;
using MagusEngine.ECS.Components.ActorComponents.Status;
using MagusEngine.ECS.Components.TilesComponents;
using MagusEngine.Serialization.EntitySerialization;
using MagusEngine.Services;
using MagusEngine.Systems.Physics;
using MagusEngine.Utils.Extensions;
using Newtonsoft.Json;
using SadRogue.Primitives;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagusEngine.Core.Entities
{
    [JsonConverter(typeof(ActorJsonConverter))]
    public class Actor : MagiEntity
    {
        #region Fields

        private RecursiveShadowcastingBooleanBasedFOV actorFov;
        private int? viewRadius;

        #endregion Fields

        #region Properties

        /// <summary>
        /// The soul of the actor, where magic happens
        /// </summary>
        public Soul Soul { get; set; }

        /// <summary>
        /// The mind of the actor, where thought and brain is
        /// </summary>
        public Mind Mind { get; set; }

        /// <summary>
        /// The body of the actor
        /// </summary>
        public Body Body { get; set; }

        /// <summary>
        /// Sets if the char has bumbed in something
        /// </summary>
        public bool Bumped { get; set; }

        // TODO: change inventory...
        /// <summary>
        /// Here we define were the inventory is
        /// </summary>
        public List<Item> Inventory { get; set; }

        public bool IsPlayer { get; set; }
        public List<SpecialFlag> Flags { get => GetAnatomy().Race.Flags; }

        /// <summary>
        /// The current state of the actor, should be usd to track multi turns stuff
        /// </summary>
        public ActorState State { get; set; }

        public List<ActorSituationalFlags> SituationalFlags { get; set; } = [];

        #endregion Properties

        #region Constructor

        /// <summary> This here defines an actor, must be used with the <see cref=
        /// "Arquimedes.Data.EntityFactory"> Normally, use the ActorCreator method! </summary>
        /// <param name="foreground"></param> <param name="background"></param> <param
        /// name="glyph"></param> <param name="layer"></param> <param name="coord"></param>
        public Actor(string name, Color foreground, Color background,
            int glyph, Point coord, int layer = (int)MapLayer.ACTORS) : base(foreground, background,
            glyph, coord, layer)
        {
            Body = new Body();
            Mind = new Mind();
            Soul = new Soul();
            Inventory = [];
            Name = name;
        }

        #endregion Constructor

        #region Methods

        #region Utils

        // Moves the Actor BY positionChange tiles in any X/Y direction returns true if actor was
        // able to move, false if failed to move Checks for Monsters, and allows the Actor to commit
        // an action if one is present.
        // TODO: an autopickup feature for items
        public override bool MoveBy(Point deltaPositionChange)
        {
            // Check the current map if we can move to this new position
            if (CurrentMagiMap?.IsTileWalkable(Position + deltaPositionChange, IgnoresWalls) == true)
            {
                bool attacked = CheckIfCanAttack(deltaPositionChange);

                if (attacked)
                    return attacked;

                Position += deltaPositionChange;

                return true;
            }
            else // Handle situations where there are non-walkable tiles that CAN be used
            {
                bool doorThere = CheckIfThereIsDoor(deltaPositionChange);

                if (doorThere)
                    return doorThere;

                // true means that he entered inside a map, and thus the turn moved, a false means
                // that there wasn't anything there
                return CheckForChangeMapChunk(Position, deltaPositionChange);
            }
        }

        private bool CheckForChangeMapChunk(Point pos, Point positionChange)
        {
            Direction dir = Direction.GetCardinalDirection(positionChange);
            if (CurrentMagiMap.MapZoneConnections.TryGetValue(dir, out MagiMap value) &&
                CurrentMagiMap.CheckForIndexOutOfBounds(pos + positionChange))
            {
                MagiMap mapToGo = value;
                Point actorPosInChunk = GetNextMapPos(mapToGo, pos + positionChange);
                // if tile in the other map isn't walkable, then it should't be possible to go there!
                if (!mapToGo.IsTileWalkable(actorPosInChunk, IgnoresWalls))
                    return false;
                //Find.Universe.ChangePlayerMap(mapToGo, actorPosInChunk, Find.CurrentMap);
                Locator.GetService<MessageBusService>()
                    .SendMessage<ChangeControlledActorMap>(new(mapToGo, actorPosInChunk));
                return true;
            }
            else
            {
                return false;
            }
        }

        private static Point GetNextMapPos(MagiMap map, Point pos)
        {
            int x = pos.X % map.Width < 0 ? map.Width + (pos.X % map.Width) : pos.X % map.Width;
            int y = pos.Y % map.Height < 0 ? map.Height + (pos.Y % map.Height) : pos.Y % map.Height;
            return new Point(x, y);
        }

        private bool CheckIfCanAttack(Point positionChange)
        {
            // if there's a monster here, do a bump attack
            Actor actor = CurrentMagiMap.GetEntityAt<Actor>(Position + positionChange);

            if (actor != null && CanBeAttacked)
            {
                // TODO: Make sure that the time to attack is properly added here!
                ActionManager.MeleeAttack(this, actor);
                Bumped = true;
                return Bumped;
            }

            Bumped = false;
            return Bumped;
        }

        private bool CheckIfThereIsDoor(Point positionChange)
        {
            // Check for the presence of a door
            Tile door = CurrentMagiMap.GetTileAt<DoorComponent>(Position + positionChange);

            // if there's a door here, try to use it
            if (door != null && CanInteract)
            {
                ActionManager.UseDoor(this, door.GetComponent<DoorComponent>()!);
                //GameLoop.UIManager.MapWindow.MapConsole.IsDirty = true;
                Locator.GetService<MessageBusService>().SendMessage<MapConsoleIsDirty>(new());
                return true;
            }

            return false;
        }

        public Item? WieldedItem()
        {
            if (!Body.Anatomy.HasAnyHands)
                return null;
            return Body?.Equipment!.GetValueOrDefault(GetAnatomy().Limbs.Find(l =>
                l.BodyPartFunction == BodyPartFunction.Grasp)?.Id, null);
        }

        public List<Item>? GetAllWieldedItems()
        {
            if (!Body.Anatomy.HasAnyHands)
                return null;
            List<Item> items = new();
            foreach (var item in Body.Equipment)
            {
                var limb = GetAnatomy().Limbs.Find(i => i.Id.Equals(item.Key));
                if (limb?.BodyPartFunction == BodyPartFunction.Grasp
                    && item.Value.EquipType == EquipType.Held)
                {
                    items.Add(item.Value);
                }
            }
            return items;
        }

        public List<Item> ItemsInLimb(Limb limb)
        {
            var list = new List<Item>
            {
                Body.Equipment[limb.Id]
            };
            list.RemoveAll(i => i is null);
            return list;
        }

        public BodyPart? GetAttackingLimb(Attack attack)
        {
            if (attack.LimbFunction is not null)
            {
                return GetAnatomy().AllBPs.FindAll(i =>
                    i.BodyPartFunction == attack.LimbFunction)
                    .GetRandomItemFromList();
            }
            else
            {
                var items = GetAllWieldedItems();
                return items?.Find(i => i.Attacks.Contains(attack))?.HeldLimb!;
            }
        }

        public void AddToEquipment(Item item)
        {
            if (item.Equip(this))
            {
                if (!Inventory.Contains(item))
                {
                    Inventory.Add(item);
                }
            }
        }

        public bool CheckIfDed()
        {
            return !GetAnatomy().EnoughBodyToLive();
        }

        #endregion Utils

        #region RegenCode

        public void ApplyAllRegen()
        {
            ApplyBodyRegen();
            ApplyStaminaRegen();
            ApplyManaRegen();
        }

        public void ApplyBodyRegen()
        {
            bool regens = GetAnatomy().Race.CanRegenarate();
            var limbsHeal = GetAnatomy().Limbs.FindAll(i => i.NeedsHeal || (regens && !i.Attached));
            int limbCount = limbsHeal.Count;
            for (int i = 0; i < limbCount; i++)
            {
                Limb limb = limbsHeal[i];

                if (limb.Attached)
                    limb.ApplyHeal(GetNormalLimbRegen(), regens);
                if (regens && !limb.Attached)
                {
                    List<Limb> connectedLimbs = GetAnatomy().GetAllParentConnectionLimb(limb);
                    if (connectedLimbs.All(i => i.Attached))
                    {
                        limb.ApplyHeal(GetNormalLimbRegen(), regens);
                        if (!limb.Wounds.Any(i => i.Severity is InjurySeverity.Missing))
                            limb.Attached = true;
                    }
                }
            }
        }

        public void ApplyManaRegen()
        {
            Soul.ApplyManaRegen(GetManaRegen());
        }

        public void ApplyStaminaRegen()
        {
            Body.ApplyStaminaRegen(GetStaminaRegen());
        }

        #endregion RegenCode

        #region GetProperties

        public int GetViewRadius()
        {
            viewRadius ??= Body.ViewRadius;
            return viewRadius.Value;
        }

        public double GetNormalLimbRegen()
        {
            return Body.Anatomy.NormalLimbRegen;
        }

        public double GetBloodCoagulation()
        {
            if (GetAnatomy().HasBlood)
            {
                return GetAnatomy().Race.BleedRegenaration + (Body.Toughness * 0.2);
            }
            return 0;
        }

        public double GetManaRegen()
        {
            return Soul.BaseManaRegen;
        }

        public Anatomy GetAnatomy() => Body.Anatomy;

        public Dictionary<string, Item> GetEquipment() => Body.Equipment;

        public double GetStaminaRegen()
        {
            return Body.StaminaRegen * GetAnatomy().FitLevel;
        }

        public double GetActorSpeed()
        {
            var speed = Body.GeneralSpeed;
            if (GetComponent<HasteComponent>(out var haste))
                speed += haste.HastePower;
            return speed;
        }

        public double GetActorBaseCastingSpeed(string shapingAbility)
        {
            return GetActorSpeed() + (GetShapingAbility(shapingAbility) * 0.7 * (Soul.WillPower * 0.3));
        }

        public double GetAttackVelocity(Attack attack)
        {
            return PhysicsSystem.GetAttackVelocity(this, attack);
        }

        public int GetPrecision()
        {
            return Mind.Precision;
        }

        public int GetDefenseAbility()
        {
            // four different ways to defend
            int shieldAbility = GetRelevantAbility(AbilityCategory.Shield);
            int armorAbility = GetRelevantAbility(AbilityCategory.ArmorUse);
            int dodgeAbility = GetRelevantAbility(AbilityCategory.Dodge);
            int weaponAbility = GetRelevantAttackAbility(WieldedItem());

            if (shieldAbility > weaponAbility)
                return shieldAbility;
            if (weaponAbility > armorAbility)
                return weaponAbility;
            if (armorAbility > dodgeAbility)
                return armorAbility;

            return dodgeAbility;
        }

        public int GetRelevantAbility(AbilityCategory ability)
        {
            return Mind.GetAbility(ability);
        }

        public int GetRelevantAbility(string ability)
        {
            return Mind.GetAbility(ability);
        }

        public int GetRelevantAttackAbility(Item? item = null)
        {
            if (item is not null)
            {
                return GetRelevantAttackAbility(item.WeaponType);
            }
            else
            {
                return 0;
            }
        }

        public int GetRelevantAttackAbility(WeaponType weaponType)
        {
            return Mind.HasSpecifiedAttackAbility(weaponType, out int abilityScore) ? abilityScore : 0;
        }

        public double GetRelevantAttackAbilityMultiplier(AbilityCategory ability)
        {
            return Mind.HasSpecifiedAttackAbility(ability, out int abilityScore) ? abilityScore * 0.3 : 0;
        }

        public double GetRelevantAbilityMultiplier(AbilityCategory ability)
        {
            return Mind.Abilities.TryGetValue((int)ability, out Ability value) ? value.Score * 0.3 : 0;
        }

        public int GetStrenght()
        {
            return Body.Strength;
        }

        public Sex GetGender()
        {
            return GetAnatomy().Gender;
        }

        public Actor WithComponents(params object[] objs)
        {
            for (int i = 0; i < objs.Length; i++)
            {
                AddComponents(objs[i]);
            }
            return this;
        }

        public List<Attack> GetAttacks()
        {
            List<Attack> list = [];
            foreach (var item in GetAllWieldedItems()!.Select(i => i.Attacks))
            {
                list.AddRange(item);
            }
            list.AddRange(GetRaceAttacks());
            return list;
        }

        public override int GetShapingAbility(string shapingAbility)
        {
            return GetRelevantAbility(shapingAbility);
        }

        public override int GetMagicResistance()
        {
            return GetRelevantAbility(AbilityCategory.MagicResistance) + GetAnatomy().Race.BaseMagicResistance;
        }

        public override int GetPenetration()
        {
            return GetRelevantAbility(AbilityCategory.MagicPenetration);
        }

        #endregion GetProperties

        #region Needs

        public void ProcessNeeds()
        {
            if (GoRogueComponents.Contains(typeof(NeedCollection)))
            {
                NeedCollection needs = GetComponent<NeedCollection>();
                int count = needs.Count;
                for (int i = 0; i < count; i++)
                {
                    Need need = needs[i];
                    if (need.TickNeed())
                    {
#if DEBUG
                        Locator.GetService<MagiLog>().Log($"Need {need.Name} is in dire need!");
#endif
                    }
                }
            }
        }

        #endregion Needs

        public double AttackRange()
        {
            var item = WieldedItem();
            if (item is null)
            {
                return 1; // punch and stuff distance!
            }
            return 1; // to be implemented item range that permits attacking and stuff!
        }

        public bool CanSee(Point pos)
        {
            UpdateFov();
            return actorFov.BooleanResultView[pos];
        }

        public IEnumerable<Point> AllThatCanSee()
        {
            UpdateFov();
            return actorFov.CurrentFOV;
        }

        private void UpdateFov()
        {
            actorFov ??= new RecursiveShadowcastingBooleanBasedFOV(CurrentMap!.TransparencyView!);
            actorFov.Calculate(Position, GetViewRadius());
        }

        public void AddMemory<T>(Point lastSeen, MemoryType memoryType, T obj)
        {
            Mind.Memories.Add(new Memory<T>(lastSeen, memoryType, obj));
        }

        public bool HasMemory<T>(MemoryType memoryType, T obj)
        {
            foreach (var item in Mind.Memories)
            {
                if (item.MemoryType == memoryType)
                {
                    var mem = item as Memory<T>;
                    if (mem!.ObjToRemember!.Equals(obj))
                        return true;
                }
            }
            return false;
        }

        public bool GetMemory<T>(MemoryType type, out Memory<T>? memory)
        {
            foreach (var item in Mind.Memories)
            {
                if (item.MemoryType == type)
                {
                    memory = (Memory<T>?)item;
                    return true;
                }
            }
            memory = null;
            return false;
        }

        public override string GetCurrentStatus()
        {
            StringBuilder bobBuilder = new();
            bobBuilder.Append(State.ToString());
            return bobBuilder.ToString();
        }

        public List<Attack> GetRaceAttacks()
        {
            return GetAnatomy().Race.Attacks;
        }

        public void UpdateBody()
        {
            var anatomy = GetAnatomy();
            if (anatomy.BloodCount <= 0)
                ActionManager.ResolveDeath(this);
            ApplyAllRegen();
            List<Wound> wounds = anatomy.GetAllWounds();
            if (wounds.Count > 0)
            {
                foreach (Wound wound in wounds)
                {
                    if (wound.Bleeding > 0)
                    {
                        anatomy.BloodCount -= wound.Bleeding;

                        if (wound.Treated)
                            wound.Bleeding -= GetBloodCoagulation();
                        else
                            wound.Bleeding -= (GetBloodCoagulation() / (int)wound.Severity) + 1;
                    }
                }
            }
        }

        #endregion Methods
    }
}
