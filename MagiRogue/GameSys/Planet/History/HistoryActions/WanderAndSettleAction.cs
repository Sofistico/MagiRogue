﻿using MagiRogue.Data;
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
    internal sealed class WanderAndSettleAction : IHistoryAct
    {
        public bool? Act(HistoricalFigure figure)
        {
            return WanderAndSettleSomewhere(figure);
        }

        private static bool WanderAndSettleSomewhere(HistoricalFigure figure)
        {
            // if figure is a noble somewhere, it shouldn't wander!
            if (figure.NobleTitles.Count > 0)
                return false;
            // one in 5 chance to settle somewhere else
            bool changedCiv = false;
            // one in 10 to migrate to another civ
            if (Mrn.OneIn(10))
            {
                changedCiv = Find.ChangeFigureCiv(figure);
            }
            int? civId = figure.GetMemberCivId();
            if (!civId.HasValue)
                return false;
            Civilization currentCiv = Find.Civs.Find(i => i.Id == civId);
            if (currentCiv?.Sites.Count > 1 || changedCiv)
            {
                var siteId = figure.GetCurrentStayingSiteId(currentCiv.Sites);
                var result = siteId.HasValue ? currentCiv.Sites
                    .Where(i => i.Id != siteId.Value)
                    .ToList()
                    .GetRandomItemFromList() : currentCiv.Sites.GetRandomItemFromList();
                Find.ChangeFigureLivingSite(figure, result);
                Find.ChangeFigureFamilyLivingSite(figure, changedCiv, result);
                return true;
            }
            return false;
        }
    }
}