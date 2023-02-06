using MagiRogue.Components;
using MagiRogue.Components.Ai;
using MagiRogue.Data;
using System;

namespace MagiRogue.GameSys.MapGen
{
    public class MiscMapGen : MapGenerator
    {
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

            return _map;
        }

        private void AppendForestTestMap()
        {
            var r = DataManager.QueryRoomInData("forest_test");

            var pointBegin = new Point(_map.Width / 2, _map.Height / 2);

            var room = _map.AddRoom(r, pointBegin);
            PlaceUnitsInForest(room);
        }

        private void PlaceUnitsInForest(Room room)
        {
            int count = 6;
            for (int i = 0; i < count; i++)
            {
                var pos = room.ReturnRandomPosRoom();
                if (i % count == 0)
                {
                    _map.AddMagiEntity(EntityFactory.ActorCreator(pos,
                        "deer",
                        Data.Enumerators.Sex.Male,
                        Data.Enumerators.AgeGroup.Adult)
                        .WithComponents(new NeedDrivenAi(), NeedCollection.WithCommonNeeds()));
                }
                else
                {
                    _map.AddMagiEntity(EntityFactory.ActorCreator(pos,
                        "wolf",
                        Data.Enumerators.Sex.Male,
                        Data.Enumerators.AgeGroup.Adult)
                        .WithComponents(new NeedDrivenAi(), NeedCollection.WithCommonNeeds()));
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