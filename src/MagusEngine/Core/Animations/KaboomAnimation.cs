using System.Linq;
using Arquimedes;
using MagusEngine.Components.TilesComponents;
using MagusEngine.Core.Entities;
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
            var particleGroup = new ParticlesGroup(
                MagiPalette.FromHexString(Colors[0]),
                Color.Black,
                Glyphs,
                (uint)LingeringTicks
            );
        }
    }
}
