using MagiRogue.Data;
using MagiRogue.GameSys;
using MagiRogue.Utils;
using Newtonsoft.Json;
using SadRogue.Primitives;
using System;
using MagiRogue.Data.Serialization;
using System.Linq;
using System.Collections.Generic;
using MagiRogue.Data.Serialization.EntitySerialization;
using MagiRogue.Data.Enumerators;

namespace MagiRogue.Entities
{
    /// <summary>
    /// Item: Describes things that can be picked up or used
    /// by actors, or destroyed on the map.
    /// </summary>
    // TODO: Remove inherting from entity and add as a component.
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
                condition = value;
                if (condition < 0)
                {
                    if (CurrentMap is not null)
                    {
                        RemoveFromMap();
                    }
                    condition = 0;
                }
            }
        }

        /// <summary>
        /// In what slot can this item be equiped? None means you can't equip the item
        /// </summary>
        public EquipType EquipType { get; set; }

        /// <summary>
        /// The damage that an item will deal if hitting someone with it, should alway be 1.
        /// </summary>
        public int BaseDmg { get; set; } = 1;

        /// <summary>
        /// The type of damage the item deals, should be default of blunt type
        /// </summary>
        public DamageType ItemDamageType { get; set; } = DamageType.Blunt;

        /// <summary>
        /// Actives that the item can do.
        /// </summary>
        public List<IActivable> Actives { get; set; }
        public List<Trait> Traits { get; set; }
        public List<Quality> Qualities { get; internal set; }
        public string ItemId { get; set; }

        // By default, a new Item is sized 1x1, with a weight of 1, and at 100% condition
        public Item(Color foreground, Color background, string name, int glyph, Point coord, int size,
            float weight = 1, int condition = 100, int layer = (int)MapLayer.ITEMS,
            string materialId = "null") :
            base(foreground, background, glyph, coord, layer)
        {
            Size = size;
            Weight = weight;
            Condition = condition;
            Material = GameSys.Physics.PhysicsManager.SetMaterial(materialId);
            Name = Material.ReturnNameFromMaterial(name);
            Actives = new();
            Traits = new();
            Qualities = new();
        }

        // removes this object from
        // the MultiSpatialMap's list of entities
        // and lets the garbage collector take it
        // out of memory automatically.
        public void RemoveFromMap()
        {
            GameLoop.GetCurrentMap().Remove(this);
        }

        public void Equip(Actor actor)
        {
            // We need to store our modifiers in variables before adding them to the stat.
            // just example code

            /*c.Strength.AddModifier(new StatModifier(10, StatModType.Flat, this));
            c.Strength.AddModifier(new StatModifier(0.1, StatModType.Percent, this));*/

            if (EquipType == EquipType.None)
            {
                GameLoop.UIManager.MessageLog.Add("This item can't be equiped!");
                return;
            }

            if (!actor.Equipment.TryAdd(actor.Anatomy.Limbs.Find
                (l => l.TypeLimb.ToString() == EquipType.ToString()), this))
            {
                GameLoop.UIManager.MessageLog.Add($"{actor.Name} has already an item equiped in addHere!");
                return;
            }

            if (EquipType == EquipType.Hand)
            {
                GameLoop.UIManager.MessageLog.Add($"{actor.Name} wields {Name}");
            }
            else
                GameLoop.UIManager.MessageLog.Add($"{actor.Name} equipped {Name} in {EquipType}");
        }

        public void Unequip(Actor actor)
        {
            // Here we need to use the stored modifiers in order to remove them.
            // Otherwise they would be "lost" in the stat forever.
            // just example code
            //c.Strength.RemoveAllModifiersFromSource(this);

            throw new NotImplementedException();
        }

        public int DamageWhenItemStrikes(int itemAceleration)
        {
            return GameSys.Physics.PhysicsManager.CalculateStrikeForce(Weight, itemAceleration) + BaseDmg;
        }

        public override string ToString()
        {
            return $"{Name} : Equip {EquipType}";
        }
    }
}