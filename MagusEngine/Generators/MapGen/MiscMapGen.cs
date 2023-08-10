using MagiRogue.Components;
using MagiRogue.Components.Ai;
using MagiRogue.Data;
using MagiRogue.Data.Enumerators;
using MagiRogue.GameSys.Tiles;
using MagiRogue.Utils;
using MagusEngine.Core.MapStuff;

namespace MagusEngine.Generators.MapGen
{
    public class MiscMapGen : MapGenerator
    {
        private WaterTile waterTile;

        public MiscMapGen()
        {
            // empty
        }

        public Map GenerateTestMap()
        {
            _map = new Map("Test Map", 180, 180);

            PrepareForFloors();
            PrepareForOuterWalls();
            AppendForestTestMap();
            AppendRitualFloor();
            FillMapWithGrass(_map);
            PutCircularRoom();
            PutFurnitureInCircularRoom();
            PutDwarfInCenterOfCircularRoom();

            return _map;
        }

        private void PutDwarfInCenterOfCircularRoom()
        {
            var r = _map.FindRoomsByTag(RoomTag.Blacksmith)[0];
            var point = r.GetCenter();
            var actor = EntityFactory.ActorCreator(point,
                "dwarf",
                "Blacksmith",
                25,
                EntityFactory.GetRandomSex()).WithComponents(new NeedDrivenAi());

            _map.AddMagiEntity(actor);
        }

        private void PutFurnitureInCircularRoom()
        {
            var r = _map.FindRoomsByTag(RoomTag.Blacksmith);
            for (int i = 0; i < r.Count; i++)
            {
                var room = r[i];
                MakeRoomWithTagUseful(room, _map);
            }
        }

        private void PutCircularRoom()
        {
            //var poitns = _map.GetRandomWalkableTile();
            var poitns = new Point(10, 10);
            Shape circle = poitns.HollowCircleFromOriginPoint(5);
            var r = new Room(poitns.CircleFromOriginPoint(5).Points, Data.Enumerators.RoomTag.Blacksmith);

            foreach (var point in circle.Points)
            {
                var tile = TileEncyclopedia.GenericStoneWall(point);
                _map.SetTerrain(tile);
            }

            _map.AddRoom(r);
        }

        private void AppendRitualFloor()
        {
            var r = DataManager.QueryRoomInData("ritual_simple");

            var pointBegin = new Point(25, 25);
            _map.AddRoom(r, pointBegin);
        }

        private void AppendForestTestMap()
        {
            var r = DataManager.QueryRoomInData("forest_test");

            var pointBegin = new Point(_map.Width / 2, _map.Height / 2);

            var room = _map.AddRoom(r, pointBegin);
            waterTile = _map.GetAllTilesWithComponents<WaterTile>()[0];
            PlaceUnitsInForest(room);
        }

        private void PlaceUnitsInForest(Room room)
        {
            const int count = 10;
            for (int i = 0; i < count; i++)
            {
                var pos = room.ReturnRandomPosRoom();
                if (i % count == 0)
                {
                    var actor = EntityFactory.ActorCreator(pos,
                        "deer",
                        Data.Enumerators.Sex.Male,
                        Data.Enumerators.AgeGroup.Adult)
                        .WithComponents(new NeedDrivenAi());
                    actor.AddMemory(waterTile.Position,
                        Data.Enumerators.MemoryType.WaterLoc,
                        waterTile);
                    _map.AddMagiEntity(actor);
                }
                else
                {
                    var actor = EntityFactory.ActorCreator(pos,
                        "wolf",
                        Data.Enumerators.Sex.Male,
                        Data.Enumerators.AgeGroup.Adult)
                        .WithComponents(new NeedDrivenAi());
                    actor.AddMemory(waterTile.Position,
                        Data.Enumerators.MemoryType.WaterLoc,
                        waterTile);
                    _map.AddMagiEntity(actor);
                }
            }
        }

        public Map GenerateStoneFloorMap()
        {
            _map = new Map("Test Stone Map");

            PrepareForFloors();

            return _map;
        }
    }
}