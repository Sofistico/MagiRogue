using MagiRogue.System.Civ;
using MagiRogue.System.Planet;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace MagiRogue.Data.Serialization
{
    public class PlanetMapJsonConverter : JsonConverter<PlanetMap>
    {
        public override PlanetMap? ReadJson(JsonReader reader, Type objectType,
            PlanetMap? existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            // put here the method to find and link the map to the id
            PlanetMapTemplate template = serializer.Deserialize<PlanetMapTemplate>(reader);

            return template;
        }

        public override void WriteJson(JsonWriter writer, PlanetMap? value, JsonSerializer serializer)
        {
            serializer.Formatting = Formatting.Indented;
            serializer.NullValueHandling = NullValueHandling.Ignore;
            serializer.Serialize(writer, (PlanetMapTemplate)value);
        }
    }

    public class PlanetMapTemplate
    {
        public float[,] HeightData { get; }
        public float[,] HeatData { get; }
        public float[,] MoistureData { get; }
        public float Min { get; set; }
        public float Max { get; set; }
        public List<CivilizationTemplate> Civilizations { get; set; }

        //public uint AssocietatedMapId { get; }
        public MapTemplate AssocietatedMap { get; set; }

        public PlanetMapTemplate(float[,] heightData,
            float[,] heatData,
            float[,] moistureData,
            float min,
            float max,
            List<Civilization> civilizations,
            uint associetatedMap)
        {
            HeightData = heightData;
            HeatData = heatData;
            MoistureData = moistureData;
            Min = min;
            Max = max;
            Civilizations = new();
            for (int i = 0; i < civilizations.Count; i++)
            {
                Civilizations.Add(civilizations[i]);
            }
            //AssocietatedMapId = associetatedMap;
        }

        public static implicit operator PlanetMapTemplate(PlanetMap map)
        {
            var template = new PlanetMapTemplate(map.HeightData, map.HeatData,
                map.MoistureData, map.Min, map.Max,
                map.Civilizations, map.AssocietatedMap.MapId)
            {
                AssocietatedMap = map.AssocietatedMap
            };
            return template;
        }

        public static implicit operator PlanetMap(PlanetMapTemplate map)
        {
            var civs = new List<Civilization>();
            for (int i = 0; i < map.Civilizations.Count; i++)
            {
                civs.Add(map.Civilizations[i]);
            }

            PlanetMap mio = new PlanetMap(map.HeightData, map.HeatData,
                map.MoistureData, map.Min, map.Max,
                civs, (MapTemplate)map.AssocietatedMap);
            /*PlanetMap mio = new PlanetMap(map.Min, map.Max,
                civs, (MapTemplate)map.AssocietatedMap);*/

            return mio;
        }
    }
}