using GoRogue.Components.ParentAware;
using GoRogue.Pathing;
using MagiRogue.Entities;
using MagiRogue.System;
using MagiRogue.System.Time;
using MagiRogue.UI.Windows;
using SadRogue.Primitives;
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

        public IObjectWithComponents Parent { get; set; }

        public (bool sucess, long ticks) RunAi(Map map, MessageLogWindow messageLog)
        {
            // TODO: Fix, because it hardly works, at least i saw the actor attacking
            if (Parent is not GoRogue.GameFramework.IGameObject parent)
            {
                return (false, -1);
            }

            Actor actor = (Actor)GameLoop.World.CurrentMap.GetEntityById(parent.ID);

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

            GoRogue.GameFramework.IGameObject iGame = (GoRogue.GameFramework.IGameObject)Parent;
            var parent = GameLoop.World.CurrentMap.GetEntityById(iGame.ID);

            Direction direction;

            if (shortPath == null || shortPath.Length > perceptionAi)
            {
                return false;
            }
            else
            {
                direction = Direction.GetDirection(shortPath.Steps.First() - parent.Position);
            }

            Point coord = new Point(direction.DeltaX, direction.DeltaY);
            return Commands.CommandManager.MoveActorBy(actor, coord);
        }
    }
}