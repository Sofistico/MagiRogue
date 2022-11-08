using MagiRogue.Data.Serialization.EntitySerialization;
using MagiRogue.Entities;
using MagiRogue.GameSys.Civ;
using MagiRogue.GameSys.Planet.History;
using MagiRogue.GameSys.Tiles;
using System;
using System.Collections.Generic;

namespace MagiRogue.Data
{
    public static class Find
    {
        // big ass singleton for ease of finding information!
        public static int Year { get; set; }
        public static WorldTile[,] Tiles { get; set; }
        public static List<Civilization> Civs { get; set; }
        public static List<HistoricalFigure> Figures { get; set; }
        public static List<Site> Sites { get; set; }
        public static List<ItemTemplate> Items { get; set; }
        public static List<Ruleset> Rules { get; set; }

        public static void PopulateValues(List<HistoricalFigure> figures,
            List<Civilization> civs,
            List<Site> allSites,
            List<ItemTemplate> importantItems,
            int year,
            WorldTile[,] tiles)
        {
            Year = year;
            Figures = figures;
            Civs = civs;
            Sites = allSites;
            Items = importantItems;
            Tiles = tiles;
            Rules ??= new(DataManager.ListOfRules);
        }
    }
}