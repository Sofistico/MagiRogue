using Arquimedes.Enumerators;
using GoRogue.Random;
using MagusEngine.Core.Civ;
using MagusEngine.Core.MapStuff;
using MagusEngine.ECS.Components.TilesComponents;
using MagusEngine.Factory;
using MagusEngine.Serialization.MapConverter;
using MagusEngine.Systems;
using MagusEngine.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MagusEngine.Generators.MapGen
{
    public class SiteGenerator : MapGenerator
    {
        // TODO: Refactor this class!
        public SiteGenerator() : base()
        {
            // empty one
        }

        public void GenerateBigSite(MagiMap map, int maxRooms,
            int minRoomSize, int maxRoomSize, string townName, List<Building> buildings)
        {
            _map = map;

            List<Building> rooms = BspMapFunction(map, maxRoomSize, minRoomSize, maxRooms);

            map.MapName = townName;

            PopulateMapWithRooms(rooms, QueryTilesForTrait(Trait.Durable),
                QueryTilesForTrait(Trait.Durable, TileType.Wall));

            ApplyRoads(rooms, TileFactory.GenericDirtRoad(Point.None));
            buildings.AddRange(rooms);
        }

        private void PopulateMapWithRooms(List<Building> rooms, Tile f, Tile w)
        {
            for (int i = 0; i < rooms.Count; i++)
            {
                Room? room = rooms[i].PhysicalRoom;
                if (Mrn.OneIn(10)
                    && DataManager.ListOfRooms.Values.Any(r => r.CompareRectanglesSameSize(room.RoomRectangle)
                    && r.Obj.Tag is not RoomTag.Debug))
                {
                    RoomTemplate tempRoom = DataManager.ListOfRooms.Values.First(r => r.CompareRectanglesSameSize(room.RoomRectangle));
                    room = tempRoom.ConfigureRoom(room.RoomRectangle.Position);
                    _rooms.Add(new Building(room));
                    _map.AddRoom(room);
                    _map.SpawnRoomThingsOnMap(room);
                    continue;
                }
                CreateRoom(room,
                    w, f);
                CreateDoor(room);
                LockedDoorsRng(room);
                _rooms.Add(new Building(room));
                _map.AddRoom(room);
            }
            /*_map.AddRooms(_rooms);*/
        }

        private static void LockedDoorsRng(Room room)
        {
            int half = GoRogue.DiceNotation.Dice.Roll("1d2");

            if (half == 1)
            {
                int indexDoor = GlobalRandom.DefaultRNG.NextInt(room.Doors.Count);
                if (room.Doors.Count > 0)
                    room.Doors[indexDoor].GetComponent<DoorComponent>().Locked = true;
            }
        }

        private void PopulateMapWithRoomsWithVariableMaterials(List<Building> rooms,
            List<Tile> possibleFloors)
        {
            throw new NotImplementedException("GET TO WORK YOU LAZY HORSE 🐎!");
        }

        public void GenerateSmallSite(MagiMap map, int maxRooms,
            int minRoomSize, int maxRoomSize, string townName, List<Building> buildings)
        {
            _map = map;
            _map.MapName = townName;

            var rooms = BspMapFunction(map, maxRoomSize, minRoomSize, maxRooms);

            PopulateMapWithRooms(rooms, QueryTilesForTrait(Trait.Inexpensive),
                QueryTilesForTrait(Trait.Inexpensive, TileType.Wall));

            ApplyRoads(_rooms, TileFactory.GenericDirtRoad(Point.None));
            buildings.AddRange(rooms);
        }

        public void GenerateMediumSite(MagiMap map, int maxRooms,
            int minRoomSize, int maxRoomSize, string townName, List<Building> buildings)
        {
            _map = map;
            _map.MapName = townName;

            var rooms = BspMapFunction(map, maxRoomSize, minRoomSize, maxRooms);

            PopulateMapWithRooms(rooms, QueryTilesForTrait(Trait.Durable),
                QueryTilesForTrait(Trait.Durable, TileType.Wall));

            ApplyRoads(_rooms, TileFactory.GenericDirtRoad(Point.None));
            buildings.AddRange(rooms);
        }
    }
}
