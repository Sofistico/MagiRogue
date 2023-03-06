﻿using GoRogue.Components.ParentAware;
using GoRogue.Pathing;
using MagiRogue.Commands;
using MagiRogue.Entities;
using MagiRogue.GameSys;
using MagiRogue.UI.Windows;
using System.Linq;

namespace MagiRogue.Components.Ai
{
    public class NeedDrivenAi : IAiComponent
    {
        private Path? previousKnowPath;
        private Need? commitedToNeed;
        private int step;
        private GoRogue.GameFramework.IGameObject? parent;
        private NeedCollection? needs;

        public IObjectWithComponents? Parent { get; set; }

        public (bool sucess, long ticks) RunAi(Map map, MessageLogWindow messageLog)
        {
            parent ??= (GoRogue.GameFramework.IGameObject)Parent;
            if (Parent.GoRogueComponents.Contains(typeof(NeedCollection))
                && map.GetEntityById(parent.ID) is Actor actor)
            {
                needs ??= actor.GetComponent<NeedCollection>();

                if (needs is null)
                    return (false, -1);
                if (!needs.GetPriority(out Need need) && commitedToNeed is null)
                    need = needs.FirstOrDefault(i => i.PercentFulfilled <= 25);
                if (commitedToNeed is not null)
                    need = commitedToNeed;
                if (need is null)
                    return (true, 100); //check if there will be a need next turn
                if (previousKnowPath is not null)
                {
                    ActionManager.MoveActorBy(actor, previousKnowPath.GetStep(step++));
                }
                else
                {
                    step = 0;
                    previousKnowPath = null;
                    switch (need.ActionToFulfillNeed)
                    {
                        case Data.Enumerators.Actions.Eat:
                            commitedToNeed = ActionManager.FindFood(actor, map);
                            break;

                        case Data.Enumerators.Actions.Sleep:
                            commitedToNeed = ActionManager.Sleep(actor, need);
                            if (commitedToNeed?.TurnCounter == 0)
                            {
                                ClearCommit();
                            }

                            break;

                        case Data.Enumerators.Actions.Drink:
                            var water = map.GetClosestWaterTile(actor.Body.ViewRadius, actor.Position);
                            if (map.DistanceMeasurement.Calculate(actor.Position - water.Position) <= 1) // right next to the water tile or in it
                            {
                                ActionManager.Drink(actor, water.MaterialOfTile, 25, need);
                            }
                            else
                            {
                                previousKnowPath = map.AStar.ShortestPath(actor.Position, water.Position)!;
                            }
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
                            var enemy = (Actor)need.Objective;
                            if (enemy is null) { break; }
                            if (map.DistanceMeasurement.Calculate(actor.Position - enemy.Position) <= actor.AttackRange())
                            {
                                ActionManager.MeleeAttack(actor, enemy);
                            }
                            else
                            {
                                previousKnowPath = map.AStar.ShortestPath(actor.Position, enemy.Position)!;
                            }
                            break;

                        case Data.Enumerators.Actions.Bully:
                            break;

                        case Data.Enumerators.Actions.PickUp:
                            Item item = (Item)need.Objective;
                            if (item is null) { break; }
                            if (map.DistanceMeasurement.Calculate(actor.Position, item.Position) <= 1)
                            {
                                ActionManager.PickUp(actor, item);
                                ActionManager.Eat(actor, item, need);
                            }
                            else
                            {
                                previousKnowPath = map.AStar.ShortestPath(actor.Position, item.Position)!;
                            }
                            break;
                    }
                }

                return (true, 100);
            }
            return (false, -1);
        }

        private void ClearCommit() => commitedToNeed = null;
    }
}
