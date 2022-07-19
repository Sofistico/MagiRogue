using MagiRogue.Data.Enumerators;
using MagiRogue.Data.Serialization;
using MagiRogue.Data.Serialization.EntitySerialization;
using MagiRogue.Entities;
using MagiRogue.GameSys.Tiles;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MagiRogue.GameSys.Civ
{
    [JsonConverter(typeof(CivilizationJsonConverter))]
    public class Civilization
    {
        public string Name { get; set; }
        public Race PrimaryRace { get; set; }
        public int TotalPopulation { get; set; }

        public CivilizationTendency Tendency { get; set; }
        public List<Point> Territory { get; set; }
        public List<Settlement> Settlements { get; set; }

        public Civilization(string name, Race primaryRace, CivilizationTendency tendency)
        {
            Name = name;
            PrimaryRace = primaryRace;
            Tendency = tendency;
            Territory = new();
            Settlements=new List<Settlement>();
        }

        public void AddSettlementToCiv(Settlement settlement) => Settlements.Add(settlement);

        public List<WorldTile> ReturnAllTerritories(Map map)
        {
            List<WorldTile> tiles = new();
            int count = Territory.Count;

            for (int i = 0; i < count; i++)
            {
                tiles.Add(map.GetTileAt<WorldTile>(Territory[i]));
            }

            return tiles;
        }

        public List<WorldTile> ReturnAllLandTerritory(Map map)
        {
            var list = ReturnAllTerritories(map);
            var landList = new List<WorldTile>();

            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Collidable)
                    landList.Add(list[i]);
            }

            return landList;
        }

        public List<WorldTile> ReturnAllWaterTerritoey(Map map)
        {
            var list = ReturnAllTerritories(map);
            var waterList = new List<WorldTile>();

            for (int i = 0; i < list.Count; i++)
            {
                if (!list[i].Collidable)
                    waterList.Add(list[i]);
            }

            return waterList;
        }

        public Settlement GetSettlement(WorldTile worldTile)
        {
            return Settlements.FirstOrDefault(o => o.WorldPos == worldTile.Position);
        }
    }
}