using Arquimedes.Enumerators;
using MagusEngine.Serialization.MapConverter;
using Newtonsoft.Json;
using SadRogue.Primitives;
using System.Collections.Generic;

namespace MagusEngine.Core.Civ
{
    [JsonConverter(typeof(RoadJsonConverter))]
    public class Road
    {
        public Dictionary<Point, Direction> RoadDirectionInPos { get; set; }
        public RoadStatus Status { get; set; }
        public int RoadId { get; set; }

        public Road()
        {
            RoadDirectionInPos = new();
        }
    }
}