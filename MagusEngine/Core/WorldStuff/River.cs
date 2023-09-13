using MagusEngine.ECS.Components.TilesComponents;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace MagusEngine.Core.WorldStuff
{
    public sealed class River
    {
        public int RiverCount { get; }
        public int Length { get; set; }
        public int Id { get; }
        public List<Point> Points { get; set; } = new();
        public Dictionary<Point, int> Intersections { get; set; } = new();
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

        public void AddIntersection(Point position)
        {
            if (!Intersections.TryAdd(position, 1))
            {
                Intersections[position]++;
            }
        }
    }
}