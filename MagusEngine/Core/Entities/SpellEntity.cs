using Arquimedes.Interfaces;
using MagusEngine.Core.Entities.Base;
using Newtonsoft.Json;
using SadRogue.Primitives;

namespace MagusEngine.Core.Entities
{
    public class SpellEntity : MagiEntity, IJsonKey
    {
        public string Id { get; set; }
        public string SpellId { get; set; }

        [JsonConstructor]
        public SpellEntity(Color foreground, Color background, int glyph, Point coord, int layer, string spellId) : base(foreground, background, glyph, coord, layer)
        {
            SpellId = spellId;
        }
    }
}
