using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.Entities
{
    public class Furniture : Entities.Entity
    {
        public List<IFurnitureEffect> FurnitureEffects { get; set; }

        public Furniture(Color foreground, Color background, int glyph, Point coord, int layer)
            : base(foreground, background, glyph, coord, layer)
        {
        }
    }

    public interface IFurnitureEffect
    {
        public string EffectString { get; set; }

        public void Effect();
    }
}