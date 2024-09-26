using MagusEngine.Core.Civ;
using MagusEngine.Core.WorldStuff;
using MagusEngine.Core.WorldStuff.History;
using MagusEngine.ECS.Components.TilesComponents;
using MagusEngine.Systems;
using MagusEngine.Systems.Time;
using MagusEngine.Utils;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace MagusEngine.Core.MapStuff
{
    // made with http://www.jgallant.com/procedurally-generating-wrapping-world-maps-in-unity-csharp-part-1/#intro
    public sealed class PlanetMap
    {
        private readonly int _height;
        private readonly int _width;

        public List<Civilization> Civilizations { get; set; }

        public MagiMap AssocietatedMap { get; }

        public int YearSinceCreation { get => WorldHistory.CreationYear; }

        public AccumulatedHistory WorldHistory { get; set; }

        public string Name => AssocietatedMap.MapName;

        public List<River> Rivers { get; set; } = new();

        public PlanetMap(int width, int height)
        {
            _height = height;
            _width = width;
            AssocietatedMap = new(RandomNames.RandomNamesFromRandomLanguage(), width, height, false);
            Civilizations = new List<Civilization>();
            WorldHistory = new();
        }

        [JsonConstructor]
        public PlanetMap(List<Civilization> civilizations, MagiMap associetatedMap)
        {
            Civilizations = civilizations;
            AssocietatedMap = associetatedMap;
            _height = associetatedMap.Height;
            _width = associetatedMap.Width;
            AssocietatedMap.
                GoRogueComponents.GetFirstOrDefault<FOVHandler>()?.Disable();
        }

        public void SetWorldTiles(WorldTile[,] tiles)
        {
            AssocietatedMap.GoRogueComponents.GetFirstOrDefault<FOVHandler>().Disable();

            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    AssocietatedMap.SetTerrain(tiles[x, y].Parent);
                }
            }
        }

        public TimeSystem GetTimePassed()
        {
            return new TimeSystem(YearSinceCreation);
        }
    }
}
