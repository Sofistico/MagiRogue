using MagiRogue.System;
using MagiRogue.System.Civ;
using MagiRogue.System.Planet;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MagiRogue.Data.Serialization
{
    public class PlanetMapJsonConverter : JsonConverter<PlanetMap>
    {
        public override PlanetMap? ReadJson(JsonReader reader, Type objectType,
            PlanetMap? existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            // put here the method to find and link the map to the id
            JObject jObject = JObject.Load(reader);
            float[,] heightData =
                JsonConvert.DeserializeObject<float[,]>(jObject["HeightData"].ToString());
            float[,] heattData =
                JsonConvert.DeserializeObject<float[,]>(jObject["HeatData"].ToString());
            float[,] moistureData =
                JsonConvert.DeserializeObject<float[,]>(jObject["MoistureData"].ToString());

            int min = int.MinValue;
            int max = int.MaxValue;

            List<Civilization> civs = JsonConvert
                .DeserializeObject<List<Civilization>>
                (jObject["Civilizations"].ToString());

            MapTemplate map =
                JsonConvert.DeserializeObject<Map>(jObject["AssocietatedMap"].ToString());

            PlanetMapTemplate template = new PlanetMapTemplate(heightData,
                heattData,
                moistureData,
                min,
                max,
                civs,
                map);

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

        [JsonIgnore]
        public float Min { get; set; }

        [JsonIgnore]
        public float Max { get; set; }
        public List<CivilizationTemplate> Civilizations { get; set; }

        //public uint AssocietatedMapId { get; }
        public MapTemplate AssocietatedMap { get; set; }

        public PlanetMapTemplate(float[,] heightData,
            float[,] heatData,
            float[,] moistureData,
            float min,
            float max,
            List<Civilization> civilizations)
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

        public PlanetMapTemplate()
        {
        }

        public PlanetMapTemplate(float[,] heightData,
            float[,] heatData,
            float[,] moistureData,
            float min,
            float max,
            List<Civilization> civilizations,
            MapTemplate associetatedMap) : this(heightData,
                heatData,
                moistureData, min, max, civilizations)
        {
            AssocietatedMap = associetatedMap;
        }

        public static implicit operator PlanetMapTemplate(PlanetMap map)
        {
            var template = new PlanetMapTemplate(map.HeightData, map.HeatData,
                map.MoistureData, map.Min, map.Max,
                map.Civilizations)
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