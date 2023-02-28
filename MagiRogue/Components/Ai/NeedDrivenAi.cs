using GoRogue.Components.ParentAware;
using GoRogue.Pathing;
using MagiRogue.Entities;
using MagiRogue.GameSys;
using MagiRogue.GameSys.Veggies;
using MagiRogue.UI.Windows;
using System.Linq;

namespace MagiRogue.Components.Ai
{
    public class NeedDrivenAi : IAiComponent
    {
        private Path previousKnowPath;
        private Need? commitedToNeed;

        public IObjectWithComponents? Parent { get; set; }

        public (bool sucess, long ticks) RunAi(Map map, MessageLogWindow messageLog)
        {
            if (Parent.GoRogueComponents.Contains(typeof(NeedCollection)) && Parent is Actor actor)
            {
                var needs = actor.GetComponent<NeedCollection>();

                if (needs is null)
                    return (false, -1);
                if (!needs.GetPriority(out Need need) && commitedToNeed is null)
                    need = needs.FirstOrDefault(i => i.PercentFulfilled <= 25);
                if (commitedToNeed is not null)
                    need = commitedToNeed.Value;

                switch (need.ActionToFulfillNeed)
                {
                    case Data.Enumerators.Actions.Eat:
                        var whatToEat = actor.GetAnatomy().WhatToEat();
                        var foodItem = map.FindTypeOfFood(whatToEat, actor.Position);
                        if (foodItem is Actor victim)
                        {
                            var killStuff = new Need($"Kill {victim}", false, 10, Data.Enumerators.Actions.Fight, "Peace", "battle")
                            {
                                Objective = actor,
                            };
                            needs.Add(killStuff);
                            commitedToNeed = need;
                        }
                        if (foodItem is Item item)
                        {
                        }
                        if (foodItem is Plant plant)
                        {
                        }
                        break;

                    case Data.Enumerators.Actions.Sleep:
                        break;

                    case Data.Enumerators.Actions.Drink:
                        break;

                    case Data.Enumerators.Actions.Fun:
                        break;

                    case Data.Enumerators.Actions.Train:
                        break;

                    case Data.Enumerators.Actions.Pray:
                        break;

                    case Data.Enumerators.Actions.Study:
                        break;

                    case Data.Enumerators.Actions.Teach:
                        break;

                    case Data.Enumerators.Actions.Craft:
                        break;

                    case Data.Enumerators.Actions.Fight:
                        break;

                    case Data.Enumerators.Actions.Bully:
                        break;

                    default:
                        break;
                }

                return (true, 100);
            }
            return (false, -1);
        }

        private void ClearCommit() => commitedToNeed = null;
    }
}
