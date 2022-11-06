using MagiRogue.Data.Enumerators;
using MagiRogue.GameSys.Planet.History.HistoryActions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.GameSys.Planet.History
{
    public class Ruleset
    {
        public RuleFor RuleFor { get; set; }
        public List<Trigger> Triggers { get; set; }
        public bool AllowMoreThanOneAction { get; set; }

        public void DoAction(HistoricalFigure figure)
        {
            throw new NotImplementedException();
        }
    }

    public class Trigger
    {
        public TriggerType TriggerType { get; set; }
        public object Values { get; set; }
        public ComparatorEnum Comparator { get; set; }

        public bool CheckIfFulfilled(HistoricalFigure figure)
        {
            bool fulfilled = false;

            if (Values != null)
            {
                fulfilled = TriggerType switch
                {
                    TriggerType.Personality => PersonalityLogic(figure),
                    TriggerType.Flag => FlagLogic(figure),
                    TriggerType.Rng => RngLogic(figure),
                    TriggerType.OneIn => OneInLogic(figure),
                    _ => false,
                };
            }

            return fulfilled;
        }

        private bool RngLogic(HistoricalFigure figure)
        {
            throw new NotImplementedException();
        }

        private bool OneInLogic(HistoricalFigure figure)
        {
            throw new NotImplementedException();
        }

        private bool FlagLogic(HistoricalFigure figure)
        {
            throw new NotImplementedException();
        }

        private bool PersonalityLogic(HistoricalFigure figure)
        {
            throw new NotImplementedException();
        }
    }
}
