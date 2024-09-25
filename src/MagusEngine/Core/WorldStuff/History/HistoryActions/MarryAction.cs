using MagusEngine.Systems;
using MagusEngine.Utils.Extensions;
using MagusEngine.Core.Civ;
using System;
using System.Linq;
using System.Text;

namespace MagusEngine.Core.WorldStuff.History.HistoryActions
{
    public sealed class MarryAction : IHistoryAct
    {
        public MarryAction()
        {
        }

        public bool? Act(HistoricalFigure figure)
        {
            return RomanceSomeoneInsideSameSite(figure);
        }

        private static bool RomanceSomeoneInsideSameSite(HistoricalFigure? figure)
        {
            if (figure is null)
                return false;
            Site site = Find.GetFigureStayingSiteIfAny(figure);
            int year = Find.Year;
            if (site is not null
                && figure?.IsMarried() != true)
            {
                var peopleInside = Find.GetAllFiguresStayingInSiteIfAny(site.Id, new() { figure.Id });
                int aceptableDiferenceAge;
                bool isAdult = figure.IsAdult();
                if (isAdult)
                {
                    aceptableDiferenceAge = Math.Max(figure.Body.Anatomy.GetRaceAdulthoodAge(),
                        figure.Body.GetCurrentAge() / 2);
                }
                else
                {
                    // kids can't marry!
                    aceptableDiferenceAge = int.MaxValue;
                }
                if (aceptableDiferenceAge == int.MaxValue)
                    return false;
                var peopleInARangeOfAgeCloseAndPredispost = peopleInside.Where(i =>
                    i.Body.GetCurrentAge() >= aceptableDiferenceAge && i.CheckForRomantic()).ToList();
                if (peopleInARangeOfAgeCloseAndPredispost.Any())
                {
                    var randomPerson = peopleInARangeOfAgeCloseAndPredispost.GetRandomItemFromList();
                    if (randomPerson == null)
                    {
                        return false;
                    }
                    if (randomPerson?.IsMarried() == true)
                        return false;
                    figure.Marry(randomPerson);
                    StringBuilder anotherB = new StringBuilder($"the {figure.Name} married with {randomPerson.Name}");
                    StringBuilder bb = new StringBuilder($"the {randomPerson.Name} married with {figure.Name}");
                    figure.AddLegend(anotherB.ToString(), year);
                    randomPerson.AddLegend(bb.ToString(), year);

                    return true;
                }
            }
            return false;
        }
    }
}
