using Arquimedes.Enumerators;
using MagusEngine.Core.Civ;
using MagusEngine.Core.Entities.Base;
using MagusEngine.Core.MapStuff;
using MagusEngine.Core.WorldStuff.History;
using MagusEngine.ECS.Components.TilesComponents;
using MagusEngine.Serialization.EntitySerialization;
using MagusEngine.Systems.Time;
using MagusEngine.Utils.Extensions;
using System.Collections.Generic;

namespace MagusEngine.Systems
{
    public static class Find
    {
        private static AccumulatedHistory? history;

        // big ass singleton for ease of finding information!
        public static WorldTile[,]? Tiles { get; private set; }
        public static List<Ruleset>? Rules { get; private set; }
        public static int Year => history!.Year;
        public static List<Civilization>? Civs => history?.Civs;
        public static List<HistoricalFigure>? Figures => history?.Figures;
        public static List<Site>? Sites => history?.AllSites;
        public static List<ItemTemplate>? Items => history?.ImportantItems;
        public static string PlayerDeathReason { get; set; } = "undefined!";
        public static Universe Universe { get; set; } = null!;
        public static MagiMap? CurrentMap => Universe?.CurrentMap;
        public static MagiEntity? ControlledEntity => CurrentMap?.ControlledEntitiy;
        public static TimeSystem Time => Universe.Time;

        public static void PopulateValues(AccumulatedHistory h, WorldTile[,] tiles)
        {
            history = h;
            Tiles = tiles;
            Rules ??= new(DataManager.ListOfRules);
        }

        public static Site? GetFigureStayingSiteIfAny(HistoricalFigure hf)
        {
            var currentSite = hf.GetCurrentStayingSiteId(Sites);
            return Sites?.Find(i => i.Id == currentSite);
        }

        public static List<HistoricalFigure> GetAllFiguresStayingInSiteIfAny(int figureSiteId, List<int>? excludeWhatId = null)
        {
            var list = new List<HistoricalFigure>();
            foreach (var item in Figures!)
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

        public static Site? GetCurrentlyStayingSite(HistoricalFigure figure)
        {
            int? site = figure.GetCurrentStayingSiteId(Sites);
            return Sites?.Find(i => i.Id == site);
        }

        public static bool ChangeFigureCiv(HistoricalFigure figure)
        {
            CivRelation? prevRelation = figure?.RelatedCivs?.Find(i => i.GetIfMember());
            var civ = Civs!.GetRandomItemFromList();
            if (civ is null)
                return false;

            HistoricalFigure.RemovePreviousCivRelationAndSetNew(prevRelation, RelationType.ExMember);
            figure?.AddNewRelationToCiv(civ.Id, RelationType.Member);
            figure?.AddLegend($"the {figure.Name} has migrated to the {civ.Name}", Year);
            civ?.AddLegend($"{figure?.Name} joined as a member of {civ.Name}", Year);
            return true;
        }

        public static void ChangeFigureFamilyLivingSite(HistoricalFigure figureToSearch,
            bool changedCiv, Site result)
        {
            foreach (FamilyNode family in figureToSearch?.FamilyLink?.Nodes!)
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

        public static Civilization? GetCivFromSite(Site site)
        {
            return Civs?.Find(i => i.Id == site?.CivOwnerIfAny);
        }

        public static HistoricalFigure? GetFigureById(int id)
        {
            return Figures?.Find(i => i.Id == id);
        }
    }
}
