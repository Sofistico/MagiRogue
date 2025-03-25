using System.Linq;
using MagusEngine.Components.TilesComponents;
using MagusEngine.Core.MapStuff;
using MagusEngine.Exceptions;
using MagusEngine.Services;
using MagusEngine.Systems;
using MagusEngine.Utils;
using SadConsole;
using SadRogue.Primitives;

namespace MagusEngine.Core.Animations
{
    public class KaboomAnimation : AnimationBase
    {
        public int Radius { get; set; }
        public int LingeringTicks { get; set; }
        public int GlyphChangeDistance { get; set; }

        public KaboomAnimation() { }

        public override void ScheduleTickAnimation(Point originPos)
        {
            var circle = GeometryUtils.CircleFromOriginPoint(originPos, Radius);
            var totalGlyphs = Glyphs.Length;
            // foreach (var item in Glyphs)
            // {
            //     var particleGroup = new ParticlesGroup();
            // }
            var pos = circle.Points.First();
            Tile terrain = Find.CurrentMap?.GetTileAt(pos)! ?? throw new NullValueException();
            var component = new ExtraAppearanceComponent(
                new ColoredGlyph(Color.GreenYellow, Color.LightBlue, Glyphs[0])
            );
            MagiLog.Log($"{component.SadGlyph.Glyph}");
            MagiLog.Log($"{pos}");
            var currentComp = terrain.GetComponent<ExtraAppearanceComponent>();
            if (currentComp is not null)
                terrain.RemoveComponent(currentComp);
            terrain.AddComponent(component);
        }
    }
}
