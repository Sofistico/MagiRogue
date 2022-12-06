using MagiRogue.Data.Enumerators;
using MagiRogue.Data.Serialization.EntitySerialization;
using MagiRogue.Entities;
using MagiRogue.GameSys.Civ;
using MagiRogue.GameSys.Planet.History;
using MagiRogue.GameSys.Tiles;
using MagiRogue.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public static Site GetFigureStayingSiteIfAny(HistoricalFigure hf)
        {
            var currentSite = hf.GetCurrentStayingSiteId(Sites);
            return currentSite.HasValue ? Sites.Find(i => i.Id == currentSite.Value) : null;
        }

        public static List<HistoricalFigure> GetAllFiguresStayingInSiteIfAny(int figureSiteId)
        {
            var list = new List<HistoricalFigure>();
            foreach (var item in Figures)
            {
                if (GetFigureIsStayingOnSiteId(figureSiteId, item))
                {
                    list.Add(item);
                }
            }
            return list;
        }

        public static bool GetFigureIsStayingOnSiteId(int siteId, HistoricalFigure hf)
        {
            var currentSiteId = hf.GetCurrentStayingSiteId(Sites);
            return currentSiteId.HasValue && siteId == currentSiteId.Value;
        }

        public static Site GetCurrentlyStayingSite(HistoricalFigure figure)
        {
            int? site = figure.GetCurrentStayingSiteId(Sites);
            if (site.HasValue)
            {
                return Sites.Find(i => i.Id == site.Value);
            }
            return null;
        }

        public static bool ChangeFigureCiv(HistoricalFigure figure)
        {
            CivRelation prevRelation = figure.RelatedCivs.Find(i => i.GetIfMember());
            HistoricalFigure.RemovePreviousCivRelationAndSetNew(prevRelation, RelationType.ExMember);
            var civ = Civs.GetRandomItemFromList();
            if (civ is null)
                return false;
            figure.AddNewRelationToCiv(civ.Id, RelationType.Member);
            figure.AddLegend($"the {figure.Name} has migrated to the {civ.Name}", Year);
            civ.AddLegend($"{figure.Name} joined as a member of {civ.Name}", Year);
            return true;
        }

        public static void ChangeFigureFamilyLivingSite(HistoricalFigure figureToSearch,
            bool changedCiv, Site result)
        {
            foreach (FamilyNode family in figureToSearch.FamilyLink.Nodes)
            {
                if (family.IsCloseFamily())
                {
                    var fig = family.Figure;
                    ChangeFigureLivingSite(fig, result);
                    if (changedCiv)
                        ChangeFigureCiv(fig);
                }
            }
        }

        public static void ChangeFigureLivingSite(HistoricalFigure figure, Site result)
        {
            figure.ChangeLivingSite(result.Id);
            string changedLoc = Legend.StringFromChangingSiteLoc(figure, result);
            figure.ChangeStayingSite(result.WorldPos);
            figure.AddLegend(changedLoc, Year);
            result.AddLegend(changedLoc, Year);
        }

        public static Civilization GetCivFromSite(Site site)
        {
            return site is null ? Civs.Find(i => i.Id == site.CivOwnerIfAny.Value) : null;
        }
    }
}