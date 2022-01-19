using Newtonsoft.Json;
using MagiRogue.Data.Serialization;

namespace MagiRogue.System
{
    /// <summary>
    /// Region chunks for the world map, each chunk contains 3 * 3 maps, in a grid like manner where the
    /// edges connect the map to each other.
    /// </summary>
    public class RegionChunk
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

        public void ActivateAllMaps()
        {
            for (int i = 0; i < LocalMaps.Length; i++)
            {
                LocalMaps[i].LoadToMemory();
            }
        }
    }
}