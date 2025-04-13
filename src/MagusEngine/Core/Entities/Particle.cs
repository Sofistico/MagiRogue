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
    }
}
