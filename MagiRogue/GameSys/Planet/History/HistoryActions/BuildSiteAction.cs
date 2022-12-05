using MagiRogue.Data;
using MagiRogue.Data.Enumerators;
using MagiRogue.GameSys.Civ;
using MagiRogue.GameSys.Planet.TechRes;
using MagiRogue.GameSys.Tiles;
using MagiRogue.Utils;
using MagiRogue.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.GameSys.Planet.History.HistoryActions
{
    public class BuildSiteAction : IHistoryAct
    {
        private readonly SiteType site;

        public BuildSiteAction(SiteType siteType)
        {
            this.site = siteType;
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
                goodLocation = rngTile.SiteInfluence is null && !rngTile.Collidable;
            }

            Site site = new Site(rngTile.Position, $"Tower of {figure.Name}",
                new Population(popNmbr, figure.Body.Anatomy.Race))
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
            rngTile.SiteInfluence = site;
            Find.Sites.Add(site);
            if (!string.IsNullOrEmpty(whyFigure))
                figure.AddLegend(whyFigure, Find.Year);
            if (!string.IsNullOrEmpty(whySite))
                site.AddLegend(whySite, Find.Year);
        }
    }
}
