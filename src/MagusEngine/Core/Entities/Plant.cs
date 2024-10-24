﻿using Arquimedes.Interfaces;
using MagusEngine.Core.Entities.Interfaces;
using MagusEngine.Serialization;
using MagusEngine.Systems;
using MagusEngine.Utils.Extensions;
using Newtonsoft.Json;
using SadConsole;
using SadRogue.Primitives;

namespace MagusEngine.Core.Entities
{
    public class Plant : IJsonKey, IAppearence
    {
        private MagiColorSerialization fore;
        private MagiColorSerialization back;

        [JsonRequired]
        public string Id { get; set; }

        [JsonRequired]
        public string Name { get; set; }

        [JsonRequired]
        public string[] Fores { get; set; }

        public string[] Backs { get; set; }

        public MagiColorSerialization Fore
        {
            get
            {
                return fore;
            }

            set
            {
                fore = new MagiColorSerialization(value);
            }
        }

        public MagiColorSerialization Back
        {
            get
            {
                return back;
            }

            set
            {
                back = new MagiColorSerialization(value);
            }
        }

        [JsonRequired]
        public char[] Glyphs { get; set; }

        [JsonRequired]
        public string MaterialId { get; set; }

        public Material Material { get; set; }

        public int MaxBundle { get; set; }

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
            Glyphs = [(char)glyph.Glyph];
        }

        public Plant(string foreground, Color background, char[] glyphs)
        {
            fore = new(foreground);
            back = new(background);
            Glyphs = glyphs;
        }

        public Plant Copy()
        {
            return new Plant(Fores.GetRandomItemFromList(), Back, Glyphs)
            {
                Id = Id,
                Name = Name,
                MaxBundle = MaxBundle,
                Material = Material,
                MaterialId = MaterialId,
                Fores = Fores,
                Glyphs = Glyphs,
            };
        }

        public ColoredGlyph GetSadGlyph() => new(Fore, Back, Glyphs.GetRandomItemFromList());
    }
}
