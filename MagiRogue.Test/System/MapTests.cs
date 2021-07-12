using Xunit;
using SadConsole;

namespace MagiRogue.System.Tests
{
    public class MapTests
    {
        private readonly Map map = new Map(20, 20);

        public MapTests()
        {
            Game.Create(20, 20);
        }

        [Fact]
        public void MapTest()
        {
            Assert.True(map.Tiles.Length == 20 * 20);
            Assert.True(map.Entities != null);
        }
    }
}