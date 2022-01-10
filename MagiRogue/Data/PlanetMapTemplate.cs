using MagiRogue.System.Civ;
using MagiRogue.System.Planet;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace MagiRogue.Data
{
    public class PlanetMapJsonConverter : JsonConverter<PlanetMap>
    {
        public override PlanetMap? ReadJson(JsonReader reader, Type objectType,
            PlanetMap? existingValue, bool hasExistingValue,
            JsonSerializer serializer) => serializer.Deserialize<PlanetMapTemplate>(reader);

        public override void WriteJson(JsonWriter writer, PlanetMap? value, JsonSerializer serializer)
            => serializer.Serialize(writer, (PlanetMapTemplate)value);
    }

    public class PlanetMapTemplate
    {
        public float[,] HeightData { get; }
        public float[,] HeatData { get; }
        public float[,] MoistureData { get; }
        public float Min { get; set; }
        public float Max { get; set; }
        public List<Civilization> Civilizations { get; set; }
        public MapTemplate AssocietatedMap { get; }

        public PlanetMapTemplate(float[,] heightData,
            float[,] heatData,
            float[,] moistureData,
            float min,
            float max,
            List<Civilization> civilizations,
            MapTemplate associetatedMap)
        {
            HeightData = heightData;
            HeatData = heatData;
            MoistureData = moistureData;
            Min = min;
            Max = max;
            Civilizations = civilizations;
            AssocietatedMap = associetatedMap;
        }

        public static implicit operator PlanetMapTemplate(PlanetMap map)
        {
            return new PlanetMapTemplate(map.HeightData, map.HeatData,
                map.MoistureData, map.Min, map.Max,
                map.Civilizations, map.AssocietatedMap);
        }

        public static implicit operator PlanetMap(PlanetMapTemplate map)
        {
            PlanetMap mio = new PlanetMap(map.HeightData, map.HeatData,
                map.MoistureData, map.Min, map.Max,
                map.Civilizations, map.AssocietatedMap);

            return mio;
        }
    }
}