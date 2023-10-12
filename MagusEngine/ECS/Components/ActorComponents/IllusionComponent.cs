using MagusEngine.Core.MapStuff;
using SadConsole;

namespace MagusEngine.ECS.Components.ActorComponents
{
    public class IllusionComponent
    {
        public const string Tag = "illusion";

        public readonly ColoredGlyph FakeAppearence;
        //public IObjectWithComponents? Parent { get; set; }

        public IllusionComponent(Tile fakeAppearence)
        {
            FakeAppearence = new ColoredGlyph(fakeAppearence.Appearence.Foreground,
                fakeAppearence.Appearence.Background, fakeAppearence.Appearence.Glyph);
            FakeAppearence.IsVisible = true;
        }
    }
}
