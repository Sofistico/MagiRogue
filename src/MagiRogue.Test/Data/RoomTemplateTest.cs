using MagusEngine;
using MagusEngine.Core.MapStuff;
using MagusEngine.Generators.MapGen;
using MagusEngine.Serialization.MapConverter;
using MagusEngine.Systems;
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
            MagiMap map = new MiscMapGen().GenerateStoneFloorMap();
            Room r = room.ConfigureRoom(map.GetRandomWalkableTile());
            map.AddRoom(r);
            map.SpawnRoomThingsOnMap(r);

            Assert.Contains(r, map.Rooms);
        }
    }
}
