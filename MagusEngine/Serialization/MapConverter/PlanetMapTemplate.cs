using MagiRogue.GameSys;
using MagusEngine.Core;
using MagusEngine.Core.Civ;
using MagusEngine.Core.WorldStuff.History;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MagusEngine.Serialization.MapConverter
{
    public sealed class PlanetMapJsonConverter : JsonConverter<PlanetMap>
    {
        public override PlanetMap? ReadJson(JsonReader reader, Type objectType,
            PlanetMap? existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            // put here the method to find and link the map to the id
            /*JObject jObject = JObject.Load(reader);
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

            return template;*/
            return serializer.Deserialize<PlanetMapTemplate>(reader);
        }

        public override void WriteJson(JsonWriter writer, PlanetMap? value, JsonSerializer serializer)
        {
            serializer.Formatting = Formatting.Indented;
            serializer.NullValueHandling = NullValueHandling.Ignore;
            serializer.Serialize(writer, (PlanetMapTemplate)value);
        }
    }

    public sealed class PlanetMapTemplate
    {
        [JsonIgnore]
        public float Min { get; set; }

        [JsonIgnore]
        public float Max { get; set; }

        [DataMember]
        public List<Civilization> Civilizations { get; set; }

        //public uint AssocietatedMapId { get; }
        [DataMember]
        public Map AssocietatedMap { get; set; }

        [DataMember]
        public long YearSinceCreation { get; set; }

        [DataMember]
        public AccumulatedHistory WorldHistory { get; set; }

        public PlanetMapTemplate(float min,
            float max,
            List<Civilization> civilizations)
        {
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

        public PlanetMapTemplate(float min,
            float max,
            List<Civilization> civilizations,
            MapTemplate associetatedMap) : this(min, max, civilizations)
        {
            AssocietatedMap = associetatedMap;
        }

        public static implicit operator PlanetMapTemplate(PlanetMap map)
        {
            var template = new PlanetMapTemplate(map.Min, map.Max,
                map.Civilizations)
            {
                AssocietatedMap = map.AssocietatedMap,
                YearSinceCreation = map.YearSinceCreation,
                WorldHistory = map.WorldHistory
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
            map.Max = int.MaxValue;
            map.Min = int.MinValue;

            PlanetMap mio = new PlanetMap(map.Min, map.Max,
                civs, (MapTemplate)map.AssocietatedMap)
            {
                WorldHistory = map.WorldHistory
            };
            /*PlanetMap mio = new PlanetMap(map.Min, map.Max,
                civs, (MapTemplate)map.AssocietatedMap);*/

            return mio;
        }
    }
}