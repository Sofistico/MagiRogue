using MagusEngine.Core.Civ;
using MagusEngine.Systems;
using MagusEngine.Utils.Extensions;
using System.Linq;

namespace MagusEngine.Core.WorldStuff.History.HistoryActions
{
    public sealed class FriendshipIsMagicAction : IHistoryAct
    {
        public bool? Act(HistoricalFigure figure)
        {
            return GetANewFriend(figure);
        }

        private static bool GetANewFriend(HistoricalFigure figure)
        {
            Site site = Find.GetFigureStayingSiteIfAny(figure);
            if (site is not null)
            {
                var peopleInside = Find.GetAllFiguresStayingInSiteIfAny(site.Id, new() { figure.Id });
                if (peopleInside.Count > 0)
                {
                    var randomPerson = peopleInside.GetRandomItemFromList();
                    if (!figure.MakeFriend(randomPerson))
                    {
                        randomPerson.MakeFriend(figure);
                        figure.AddLegend(Legend.ReturnMadeFriendString(figure, randomPerson), Find.Year);
                        randomPerson.AddLegend(Legend.ReturnMadeFriendString(randomPerson, figure), Find.Year);
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
