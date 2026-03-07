using MagusEngine.Actions.Interfaces;
using MagusEngine.Bus;
using MagusEngine.Bus.UiBus;
using MagusEngine.Components.TilesComponents;
using MagusEngine.Core.Entities;
using MagusEngine.Core.MapStuff;
using MagusEngine.Services;
using MagusEngine.Systems;

namespace MagusEngine.Actions
{
    public class UpDownMovement : IExecuteAction
    {
        private readonly int _zDelta;
        private readonly MessageBusService _bus;

        public UpDownMovement(int zDelta)
        {
            _zDelta = zDelta;
            _bus = Locator.GetService<MessageBusService>();
        }

        public bool Execute(Universe world)
        {
            if (_zDelta > 0)
                return UpMovement();
            else
                return DownMovement();
        }

        private bool UpMovement()
        {
            var point = Find.CurrentMap!.ControlledEntitiy!.Position;
            bool possibleChangeMap = Find.Universe.PossibleChangeMap;
            Furniture? possibleStairs = Find.CurrentMap?.GetEntityAt<Furniture>(point);
            MagiMap? currentMap = Find.CurrentMap;

            if (possibleChangeMap)
            {
                if (possibleStairs is not null && !Find.Universe.MapIsWorld()
                    && possibleStairs.MapIdConnection is not null)
                {
                    MagiMap map = Universe.GetMapById(possibleStairs.MapIdConnection.Value)!;
                    _bus.SendMessage<ChangeControlledActorMap>(new(map, map.GetRandomWalkableTile(), currentMap));

                    return true;
                }
                else if (!Find.Universe.MapIsWorld())
                {
                    MagiMap map = Find.Universe.WorldMap.AssocietatedMap;
                    Point playerLastPos = Find.Universe.WorldMap.AssocietatedMap.LastPlayerPosition;
                    _bus.SendMessage<ChangeControlledActorMap>(new(map, playerLastPos, currentMap));
                    Locator.GetService<SavingService>().SaveChunkInPos(Find.Universe.CurrentChunk,
                        Find.Universe.CurrentChunk.ToIndex(map.Width));
                    Find.Universe.CurrentChunk = null!;
                    return true;
                }
                else if (Find.Universe.MapIsWorld())
                {
                    _bus.SendMessage<AddMessageLog>(new("Can't go to the overworld since you are there!"));
                    return false;
                }
                else if (possibleStairs is null && !Find.Universe.MapIsWorld())
                {
                    _bus.SendMessage<AddMessageLog>(new("Can't go up here!"));
                    return false;
                }
                else
                {
                    _bus.SendMessage<AddMessageLog>(new("Can't exit the map!"));
                    return false;
                }
            }
            else
            {
                _bus.SendMessage<AddMessageLog>(new("You can't change the map right now!"));
                return false;
            }
        }

        private bool DownMovement()
        {
            var point = Find.CurrentMap!.ControlledEntitiy!.Position;
            Furniture? possibleStairs = Find.CurrentMap?.GetEntityAt<Furniture>(point);
            var possibleWorldTileHere = Find.CurrentMap?.GetComponentInTileAt<WorldTile>(point);
            MagiMap currentMap = Find.CurrentMap!;
            if (possibleStairs?.MapIdConnection.HasValue == true)
            {
                MagiMap map = Universe.GetMapById(possibleStairs.MapIdConnection.Value)!;
                _bus.SendMessage<ChangeControlledActorMap>(new(map, map.GetRandomWalkableTile(), currentMap));
                return true;
            }
            else if (possibleStairs is null && possibleWorldTileHere is null)
            {
                _bus.SendMessage<AddMessageLog>(new("There is no way to go down from here!"));
                return false;
            }

            if (possibleWorldTileHere?.Visited == false)
            {
                possibleWorldTileHere.Visited = true;

                RegionChunk chunk = Find.Universe.GenerateChunck(point);
                Find.Universe.CurrentChunk = chunk;
                Locator.GetService<MessageBusService>().SendMessage<ChangeControlledActorMap>(new(chunk.LocalMaps[0],
                    chunk.LocalMaps[0].GetRandomWalkableTile(), currentMap));
                return true;
            }
            else if (possibleWorldTileHere?.Visited == true)
            {
                RegionChunk chunk = Find.Universe.GetChunckByPos(point)!;
                Find.Universe.CurrentChunk = chunk;
                // if entering the map again, set to update
                chunk.SetMapsToUpdate();
                Locator.GetService<MessageBusService>().SendMessage<ChangeControlledActorMap>(new(chunk.LocalMaps[0],
                    chunk.LocalMaps[0].LastPlayerPosition, currentMap));

                return true;
            }
            else
            {
                Locator.GetService<MessageBusService>().SendMessage<AddMessageLog>(new("There is nowhere to go!"));
                return false;
            }
        }
    }
}
