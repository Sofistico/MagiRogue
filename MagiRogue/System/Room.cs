using MagiRogue.System.Tiles;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public void LockedDoorsRng()
        {
            int half = GoRogue.DiceNotation.Dice.Roll("1d2");

            if (half == 1)
            {
                int indexDoor = GoRogue.Random.GlobalRandom.DefaultRNG.NextInt(Doors.Count);
                if (Doors.Count > 0)
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

        public Point[] PositionInsideRoom()
        {
            return RoomRectangle.Positions().ToArray();
        }

        public List<Point> ReturnRecPerimiter()
        {
            //establish room boundaries
            int xMin = RoomRectangle.ToMonoRectangle().Left;
            int xMax = RoomRectangle.ToMonoRectangle().Right;
            int yMin = RoomRectangle.ToMonoRectangle().Bottom;
            int yMax = RoomRectangle.ToMonoRectangle().Top;

            // build a list of room border cells using a series of
            // straight lines
            List<Point> borderCells = GetTileLocationsAlongLine(xMin, yMin, xMax, yMin).ToList();
            borderCells.AddRange(GetTileLocationsAlongLine(xMin, yMin, xMin, yMax));
            borderCells.AddRange(GetTileLocationsAlongLine(xMin, yMax, xMax, yMax));
            borderCells.AddRange(GetTileLocationsAlongLine(xMax, yMin, xMax, yMax));

            return borderCells;
        }

        private IEnumerable<Point> GetTileLocationsAlongLine(
            int xOrigin, int yOrigin, int xDestination, int yDestination)
        {
            int dx = Math.Abs(xDestination - xOrigin);
            int dy = Math.Abs(yDestination - yOrigin);

            int sx = xOrigin < xDestination ? 1 : -1;
            int sy = yOrigin < yDestination ? 1 : -1;
            int err = dx - dy;

            while (true)
            {
                yield return new Point(xOrigin, yOrigin);
                if (xOrigin == xDestination && yOrigin == yDestination)
                    break;

                int e2 = 2 * err;
                if (e2 > -dy)
                {
                    err -= dy;
                    xOrigin += sx;
                }
                if (e2 < dx)
                {
                    err += dx;
                    yOrigin += sy;
                }
            }
        }
    }

    public enum RoomTag
    {
        Generic, Inn, Temple, Blacksmith, Clothier, Alchemist, PlayerHouse, Hovel, Abandoned
    }
}