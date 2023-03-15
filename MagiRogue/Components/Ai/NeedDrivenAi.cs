using GoRogue.Components.ParentAware;
using GoRogue.Pathing;
using MagiRogue.Commands;
using MagiRogue.Entities;
using MagiRogue.GameSys;
using MagiRogue.UI.Windows;
using System.Linq;
using MagiRogue.Data.Enumerators;

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
                        case Actions.Eat:
                            commitedToNeed ??= ActionManager.FindFood(actor, map);
                            if (commitedToNeed is not null)
                            {
                                var obj = commitedToNeed.Objective;
                                if (map.DistanceMeasurement.Calculate(actor.Position - obj.Position) <= 1) // right next to the water tile or in it
                                {
                                    ActionManager.Eat(actor, obj.MaterialOfTile, 25, need);
                                }
                                else
                                {
                                    previousKnowPath = map.AStar.ShortestPath(actor.Position, water.Position)!;
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
                    }
                }

                return (true, 100);
            }
            return (false, -1);
        }

        private void ClearCommit() => commitedToNeed = null;
    }
}
