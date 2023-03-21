using GoRogue.FOV;
using MagiRogue.Commands;
using MagiRogue.Components;
using MagiRogue.Data.Enumerators;
using MagiRogue.GameSys;
using MagiRogue.GameSys.Physics;
using MagiRogue.GameSys.Tiles;
using MagiRogue.Utils.Extensions;
using Newtonsoft.Json;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MagiRogue.Entities
{
    [JsonConverter(typeof(Data.Serialization.EntitySerialization.ActorJsonConverter))]
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

        #endregion Properties

        #region Constructor

        /// <summary>
        /// This here defines an actor, must be used with the <see cref= "Data.EntityFactory">
        /// Normally, use the ActorCreator method!
        /// </summary>
        /// <param name="foreground"></param>
        /// <param name="background"></param>
        /// <param name="glyph"></param>
        /// <param name="layer"></param>
        /// <param name="coord"></param>
        public Actor(string name, Color foreground, Color background,
            int glyph, Point coord, int layer = (int)MapLayer.ACTORS
            ) : base(foreground, background,
            glyph, coord, layer)
        {
            Body = new Body();
            Mind = new Mind();
            Soul = new Soul();
            Inventory = new List<Item>();
            Name = name;
            // by default the material of the actor will be mostly skin
            //Material = GameSys.Physics.PhysicsManager.SetMaterial("skin");
        }

        #endregion Constructor

        #region Methods

        #region Utils

        // Moves the Actor BY positionChange tiles in any X/Y direction
        // returns true if actor was able to move, false if failed to move
        // Checks for Monsters, and allows the Actor to commit
        // an action if one is present.
        // TODO: an autopickup feature for items
        public bool MoveBy(Point positionChange)
        {
            // Check the current map if we can move to this new position
            if (GameLoop.GetCurrentMap().IsTileWalkable(Position + positionChange, this))
            {
                bool attacked = CheckIfCanAttack(positionChange);

                if (attacked)
                    return attacked;

                Position += positionChange;

                return true;
            }

            // Handle situations where there are non-walkable tiles that CAN be used
            else
            {
                bool doorThere = CheckIfThereIsDoor(positionChange);

                if (doorThere)
                    return doorThere;

                // true means that he entered inside a map,
                // and thus the turn moved, a false means that there wasn't anything there
                return CheckForChangeMapChunk(Position, positionChange);
            }
        }

        private bool CheckForChangeMapChunk(Point pos, Point positionChange)
        {
            Direction dir = Direction.GetCardinalDirection(positionChange);
            if (GameLoop.GetCurrentMap().MapZoneConnections.ContainsKey(dir) &&
                GameLoop.GetCurrentMap().CheckForIndexOutOfBounds(pos + positionChange))
            {
                Map mapToGo = GameLoop.GetCurrentMap().MapZoneConnections[dir];
                Point actorPosInChunk = GetNextMapPos(mapToGo, pos + positionChange);
                // if tile in the other map isn't walkable, then it should't be possible to go there!
                if (!mapToGo.IsTileWalkable(actorPosInChunk, this))
                    return false;
                GameLoop.Universe.ChangePlayerMap(mapToGo, actorPosInChunk, GameLoop.GetCurrentMap());
                return true;
            }
            else
                return false;
        }

        private static Point GetNextMapPos(Map map, Point pos)
        {
            int x = pos.X % map.Width < 0 ? map.Width + (pos.X % map.Width) : pos.X % map.Width;
            int y = pos.Y % map.Height < 0 ? map.Height + (pos.Y % map.Height) : pos.Y % map.Height;
            return new SadRogue.Primitives.Point(x, y);
        }

        private bool CheckIfCanAttack(Point positionChange)
        {
            // if there's a monster here,
            // do a bump attack
            Actor actor = GameLoop.GetCurrentMap().GetEntityAt<Actor>(Position + positionChange);

            if (actor != null && CanBeAttacked)
            {
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
            TileDoor door = GameLoop.GetCurrentMap().GetTileAt<TileDoor>(Position + positionChange);

            // if there's a door here,
            // try to use it
            if (door != null && CanInteract)
            {
                ActionManager.UseDoor(this, door);
                GameLoop.UIManager.MapWindow.MapConsole.IsDirty = true;
                return true;
            }

            return false;
        }

        // Moves the Actor TO newPosition location
        // returns true if actor was able to move, false if failed to move
        public bool MoveTo(Point newPosition)
        {
            if (GameLoop.GetCurrentMap().IsTileWalkable(newPosition))
            {
                Position = newPosition;
                return true;
            }
            else
            {
                return false;
            }
        }

        public Item WieldedItem()
        {
            return Body.Equipment?.GetValueOrDefault(GetAnatomy().Limbs.Find(l =>
                l.LimbType == TypeOfLimb.Hand)?.Id, null);
        }

        public Limb GetAttackingLimb(Item item)
        {
            var key = Body.Equipment.FirstOrDefault(x => x.Value == item).Key;
            var limb = GetAnatomy().Limbs.Find(i => i.Id.Equals(key));
            if (item is null)
            {
                List<Limb> graspersAndStances = GetAnatomy().Limbs.FindAll(i =>
                    (i.BodyPartFunction is BodyPartFunction.Grasp
                    || i.BodyPartFunction is BodyPartFunction.Stance)
                    && i.Working);
                return graspersAndStances.GetRandomItemFromList();
            }
            return limb;
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
            #region Broken dreams lies here....

            //Parallel.ForEach(GetAnatomy().Limbs, limb =>
            //{
            //    if (limb.BodyPartHp < limb.MaxBodyPartHp)
            //    {
            //        if (limb.Attached)
            //        {
            //            if (limb.CanHeal || GetAnatomy().GetActorRace().CanRegenLostLimbs)
            //            {
            //                limb.ApplyHeal(GetNormalLimbRegen() * limb.RateOfHeal);
            //            }
            //        }
            //        if (!limb.Attached && GetAnatomy().GetActorRace().CanRegenLostLimbs)
            //        {
            //            List<Limb> connectedLimbs = GetAnatomy().GetAllParentConnectionLimb(limb);
            //            if (!connectedLimbs.Any(i => !i.Attached))
            //            {
            //                limb.ApplyHeal(GetNormalLimbRegen() * limb.RateOfHeal + 0.5);
            //                if (!limb.Wounds.Any(i => i.Severity is InjurySeverity.Missing))
            //                    limb.Attached = true;
            //            }
            //        }
            //    }
            //});

            #endregion Broken dreams lies here....

            bool regens = GetAnatomy().Race.CanRegenarate();
            var limbsHeal = GetAnatomy().Limbs.FindAll(i => i.NeedsHeal || (regens && !i.Attached));
            int limbCount = limbsHeal.Count;
            for (int i = 0; i < limbCount; i++)
            {
                Limb limb = limbsHeal[i];
                if (limb.Attached)
                {
                    if (limb.CanHeal || regens)
                    {
                        limb.ApplyHeal(GetNormalLimbRegen() * limb.RateOfHeal);
                    }
                }
                if (!limb.Attached && regens)
                {
                    List<Limb> connectedLimbs = GetAnatomy().GetAllParentConnectionLimb(limb);
                    if (connectedLimbs.All(i => i.Attached))
                    {
                        limb.ApplyHeal((GetNormalLimbRegen() * limb.RateOfHeal) + (0.5 * 2), regens);
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

        public double GetActorBaseSpeed()
        {
            return Body.GeneralSpeed;
        }

        public double GetActorCastingSpeed()
        {
            return GetActorBaseSpeed() + ((Magic.ShapingSkill * 0.7) * (Soul.WillPower * 0.3));
        }

        public double GetAttackVelocity()
        {
            return PhysicsManager.GetAttackVelocity(this);
        }

        public double GetProtection(Limb limb)
        {
            var item = Body.GetArmorOnLimbIfAny(limb);
            var armorModifier = item is not null ?
                (item.Material.Hardness * Body.GetArmorOnLimbIfAny(limb).Material.Density) : 0;
            return Body.Toughness +
                armorModifier
                + GetRelevantAbility(AbilityName.ArmorUse);
        }

        public int GetPrecision()
        {
            return Mind.Precision;
        }

        public int GetDefenseAbility()
        {
            // four different ways to defend
            int shieldAbility = GetRelevantAbility(AbilityName.Shield);
            int armorAbility = GetRelevantAbility(AbilityName.ArmorUse);
            int dodgeAbility = GetRelevantAbility(AbilityName.Dodge);
            int weaponAbility = GetRelevantAttackAbility(WieldedItem());

            if (shieldAbility > weaponAbility)
                return shieldAbility;
            if (weaponAbility > armorAbility)
                return weaponAbility;
            if (armorAbility > dodgeAbility)
                return armorAbility;

            return dodgeAbility;
        }

        public int GetRelevantAbility(AbilityName ability)
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
                return GetRelevantAbility(AbilityName.Unarmed);
            }
        }

        public int GetRelevantAttackAbility(WeaponType weaponType)
        {
            if (Mind.HasSpecifiedAttackAbility(weaponType, out int abilityScore))
            {
                return abilityScore;
            }
            else
                return 0;
        }

        public double GetRelevantAttackAbilityMultiplier(WeaponType weaponType)
        {
            if (Mind.HasSpecifiedAttackAbility(weaponType, out int abilityScore))
            {
                return abilityScore * 0.3;
            }
            else
                return 0;
        }

        public double GetRelevantAbilityMultiplier(AbilityName ability)
        {
            if (Mind.Abilities.ContainsKey((int)ability))
            {
                return Mind.Abilities[(int)ability].Score * 0.3;
            }
            else
                return 0;
        }

        public int GetStrenght()
        {
            return Body.Strength;
        }

        public DamageTypes GetDamageType()
        {
            if (WieldedItem() is not null && WieldedItem() is Item item)
            {
                return item.ItemDamageType;
            }
            else
            {
                return DamageTypes.Blunt;
            }
        }

        public Sex GetGender()
        {
            return GetAnatomy().Gender;
        }

        public MagiEntity WithComponents(params object[] objs)
        {
            for (int i = 0; i < objs.Length; i++)
            {
                AddComponent(objs[i]);
            }
            return this;
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
                        GameLoop.AddMessageLog("Need is in dire need!");
                }
            }
        }

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
            actorFov ??= new RecursiveShadowcastingBooleanBasedFOV(CurrentMap.TransparencyView);
            actorFov.Calculate(Position, GetViewRadius());
        }

        #endregion Needs

        #endregion Methods
    }
}