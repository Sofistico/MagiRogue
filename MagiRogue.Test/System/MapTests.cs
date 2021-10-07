using MagiRogue.Entities;
using SadConsole;
using SadRogue.Primitives;
using Xunit;

namespace MagiRogue.System.Tests
{
    public class MapTests
    {
        private readonly Map map = new("Test", 20, 20);

        public MapTests()
        {
            Game.Create(20, 20);
        }

        [Fact]
        public void MapTest()
        {
            Assert.True(map.Tiles.Length == 20 * 20);
            Actor actor = new Actor("Test", Color.Black, Color.Black, '3', new Point(1, 1));
            map.Add(actor);
            Assert.True(map.Entities.Contains(actor));
        }
    }
}