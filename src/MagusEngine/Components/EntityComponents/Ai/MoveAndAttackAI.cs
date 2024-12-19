using GoRogue.Pathing;
using MagusEngine.Core.Entities;
using MagusEngine.Core.MapStuff;
using MagusEngine.Systems;
using MagusEngine.Systems.Time;
using SadRogue.Primitives;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace MagusEngine.Components.EntityComponents.Ai
{
    public class MoveAndAttackAI : IAiComponent
    {
        private readonly int perceptionAi;

        public MoveAndAttackAI(int perception)
        {
            perceptionAi = perception;
        }

        public object? Parent { get; set; }

        public (bool sucess, long ticks) RunAi(MagiMap? map)
        {
            // Fix, because it hardly works, at least i saw the actor attacking
            if (Parent is not GoRogue.GameFramework.IGameObject parent || map is null)
            {
                return (false, -1);
            }

            Actor? actor = (Actor?)Find.CurrentMap?.GetEntityById(parent.ID);

            if (actor is not null && TryGetDirectionMove(map, actor))
            {
                var walkSpeed = TimeHelper.GetWalkTime(actor, actor.Position);
                return (true, walkSpeed);
            }
            else
            {
                return (true, TimeHelper.Wait);
            }
        }

        public bool TryGetDirectionMove([NotNull] MagiMap map, [NotNull] Actor actor)
        {
            Path? shortPath = map.AStar.ShortestPath(actor.Position, Find.Universe!.Player!.Position);

            GoRogue.GameFramework.IGameObject iGame = (GoRogue.GameFramework.IGameObject)Parent!;
            var parent = Find.CurrentMap?.GetEntityById(iGame.ID);

            if (parent is null)
                return false;

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

            var coord = new Point(direction.DeltaX, direction.DeltaY);
            return actor.MoveBy(coord);
        }
    }
}
