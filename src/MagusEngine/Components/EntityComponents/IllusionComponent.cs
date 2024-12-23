using MagusEngine.Core.MapStuff;
using SadConsole;

namespace MagusEngine.Components.EntityComponents
{
    public class IllusionComponent
    {
        public const string Tag = "illusion";

        public ColoredGlyph FakeAppearence { get; }
        public bool IsMagical { get; set; }
        public bool CanBeBypassed { get; set; }

        public IllusionComponent(Tile fakeAppearence)
        {
            FakeAppearence = new ColoredGlyph(fakeAppearence.Appearence.Foreground,
                fakeAppearence.Appearence.Background, fakeAppearence.Appearence.Glyph)
            {
                IsVisible = true
            };
        }
    }
}
