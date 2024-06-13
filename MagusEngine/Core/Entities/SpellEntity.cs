using Arquimedes.Interfaces;
using MagusEngine.Core.Entities.Base;
using Newtonsoft.Json;
using SadRogue.Primitives;

namespace MagusEngine.Core.Entities
{
    public class SpellEntity : IJsonKey
    {
        public string Id { get; set; } = null!;
        public string SpellId { get; set; }
        public MagiEntity Caster { get; set; }

        [JsonConstructor]
        public SpellEntity()
        {
        }

        public SpellEntity(Color foreground, Color background, int glyph, Point coord, int layer, string spellId, MagiEntity caster) /*: base(foreground, background, glyph, coord, layer)*/
        {
            SpellId = spellId;
            Caster = caster;
        }
    }
}
