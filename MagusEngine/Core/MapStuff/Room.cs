using Arquimedes.Enumerators;
using MagusEngine.ECS.Components.TilesComponents;
using MagusEngine.Serialization.MapConverter;
using MagusEngine.Utils.Extensions;
using Newtonsoft.Json;
using SadRogue.Primitives;

namespace MagusEngine.Core.MapStuff
{
    public sealed class Room
    {
        #region Fields

        private RoomTag previousTag;

        #endregion Fields

        #region Props

        public Rectangle RoomRectangle { get; set; }
        public Point[] RoomPoints { get; set; } = default!;
        public bool NonRectangleRoom => RoomRectangle == Rectangle.Empty;

        public RoomTag Tag { get; set; }

        [JsonIgnore]
        public List<Tile> Doors { get; set; } = new List<Tile>();

        public List<Point> DoorsPoint { get; set; } = new List<Point>();

        [JsonIgnore]
        public Dictionary<string, object> Furniture { get; set; } = new();

        [JsonIgnore]
        public Dictionary<string, object> Terrain { get; set; } = new();

        [JsonIgnore]
        public RoomTemplate? Template { get; set; }

        #endregion Props

        public Room(Rectangle roomRectangle, RoomTag tag)
        {
            RoomRectangle = roomRectangle;
            RoomPoints = roomRectangle.Positions().ToArray();
            Tag = tag;
        }

        public Room(Point[] points)
        {
            RoomPoints = points;
        }

        public Room(Point[] points, RoomTag tag)
        {
            RoomPoints = points;
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
            RoomPoints = roomRectangle.Positions().ToArray();
            Tag = RoomTag.Generic;
        }

        public Room()
        {
        }

        public Room(RoomTag tag)
        {
            Tag = tag;
        }

        public List<Tile> GetAllDoorsInRoom()
        {
            List<Tile> doors = new List<Tile>();

            for (int i = 0; i < DoorsPoint.Count; i++)
            {
                Point point = DoorsPoint[i];
                Tile? door = null;
                doors.Add(door);
            }

            return doors;
        }

        public void UpdateListOfDoorPoints()
        {
            for (int i = 0; i < Doors.Count; i++)
            {
                Point point = Doors[i].Position;
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
                    Doors[indexDoor].GoRogueComponents.GetFirst<DoorComponent>().Locked = true;
            }
        }

        internal void ForceUnlock()
        {
            foreach (var door in Doors)
            {
                var comp = door.GetComponent<DoorComponent>();
                if (comp is not null)
                    comp.Locked = false;
            }
        }

        public List<Point> ReturnRecPerimiter()
        {
            //establish room boundaries
            int xMin = RoomRectangle.MinExtentX;
            int xMax = RoomRectangle.MaxExtentX;
            int yMin = RoomRectangle.MinExtentY;
            int yMax = RoomRectangle.MaxExtentY;

            // build a list of room border cells using a series of straight lines
            List<Point> borderCells = PointUtils.GetTileLocationsAlongLine(xMin, yMin, xMax, yMin).ToList();
            borderCells.AddRange(PointUtils.GetTileLocationsAlongLine(xMin, yMin, xMin, yMax));
            borderCells.AddRange(PointUtils.GetTileLocationsAlongLine(xMin, yMax, xMax, yMax));
            borderCells.AddRange(PointUtils.GetTileLocationsAlongLine(xMax, yMin, xMax, yMax));

            return borderCells;
        }

        public Point ReturnRandomPosRoom()
        {
            return RoomPoints.GetRandomItemFromList();
        }

        public void ChangeRoomPos(Point point)
        {
            RoomRectangle = RoomRectangle.WithPosition(point);
        }

        public void ChangeRoomPos(int newRoomX, int newRoomY)
        {
            ChangeRoomPos(new Point(newRoomX, newRoomY));
        }

        internal void SetPreviousTag()
        {
            previousTag = Tag;
            Tag = previousTag;
        }

        internal void AbandonPreviousTag(RoomTag newTag)
        {
            previousTag = Tag;
            Tag = newTag;
        }

        public Point GetCenter()
        {
            return new Point((int)RoomPoints.Average(x => x.X),
                (int)RoomPoints.Average(y => y.Y));
        }
    }
}