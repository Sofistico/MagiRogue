using MagiRogue.Entities;
using MagiRogue.System;
using MagiRogue.System.Tiles;
using SadConsole;
using SadRogue.Primitives;
using System;
using Xunit;

namespace MagiRogue.Test.System
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

        [Fact]
        public void ChangeEntityCurrentMap()
        {
            var actor = new Player("Test", Color.Black, Color.Black, new Point(1, 1));
            World world = new(actor);
            world.AllMaps.Add(map);
            map.Add(actor);
            var newMap = new Map("MapTest", 1, 1);
            newMap.Tiles[0] = new TileFloor(new Point(0, 0));
            world.ChangeActorMap(actor, newMap, new Point(0, 0));
            Assert.True(actor.CurrentMap.Equals(newMap));
        }

        [Fact]
        public void CheckIfCanGoBeyondBoundsEntity()
        {
            var actor = new Player("Name", Color.Black, Color.Black, new Point(21, 21));
            Action testCode = delegate () { map.Add(actor); };
            Assert.Throws<NullReferenceException>(testCode);
        }
    }
}