using Arquimedes.Enumerators;
using MagusEngine.Core.Magic;
using MagusEngine.Systems;
using Newtonsoft.Json;
using Xunit;

namespace MagiRogue.Test.Data
{
    public class SpellTemplateTests
    {
        private readonly Spell missile = new("magic_missile",
            "Magic Missile",
            ArtMagic.Projection,
            5,
           "Mana Shaping",
            magicCost: 1.0)
        {
            Proficiency = 1
        };

        [Fact]
        public void SpellTemplateSerializationTest()
        {
            var spell = DataManager.QuerySpellInData("magic_missile");


            string spellSerialized = JsonConvert.SerializeObject(spell, Formatting.Indented);
            Spell spellDeserialized = JsonConvert.DeserializeObject<Spell>(spellSerialized);
            Assert.Equal(spellDeserialized.Id, missile.Id);
        }
    }
}
