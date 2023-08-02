using Arquimedes.Enumerators;
using MagiRogue.GameSys.Planet.History;
using MagusEngine.Core.Entities.Base;
using MagusEngine.Core.Entities.Interfaces;
using MagusEngine.Serialization;
using MagusEngine.Serialization.EntitySerialization;
using MagusEngine.Utils;
using Newtonsoft.Json;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;

namespace MagusEngine.Core.Entities
{
    /// <summary>
    /// Item: Describes things that can be picked up or used
    /// by actors, or destroyed on the map.
    /// </summary>
    // TODO: Remove inherting from entity and add as a component.
    [JsonConverter(typeof(ItemJsonConverter))]
    public class Item : MagiEntity
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

        public override double Weight
        {
            get
            {
                return MathMagi.GetWeightWithDensity(Material.Density ?? 0, Volume);
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
        /// How fast the item is to attack, heavier itens and lenghtier item suffer
        /// </summary>
        public int SpeedOfAttack { get; set; } = 100;

        /// <summary>
        /// The type of damage the item deals, should be default of blunt type
        /// </summary>
        public DamageTypes ItemDamageType { get; set; } = DamageTypes.Blunt;

        /// <summary>
        /// Actives that the item can do.
        /// </summary>
        public List<IActivable> UseAction { get; set; }
        public List<Trait> Traits { get; set; }
        public List<Quality> Qualities { get; set; }
        public string ItemId { get; set; }
        public WeaponType WeaponType { get; set; }

        public MaterialTemplate Material { get; set; }
        public List<Legend> Legends { get; set; }
        public ItemType ItemType { get; set; }
        public List<Attack> Attacks { get; set; }
        public Limb HeldLimb { get; set; }
        public ArmorType ArmorType { get; set; }
        public int Coverage { get; set; }

        // By default, a new Item is sized 1x1, with a weight of 1, and at 100% condition
        public Item(Color foreground, Color background, string name, int glyph, Point coord, int size,
            int condition = 100, int layer = (int)MapLayer.ITEMS,
            string materialId = "null") :
            base(foreground, background, glyph, coord, layer)
        {
            Volume = size;
            Condition = condition;
            Material = GameSys.Physics.PhysicsManager.SetMaterial(materialId);
            Name = Material is not null ? Material.ReturnNameFromMaterial(name) : "Bugged!";
            UseAction = new();
            Traits = new();
            Qualities = new();
            Legends = new();
            Attacks = new();
        }

        // removes this object from
        // the MultiSpatialMap's list of entities
        // and lets the garbage collector take it
        // out of memory automatically.
        public void RemoveFromMap()
        {
            GameLoop.GetCurrentMap().Remove(this);
        }

        public bool Equip(Actor actor)
        {
            // We need to store our modifiers in variables before adding them to the stat.
            // just example code

            /*c.Strength.AddModifier(new StatModifier(10, StatModType.Flat, this));
            c.Strength.AddModifier(new StatModifier(0.1, StatModType.Percent, this));*/

            if (EquipType == EquipType.None)
            {
                GameLoop.AddMessageLog("This item can't be equiped!");
                return false;
            }

            if (!actor.GetEquipment().TryAdd(actor.GetAnatomy().Limbs.Find
                (l => l.LimbType.ToString() == EquipType.ToString()).Id, this))
            {
                GameLoop.AddMessageLog($"{actor.Name} has already an item equiped in addHere!");
                return false;
            }

            if (EquipType == EquipType.Hand)
            {
                GameLoop.AddMessageLog($"{actor.Name} wields {Name}");
            }
            else
                GameLoop.AddMessageLog($"{actor.Name} equipped {Name} in {EquipType}");
            return true;
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

        public double QualityMultiplier()
        {
            return Qualities.Find(i => i.QualityType is QualityType.ItemQuality).QualitySuitabiliy * 0.3;
        }

        public override string ToString()
        {
            return $"{Name} : Equip {EquipType}";
        }
    }
}