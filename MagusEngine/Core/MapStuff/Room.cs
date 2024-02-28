using Arquimedes.Enumerators;
using MagusEngine.ECS.Components.TilesComponents;
using MagusEngine.Serialization.MapConverter;
using MagusEngine.Utils.Extensions;
using Newtonsoft.Json;
using SadRogue.Primitives;
using System.Collections.Generic;
using System.Linq;

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

        #region ctors

        public Room()
        {
        }

        public Room(RoomTag tag)
        {
            Tag = tag;
        }

        public Room(Point[] points)
        {
            RoomPoints = points;
        }

        public Room(Point[] points, RoomTag tag) : this(points)
        {
            Tag = tag;
        }

        public Room(IEnumerable<Point> points) : this(points.ToArray())
        {
        }

        public Room(IEnumerable<Point> points, RoomTag tag) : this(points.ToArray(), tag)
        {
        }

        public Room(Rectangle roomRectangle)
        {
            RoomRectangle = roomRectangle;
            RoomPoints = [.. roomRectangle.Positions()];
            Tag = RoomTag.Generic;
        }

        public Room(Rectangle roomRectangle, RoomTag tag) : this(roomRectangle)
        {
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

        #endregion ctors

        public void UpdateListOfDoorPoints()
        {
            for (int i = 0; i < Doors.Count; i++)
            {
                Point point = Doors[i].Position;
                DoorsPoint.Add(point);
            }
        }

        public void ForceUnlock()
        {
            foreach (var door in Doors)
            {
                var comp = door.GetComponent<DoorComponent>();
                if (comp is not null)
                    comp.Locked = false;
            }
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
