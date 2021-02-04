using GoRogue;
using MagiRogue.Commands;
using MagiRogue.System;
using MagiRogue.System.Tiles;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MagiRogue.Entities
{
    #region Enums

    public enum Archtypes
    {
        WARRIOR,
        WIZARD,
        ROGUE,
        SORCEROR,
        WARLOCK,
        DRUID,
        RANGER,
        PRIEST,
        GODLING, // This one will be fun, caracterized as a lovecraftian monstrosity
    }

    public enum HumanoidRace
    {
        HUMAN,
        ELF,
        DWARF,
        HALFELF,
        GNOME,
        LIZARD,
        GOBLIN,
        OGRE,
        NYMPH,
    }

    public enum MonsterRace
    {
        GENERICBEAST,
        DRAGON,
        DOG,
        CAT,
        BEAR,
        DONKEY,
        HORSE
    }

    #endregion Enums

    public abstract class Actor : Entity
    {
        #region Fields

        public List<Item> Inventory = new List<Item>(); // the inventory of the actor;

        public Stat Stats = new Stat();

        public bool Bumped = false;

        #endregion Fields

        #region Constructor

        protected Actor(Color foreground, Color background, int glyph, int layer, Coord coord, int width = 1, int height = 1) : base(foreground, background,
            glyph, coord, layer, width, height)
        {
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
            if (GameLoop.World.CurrentMap.WalkabilityView[Position + positionChange])
            {
                bool attacked = CheckIfCanAttack(positionChange);

                if (attacked)
                    return attacked;

                Position += positionChange;
                GameLoop.UIManager.IsDirty = true;
                return true;
            }
            // Handle situations where there are non-walkable tiles that CAN be used
            else
            {
                // Check for the presence of a door
                TileDoor door = GameLoop.World.CurrentMap.GetTileAt<TileDoor>(Position + positionChange);

                // if there's a door here,
                // try to use it
                if (door != null)
                {
                    CommandManager.UseDoor(this, door);
                    return true;
                }
                return false;
            }
        }

        private bool CheckIfCanAttack(Point positionChange)
        {
            // if there's a monster here,
            // do a bump attack
            Actor actor = GameLoop.World.CurrentMap.GetEntity<Actor>(Position + positionChange);

            if (actor != null)
            {
                CommandManager.Attack(this, actor);
                Bumped = true;
                return Bumped;
            }

            Bumped = false;
            return Bumped;
        }

        // Moves the Actor TO newPosition location
        // returns true if actor was able to move, false if failed to move
        public bool MoveTo(Point newPosition)
        {
            if (GameLoop.World.CurrentMap.IsTileWalkable(newPosition))
            {
                Position = newPosition;
                return true;
            }
            else
            {
                return false;
            }
        }

        public void ApplyHpRegen()
        {
            if (this.Stats.Health < this.Stats.MaxHealth)
            {
                float newHp = (this.Stats.BaseHpRegen + this.Stats.Health);
                this.Stats.Health = (float)Math.Round(newHp);
            }
        }

        #endregion HelpCode
    }
}