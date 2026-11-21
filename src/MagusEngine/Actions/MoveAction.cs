using MagusEngine.Actions.Interfaces;
using MagusEngine.Bus.MapBus;
using MagusEngine.Core.Entities;
using MagusEngine.Core.MapStuff;
using MagusEngine.Exceptions;
using MagusEngine.Services;
using MagusEngine.Systems;
using MagusEngine.Systems.Time;
using MagusEngine.Utils.Extensions;

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
            if (world!.CurrentMap!.ControlledEntitiy!.MoveFreeTurn)
            {
                if (world.CurrentMap.CheckForIndexOutOfBounds(world.CurrentMap.ControlledEntitiy!.Position + _delta))
                    return false;

                var targetCursor = world.CurrentMap.TargetCursor;
                int distance = HandleNonPlayerMoveAndReturnDistance(world, _delta, targetCursor);
                return world.CurrentMap.PlayerExplored[world.CurrentMap.ControlledEntitiy.Position + _delta]
                    && distance <= targetCursor?.MaxDistance
                    && actor!.MoveBy(_delta);
            }
            else
            {
                if (!actor.Bumped)
                {
                    Locator.GetService<MessageBusService>().SendMessage<ProcessTurnEvent>(new(TimeHelper.GetWalkTime(actor,
                                    world.CurrentMap.GetTileAt<Tile>(actor.Position)!), true));
                }
                else
                {
                    var attack = actor.GetAttacks().GetRandomItemFromList() ?? throw new NullValueException("Attack was null", null);
                    Locator.GetService<MessageBusService>().SendMessage<ProcessTurnEvent>(new(TimeHelper.GetAttackTime(actor, attack), true));
                }
            }
            return actor!.MoveBy(_delta);
        }

        private static int HandleNonPlayerMoveAndReturnDistance(Universe world, Point delta, Target? targetCursor)
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
