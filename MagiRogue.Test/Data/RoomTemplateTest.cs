using MagiRogue.Data;
using MagiRogue.Data.Serialization.MapSerialization;
using MagiRogue.GameSys;
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
        private RoomTemplate room;

        public RoomTemplateTest()
        {
            room = DataManager.QueryRoomInData("room_city1");
        }

        [Fact]
        public void RoomDeserializeAndGetToMap()
        {
            Map map = new Map("Test_map");
            Room r = room.ConfigureRoom();
            map.AddRoom(r);

            Assert.Contains(r, map.Rooms);
        }
    }
}