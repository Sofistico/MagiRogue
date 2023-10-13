using MagusEngine.Serialization;
using MagusEngine.Systems.Physics;
using MagusEngine.Utils.Extensions;
using Newtonsoft.Json;
using SadConsole;
using SadRogue.Primitives;

namespace MagusEngine.ECS.Components.TilesComponents
{
    public class Plant
    {
        private ColoredGlyph sadGlyph;
        private MagiColorSerialization fore;
        private MagiColorSerialization back;

        [JsonRequired]
        public string Id { get; set; }

        [JsonRequired]
        public string Name { get; set; }

        [JsonRequired]
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

        public MaterialTemplate Material { get; }

        public int Bundle { get; set; }

        [JsonConstructor]
        public Plant(string? materialId = null)
        {
            Material = materialId!.IsNullOrEmpty() ? MaterialTemplate.None! : PhysicsManager.SetMaterial(materialId)!;
        }

        public Plant(ColoredGlyph glyph, string? materialId) : this(materialId)
        {
            fore = new(glyph.Foreground);
            back = new(glyph.Background);
            Glyphs = new char[] { (char)glyph.Glyph };
        }

        public Plant(Color foreground, Color background, char[] glyphs, string materialId) : this(materialId)
        {
            fore = new(foreground);
            back = new(background);
            Glyphs = glyphs;
        }

        public Plant Clone()
        {
            return new Plant(Foreground, Background, Glyphs, MaterialId)
            {
                Id = Id,
                Name = Name,
                Bundle = Bundle,
            };
        }
    }
}
