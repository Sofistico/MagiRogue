using MagiRogue.System.Tiles;
using SadRogue.Primitives;
using System.Collections.Generic;

namespace MagiRogue.System
{
    public class Room
    {
        public Rectangle RoomRectangle { get; set; }
        public RoomTag Tag { get; set; }
        public List<TileDoor> Doors { get; set; } = new List<TileDoor>();

        public Room(Rectangle roomRectangle, RoomTag tag)
        {
            RoomRectangle = roomRectangle;
            Tag = tag;
        }

        public Room(Rectangle roomRectangle)
        {
            RoomRectangle = roomRectangle;
            Tag = RoomTag.Generic;
        }

        public void LockDoorsRng()
        {
            int half = GoRogue.DiceNotation.Dice.Roll("1d2");

            if (half == 1)
            {
                int indexDoor = GoRogue.Random.GlobalRandom.DefaultRNG.NextInt(Doors.Count);

                Doors[indexDoor].Locked = true;
            }
        }

        internal void ForceUnlock()
        {
            foreach (var door in Doors)
            {
                door.Locked = false;
            }
        }
    }

    public enum RoomTag
    {
        Generic, Inn, Temple, Blacksmith, Clothier, Alchemist, PlayerHouse, Hovel, Abandoned
    }
}