using MagiRogue.System;
using Microsoft;
using Microsoft.Xna.Framework;
using System.Dynamic;

namespace MagiRogue.Entities
{
    // Item: Describes things that can be picked up or used
    // by actors, or destroyed on the map.
    public class Item : Entity
    {
        // backing field for Condition
        private int _condition;

        public double Weight { get; set; } // Weight of the item in Kilo

        // physical condition of item, in percent
        // 100 = item undamaged
        // 0 = item is destroyed
        public int Condition
        {
            get { return _condition; }
            set
            {
                _condition += value;
                if (_condition < 0)
                    Destroy();
            }
        }

        // By default, a new Item is sized 1x1, with a weight of 1, and at 100% condition
        public Item(Color foreground, Color background, string name, int glyph,
            double weight = 1, int condition = 100, int width = 1, int height = 1, int layer = (int)MapLayer.ITEMS) :
            base(foreground, background, glyph, layer)
        {
            Animation.CurrentFrame[0].Foreground = foreground;
            Animation.CurrentFrame[0].Background = background;
            Animation.CurrentFrame[0].Glyph = glyph;
            Weight = weight;
            Condition = condition;
            Name = name;
        }

        // Destroy this object by removing it from
        // the MultiSpatialMap's list of entities
        // and lets the garbage collector take it
        // out of memory automatically.
        public void Destroy()
        {
            GameLoop.World.CurrentMap.Remove(this);
        }
    }
}