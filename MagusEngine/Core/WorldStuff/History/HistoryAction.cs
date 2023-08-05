using MagiRogue.Data;
using MagiRogue.Data.Enumerators;
using MagiRogue.GameSys.Planet.TechRes;
using MagiRogue.GameSys.Civ;
using MagiRogue.GameSys.Tiles;
using MagiRogue.Utils;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Text;
using MagiRogue.Entities;
using SadConsole.Renderers;
using System.Data;
using MagiRogue.Utils.Extensions;
using System.Threading.Tasks;

namespace MagusEngine.Core.WorldStuff.History
{
    public sealed class HistoryAction
    {
        #region Actions

        public static void Act(HistoricalFigure figure)
        {
            var rules = Find.Rules;
            var fulfilledRules = rules.AllFulfilled(figure); // some list for every fulfilled rule
            fulfilledRules.ShuffleAlgorithm();
            var moreThanOneActRules = fulfilledRules.FindAll(i => i.AllowMoreThanOneAction);
            var onlyOneAct = fulfilledRules.FindAll(i => !i.AllowMoreThanOneAction);

            Parallel.ForEach(moreThanOneActRules, rule =>
            {
                var act = rule.DoAction(figure);
                if (act is null)
                    GameLoop.WriteToLog($"For some reason the action was null, here is the action: {rule.RuleFor}");
            });

            bool acted = false;
            foreach (var rule in onlyOneAct)
            {
                if (!acted)
                {
                    var act = rule.DoAction(figure);
                    if (act is null)
                        GameLoop.WriteToLog($"For some reason the action was null, here is the action: {rule.RuleFor}");
                    if (act.GetValueOrDefault())
                        acted = true;
                }
                else
                {
                    break;
                }
            }
#if DEBUG
            if (!acted)
                figure.DebugNumberOfLostYears++;
#endif
        }

        #endregion Actions
    }
}