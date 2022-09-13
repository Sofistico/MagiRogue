using MagiRogue.Data.Enumerators;
using MagiRogue.Data.Serialization.MapSerialization;
using MagiRogue.GameSys.Tiles;
using MagiRogue.Utils;
using Newtonsoft.Json;
using SadRogue.Primitives;
using System.Collections.Generic;
using System.Linq;

namespace MagiRogue.GameSys
{
    public sealed class Room
    {
        public Rectangle RoomRectangle { get; set; }
        public RoomTag Tag { get; set; }

        [JsonIgnore]
        public List<TileDoor> Doors { get; set; } = new List<TileDoor>();
        public List<Point> DoorsPoint { get; set; } = new List<Point>();

        [JsonIgnore]
        public Dictionary<string, object> Furniture { get; set; } = new();

        [JsonIgnore]
        public Dictionary<string, object> Terrain { get; set; } = new();

        [JsonIgnore]
        public RoomTemplate Template { get; set; }

        public Room(Rectangle roomRectangle, RoomTag tag)
        {
            RoomRectangle = roomRectangle;
            Tag = tag;
        }

        public Room(Rectangle roomRectangle,
            RoomTag tag,
            Dictionary<string, object> furniture,
            Dictionary<string, object> terrain) : this(roomRectangle, tag)
        {
            Furniture = furniture;
            Terrain = terrain;
        }

        public Room(Rectangle roomRectangle)
        {
            RoomRectangle = roomRectangle;
            Tag = RoomTag.Generic;
        }

        public Room()
        {
        }

        public Room(RoomTag tag)
        {
            Tag = tag;
        }

        public List<TileDoor> GetAllDoorsInRoom()
        {
            List<TileDoor> doors = new List<TileDoor>();

            for (int i = 0; i < DoorsPoint.Count; i++)
            {
                Point point = DoorsPoint[i];
                TileDoor door = GameLoop.GetCurrentMap().GetTileAt<TileDoor>(point);
                doors.Add(door);
            }

            return doors;
        }

        public void UpdateListOfDoorPoints()
        {
            for (int i = 0; i < Doors.Count; i++)
            {
                Point point = Doors[i].Position;
                //TileDoor door = GameLoop.GetCurrentMap().GetTileAt<TileDoor>(point);
                DoorsPoint.Add(point);
            }
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

        public Point[] PositionsRoom()
        {
            var points = RoomRectangle.Positions().ToList();
            foreach (var item in ReturnRecPerimiter())
            {
                if (!points.Contains(item))
                {
                    points.Add(item);
                }
            }
            return points.ToArray();
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
            List<Point> borderCells = PointUtils.GetTileLocationsAlongLine(xMin, yMin, xMax, yMin).ToList();
            borderCells.AddRange(PointUtils.GetTileLocationsAlongLine(xMin, yMin, xMin, yMax));
            borderCells.AddRange(PointUtils.GetTileLocationsAlongLine(xMin, yMax, xMax, yMax));
            borderCells.AddRange(PointUtils.GetTileLocationsAlongLine(xMax, yMin, xMax, yMax));

            return borderCells;
        }

        internal Point ReturnRandomPosRoom()
        {
            int x = GoRogue.Random.GlobalRandom.DefaultRNG.NextInt(RoomRectangle.MinExtentX, RoomRectangle.MaxExtentX + 1);
            int y = GoRogue.Random.GlobalRandom.DefaultRNG.NextInt(RoomRectangle.MinExtentY, RoomRectangle.MaxExtentY + 1);

            Point pos = new SadRogue.Primitives.Point(x, y);
            return pos;
        }

        public void ChangeRoomPos(Point point)
        {
            RoomRectangle = RoomRectangle.WithPosition(point);
        }

        public void ChangeRoomPos(int newRoomX, int newRoomY)
        {
            ChangeRoomPos(new SadRogue.Primitives.Point(newRoomX, newRoomY));
        }
    }
}