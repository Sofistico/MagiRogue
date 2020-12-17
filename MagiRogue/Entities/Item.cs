using MagiRogue.System;
using MagiRogue.Entities.Materials;
using Microsoft.Xna.Framework;

namespace MagiRogue.Entities
{
    // Item: Describes things that can be picked up or used
    // by actors, or destroyed on the map.
    // TODO: jsonize it.
    public class Item : Entity
    {
        // backing field for Condition
        private int condition;

        public double Weight { get; set; } // Weight of the item in Kilo

        public Material Material { get; set; }

        // physical condition of item, in percent
        // 100 = item undamaged
        // 0 = item is destroyed
        public int Condition
        {
            get { return condition; }
            set
            {
                condition += value;
                if (condition < 0)
                    Destroy();
            }
        }

        // By default, a new Item is sized 1x1, with a weight of 1, and at 100% condition
        public Item(Color foreground, Color background, string name, int glyph,
            double weight = 1, int condition = 100, int width = 1, int height = 1, int layer = (int)MapLayer.ITEMS) :
            base(foreground, background, glyph, layer)
        {
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