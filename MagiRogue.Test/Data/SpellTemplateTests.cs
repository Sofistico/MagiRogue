using Arquimedes.Enumerators;
using MagusEngine.Core.Magic;
using Newtonsoft.Json;
using Xunit;

namespace MagiRogue.Test.Data
{
    public class SpellTemplateTests
    {
        private readonly SpellBase missile = new SpellBase("magic_missile",
            "Magic Missile", ArtMagic.Projection, 5, manaCost: 1.0f)
        { Proficiency = 1 };

        [Fact()]
        public void SpellTemplateSerializationTest()
        {
            string spellSerialized = JsonConvert.SerializeObject(missile, Formatting.Indented);
            SpellBase spellDeserialized = JsonConvert.DeserializeObject<SpellBase>(spellSerialized);
            Assert.True(spellDeserialized.SpellId.Equals(missile.SpellId));
        }
    }
}