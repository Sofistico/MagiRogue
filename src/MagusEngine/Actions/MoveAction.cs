using MagusEngine.Actions.Interfaces;
using MagusEngine.Core.Entities;
using MagusEngine.Exceptions;
using MagusEngine.Systems;

namespace MagusEngine.Actions
{
    public class MoveAction : IExecuteAction
    {
        private readonly Point _delta;
        private readonly Target _targetCursor;

        public MoveAction(Point delta, Target targetCursor)
        {
            _delta = delta;
            _targetCursor = targetCursor;
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

                int distance = HandleNonPlayerMoveAndReturnDistance(world, _delta);

                return world.CurrentMap.PlayerExplored[world.CurrentMap.ControlledEntitiy.Position + _delta]
                    && distance <= _targetCursor?.MaxDistance
                    && actor!.MoveBy(_delta);
            }
            return actor!.MoveBy(_delta);
        }

        private int HandleNonPlayerMoveAndReturnDistance(Universe world, Point delta)
        {
            int distance = 0;
            _ = world.CurrentMap ?? throw new NullValueException(nameof(world.CurrentMap));

            if (world.CurrentMap.ControlledEntitiy == _targetCursor?.Cursor)
            {
                _ = _targetCursor ?? throw new NullValueException(nameof(_targetCursor));

                if (_targetCursor.TravelPath is not null)
                    distance = _targetCursor.TravelPath.LengthWithStart;
                if (_targetCursor.TravelPath is not null && _targetCursor.TravelPath.LengthWithStart >= _targetCursor.MaxDistance)
                {
                    distance = world!.CurrentMap!.AStar!.ShortestPath(_targetCursor.OriginCoord, world!.CurrentMap!.ControlledEntitiy!.Position + delta)!.Length;
                }
            }
            return distance;
        }
    }
}
