using GoRogue.Components.ParentAware;
using GoRogue.GameFramework;
using GoRogue.Pathing;
using MagiRogue.Commands;
using MagiRogue.Data.Enumerators;
using MagiRogue.Entities;
using MagiRogue.GameSys.Time;
using MagiRogue.UI.Windows;
using SadRogue.Primitives;
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
        private IGameObject? parent;
        private NeedCollection? needs;

        public IObjectWithComponents? Parent { get; set; }

        public (bool sucess, long ticks) RunAi(Map map, MessageLogWindow messageLog)
        {
            parent ??= (IGameObject)Parent;
            if (Parent.GoRogueComponents.Contains(typeof(NeedCollection))
                && map.GetEntityById(parent.ID) is Actor actor)
            {
                needs ??= actor.GetComponent<NeedCollection>();

                int timeTakenAction = 0;
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
                    timeTakenAction = MoveActorAStep(actor, map);
                }
                else
                {
                    step = 0;
                    previousKnowPath = null;
                    switch (need?.ActionToFulfillNeed)
                    {
                        case Actions.Eat:
                            commitedToNeed ??= ActionManager.FindFood(actor, map);
                            if (commitedToNeed?.Objective is not null)
                            {
                                var obj = commitedToNeed.Objective;
                                if (map.DistanceMeasurement.Calculate(actor.Position - obj.Position) <= 1) // right next to the water tile or in it
                                {
                                    timeTakenAction = ActionManager.Eat(actor, obj, need);
                                    ClearCommit();
                                }
                                else
                                {
                                    timeTakenAction = FindPathAndMoveOneStep(map, actor, obj);
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
                            {
                                timeTakenAction = ActionManager.Wander(actor);
                                break;
                            }
                            if (!actor.CanSee(water.Position))
                                break;
                            if (map.DistanceMeasurement.Calculate(actor.Position - water.Position) <= 1) // right next to the water tile or in it
                            {
                                timeTakenAction = ActionManager.Drink(actor, water.MaterialOfTile, 25, need);
                            }
                            else
                            {
                                timeTakenAction = FindPathAndMoveOneStep(map, actor, water);
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
                                timeTakenAction = ActionManager.MeleeAttack(actor, enemy);
                            }
                            else
                            {
                                timeTakenAction = FindPathAndMoveOneStep(map, actor, enemy);
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
                                timeTakenAction = ActionManager.Eat(actor, item, need);
                            }
                            else
                            {
                                timeTakenAction = FindPathAndMoveOneStep(map, actor, item);
                            }
                            break;

                        case Actions.Wander:
                            timeTakenAction = ActionManager.Wander(actor);
#if DEBUG
                            GameLoop.AddMessageLog($"{actor.Name} wanders");
#endif
                            need?.Fulfill();
                            break;

                        case Actions.Flee:
                            break;
                    }
                }

                return (true, need is null ? 100 : timeTakenAction); // if need is null, then it must check next turn again!
            }
            return (false, -1);
        }

        private int FindPathAndMoveOneStep(Map map, Actor actor, IGameObject item)
        {
            previousKnowPath = map.AStar.ShortestPath(actor.Position, item.Position)!;
            return MoveActorAStep(actor, map);
        }

        private int MoveActorAStep(Actor actor, Map map)
        {
            ActionManager.MoveActorBy(actor, previousKnowPath.GetStep(step++).Subtract(actor.Position));
            return TimeHelper.GetWalkTime(actor, map.GetTileAt(actor.Position));
        }

        private void ClearCommit() => commitedToNeed = null;
    }
}
