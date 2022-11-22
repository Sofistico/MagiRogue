using GoRogue.Components.ParentAware;
using GoRogue.Pathing;
using MagiRogue.Entities;
using MagiRogue.GameSys;
using MagiRogue.GameSys.Time;
using MagiRogue.UI.Windows;
using SadRogue.Primitives;
using System;
using System.Linq;

namespace MagiRogue.Components
{
    public class MoveAndAttackAI : IAiComponent
    {
        private readonly int perceptionAi;

        public MoveAndAttackAI(int perception)
        {
            perceptionAi = perception;
        }

        public IObjectWithComponents? Parent { get; set; }

        public (bool sucess, long ticks) RunAi(Map map, MessageLogWindow messageLog)
        {
            // TODO: Fix, because it hardly works, at least i saw the actor attacking
            if (Parent is not GoRogue.GameFramework.IGameObject parent)
            {
                return (false, -1);
            }

            Actor actor = (Actor)GameLoop.GetCurrentMap().GetEntityById(parent.ID);

            if (TryGetDirectionMove(map, actor))
            {
                int walkSpeed = TimeHelper.GetWalkTime(actor, actor.Position);
                return (true, walkSpeed);
            }
            else
            {
                return (true, TimeHelper.Wait);
            }
        }

        public bool TryGetDirectionMove(Map map, Actor actor)
        {
            Path shortPath = map.AStar.ShortestPath(actor.Position, GameLoop.Universe.Player.Position);

            GoRogue.GameFramework.IGameObject iGame = (GoRogue.GameFramework.IGameObject)Parent;
            var parent = GameLoop.GetCurrentMap().GetEntityById(iGame.ID);

            Direction direction;

            if (shortPath == null || shortPath.Length > perceptionAi)
            {
                return false;
            }
            else
            {
                try
                {
                    direction = Direction.GetDirection(shortPath.Steps.First() - parent.Position);
                }
                catch (InvalidOperationException)
                {
                    return false;
                }
            }

            Point coord = new Point(direction.DeltaX, direction.DeltaY);
            return Commands.ActionManager.MoveActorBy(actor, coord);
        }
    }
}