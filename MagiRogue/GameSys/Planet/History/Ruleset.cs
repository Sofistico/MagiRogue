using GoRogue;
using MagiRogue.Data.Enumerators;
using MagiRogue.Entities;
using MagiRogue.GameSys.Planet.History.HistoryActions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        private PropertyInfo[] valueProperties;
        private Type valueType;

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
            // do the case where there are more than one flag to check for!
            string str = Values.ToString();

            if (Enum.TryParse(str, out SpecialFlag result))
            {
                return figure.SpecialFlags.Contains(result);
            }
            return false;
        }

        private bool PersonalityLogic(HistoricalFigure figure)
        {
            valueType ??= Values.GetType();
            valueProperties ??= valueType.GetProperties();
            var personalityProps = figure.GetPersonality().ReturnAsDictionary();

            foreach (var prop in valueProperties)
            {
                if (personalityProps.TryGetValue(
                    prop.Name, out int personality))
                {
                    int value = (int)prop.GetValue(prop);
                    return CompareIntValue(
                        personality,
                        value);
                }
            }

            return false;
        }

        private bool CompareIntValue(int compared, int toCompare)
        {
            switch (Comparator)
            {
                case ComparatorEnum.NotEqual:
                    return compared != toCompare;

                case ComparatorEnum.Equal:
                    return compared == toCompare;

                case ComparatorEnum.EqualOrMore:
                    return compared >= toCompare;

                case ComparatorEnum.More:
                    return compared > toCompare;

                case ComparatorEnum.Less:
                    return compared < toCompare;

                case ComparatorEnum.LessOrEqual:
                    return compared <= toCompare;

                default:
                    return false;
            }
        }
    }
}
