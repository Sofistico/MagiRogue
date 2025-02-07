using Arquimedes.Enumerators;
using MagusEngine.Core.Entities.Base;
using SadRogue.Primitives;

namespace MagusEngine.Core.Entities
{
    public class Particle : MagiEntity
    {
        public Particle(Color foreground, Color background, int glyph, Point coord, int layer = (int)MapLayer.SPECIAL) : base(foreground, background, glyph, coord, layer)
        {
        }

        public static Particle[] CreateVariousParticles(int count, Color foreground, Color background, int glyph)
        {
            var particles = new Particle[count];

            for (int i = 0; i < count; i++)
            {
                var particle = new Particle(foreground, background, glyph, Point.None);

                particles[i] = particle;
            }
            return particles;
        }
    }

    public class ParticlesGroup
    {
        public Particle[] Particles { get; set; }

        public ParticlesGroup(int count, Color foreground, Color background, int glyph)
        {
            Particles = Particle.CreateVariousParticles(count, foreground, background, glyph);
        }
    }
}
