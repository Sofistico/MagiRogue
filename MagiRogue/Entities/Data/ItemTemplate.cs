using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.Entities.Data
{
    public class ItemTemplate
    {
        /// <summary>
        /// An item creator, defines the template in how it should be created, acess this to create new items.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="foreground"></param>
        /// <param name="background"></param>
        /// <param name="glyph"></param>
        /// <param name="weight"></param>
        /// <param name="condition">Defaults to 100%</param>
        public ItemTemplate(string name, Color foreground, Color background, int glyph, double weight, int condition = 100)
        {
            Name = name;
            Foreground = foreground;
            Background = background;
            Glyph = glyph;
            Weight = weight;
            Condition = condition;
        }

        public string Name { get; internal set; }
        public Color Foreground { get; internal set; }
        public Color Background { get; internal set; }
        public int Glyph { get; internal set; }
        public double Weight { get; internal set; }
        public int Condition { get; internal set; }
    }
}