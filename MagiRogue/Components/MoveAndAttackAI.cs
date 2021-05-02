using GoRogue;
using GoRogue.Pathing;
using MagiRogue.Entities;
using MagiRogue.System;
using MagiRogue.System.Time;
using MagiRogue.UI.Windows;
using System.Linq;

namespace MagiRogue.Components
{
    public class MoveAndAttackAI : IAiComponent
    {
        public GoRogue.GameFramework.IGameObject Parent { get; set; }

        private readonly int perceptionAi;

        public MoveAndAttackAI(int perception)
        {
            perceptionAi = perception;
        }

        public (bool sucess, long ticks) RunAi(Map map, MessageLogWindow messageLog)
        {
            // TODO: Fix, because it hardly works, at least i saw the actor attacking
            if (Parent is not Actor actor)
            {
                return (false, -1);
            }

            int walkSpeed = TimeHelper.GetWalkTime(actor);

            if (TryGetDirectionMove(map, actor))
            {
                return (true, walkSpeed);
            }
            else
            {
                return (true, TimeHelper.Wait);
            }
        }

        public bool TryGetDirectionMove(Map map, Actor actor)
        {
            Path shortPath = map.AStar.ShortestPath(actor.Position, GameLoop.World.Player.Position);

            Direction direction;

            if (shortPath == null || shortPath.Length > perceptionAi)
            {
                return false;
            }
            else
            {
                direction = Direction.GetDirection(shortPath.Steps.First() - Parent.Position);
            }

            Coord coord = new Coord(direction.DeltaX, direction.DeltaY);
            return Commands.CommandManager.MoveActorBy(actor, coord);
        }
    }
}