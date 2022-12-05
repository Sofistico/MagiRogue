using MagiRogue.Data;
using MagiRogue.GameSys.Civ;
using MagiRogue.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.GameSys.Planet.History.HistoryActions
{
    internal class FriendshipIsMagicAction : IHistoryAct
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
                var peopleInside = Find.GetAllFiguresStayingInSiteIfAny(site.Id);
                if (peopleInside.Count >= 0)
                {
                    var randomPerson = peopleInside.GetRandomItemFromList();
                    if (!figure.MakeFriend(randomPerson))
                    {
                        randomPerson.MakeFriend(figure);
                        //figure.AddLegend(ReturnMadeFriendString(figure, randomPerson), year);
                        //randomPerson.AddLegend(ReturnMadeFriendString(randomPerson, figure), year);
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
