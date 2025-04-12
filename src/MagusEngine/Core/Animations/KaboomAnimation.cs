using System.Linq;
using Arquimedes;
using MagusEngine.Bus.MapBus;
using MagusEngine.Core.Entities;
using MagusEngine.Core.Entities.Base;
using MagusEngine.Exceptions;
using MagusEngine.Services;
using MagusEngine.Systems;
using MagusEngine.Utils;
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
            var busService = Locator.GetService<MessageBusService>();
            MagiEntity particle = particleGroup.Particles[1];
            particle.Position = circle.Points.First();
            busService.SendMessage(new AddEntitiyCurrentMap(particle));

            // foreach (var particle in particleGroup.Particles)
            // {
            //     // particle.Position = circle.Points[counter++];
            //     currentMap.AddMagiEntity(particle.);
            // }
            // foreach (var point in circle.Points)
            // {
            //     var particle = particleGroup.Particles[0];
            //     particle.Position = point;
            //     currentMap.AddMagiEntity(particle.);
            // }
        }
    }
}
