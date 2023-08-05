using GoRogue.Components.ParentAware;
using MagiRogue.GameSys.Tiles;
using SadConsole;

namespace MagusEngine.ECS.Components.ActorComponents
{
    public class IllusionComponent
    {
        public const string Tag = "illusion";

        public readonly ColoredGlyph FakeAppearence;
        //public IObjectWithComponents? Parent { get; set; }

        public IllusionComponent(TileBase fakeAppearence)
        {
            FakeAppearence = new ColoredGlyph(fakeAppearence.Foreground,
                fakeAppearence.Background, fakeAppearence.Glyph);
            FakeAppearence.IsVisible = true;
        }
    }
}