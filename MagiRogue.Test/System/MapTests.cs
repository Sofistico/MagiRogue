using Xunit;
using MagiRogue.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.System.Tests
{
    public class MapTests
    {
        private readonly Map map = new Map(20, 20);
        //private MagiRogue.Entities.Materials.Material Material = new Entities.Materials.Material();
        //private readonly SadConsole.Console ui = new SadConsole.Console(10, 10);

        public MapTests()
        {
            //uiManger.Init();
            //uiManger
        }

        /*[Fact()]
        public void AddEntityTest()
        {
            var entity = new Entities.Monster(Microsoft.Xna.Framework.Color.Red, Microsoft.Xna.Framework.Color.Green);
            map.Add(entity);
            Assert.True(map.Entities.Contains(entity));
        }*/

        [Fact()]
        public void MapTest()
        {
            //Map map = new Map(10, 10);
            Assert.True(map.Tiles.Length == 20 * 20);
            Assert.True(map.Entities != null);
        }

        /*[Fact()]
        public void IsTileWalkableTest()
        {
            map.SetTerrain(new Tiles.TileFloor(new Microsoft.Xna.Framework.Point(10, 10), "stone"));
            var walkability = map.IsTileWalkable(new Microsoft.Xna.Framework.Point(10, 10));
            if (walkability)
                Assert.True(walkability);
            else
                Assert.False(walkability);
        }*/

        /*[Fact()]
        public void GetEntityAtTest()
        {
            throw new NotImplementedException();
        }

        [Fact()]
        public void RemoveTest()
        {
            throw new NotImplementedException();
        }

        [Fact()]
        public void AddTest1()
        {
            throw new NotImplementedException();
        }

        [Fact()]
        public void GetTileAtTest()
        {
            throw new NotImplementedException();
        }

        [Fact()]
        public void GetTileAtTest1()
        {
            throw new NotImplementedException();
        }

        [Fact()]
        public void CalculateFOVTest()
        {
            throw new NotImplementedException();
        }

        [Fact()]
        public void CalculateFOVTest1()
        {
            throw new NotImplementedException();
        }*/
    }
}