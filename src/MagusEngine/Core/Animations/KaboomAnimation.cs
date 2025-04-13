using System;
using Arquimedes;
using MagusEngine.Bus.MapBus;
using MagusEngine.Core.Entities;
using MagusEngine.Services;
using MagusEngine.Systems;
using MagusEngine.Utils;
using MagusEngine.Utils.Extensions;
using SadRogue.Primitives;

namespace MagusEngine.Core.Animations
{
    public class KaboomAnimation : AnimationBase
    {
        public int Radius { get; set; }
        public int LingeringTicks { get; set; }
        public int GlyphChangeDistance { get; set; }

        public KaboomAnimation() { }

        public override void ScheduleTickAnimation(Point originPos, bool ignoreWall)
        {
            var circle = GeometryUtils.CircleFromOriginPoint(originPos, Radius);
            var busService = Locator.GetService<MessageBusService>();

            foreach (var point in circle.Points)
            {
                var index = Math.Clamp((int)originPos.GetDistance(point), 0, Glyphs.Length - 1);
                var particle = new Particle(
                    MagiPalette.FromHexString(Colors[index]),
                    Color.Black,
                    Glyphs[index],
                    point,
                    LingeringTicks
                );
                var isWall = Find.CurrentMap?.IsTileWalkable(point) ?? false;
                if (!ignoreWall && !isWall)
                    continue;

                busService.SendMessage(new AddEntitiyCurrentMap(particle));
            }
        }
    }
}
