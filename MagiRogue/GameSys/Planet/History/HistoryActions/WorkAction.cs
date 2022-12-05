using MagiRogue.Data;
using MagiRogue.Data.Enumerators;
using MagiRogue.GameSys.Civ;
using MagiRogue.Utils;
using MagiRogue.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.GameSys.Planet.History.HistoryActions
{
    internal class WorkAction : IHistoryAct
    {
        private readonly WorkType work;

        public WorkAction(WorkType workType)
        {
            this.work = workType;
        }

        public bool? Act(HistoricalFigure figure)
        {
            switch (work)
            {
                case WorkType.None:
                    return false;

                case WorkType.Research:
                    return ResearchActiviy(figure);

                default:
                    return false;
            }
        }

        private static bool? ResearchActiviy(HistoricalFigure figure)
        {
            if (DecideWhatToResearch(figure))
            {
                DoResearchIfPossible(figure);
                if (figure.AnxiousInRegardsToActivity)
                {
                    DoSometingReckless(figure);
                }
            }
            return true;
        }

        private static void DoResearchIfPossible(HistoricalFigure figure)
        {
            double resarchPower = 0;
            Site site = Find.GetFigureStayingSiteIfAny(figure);

            if (figure.ResearchTree.CurrentResearchFocus is not null)
            {
                resarchPower += Mrn.Exploding2D6Dice;
                if (site is not null)
                {
                    double modifier = (double)((double)site.MundaneResources + 1) / 100;
                    modifier = modifier <= 0 ? 1 : modifier;
                    if (site.SiteType is SiteType.Tower)
                        modifier *= 2; // research is doubly effective on a tower!

                    resarchPower *= modifier;
                    resarchPower = MathMagi.Round(resarchPower);
                }
                if (figure.DoResearch(resarchPower))
                {
                    figure.CleanupResearch(site, Find.Year);
                }
            }
        }

        private static bool DecideWhatToResearch(HistoricalFigure figure)
        {
            bool canMagicalResearch = figure.SpecialFlags.Contains(SpecialFlag.MagicUser);

            if (figure.ResearchTree is null)
                figure.SetupResearchTree(canMagicalResearch);

            if (figure.ResearchTree.CurrentResearchFocus is not null)
                return false;

            figure.ResearchTree.GetNodeForResearch(figure);
            return true;
        }

        private static void DoSometingReckless(HistoricalFigure figure)
        {
            Site site = Find.GetFigureStayingSiteIfAny(figure);
            if (site is not null && figure.CheckForAnger())
            {
                // murder someone! rob them! do something bad!
                bool checkMurder = figure.GetPersonality().Anger >= 30;
                if (checkMurder)
                {
                    Civilization civ = Find.GetCivFromSite(site);
                    HistoricalFigure deadThing =
                        civ.ImportantPeople
                        .Where(i => i.GetCurrentStayingSiteId(civ.Sites) == site.Id)
                        .ToList()
                        .GetRandomItemFromList();
                    figure.TryAttackAndMurder(deadThing, Find.Year, $", so that {figure.Pronoum} could get inspiration!");
                    figure.ClearAnxiousness();
                }
            }
        }
    }
}
