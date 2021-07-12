using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoRogue.Components.ParentAware;
using SadConsole;
using MagiRogue.System.Tiles;

namespace MagiRogue.Components
{
    public class IllusionComponent : GoRogue.Components.ParentAware.IParentAwareComponent
    {
        public const string Tag = "illusion";

        public readonly ColoredGlyph FakeAppearence;
        public IObjectWithComponents Parent { get; set; }

        public IllusionComponent(TileBase fakeAppearence)
        {
            FakeAppearence = new ColoredGlyph(fakeAppearence.Foreground,
                fakeAppearence.Background, fakeAppearence.Glyph);
            FakeAppearence.IsVisible = true;
        }
    }
}