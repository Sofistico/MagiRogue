using Xunit;

namespace MagiRogue.System.Tests
{
    public class MapTests
    {
        private readonly Map map = new Map(20, 20);

        public MapTests()
        {
        }

        [Fact()]
        public void MapTest()
        {
            Assert.True(map.Tiles.Length == 20 * 20);
            Assert.True(map.Entities != null);
        }
    }
}