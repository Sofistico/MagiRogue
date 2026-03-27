using MagusEngine.Actions.Interfaces;
using MagusEngine.Core.Entities;
using MagusEngine.Exceptions;
using MagusEngine.Systems;

namespace MagusEngine.Actions
{
    public class MoveAction : IExecuteAction
    {
        private readonly Point _delta;

        public MoveAction(Point delta)
        {
            _delta = delta;
        }

        public bool Execute(Universe world)
        {
            if (world.CurrentMap is null)
                return false;
            var actor = (Actor)world.CurrentMap.ControlledEntitiy!;
            if (world.CurrentMap.ControlledEntitiy is not Player)
            {
                if (world.CurrentMap.CheckForIndexOutOfBounds(world.CurrentMap.ControlledEntitiy!.Position + _delta))
                    return false;

                var targetCursor = world.CurrentMap.TargetCursor;
                int distance = HandleNonPlayerMoveAndReturnDistance(world, _delta, targetCursor);
                return world.CurrentMap.PlayerExplored[world.CurrentMap.ControlledEntitiy.Position + _delta]
                    && distance <= targetCursor?.MaxDistance
                    && actor!.MoveBy(_delta);
            }
            return actor!.MoveBy(_delta);
        }

        private int HandleNonPlayerMoveAndReturnDistance(Universe world, Point delta, Target? targetCursor)
        {
            int distance = 0;
            _ = world.CurrentMap ?? throw new NullValueException(nameof(world.CurrentMap));

            if (world.CurrentMap.ControlledEntitiy == targetCursor?.Cursor)
            {
                _ = targetCursor ?? throw new NullValueException(nameof(targetCursor));

                if (targetCursor.TravelPath is not null)
                    distance = targetCursor.TravelPath.LengthWithStart;
                if (targetCursor.TravelPath is not null && targetCursor.TravelPath.LengthWithStart >= targetCursor.MaxDistance)
                {
                    distance = world!.CurrentMap!.AStar!.ShortestPath(targetCursor.OriginCoord, world!.CurrentMap!.ControlledEntitiy!.Position + delta)!.Length;
                }
            }
            return distance;
        }
    }
}
