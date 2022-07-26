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
        /// The anatomy of the actor
        /// </summary>
        public Anatomy Anatomy { get; set; }

        /// <summary>
        /// Sets if the char has bumbed in something
        /// </summary>
        public bool Bumped { get => bumped; set => bumped = value; }

        internal bool CheckIfDed()
        {
            throw new NotImplementedException();
        }

        // TODO: change inventory...
        /// <summary>
        /// Here we define were the inventory is
        /// </summary>
        public List<Item> Inventory { get; set; }

        /// <summary>
        /// The equipment that the actor is curently using
        /// </summary>
        [JsonIgnore]
        public Dictionary<Limb, Item> Equipment { get; set; }

        [JsonIgnore]
        public int XP { get; set; }

        /// <summary>
        /// Dictionary of the Abilities of an actor.
        /// Never add directly to the dictionary, use the method AddAbilityToDictionary to add new abilities
        /// </summary>
        public Dictionary<int, Ability> Abilities { get; set; }

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
            Anatomy = new Anatomy(this);
            Inventory = new List<Item>();
            Equipment = new Dictionary<Limb, Item>();
            Abilities = new();
            Name = name;
            // by default the material of the actor will be mostly flesh
            Material = GameSys.Physics.PhysicsManager.SetMaterial("flesh");
        }

        #endregion Constructor

        #region HelpCode

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

        internal int GetAttackSpeed()
        {
            throw new NotImplementedException();
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

        internal int GetPrecisionAbility()
        {
            throw new NotImplementedException();
        }

        internal int GetDefenseAbility()
        {
            throw new NotImplementedException();
        }

        internal int GetAttackAbility()
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
                CommandManager.Attack(this, actor);
                Bumped = true;
                return Bumped;
            }

            Bumped = false;
            return Bumped;
        }

        internal int GetProtection()
        {
            throw new NotImplementedException();
        }

        internal int GetStrenght()
        {
            throw new NotImplementedException();
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

        public Item? WieldedItem()
        {
            return Equipment.GetValueOrDefault(Anatomy.Limbs.Find(l =>
            l.TypeLimb == TypeOfLimb.Hand));
        }

        public void AddAbilityToDictionary(Ability ability)
        {
            Abilities.Add(ability.Id, ability);
        }

        internal float GetActorBaseSpeed()
        {
            throw new NotImplementedException();
        }

        public void ApplyAllRegen()
        {
            ApplyHpRegen();
            ApplyManaRegen();
        }

        public void ApplyHpRegen()
        {
            if (Health < MaxHealth)
            {
                double newHp = (GetBaseHpRegen() + Health);
                Health = MathMagi.Round(newHp);
            }
        }

        public void ApplyManaRegen()
        {
            if (Soul.CurrentMana < Soul.MaxMana)
            {
                double newMana = (GetBaseManaRegen() + Soul.CurrentMana);
                Soul.CurrentMana = MathMagi.Round(newMana);
            }
        }

        public int GetViewRadius()
        {
            return Anatomy.GetActorRace().RaceViewRadius;
        }

        public double GetBaseHpRegen()
        {
            throw new NotImplementedException();
        }

        public double GetBaseManaRegen()
        {
            throw new NotImplementedException();
        }

        #endregion HelpCode
    }
}