﻿using MagiRogue.Data;
using MagiRogue.Data.Enumerators;
using MagiRogue.Data.Serialization.MapSerialization;
using MagiRogue.GameSys.Tiles;
using MagiRogue.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MagiRogue.GameSys.MapGen
{
    public class SiteGenerator : MapGenerator
    {
        // TODO: Refactor this class!
        public SiteGenerator() : base()
        {
            // empty one
        }

        public void GenerateBigSite(Map map, int maxRooms,
            int minRoomSize, int maxRoomSize, string townName, List<Room> buildings)
        {
            _map = map;

            List<Room> rooms = BspMapFunction(map, maxRoomSize, minRoomSize, maxRooms);

            map.MapName = townName;

            PopulateMapWithRooms(rooms, QueryTilesForTrait<TileFloor>(Trait.Durable),
                QueryTilesForTrait<TileWall>(Trait.Durable));

            ApplyRoads(rooms, TileEncyclopedia.GenericDirtRoad(Point.None));
            buildings.AddRange(rooms);
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

        public void GenerateSmallSite(Map map, int maxRooms,
            int minRoomSize, int maxRoomSize, string townName, List<Room> buildings)
        {
            _map = map;
            _map.MapName = townName;

            var rooms = BspMapFunction(map, maxRoomSize, minRoomSize, maxRooms);

            PopulateMapWithRooms(rooms, QueryTilesForTrait<TileFloor>(Trait.Inexpensive),
                QueryTilesForTrait<TileWall>(Trait.Inexpensive));

            ApplyRoads(_rooms, TileEncyclopedia.GenericDirtRoad(Point.None));
            buildings.AddRange(rooms);
        }

        public void GenerateMediumSite(Map map, int maxRooms,
            int minRoomSize, int maxRoomSize, string townName, List<Room> buildings)
        {
            _map = map;
            _map.MapName = townName;

            var rooms = BspMapFunction(map, maxRoomSize, minRoomSize, maxRooms);

            PopulateMapWithRooms(rooms, QueryTilesForTrait<TileFloor>(Trait.Durable),
                QueryTilesForTrait<TileWall>(Trait.Durable));

            ApplyRoads(_rooms, TileEncyclopedia.GenericDirtRoad(Point.None));
            buildings.AddRange(rooms);
        }
    }
}