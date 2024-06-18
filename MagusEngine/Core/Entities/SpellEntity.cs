using Arquimedes.Enumerators;
using Arquimedes.Interfaces;
using MagusEngine.Core.Entities.Base;
using MagusEngine.Serialization;
using Newtonsoft.Json;
using SadConsole;

namespace MagusEngine.Core.Entities
{
    public class SpellEntity : MagiEntity, IJsonKey
    {
        [JsonRequired]
        public string Id { get; set; } = null!;

        [JsonRequired]
        public string SpellId { get; set; }
        public MagiEntity? Caster { get; set; }

        [JsonConstructor]
        public SpellEntity(string id, MagiColorSerialization fore, MagiColorSerialization back, char glyph, string spellId) : base(fore, back, glyph, Point.None, (int)MapLayer.ITEMS)
        {
            Id = id;
            SpellId = spellId;
        }
    }
}
