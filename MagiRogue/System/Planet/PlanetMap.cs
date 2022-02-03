using MagiRogue.Data.Serialization;
using MagiRogue.System.Civ;
using MagiRogue.System.Tiles;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.System.Planet
{
    // made with http://www.jgallant.com/procedurally-generating-wrapping-world-maps-in-unity-csharp-part-1/#intro
    [JsonConverter(typeof(PlanetMapJsonConverter))]
    public sealed class PlanetMap
    {
        private readonly int _height;
        private readonly int _width;

        [DataMember]
        public float[,] HeightData { get; }

        [DataMember]
        public float[,] HeatData { get; }

        [DataMember]
        public float[,] MoistureData { get; }

        [DataMember]
        public float Min { get; set; }

        [DataMember]
        public float Max { get; set; }

        [DataMember]
        public List<Civilization> Civilizations { get; set; }

        [DataMember]
        public Map AssocietatedMap { get; }

        public PlanetMap(int width, int height)
        {
            _height = height;
            _width = width;
            HeightData = new float[width, height];
            HeatData = new float[width, height];
            MoistureData = new float[width, height];
            Min = float.MinValue;
            Max = float.MaxValue;
            AssocietatedMap = new("Planet", width, height);
            Civilizations = new List<Civilization>();
        }

        public PlanetMap(float[,] heightData, float[,] heatData,
            float[,] moistureData, float min, float max,
            List<Civilization> civilizations, Map associetatedMap)
        {
            HeightData = heightData;
            HeatData = heatData;
            MoistureData = moistureData;
            Min = min;
            Max = max;
            Civilizations = civilizations;
            AssocietatedMap = associetatedMap;
            _height = associetatedMap.Height;
            _width = associetatedMap.Width;
            AssocietatedMap.
                GoRogueComponents.GetFirstOrDefault<FOVHandler>().Disable();
        }

        public PlanetMap(float min, float max,
            List<Civilization> civilizations, Map associetatedMap)
        {
            _height = associetatedMap.Height;
            _width = associetatedMap.Width;
            Min = min;
            Max = max;
            Civilizations = civilizations;
            AssocietatedMap = associetatedMap;
        }

        public void SetWorldTiles(WorldTile[,] tiles)
        {
            AssocietatedMap.GoRogueComponents.GetFirstOrDefault<FOVHandler>().Disable();

            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    AssocietatedMap.SetTerrain(tiles[x, y]);
                }
            }
        }
    }
}