using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Newtonsoft.Json;
using MagiRogue.Entities;
using SadRogue.Primitives;

namespace MagiRogue.Test.Data
{
    public class FurnitureTests
    {
        private const string materialWoodId = "wood";

        public FurnitureTests()
        {
        }

        [Fact]
        public void FurnitureSerializationTest()
        {
            Furniture fur = new Furniture(Color.AliceBlue, Color.Black, ' ', Point.None,
                FurnitureType.General, materialWoodId, "Test Thingy");
            string json = JsonConvert.SerializeObject(fur);
            Assert.Contains("Fur", json);
        }

        [Fact]
        public void FurnitureDeserializationTest()
        {
            Furniture fur = new Furniture(Color.AliceBlue, Color.Black, ' ', Point.None, FurnitureType.General,
                materialWoodId, "flehsy wood bits");
            string json = JsonConvert.SerializeObject(fur);
            Furniture newfur = JsonConvert.DeserializeObject<Furniture>(json);
            Assert.Equal(fur.Name, newfur.Name);
        }

        [Fact]
        public void FurnitureDeserializeFromString()
        {
            string json = @" {'Id': 'chair',
                    'Name': 'Chair',
                    'MaterialId': 'wood',
                    'Glyph': '┬',
                    'Weight': '45',
                    'Size': '45',
                    'Durability': '2',
                    'FurnitureType': 'Chair',
                    'UseActions': [ 'Sit' ],
                    'Traits': ['Confortable']
                    }";

            var fur = JsonConvert.DeserializeObject<Furniture>(json);

            Assert.Contains(fur.FurId, json);
        }
    }
}