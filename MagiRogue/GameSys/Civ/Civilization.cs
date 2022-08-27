using MagiRogue.Data.Enumerators;
using MagiRogue.Data.Serialization;
using MagiRogue.Data.Serialization.EntitySerialization;
using MagiRogue.Entities;
using MagiRogue.GameSys.Tiles;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MagiRogue.GameSys.Civ
{
    //[JsonConverter(typeof(CivilizationJsonConverter))]
    [DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
    public class Civilization
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Race PrimaryRace { get; set; }
        public int TotalPopulation { get => Settlements.Select(i => i.Population).Sum(); }

        public CivilizationTendency Tendency { get; set; }
        public List<Point> Territory { get; set; }
        public List<Settlement> Settlements { get; set; }
        public int Wealth { get; set; }
        public List<CivRelation> Relations { get; set; }

        public CivRelation this[int otherCivId]
        {
            get => Relations.FirstOrDefault(i => i.OtherCivId == otherCivId);

            set
            {
                var relation = Relations.FirstOrDefault(i => i.OtherCivId == otherCivId);
                relation = value;
            }
        }
        public List<HistoricalFigure> ImportantPeople { get; set; }
        public bool Dead { get; set; }

        [JsonConstructor]
        public Civilization(string name, Race primaryRace, CivilizationTendency tendency)
        {
            Name = name;
            PrimaryRace = primaryRace;
            Tendency = tendency;
            Territory = new();
            Settlements = new List<Settlement>();
            ImportantPeople = new();
            Relations = new();
        }

        public void AddSettlementToCiv(Settlement settlement)
        {
            Settlements.Add(settlement);
            Wealth += settlement.MundaneResources;
        }

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

        public void AddCivToRelations(Civilization civ, RelationType relationType)
        {
            if (!Relations.Any(i => i.OtherCivId.Equals(civ.Id) && i.Relation == relationType))
                Relations.Add(new CivRelation(Id, civ.Id, relationType));
        }

        public void SimulateImportantStuff()
        {
            if (Dead)
                return;

            if (TotalPopulation <= 0)
            {
                Dead = true;
                return;
            }

            
            foreach (HistoricalFigure figure in ImportantPeople)
            {
                // HALP! WHY DID I MAKE THIS!
            }
        }

        private string GetDebuggerDisplay()
        {
            return ToString();
        }

        public override string ToString()
        {
            return $"{Name} - {PrimaryRace}";
        }
    }
}