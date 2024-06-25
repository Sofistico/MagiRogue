using Arquimedes.Enumerators;
using Arquimedes.Interfaces;
using MagusEngine.Core.Entities.Base;
using MagusEngine.Core.Magic.Interfaces;
using MagusEngine.Serialization;

namespace MagusEngine.Core.Entities
{
    public class SpellEntity : MagiEntity, IJsonKey
    {
        public string Id { get; set; } = null!;
        public string SpellId { get; set; }
        public MagiEntity? Caster { get; set; }
        public ISpell Spell { get; set; } = null!;

        public SpellEntity(string id, MagiColorSerialization fore, MagiColorSerialization back, char glyph, string spellId, ISpell spell, Point pos) : base(fore, back, glyph, pos, (int)MapLayer.ITEMS)
        {
            Id = id;
            SpellId = spellId;
            Spell = spell;
        }

        public SpellEntity(string id, MagiColorSerialization fore, MagiColorSerialization back, char glyph, string spellId, ISpell spell, Point pos, MagiEntity caster) : this(id, fore, back, glyph, spellId, spell, pos)
        {
            Caster = caster;
        }
    }
}
