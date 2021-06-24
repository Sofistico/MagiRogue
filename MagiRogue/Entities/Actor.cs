using GoRogue;
using MagiRogue.Commands;
using MagiRogue.System.Tiles;
using SadRogue.Primitives;
using MagiRogue.System;
using System.Collections.Generic;
using Newtonsoft.Json;

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

    [JsonConverter(typeof(Data.ActorJsonConverter))]
    public class Actor : Entity
    {
        #region Fields

        private bool bumped = false;
        private Stat stats = new Stat();

        #endregion Fields

        #region Properties

        /// <summary>
        /// The stats of the actor
        /// </summary>
        public Stat Stats { get => stats; set => stats = value; }

        /// <summary>
        /// The anatomy of the actor
        /// </summary>
        public Anatomy Anatomy { get; set; }

        /// <summary>
        /// Sets if the char has bumbed in something
        /// </summary>
        public bool Bumped { get => bumped; set => bumped = value; }

        /// <summary>
        /// Here we define were the inventory is
        /// </summary>
        public List<Item> Inventory { get; set; }

        /// <summary>
        /// Defines if this actor can be killed
        /// </summary>
        public bool CanBeKilled { get; set; } = true;

        /// <summary>
        /// Defines if a actor can target or be attacked by this actor
        /// </summary>
        public bool CanBeAttacked { get; set; } = true;

        /// <summary>
        /// Defines if the actor can interact with it's surrondings
        /// </summary>
        public bool CanInteract { get; set; } = true;

        #endregion Properties

        #region Constructor

        /// <summary>
        /// This here defines an actor, must be used with the <see cref= "Data.EntityFactory">
        /// </summary>
        /// <param name="foreground"></param>
        /// <param name="background"></param>
        /// <param name="glyph"></param>
        /// <param name="layer"></param>
        /// <param name="coord"></param>
        public Actor(string name, Color foreground, Color background, int glyph, Point coord, int layer = (int)MapLayer.ACTORS
            ) : base(foreground, background,
            glyph, coord, layer)
        {
            Anatomy = new Anatomy();
            Inventory = new List<Item>();
            Name = name;
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

                //GameLoop.UIManager.IsDi = true;
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
            Actor actor = GameLoop.World.CurrentMap.GetEntityAt<Actor>(Position + positionChange);

            if (actor != null && CanBeAttacked)
            {
                CommandManager.Attack(this, actor);
                Bumped = true;
                return Bumped;
            }

            Bumped = false;
            return Bumped;
        }

        private bool CheckIfThereIsDoor(Point positionChange)
        {
            // Check for the presence of a door
            TileDoor door = GameLoop.World.CurrentMap.GetTileAt<TileDoor>(Position + positionChange);

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