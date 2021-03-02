using GoRogue;
using MagiRogue.Commands;
using MagiRogue.System.Tiles;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

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

    #endregion Enums

    public class Actor : Entity
    {
        #region Fields

        public List<Item> Inventory = new List<Item>();

        private bool bumped = false;
        private Stat stats = new Stat();
        private Anatomy anatomy;

        #endregion Fields

        #region Properties

        public Stat Stats { get => stats; set => stats = value; }
        public Anatomy Anatomy { get => anatomy; set => anatomy = value; }
        public bool Bumped { get => bumped; set => bumped = value; }

        #endregion Properties

        #region Constructor

        public Actor(Color foreground, Color background, int glyph, int layer, Coord coord
            ) : base(foreground, background,
            glyph, coord, layer)
        {
            anatomy = new Anatomy();
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
                bool doorThere = CheckIfThereIsDoor(positionChange);

                if (doorThere)
                    return doorThere;

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

        private bool CheckIfThereIsDoor(Coord positionChange)
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

        #endregion HelpCode
    }
}