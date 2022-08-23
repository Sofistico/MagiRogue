using MagiRogue.Data.Serialization.MapSerialization;
using MagiRogue.GameSys.Civ;
using MagiRogue.GameSys.Tiles;
using MagiRogue.GameSys.Time;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MagiRogue.GameSys.Planet
{
    // made with http://www.jgallant.com/procedurally-generating-wrapping-world-maps-in-unity-csharp-part-1/#intro
    [JsonConverter(typeof(PlanetMapJsonConverter))]
    public sealed class PlanetMap
    {
        private readonly int _height;
        private readonly int _width;

        [DataMember]
        public float Min { get; set; }

        [DataMember]
        public float Max { get; set; }

        [DataMember]
        public List<Civilization> Civilizations { get; set; }

        [DataMember]
        public Map AssocietatedMap { get; }

        [DataMember]
        public long TicksSinceCreation { get; set; }

        public PlanetMap(int width, int height)
        {
            _height = height;
            _width = width;
            Min = float.MinValue;
            Max = float.MaxValue;
            AssocietatedMap = new("Planet", width, height);
            Civilizations = new List<Civilization>();
        }

        public PlanetMap(float min, float max,
            List<Civilization> civilizations, Map associetatedMap)
        {
            Min = min;
            Max = max;
            Civilizations = civilizations;
            AssocietatedMap = associetatedMap;
            _height = associetatedMap.Height;
            _width = associetatedMap.Width;
            AssocietatedMap.
                GoRogueComponents.GetFirstOrDefault<FOVHandler>().Disable();
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

        public TimeSystem GetTimePassed()
        {
            return new TimeSystem(TicksSinceCreation);
        }
    }
}