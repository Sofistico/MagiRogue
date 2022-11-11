using GoRogue;
using MagiRogue.Data.Enumerators;
using MagiRogue.Entities;
using MagiRogue.GameSys.Planet.History.HistoryActions;
using MagiRogue.Utils;
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
            switch (RuleFor)
            {
                case RuleFor.Marriage:
                    break;

                case RuleFor.HaveChild:
                    break;

                default:
                    GameLoop.WriteToLog($"The {RuleFor} is not supported!");
                    break;
            }
        }
    }

    public class Trigger
    {
        public TriggerType TriggerType { get; set; }

        // its a JObject!
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
                    TriggerType.DiceRng => DiceRngLogic(),
                    TriggerType.OneIn => OneInLogic(),
                    _ => false,
                };
            }

            return fulfilled;
        }

        private bool DiceRngLogic()
        {
            // parser
            //var str = Values.ToString();
            //GoRogue.DiceNotation.Dice.R
            throw new NotImplementedException();
        }

        private bool OneInLogic()
        {
            string str = Values.ToString();
            if (int.TryParse(str, out int oneIn))
            {
                return Mrn.OneIn(oneIn);
            }
            return false;
        }

        private bool FlagLogic(HistoricalFigure figure)
        {
            // do the case where there are more than one flag to check for!
            string str = Values.ToString();

            if (Enum.TryParse(str, out SpecialFlag result))
            {
                return CompareFlagValue(figure.SpecialFlags, result);
            }
            return false;
        }

        private bool CompareFlagValue(List<SpecialFlag> listToCheck, SpecialFlag toCompare)
        {
            return Comparator switch
            {
                ComparatorEnum.NotEqual => !listToCheck.Contains(toCompare),
                ComparatorEnum.Equal => listToCheck.Contains(toCompare),
                _ => false,
            };
        }

        private bool PersonalityLogic(HistoricalFigure figure)
        {
            var personalityProps = figure.GetPersonality().ReturnAsDictionary();
            var jValue = (JObject)Values;
            var valueProperties = jValue.ToObject<Dictionary<string, int>>();

            // Values is a json object
            foreach (var prop in valueProperties)
            {
                if (personalityProps.TryGetValue(
                    prop.Key, out int personality))
                {
                    int value = prop.Value;
                    return CompareIntValue(
                        personality,
                        value);
                }
            }

            return false;
        }

        private bool CompareIntValue(int compared, int toCompare)
        {
            return Comparator switch
            {
                ComparatorEnum.NotEqual => compared != toCompare,
                ComparatorEnum.Equal => compared == toCompare,
                ComparatorEnum.EqualOrMore => compared >= toCompare,
                ComparatorEnum.More => compared > toCompare,
                ComparatorEnum.Less => compared < toCompare,
                ComparatorEnum.LessOrEqual => compared <= toCompare,
                _ => false,
            };
        }
    }
}
