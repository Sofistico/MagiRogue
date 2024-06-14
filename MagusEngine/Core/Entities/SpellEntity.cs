using Arquimedes.Interfaces;
using MagusEngine.Core.Entities.Base;
using MagusEngine.Core.Entities.Interfaces;
using MagusEngine.Utils.Extensions;
using Newtonsoft.Json;
using SadRogue.Primitives;

namespace MagusEngine.Core.Entities
{
    public class SpellEntity : IJsonKey, IAppearence
    {
        [JsonRequired]
        public string Id { get; set; } = null!;

        [JsonRequired]
        public string SpellId { get; set; }
        public MagiEntity Caster { get; set; }

        [JsonRequired]
        public string[] Fores { get; set; }
        public string Fore { get; set; }

        [JsonRequired]
        public string[] Backs { get; set; }
        public string Back { get; set; }

        [JsonRequired]
        public char[] Glyphs { get; set; }

        [JsonConstructor]
        public SpellEntity(string id, string[] fores, string[] backs, string spellId)
        {
            Id = id;
            Fores = fores;
            Fore = fores.GetRandomItemFromList();
            Backs = backs;
            Back = backs.GetRandomItemFromList();
            SpellId = spellId;
        }

        public SpellEntity(Color foreground, Color background, int glyph, Point coord, int layer, string spellId, MagiEntity caster) /*: base(foreground, background, glyph, coord, layer)*/
        {
            SpellId = spellId;
            Caster = caster;
        }
    }
}
