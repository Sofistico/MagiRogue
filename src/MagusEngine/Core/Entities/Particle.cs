using Arquimedes.Enumerators;
using MagusEngine.Components.EntityComponents.Effects;
using MagusEngine.Core.Entities.Base;
using MagusEngine.Systems;
using SadRogue.Primitives;

namespace MagusEngine.Core.Entities
{
    public class Particle : MagiEntity
    {
        public Particle(Color foreground, Color background, int glyph, Point coord, long ticks, int layer = (int)MapLayer.SPECIAL) : base(foreground, background, glyph, coord, layer)
        {
            var comp = new LimitedLifeComponent(Find.Time.Tick, Find.Time.Tick + ticks);
            AddComponent(comp, comp.Tag);
        }

        public static Particle[] CreateVariousParticles(Color foreground, Color background, char[] glyphs, long ticks)
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

        public ParticlesGroup(Color foreground, Color background, char[] glyphs, long ticks)
        {
            Particles = Particle.CreateVariousParticles(foreground, background, glyphs, ticks);
        }
    }
}
