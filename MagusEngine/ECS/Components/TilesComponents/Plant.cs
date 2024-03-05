using MagusEngine.Core;
using MagusEngine.Core.Entities.Base;
using MagusEngine.Serialization;
using MagusEngine.Systems;
using MagusEngine.Systems.Physics;
using MagusEngine.Utils.Extensions;
using Newtonsoft.Json;
using SadConsole;
using SadRogue.Primitives;

namespace MagusEngine.ECS.Components.TilesComponents
{
    public class Plant : IJsonKey
    {
        private ColoredGlyph sadGlyph;
        private MagiColorSerialization fore;
        private MagiColorSerialization back;

        [JsonRequired]
        public string Id { get; set; }

        [JsonRequired]
        public string Name { get; set; }

        [JsonRequired]
        public string[] Fores { get; set; }

        public string Fore
        {
            get
            {
                return fore.ColorName;
            }

            set
            {
                fore = new MagiColorSerialization(value);
            }
        }

        public string Back
        {
            get
            {
                return back.ColorName;
            }

            set
            {
                back = new MagiColorSerialization(value);
            }
        }

        [JsonRequired]
        public char[] Glyphs { get; set; }

        public ColoredGlyph SadGlyph => sadGlyph ??= new ColoredGlyph(Foreground, Background, Glyphs.GetRandomItemFromList());
        public Color Foreground => fore.Color;
        public Color Background => back.Color;

        [JsonRequired]
        public string MaterialId { get; set; }

        public Material Material { get; set; }

        public int Bundle { get; set; }

        [JsonConstructor]
        public Plant(string? materialId = null, string[] fores = null)
        {
            Material = materialId.IsNullOrEmpty() ? Material.None : DataManager.QueryMaterial(materialId)!;
            Fores = fores;
            Fore = Fores.GetRandomItemFromList();
        }

        public Plant(ColoredGlyph glyph, string? materialId) : this(materialId)
        {
            fore = new(glyph.Foreground);
            back = new(glyph.Background);
            Glyphs = new char[] { (char)glyph.Glyph };
        }

        public Plant(string foreground, Color background, char[] glyphs)
        {
            fore = new(foreground);
            back = new(background);
            Glyphs = glyphs;
        }

        public Plant Copy()
        {
            return new Plant(Fores.GetRandomItemFromList(), Background, Glyphs)
            {
                Id = Id,
                Name = Name,
                Bundle = Bundle,
                Material = Material,
                MaterialId = MaterialId,
                Fores = Fores,
                Glyphs = Glyphs,
            };
        }
    }
}
