using MagiRogue.Data;
using MagiRogue.GameSys.Civ;
using MagiRogue.GameSys.Tiles;
using MagiRogue.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.GameSys.Planet.History.HistoryActions
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

        private static bool RomanceSomeoneInsideSameSite(HistoricalFigure figure)
        {
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
                    (i.Body.GetCurrentAge() >= aceptableDiferenceAge) && i.CheckForRomantic()).ToList();
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
