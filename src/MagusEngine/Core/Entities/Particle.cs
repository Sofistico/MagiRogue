using MagusEngine.Core.Entities.Base;
using SadRogue.Primitives;

namespace MagusEngine.Core.Entities
{
    public class Particle : MagiEntity
    {
        public Particle(Color foreground, Color background, int glyph, Point coord, int layer) : base(foreground, background, glyph, coord, layer)
        {
        }
    }

    public class ParticlesGroup
    {
        public uint ID { get; set; }
        public Particle[] Particles { get; set; }
        public char PredominantGlyph { get; set; }

        public ParticlesGroup(Particle[] particles, char predominantGlyph)
        {
            Particles = particles;
            PredominantGlyph = predominantGlyph;
            ID = Locator.GetService<IDGenerator>().UseID();
        }
    }
}
