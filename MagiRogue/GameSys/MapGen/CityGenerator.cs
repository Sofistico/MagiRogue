using MagiRogue.Data;
using MagiRogue.Data.Enumerators;
using MagiRogue.Data.Serialization.MapSerialization;
using MagiRogue.GameSys.Tiles;
using MagiRogue.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MagiRogue.GameSys.MapGen
{
    public class CityGenerator : MapGenerator
    {
        public CityGenerator() : base()
        {
            // empty one
        }

        public void GenerateBigCityFromMapBSP(Map map, int maxRooms,
            int minRoomSize, int maxRoomSize, string townName)
        {
            _map = map;

            List<Room> rooms = BspMapFunction(map, maxRoomSize, minRoomSize, maxRooms);

            map.MapName = townName;

            PopulateMapWithRooms(_rooms, QueryTilesForTrait<TileFloor>(Trait.Durable),
                QueryTilesForTrait<TileWall>(Trait.Durable));

            ApplyRoads(rooms, TileEncyclopedia.GenericDirtRoad(Point.None));
        }

        private void PopulateMapWithRooms(List<Room> rooms, TileFloor f, TileWall w)
        {
            for (int i = 0; i < rooms.Count; i++)
            {
                Room? room = rooms[i];
                if (Mrn.OneIn(10))
                {
                    if (DataManager.ListOfRooms.Any(r => r.CompareRectanglesSameSize(room.RoomRectangle)))
                    {
                        RoomTemplate tempRoom =
                            DataManager.ListOfRooms.First(r => r.CompareRectanglesSameSize(room.RoomRectangle));
                        room = tempRoom.ConfigureRoom(room.RoomRectangle.Position);
                        _rooms.Add(room);
                        _map.AddRoom(room);
                        _map.SpawnRoomThingsOnMap(room);
                        continue;
                    }
                }
                CreateRoom(room,
                    w, f);
                CreateDoor(room);
                room.LockedDoorsRng();
                _rooms.Add(room);
                _map.AddRoom(room);
            }
            /*_map.AddRooms(_rooms);*/
        }

        private void PopulateMapWithRoomsWithVariableMaterials(List<Room> rooms,
            List<TileFloor> possibleFloors)
        {
            throw new NotImplementedException("GET TO WORK YOU LAZY HORSE 🐎!");
        }

        public void GenerateSmallVillage(Map map, int maxRooms,
            int minRoomSize, int maxRoomSize, string townName)
        {
            _map = map;
            _map.MapName = townName;

            _rooms = BspMapFunction(map, maxRoomSize, minRoomSize, maxRooms);

            PopulateMapWithRooms(_rooms, QueryTilesForTrait<TileFloor>(Trait.Inexpensive),
                QueryTilesForTrait<TileWall>(Trait.Inexpensive));

            ApplyRoads(_rooms, TileEncyclopedia.GenericDirtRoad(Point.None));
        }

        public void GenerateMediumTown(Map map, int maxRooms,
            int minRoomSize, int maxRoomSize, string townName)
        {
            _map = map;
            _map.MapName = townName;

            _rooms = BspMapFunction(map, maxRoomSize, minRoomSize, maxRooms);

            PopulateMapWithRooms(_rooms, QueryTilesForTrait<TileFloor>(Trait.Durable),
                QueryTilesForTrait<TileWall>(Trait.Durable));

            ApplyRoads(_rooms, TileEncyclopedia.GenericDirtRoad(Point.None));
        }
    }
}