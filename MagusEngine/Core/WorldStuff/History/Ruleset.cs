using Arquimedes.Enumerators;
using MagusEngine.Core.WorldStuff.History.HistoryActions;
using MagusEngine.Services;
using MagusEngine.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MagusEngine.Core.WorldStuff.History
{
    public class Ruleset
    {
        private IHistoryAct act;

        public RuleFor RuleFor { get; set; }
        public List<Trigger> Triggers { get; set; }
        public bool AllowMoreThanOneAction { get; set; }

        [JsonConstructor]
        public Ruleset(RuleFor ruleFor, List<Trigger> triggers, bool allowMoreThanOneAct = false)
        {
            RuleFor = ruleFor;
            Triggers = triggers;
            AllowMoreThanOneAction = allowMoreThanOneAct;
            DetermineWhichActionItIs();
        }

        private void DetermineWhichActionItIs()
        {
            switch (RuleFor)
            {
                case RuleFor.Null:
                    Locator.GetService<MagiLog>().Log($"The {RuleFor} is not supported");
                    throw new ApplicationException($"The rule isn't supported! {Triggers.First()}");

                case RuleFor.Marriage:
                    act = new MarryAction();
                    break;

                case RuleFor.HaveChild:
                    act = new HaveChildAction();
                    break;

                case RuleFor.TrainAbility:
                    act = new TrainAbilityAction();
                    break;

                case RuleFor.GenerateMagicalResources:
                    act = new GenerateResourcesAction(ResourceType.Magical);
                    break;

                case RuleFor.GetAFriend:
                    act = new FriendshipIsMagicAction();
                    break;

                case RuleFor.LearnDiscoveriesKnowToSite:
                    act = new LearnNewDiscoveriesAction();
                    break;

                case RuleFor.WanderAndSettle:
                    act = new WanderAndSettleAction();
                    break;

                case RuleFor.ResearchWork:
                    act = new WorkAction(WorkType.Research);
                    break;

                case RuleFor.BuildATower:
                    act = new BuildSiteAction(SiteType.Tower);
                    break;

                case RuleFor.BuildACity:
                    act = new BuildSiteAction(SiteType.City);
                    break;

                case RuleFor.CreateNewCiv:
                    act = new CreateNewCivAction();
                    break;

                case RuleFor.MythGenericStuff:
                    act = new GenericDeityStuff();
                    break;

                default:
                    Locator.GetService<MagiLog>().Log($"The {RuleFor} is not supported!");
                    break;
            }
        }

        public bool? DoAction(HistoricalFigure figure)
        {
            return act?.Act(figure);
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
            var valueProperties = jValue.ToObject<Dictionary<string, object>>();
            bool result = false;

            // Values is a json object
            foreach (var prop in valueProperties)
            {
                if (prop.Key.Equals("Or"))
                {
                    var obj = (JObject)prop.Value;
                    var valProps = obj.ToObject<Dictionary<string, int>>();
                    result = CompareIntOrValue(valProps);
                }
                else
                {
                    if (personalityProps.TryGetValue(
                        prop.Key,
                        out int personality))
                    {
                        int value = Convert.ToInt32((int)(long)prop.Value);
                        result = CompareIntValue(
                            personality,
                            value);
                    }
                }
            }

            return result;
        }

        private bool CompareIntOrValue(Dictionary<string, int> values)
        {
            bool result = false;
            bool any = false;
            foreach (var item in values)
            {
                if (!any)
                {
                    if (values.TryGetValue(
                        item.Key,
                        out int personality))
                    {
                        int value = item.Value;
                        any = CompareIntValue(
                            personality,
                            value);
                        break;
                    }
                }
            }
            if (any)
                return result;

            return result;
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

        public override string ToString()
        {
            return $"Trigger - {TriggerType} | {Values} | {Comparator}";
        }
    }
}
