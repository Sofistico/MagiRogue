using Arquimedes.Enumerators;
using Arquimedes.Interfaces;
using MagusEngine.Core.Entities.Base;
using MagusEngine.Serialization;

namespace MagusEngine.Core.Entities
{
    public class SpellEntity : MagiEntity, IJsonKey
    {
        public string Id { get; set; } = null!;
        public string SpellId { get; set; }
        public MagiEntity? Caster { get; set; }

        public SpellEntity(string id, MagiColorSerialization fore, MagiColorSerialization back, char glyph, string spellId) : base(fore, back, glyph, Point.None, (int)MapLayer.ITEMS)
        {
            Id = id;
            SpellId = spellId;
        }
    }
}
