using Arquimedes.Enumerators;
using MagusEngine.Core.Civ;
using Newtonsoft.Json;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;

namespace MagusEngine.Serialization.MapConverter
{
    // TODO: REDO
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
        public RoadStatus Status { get; set; }
        public int RoadId { get; set; }

        public RoadTemplate(Dictionary<Point, Direction> roadDirectionInPos, RoadStatus status)
        {
            foreach (Point pos in roadDirectionInPos.Keys)
            {
                RoadDirectionInPos.Add(new WorldPosDictionary(pos, roadDirectionInPos[pos]));
            }
            Status = status;
        }

        public RoadTemplate()
        {
            // empty
        }

        public static implicit operator Road(RoadTemplate template)
        {
            if (template is null)
                return null!;
            Dictionary<Point, Direction> roadDirectoin = new();
            for (int i = 0; i < template.RoadDirectionInPos.Count; i++)
            {
                var (point, direction) = template.RoadDirectionInPos[i].ReturnValue();
                roadDirectoin.TryAdd(point, direction);
            }
            return new Road()
            {
                RoadDirectionInPos = roadDirectoin,
                Status = template.Status,
                RoadId = template.RoadId
            };
        }

        public static implicit operator RoadTemplate(Road road)
        {
            try
            {
                if (road is null)
                    return null!;

                return new RoadTemplate(road.RoadDirectionInPos, road.Status)
                {
                    RoadId = road.RoadId
                };
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
        public Direction Direction { get; set; }

        public WorldPosDictionary(Point point, Direction direction)
        {
            Point = point;
            Direction = direction;
        }

        public (Point, Direction) ReturnValue() => (Point, Direction);
    }
}