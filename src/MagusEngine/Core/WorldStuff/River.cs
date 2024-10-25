using MagusEngine.Components.TilesComponents;
using System.Collections.Generic;

namespace MagusEngine.Core.WorldStuff
{
    public sealed class River
    {
        public int RiverCount { get; }
        public int Length { get; set; }
        public int Id { get; }
        public List<Point> Points { get; set; } = new();

        /// <summary>
        /// Uses the point index for serialization
        /// </summary>
        public Dictionary<int, int> Intersections { get; set; } = new();
        public float TurnCount { get; set; }
        //public Direction CurrentDirection { get; set; }

        public River(int id)
        {
            Id = id;
        }

        public void AddTile(WorldTile tile)
        {
            Points.Add(tile.Position);
        }

        public void AddIntersection(Point position, int width)
        {
            var index = position.ToIndex(width);
            if (!Intersections.TryAdd(index, 1))
            {
                Intersections[index]++;
            }
        }
    }
}
