using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace MagiRogue.Entities.Data
{
    public class ActorTemplate
    {
        public Color Foreground { get; set; }
        public Color Background { get; set; }
        public int Glyph { get; set; }
        public Stat Stats { get; set; }
        public Anatomy Anatomy { get; set; }

        public ActorTemplate(Color foreground, Color background, int glyph, Stat stats, Anatomy anatomy)
        {
            Foreground = foreground;
            Background = background;
            Glyph = glyph;
            Stats = stats;
            Anatomy = anatomy;
        }
    }
}