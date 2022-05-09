using MagiRogue.Data;
using MagiRogue.GameSys.Tiles;
using System.Collections.Generic;

namespace MagiRogue.GameSys.MapGen
{
    public class CityGenerator : MapGenerator
    {
        public CityGenerator() : base()
        {
            // empty one
        }

        public void GenerateSmallVillageFromMapBSP(Map map, int maxRooms,
            int minRoomSize, int maxRoomSize, string townName)
        {
            _map = map;

            List<Room> rooms = BspMapFunction(map, maxRoomSize, minRoomSize, maxRooms);

            map.MapName = townName;

            PopulateMapWithRooms(rooms, (TileFloor)DataManager.QueryTileInData("stone_floor"),
                (TileWall)DataManager.QueryTileInData("stone_wall"));

            ApplyRoads(rooms, TileEncyclopedia.GenericDirtRoad(Point.None));
        }

        private void PopulateMapWithRooms(List<Room> rooms, TileFloor f, TileWall w)
        {
            for (int i = 0; i < rooms.Count; i++)
            {
                Room? room = rooms[i];
                CreateRoom(room.RoomRectangle,
                    w, f);
                CreateDoor(room);
                room.LockedDoorsRng();
                _rooms.Add(room);
            }
            _map.Rooms = _rooms;
        }
    }
}