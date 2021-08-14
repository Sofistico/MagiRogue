using Xunit;
using MagiRogue.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MagiRogue.System.Magic;
using Newtonsoft.Json;

namespace MagiRogue.Data.Tests
{
    public class SpellTemplateTests
    {
        private readonly SpellBase missile = new SpellBase("magic_missile",
            "Magic Missile", MagicSchool.Projection, 5, manaCost: 1.0f)
        { Proficiency = 1 };

        [Fact()]
        public void SpellTemplateSerializationTest()
        {
            string spellSerialized = JsonConvert.SerializeObject(missile);
            SpellBase spellDeserialized = JsonConvert.DeserializeObject<SpellBase>(spellSerialized);

            Assert.True(spellDeserialized.SpellId.Equals(missile.SpellId));
        }
    }
}