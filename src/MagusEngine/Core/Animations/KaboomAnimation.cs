using Arquimedes.Enumerators;
using MagusEngine.Bus.UiBus;
using MagusEngine.Components.EntityComponents;
using MagusEngine.Components.TilesComponents;
using MagusEngine.Core.Animations.Interfaces;
using MagusEngine.Core.Entities;
using MagusEngine.Core.MapStuff;
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

        public KaboomAnimation()
        {
        }

        public override void ScheduleTickAnimation(Point originPos)
        {
            var circle = GeometryUtils.CircleFromOriginPoint(originPos, Radius);
            var totalGlyphs = Glyphs.Length;
            // foreach (var item in Glyphs)
            // {
            //     var particleGroup = new ParticlesGroup();
            // }
            foreach (var pos in circle.Points)
            {
                var terrain = Find.CurrentMap.GetTileAt(pos);
                var component = new ExtraAppearanceComponent(new ColoredGlyph(Color.GreenYellow, Color.LightBlue, Glyphs[0]));
                Locator.GetService<MagiLog>().Log($"{component.SadGlyph.Glyph}");
                Locator.GetService<MagiLog>().Log($"{pos}");
                var currentComp = terrain.GetComponent<ExtraAppearanceComponent>();
                terrain.RemoveComponent(currentComp);
                terrain.AddComponent(component);
            }
        }
    }
}
