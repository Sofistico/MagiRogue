using Arquimedes.Enumerators;
using GoRogue.GameFramework;
using GoRogue.Pathing;
using MagusEngine.Bus.UiBus;
using MagusEngine.Commands;
using MagusEngine.Core.Entities;
using MagusEngine.Services;
using MagusEngine.Systems.Time;
using SadRogue.Primitives;
using System;
using MagiMap = MagusEngine.Core.MapStuff.MagiMap;

namespace MagusEngine.ECS.Components.ActorComponents.Ai
{
    // refactor the shit out of this class when i'm done with it!
    public class NeedDrivenAi : IAiComponent, IDisposable
    {
        private Path? previousKnowPath;
        private Need? commitedToNeed;
        private int step;
        private IGameObject? parent;
        private NeedCollection? needs;

        public object? Parent { get; set; }

        public (bool sucess, long ticks) RunAi(MagiMap map)
        {
            if (Parent is not IGameObject)
            {
                return (false, -1);
            }

            parent ??= (IGameObject)Parent;
            if (parent.GoRogueComponents.Contains(typeof(NeedCollection))
                && map.GetEntityById(parent.ID) is Actor actor)
            {
                needs ??= actor.GetComponent<NeedCollection>();
                if (needs is null)
                    return (false, -1);

                int timeTakenAction = 0;
                needs.GetPriority(out Need? need);
                FindIfDangerExists(map, actor);
                if (commitedToNeed is not null)
                    need = commitedToNeed;
                timeTakenAction = ActOnNeed(map, actor, timeTakenAction, need);

                return (true, need is null ? 100 : timeTakenAction); // if need is null, then it must check next turn again!
            }
            return (false, -1);
        }

        private int ActOnNeed(MagiMap map, Actor actor, int timeTakenAction, Need need)
        {
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
                        timeTakenAction = 100;
                        if (commitedToNeed?.TurnCounter == 0)
                        {
                            ClearCommit();
                        }

                        break;

                    case Actions.Drink:
                        if (ActionManager.FindWater(actor, map, out var water))
                        {
                            if (map.DistanceMeasurement.Calculate(actor.Position - water.Position) <= 1) // right next to the water tile or in it
                            {
                                timeTakenAction = ActionManager.Drink(actor, water.GetMaterial(), 25, need);
                            }
                            else
                            {
                                timeTakenAction = FindPathAndMoveOneStep(map, actor, water);
                            }
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
                        Locator.GetService<MessageBusService>()
                            .SendMessage<AddMessageLog>(new($"{actor.Name} wanders", true));
#endif
                        need?.Fulfill();
                        break;

                    case Actions.Flee:
                        break;
                }
            }

            return timeTakenAction;
        }

        private void FindIfDangerExists(MagiMap map, Actor actor)
        {
            if (ActionManager.SearchForDangerAction(actor, map, out var danger))
            {
                previousKnowPath = ActionManager.FindFleeAction(map, actor, danger);
            }
        }

        private int FindPathAndMoveOneStep(MagiMap map, Actor actor, IGameObject item)
        {
            previousKnowPath = map.AStar.ShortestPath(actor.Position, item.Position)!;
            return MoveActorAStep(actor, map);
        }

        private int MoveActorAStep(Actor actor, MagiMap map)
        {
            ActionManager.MoveActorBy(actor, previousKnowPath.GetStep(step++).Subtract(actor.Position));
            return TimeHelper.GetWalkTime(actor, map.GetTileAt(actor.Position));
        }

        private void ClearCommit() => commitedToNeed = null;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting
        /// unmanaged resources.
        /// </summary>
#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize

        public void Dispose()
#pragma warning restore CA1816 // Dispose methods should call SuppressFinalize
        {
            parent?.GoRogueComponents.Remove(this);
            ClearCommit();
            needs = null;
        }
    }
}
