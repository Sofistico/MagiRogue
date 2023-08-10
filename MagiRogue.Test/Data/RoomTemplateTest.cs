using MagiRogue.Data;
using MagiRogue.Data.Serialization.MapSerialization;
using MagusEngine.Core.MapStuff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MagiRogue.Test.Data
{
    public class RoomTemplateTest
    {
        private readonly RoomTemplate room;

        public RoomTemplateTest()
        {
            room = DataManager.QueryRoomInData("room_city1");
        }

        [Fact]
        public void RoomDeserializeAndGetToMap()
        {
            Map map = new GameSys.MapGen.MiscMapGen().GenerateStoneFloorMap();
            Room r = room.ConfigureRoom(map.GetRandomWalkableTile());
            map.AddRoom(r);
            map.SpawnRoomThingsOnMap(r);

            Assert.Contains(r, map.Rooms);
        }
    }
}