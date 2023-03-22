using GoRogue.Components.ParentAware;
using GoRogue.Pathing;
using MagiRogue.Commands;
using MagiRogue.Data.Enumerators;
using MagiRogue.Entities;
using MagiRogue.UI.Windows;
using MagiRogue.Utils.Extensions;
using System.Linq;
using Map = MagiRogue.GameSys.Map;

namespace MagiRogue.Components.Ai
{
    // refactor the shit out of this class when i'm done with it!
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

                // test code!!
                //if (previousKnowPath is not null && step < previousKnowPath.Length)
                //{
                //    MoveActorAStep(actor);
                //}
                //else
                //{
                //    previousKnowPath = ActionManager.FindFleeAction(map, actor, actor);
                //    step = 0;
                //}
                //return (true, 100);
                if (needs is null)
                    return (false, -1);
                if (!needs.GetPriority(out Need need) && commitedToNeed is null)
                    need = needs.FirstOrDefault(i => i.PercentFulfilled <= 25);
                if (ActionManager.SearchForDangerAction(actor, map, out var danger))
                {
                    previousKnowPath = ActionManager.FindFleeAction(map, actor, danger);
                }
                if (commitedToNeed is not null)
                    need = commitedToNeed;
                if (previousKnowPath is not null && step < previousKnowPath.Length)
                {
                    MoveActorAStep(actor);
                }
                else
                {
                    step = 0;
                    previousKnowPath = null;
                    switch (need?.ActionToFulfillNeed)
                    {
                        case Actions.Eat:
                            commitedToNeed ??= ActionManager.FindFood(actor, map);
                            if (commitedToNeed is not null)
                            {
                                var obj = commitedToNeed.Objective;
                                if (map.DistanceMeasurement.Calculate(actor.Position - obj.Position) <= 1) // right next to the water tile or in it
                                {
                                    ActionManager.Eat(actor, obj, need);
                                    ClearCommit();
                                }
                                else
                                {
                                    previousKnowPath = map.AStar.ShortestPath(actor.Position, obj.Position)!;
                                    ActionManager.MoveActorBy(actor, previousKnowPath.GetStep(step++) - actor.Position);
                                }
                            }
                            break;

                        case Actions.Sleep:
                            commitedToNeed = ActionManager.Sleep(actor, need);
                            if (commitedToNeed?.TurnCounter == 0)
                            {
                                ClearCommit();
                            }

                            break;

                        case Actions.Drink:
                            var water = map.GetClosestWaterTile(actor.Body.ViewRadius, actor.Position);
                            if (water is null)
                                ActionManager.Wander(actor);
                            if (!actor.CanSee(water.Position))
                                break;
                            if (map.DistanceMeasurement.Calculate(actor.Position - water.Position) <= 1) // right next to the water tile or in it
                            {
                                ActionManager.Drink(actor, water.MaterialOfTile, 25, need);
                            }
                            else
                            {
                                previousKnowPath = map.AStar.ShortestPath(actor.Position, water.Position)!;
                            }
                            break;

                        case Actions.Fun:
                            break;

                        case Actions.Train:
                            break;

                        case Actions.Pray:
                            break;

                        case Actions.Study:
                            break;

                        case Actions.Teach:
                            break;

                        case Actions.Craft:
                            break;

                        case Actions.Fight:
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

                        case Actions.Bully:
                            break;

                        case Actions.PickUp:
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

                        case Actions.Wander:
                            ActionManager.Wander(actor);
#if DEBUG
                            GameLoop.AddMessageLog($"{actor.Name} wanders");
#endif
                            need?.Fulfill();
                            break;

                        case Actions.Flee:
                            break;
                    }
                }

                return (true, 100);
            }
            return (false, -1);
        }

        private void MoveActorAStep(Actor actor)
        {
            ActionManager.MoveActorBy(actor, actor.Position - previousKnowPath.GetStep(step++));
        }

        private void ClearCommit() => commitedToNeed = null;
    }
}
