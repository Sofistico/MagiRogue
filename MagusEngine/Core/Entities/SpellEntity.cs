using Arquimedes.Interfaces;
using MagusEngine.Core.Entities.Base;
using MagusEngine.Core.Entities.Interfaces;
using MagusEngine.Serialization;
using MagusEngine.Utils.Extensions;
using Newtonsoft.Json;
using SadConsole;
using SadRogue.Primitives;

namespace MagusEngine.Core.Entities
{
    public class SpellEntity : IJsonKey, IAppearence
    {
        private MagiColorSerialization _fore;
        private MagiColorSerialization _back;

        [JsonRequired]
        public string Id { get; set; } = null!;

        [JsonRequired]
        public string SpellId { get; set; }
        public MagiEntity Caster { get; set; }

        [JsonRequired]
        public string[] Fores { get; set; }

        public string Fore
        {
            get
            {
                return _fore.ColorName;
            }

            set
            {
                _fore = new MagiColorSerialization(value);
            }
        }

        public string Back
        {
            get
            {
                return _back.ColorName;
            }

            set
            {
                _back = new MagiColorSerialization(value);
            }
        }

        [JsonRequired]
        public string[] Backs { get; set; }

        [JsonRequired]
        public char[] Glyphs { get; set; }
        public Color Foreground => _fore;
        public Color Background => _back;

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

        public ColoredGlyph GetSadGlyph()
        {
            throw new System.NotImplementedException();
        }
    }
}
