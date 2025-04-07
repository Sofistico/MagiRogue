using Arquimedes.Enumerators;
using MagusEngine.Components.EntityComponents;
using MagusEngine.Core.Entities.Base;
using SadRogue.Primitives;

namespace MagusEngine.Core.Entities
{
    public class Particle : MagiEntity
    {
        public Particle(Color foreground, Color background, int glyph, Point coord, uint ticks, int layer = (int)MapLayer.SPECIAL) : base(foreground, background, glyph, coord, layer)
        {
            AddComponent(new LimitedLifeComponent(ticks));
        }

        public static Particle[] CreateVariousParticles(Color foreground, Color background, char[] glyphs, uint ticks)
        {
            var particles = new Particle[glyphs.Length];

            for (int i = 0; i < glyphs.Length; i++)
            {
                var glyph = glyphs[i];
                var particle = new Particle(foreground, background, glyph, Point.None, ticks);

                particles[i] = particle;
            }
            return particles;
        }
    }

    public class ParticlesGroup
    {
        public Particle[] Particles { get; set; }

        public ParticlesGroup(Color foreground, Color background, char[] glyphs, uint ticks)
        {
            Particles = Particle.CreateVariousParticles(foreground, background, glyphs, ticks);
        }
    }
}
