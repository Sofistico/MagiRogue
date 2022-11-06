using MagiRogue.Data.Serialization.EntitySerialization;
using MagiRogue.Entities;
using MagiRogue.GameSys.Civ;
using MagiRogue.GameSys.Planet.History;
using MagiRogue.GameSys.Tiles;
using System;
using System.Collections.Generic;

namespace MagiRogue.Data
{
    public class Find
    {
        // big ass singleton for ease of finding information!
        public int Year { get; set; }
        public WorldTile[,] Tiles { get; set; }
        public List<Civilization> Civs { get; set; }
        public List<HistoricalFigure> Figures { get; set; }
        public List<Site> Sites { get; set; }
        public List<ItemTemplate> Items { get; set; }
        public List<Ruleset> Rules { get; set; }

        public void PopulateValues(List<HistoricalFigure> figures,
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