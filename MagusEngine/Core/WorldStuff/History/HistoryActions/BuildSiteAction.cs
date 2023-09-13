using Arquimedes.Enumerators;
using MagusEngine.Core.Civ;
using MagusEngine.Core.WorldStuff.TechRes;
using MagusEngine.ECS.Components.TilesComponents;
using MagusEngine.Systems;
using MagusEngine.Utils;
using MagusEngine.Utils.Extensions;
using System.Collections.Generic;

namespace MagusEngine.Core.WorldStuff.History.HistoryActions
{
    public sealed class BuildSiteAction : IHistoryAct
    {
        private readonly SiteType site;

        public BuildSiteAction(SiteType siteType)
        {
            site = siteType;
        }

        public bool? Act(HistoricalFigure figure)
        {
            switch (site)
            {
                case SiteType.City:
                    break;

                case SiteType.Dungeon:
                    break;

                case SiteType.Camp:
                    break;

                case SiteType.Tower:
                    return BuildATower(figure);

                default:
                    return false;
            }

            return false;
        }

        private static bool? BuildATower(HistoricalFigure figure)
        {
            int pop = Mrn.Normal2D6Dice;
            WorldTile rngTile = new WorldTile();
            var site = FigureCreatesNewSite(figure, pop, SiteType.Tower, ref rngTile);
            if (site is null)
            {
                return false;
            }
            figure.AddNewFlag(SpecialFlag.BuiltTower);
            Find.ChangeFigureLivingSite(figure, site);
            Find.ChangeFigureFamilyLivingSite(figure, false, site);
            return true;
        }

        private static Site FigureCreatesNewSite(HistoricalFigure figure, int popNmbr, SiteType siteType, ref WorldTile rngTile)
        {
            bool goodLocation = true;

            while (goodLocation)
            {
                rngTile = Find.Tiles.Transform2DTo1D().GetRandomItemFromList();
                goodLocation = rngTile.ParentTile.GetComponent<SiteTile>().SiteInfluence is null && !rngTile.Collidable;
            }

            Site site = new Site(rngTile.Position, $"Tower of {figure.Name}",
                new Population(popNmbr, figure.GetRaceId()))
            {
                SiteLeader = figure,
                SiteType = siteType,
                DiscoveriesKnow = new List<Discovery>(figure.DiscoveriesKnow),
            };

            OnNewSiteCreated(rngTile,
                site,
                figure,
                $"the {figure.Name} created the {site.Name}, leaving {figure.PronoumPossesive()} previous home to live on it and continue it's research!",
                $"the {site.Name} was created by {figure.Name} for the continuation of {figure.PronoumPossesive()} research!");
            return site;
        }

        private static void OnNewSiteCreated(WorldTile rngTile, Site site, HistoricalFigure figure,
            string whyFigure = "", string whySite = "")
        {
            rngTile.ParentTile.AddComponent<SiteTile>(new(site));
            Find.Sites.Add(site);
            if (!string.IsNullOrEmpty(whyFigure))
                figure.AddLegend(whyFigure, Find.Year);
            if (!string.IsNullOrEmpty(whySite))
                site.AddLegend(whySite, Find.Year);
        }
    }
}
