using GoRogue.Components.ParentAware;
using GoRogue.Pathing;
using MagiRogue.Entities;
using MagiRogue.GameSys;
using MagiRogue.GameSys.Veggies;
using MagiRogue.Utils.Extensions;
using MagiRogue.UI.Windows;
using System;
using System.Linq;
using MagiRogue.Data.Enumerators;

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
                        if (foodItem is null)
                            Wander(map, actor);
                        if (foodItem is Actor victim)
                        {
                            commitedToNeed = new Need($"Kill {victim}", false, 0, Data.Enumerators.Actions.Fight, "Peace", "battle")
                            {
                                Objective = actor,
                            };
                        }
                        if (foodItem is Item item)
                        {
                            commitedToNeed = new Need($"Pickup {item.Name}", false, 0, Actions.PickUp, "Greed", $"{item.ItemType}")
                            {
                                Objective = foodItem
                            };
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

                    case Data.Enumerators.Actions.PickUp:
                        break;

                    default:
                        break;
                }

                return (true, 100);
            }
            return (false, -1);
        }

        private void Wander(Map map, Actor actor)
        {
            int tries = 0;
            int maxTries = 10;
            bool tileIsInvalid = true;
            do
            {
                var posToGo = actor.Position.GetPointNextToWithCardinals();
            } while (tileIsInvalid);
        }

        private void ClearCommit() => commitedToNeed = null;
    }
}
