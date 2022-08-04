using MagiRogue.Commands;
using MagiRogue.Data.Enumerators;
using MagiRogue.GameSys;
using MagiRogue.GameSys.Tiles;
using MagiRogue.Utils;
using Newtonsoft.Json;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;

namespace MagiRogue.Entities
{
    [JsonConverter(typeof(Data.Serialization.EntitySerialization.ActorJsonConverter))]
    public class Actor : Entity
    {
        #region Fields

        private bool bumped = false;

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
        public bool Bumped { get => bumped; set => bumped = value; }

        // TODO: change inventory...
        /// <summary>
        /// Here we define were the inventory is
        /// </summary>
        public List<Item> Inventory { get; set; }

        [JsonIgnore]
        public int XP { get; set; }

        public bool IsPlayer { get; set; }

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
            Body = new Body(this);
            Inventory = new List<Item>();
            Name = name;
            // by default the material of the actor will be mostly flesh
            Material = GameSys.Physics.PhysicsManager.SetMaterial("flesh");
            Mind = new Mind();
            Soul = new Soul();
        }

        #endregion Constructor

        #region Methods

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

        public int GetPrecisionAbility()
        {
            throw new NotImplementedException();
        }

        public int GetDefenseAbility()
        {
            throw new NotImplementedException();
        }

        public int GetRelevantAbility()
        {
            throw new NotImplementedException();
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
                CommandManager.MeleeAttack(this, actor);
                Bumped = true;
                return Bumped;
            }

            Bumped = false;
            return Bumped;
        }

        public double GetProtection(Limb limb)
        {
            var item = Body.GetArmorOnLimbIfAny(limb);
            return Body.Toughness + (item.Material.Hardness * Body.GetArmorOnLimbIfAny(limb).Material.Density);
        }

        private bool CheckIfThereIsDoor(Point positionChange)
        {
            // Check for the presence of a door
            TileDoor door = GameLoop.GetCurrentMap().GetTileAt<TileDoor>(Position + positionChange);

            // if there's a door here,
            // try to use it
            if (door != null && CanInteract)
            {
                CommandManager.UseDoor(this, door);
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
            return Body.Equipment.GetValueOrDefault(GetAnatomy().Limbs.Find(l =>
            l.TypeLimb == TypeOfLimb.Hand));
        }

        #region RegenCode

        public void ApplyAllRegen()
        {
            ApplyBodyRegen();
            ApplyStaminaRegen();
            ApplyManaRegen();
        }

        public void ApplyBodyRegen()
        {
            foreach (Limb limb in GetAnatomy().Limbs)
            {
                if (limb.Attached && !GetAnatomy().GetActorRace().CanRegenLostLimbs)
                {
                    if (limb.CanHeal)
                    {
                        limb.ApplyHeal(GetNormalLimbRegen() * limb.RateOfHeal);
                    }
                }
                // ignore the limb if the actor has lost them and can't regenerate
                else
                {
                    continue;
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
            return GetAnatomy().GetActorRace().RaceViewRadius + Body.ViewRadius;
        }

        public double GetNormalLimbRegen()
        {
            return GetAnatomy().GetActorRace().RaceNormalLimbRegen + Body.Anatomy.NormalLimbRegen;
        }

        public double GetManaRegen()
        {
            return Soul.BaseManaRegen;
        }

        public Anatomy GetAnatomy() => Body.Anatomy;

        public Dictionary<Limb, Item> GetEquipment() => Body.Equipment;

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
            var itemHeld = WieldedItem();
            if (itemHeld is not null)
            {
                var speed = ((itemHeld.SpeedOfAttack + itemHeld.Weight * itemHeld.Size)
                    / GetActorBaseSpeed());
                var ability = GetRelevantAttackAbility(itemHeld.WeaponType);
                var finalSpeed = ability != 0 ? speed / ability : speed;
                return finalSpeed;
            }
            return GetActorBaseSpeed();
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

        public int GetStrenght()
        {
            return Body.Strength;
        }

        public DamageType GetDamageType()
        {
            if (WieldedItem() is not null && WieldedItem() is Item item)
            {
                return item.ItemDamageType;
            }
            else
            {
                return DamageType.Blunt;
            }
        }

        #endregion GetProperties

        public bool CheckIfDed()
        {
            return !GetAnatomy().EnoughBodyToLive();
        }

        #endregion Methods
    }
}