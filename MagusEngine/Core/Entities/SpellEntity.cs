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
        public MagiEntity? Caster { get; set; }
        public ISpell Spell { get; set; } = null!;

        public SpellEntity(string id, MagiColorSerialization fore, MagiColorSerialization back, char glyph, ISpell spell, Point pos, MagiEntity caster) : base(fore, back, glyph, pos, (int)MapLayer.ITEMS)
        {
            Id = id;
            Spell = spell;
            Caster = caster;
        }
    }
}
