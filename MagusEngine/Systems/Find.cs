using Arquimedes.Data.Serialization.EntitySerialization;
using MagiRogue.Data.Enumerators;
using MagiRogue.GameSys.Tiles;
using MagiRogue.Utils.Extensions;
using MagusEngine.Core.Civ;
using MagusEngine.Core.WorldStuff.History;
using System.Collections.Generic;

namespace MagusEngine.Systems
{
    public static class Find
    {
        private static AccumulatedHistory history;

        // big ass singleton for ease of finding information!
        public static int Year { get => history.Year; }
        public static WorldTile[,] Tiles { get; private set; }
        public static List<Civilization> Civs { get => history.Civs; }
        public static List<HistoricalFigure> Figures { get => history.Figures; }
        public static List<Site> Sites { get => history.AllSites; }
        public static List<ItemTemplate> Items { get => history.ImportantItems; }
        public static List<Ruleset> Rules { get; private set; }
        public static string PlayerDeathReason { get; set; } = "undefined!";

        public static void PopulateValues(AccumulatedHistory h, WorldTile[,] tiles)
        {
            history = h;
            //Figures = figures;
            //Civs = civs;
            //Sites = allSites;
            //Items = importantItems;
            Tiles = tiles;
            Rules ??= new(DataManager.ListOfRules);
        }

        public static Site GetFigureStayingSiteIfAny(HistoricalFigure hf)
        {
            var currentSite = hf.GetCurrentStayingSiteId(Sites);
            return currentSite.HasValue ? Sites.Find(i => i.Id == currentSite.Value) : null;
        }

        public static List<HistoricalFigure> GetAllFiguresStayingInSiteIfAny(int figureSiteId, List<int>? excludeWhatId = null)
        {
            var list = new List<HistoricalFigure>();
            foreach (var item in Figures)
            {
                if (GetFigureIsStayingOnSiteId(figureSiteId, item) && excludeWhatId?.Contains(item.Id) == false)
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
            var civ = Civs.GetRandomItemFromList();
            if (civ is null)
                return false;

            HistoricalFigure.RemovePreviousCivRelationAndSetNew(prevRelation, RelationType.ExMember);
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
            return site is null ? Civs.Find(i => i.Id == site?.CivOwnerIfAny.Value!) : null;
        }

        public static HistoricalFigure GetFigureById(int id)
        {
            return Figures.Find(i => i.Id == id);
        }
    }
}