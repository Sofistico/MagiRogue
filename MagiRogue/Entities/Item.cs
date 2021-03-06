﻿using MagiRogue.System;
using SadRogue.Primitives;
using System;
using Newtonsoft.Json;
using MagiRogue.Entities.Data;

namespace MagiRogue.Entities
{
    /// <summary>
    /// Item: Describes things that can be picked up or used
    /// by actors, or destroyed on the map.
    /// </summary>
    [JsonConverter(typeof(ItemJsonConverter))]
    public class Item : Entity
    {
        // backing field for Condition
        private int condition;

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
        public Item(Color foreground, Color background, string name, int glyph, Point coord, int size,
            float weight = 1, int condition = 100, int layer = (int)MapLayer.ITEMS) :
            base(foreground, background, glyph, coord, layer)
        {
            Size = size;
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

        public void Equip(Actor actor)
        {
            // We need to store our modifiers in variables before adding them to the stat.
            // just example code

            /*c.Strength.AddModifier(new StatModifier(10, StatModType.Flat, this));
            c.Strength.AddModifier(new StatModifier(0.1, StatModType.Percent, this));*/

            throw new NotImplementedException();
        }

        public void Unequip(Actor actor)
        {
            // Here we need to use the stored modifiers in order to remove them.
            // Otherwise they would be "lost" in the stat forever.
            // just example code
            //c.Strength.RemoveAllModifiersFromSource(this);

            throw new NotImplementedException();
        }
    }
}