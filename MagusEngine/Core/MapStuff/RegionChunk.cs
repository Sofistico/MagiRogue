using Arquimedes.Enumerators;
using MagusEngine.Core.Entities;
using MagusEngine.Serialization.MapConverter;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace MagusEngine.Core.MapStuff
{
    /// <summary>
    /// Region chunks for the world map, each chunk contains 3 * 3 maps, in a grid like manner where
    /// the edges connect the map to each other.
    /// </summary>
    [JsonConverter(typeof(RegionChunkJsonConverter))]
    public sealed class RegionChunk
    {
        /// <summary>
        /// The max amount of local maps the region chunks hold, should be 3*3 = 9 maps.
        /// </summary>
        public const int MAX_LOCAL_MAPS = 3 * 3;

        public int X { get; }
        public int Y { get; }
        public Map[] LocalMaps { get; set; }

        public RegionChunk(int x, int y)
        {
            X = x;
            Y = y;
            LocalMaps = new Map[MAX_LOCAL_MAPS];
        }

        public RegionChunk(Point point)
        {
            X = point.X;
            Y = point.Y;
            LocalMaps = new Map[MAX_LOCAL_MAPS];
        }

        public Point ChunckPos() => new Point(X, Y);

        public int ToIndex(int width) => Point.ToIndex(X, Y, width);

        public bool MapsAreConnected()
        {
            for (int i = 0; i < MAX_LOCAL_MAPS; i++)
            {
                Map map = LocalMaps[i];
                if (map.MapZoneConnections.Count <= 0)
                    return false;
            }
            return true;
        }

        public List<Room> ReturnAllRoomsOnChunk()
        {
            List<Room> rooms = new List<Room>();
            for (int i = 0; i < LocalMaps.Length; i++)
            {
                foreach (Room room in LocalMaps[i].Rooms)
                {
                    rooms.Add(room);
                }
            }
            return rooms;
        }

        internal void SetMapsToUpdate()
        {
            for (int i = 0; i < MAX_LOCAL_MAPS; i++)
            {
                LocalMaps[i].NeedsUpdate = true;
            }
        }

        public IEnumerable<Actor> TotalPopulation()
        {
            List<Actor> actors = new List<Actor>();
            for (int m = 0; m < LocalMaps.Length; m++)
            {
                Map? map = LocalMaps[m];
                if (map is null) continue;
                var ac = map.Entities.GetLayer((int)MapLayer.ACTORS).Items.ToArray();
                for (int i = 0; i < ac.Length; i++)
                {
                    if (ac[i] is Actor actor)
                    {
                        if (map.ControlledEntitiy is not null && map.ControlledEntitiy.ID == actor.ID)
                            continue;
                        actors.Add(actor);
                    }
                }
            }

            return actors;
        }
    }
}