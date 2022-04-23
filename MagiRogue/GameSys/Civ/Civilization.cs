﻿using MagiRogue.Data;
using MagiRogue.Data.Serialization;
using MagiRogue.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using MagiRogue.GameSys.Tiles;

namespace MagiRogue.GameSys.Civ
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum CivilizationTendency
    {
        Normal,
        Aggresive,
        Studious,
    }

    [JsonConverter(typeof(CivilizationJsonConverter))]
    public class Civilization
    {
        public string Name { get; set; }
        public Race PrimaryRace { get; set; }
        public int MilitaryStrenght { get; set; }
        public int MagicStrenght { get; set; }
        public int Population { get; set; }
        public CivilizationTendency Tendency { get; set; }
        public float MundaneResources { get; set; }
        public float MagicalResources { get; set; }
        public List<ItemTemplate> Nodes { get; set; }
        public List<Point> Territory { get; set; }

        public Civilization(string name, Race primaryRace, int population,
            CivilizationTendency tendency)
        {
            Name = name;
            PrimaryRace = primaryRace;
            Population = population;
            Tendency = tendency;
            Nodes = new();
            Territory = new();
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
    }
}