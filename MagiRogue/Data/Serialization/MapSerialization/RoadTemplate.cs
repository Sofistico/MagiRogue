using MagiRogue.Data.Enumerators;
using MagiRogue.GameSys.Civ;
using MagiRogue.GameSys.Tiles;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace MagiRogue.Data.Serialization.MapSerialization
{
    public class RoadJsonConverter : JsonConverter<Road>
    {
        public override Road? ReadJson(JsonReader reader,
            Type objectType,
            Road? existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            serializer.NullValueHandling = NullValueHandling.Ignore;
            return serializer.Deserialize<RoadTemplate>(reader);
        }

        public override void WriteJson(JsonWriter writer, Road? value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, (RoadTemplate)value);
        }
    }

    public class RoadTemplate
    {
        public List<WorldPosDictionary> RoadDirectionInPos { get; set; } = new();
        public List<Point> RoadTiles { get; set; }
        public RoadStatus Status { get; set; }

        public RoadTemplate(Dictionary<Point, WorldDirection> roadDirectionInPos,
            List<Point> roadTiles, RoadStatus status)
        {
            foreach (Point pos in roadDirectionInPos.Keys)
            {
                RoadDirectionInPos.Add(new WorldPosDictionary(pos, roadDirectionInPos[pos]));
            }
            RoadTiles = roadTiles;
            Status = status;
        }

        public RoadTemplate()
        {
            // empty
        }

        public static implicit operator Road(RoadTemplate template)
        {
            var tiles = new List<WorldTile>();
            if (template is null)
                return null;
            for (int i = 0; i < template.RoadTiles.Count; i++)
            {
                // TODO: Need to find a better way
                //tiles.Add((WorldTile)template.RoadTiles[i]);
            }
            Dictionary<Point, WorldDirection> roadDirectoin = new();
            for (int i = 0; i < template.RoadDirectionInPos.Count; i++)
            {
                var (point, direction) = template.RoadDirectionInPos[i].ReturnValue();
                roadDirectoin.TryAdd(point, direction);
            }
            var road = new Road()
            {
                RoadTiles = tiles,
                RoadDirectionInPos = roadDirectoin,
                Status = template.Status
            };
            return road;
        }

        public static implicit operator RoadTemplate(Road road)
        {
            try
            {
                if (road is null)
                    return null;
                List<Point> tiles = new();
                for (int i = 0; i < road.RoadTiles.Count; i++)
                {
                    tiles.Add(road.RoadTiles[i].Position);
                }

                return new RoadTemplate(road.RoadDirectionInPos,
                tiles,
                road.Status);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

    public class WorldPosDictionary
    {
        public Point Point { get; set; }
        public WorldDirection Direction { get; set; }

        public WorldPosDictionary(Point point, WorldDirection direction)
        {
            Point = point;
            Direction = direction;
        }

        public (Point, WorldDirection) ReturnValue() => (Point, Direction);
    }
}