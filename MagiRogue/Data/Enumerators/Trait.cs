using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.Data.Enumerators
{
    /// <summary>
    /// Trait is everything that is passive to the object.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Trait
    {
        // TODO: Some will be moved to quality, if something can be interacted it's a quality, if it's a passive quality, it's a trait
        /// <summary>
        /// Shouldn't see it.
        /// </summary>
        None,
        /// <summary>
        /// Confers rest bonus to those who rest on it
        /// </summary>
        Confortable,
        /// <summary>
        /// Is used to research
        /// </summary>
        Research,
        /// <summary>
        /// Used in rituals.
        /// </summary>
        RitualFoci,
        /// <summary>
        /// Can be the foci for spells, raising the shaping skill by half of the ownear while using it, max 2 such items.
        /// </summary>
        SpellFoci,
        /// <summary>
        /// Can inspire research
        /// </summary>
        Inspirational,
        /// <summary>
        /// Can hold items inside of it
        /// </summary>
        HoldItems,
        /// <summary>
        /// Holy to some religion
        /// </summary>
        Holy,
        /// <summary>
        /// Doubles the HP of the item and doubles the force necessary to break it
        /// </summary>
        Durable,
        /// <summary>
        /// Indicates it's something fit for a ruler.
        /// </summary>
        Regal,
        /// <summary>
        /// For the ai to see if it's something it should use for work
        /// </summary>
        WorkItem,
        /// <summary>
        /// Consumes mana to work
        /// </summary>
        Magical,
        /// <summary>
        /// Breaks by applyng pretty much any kind of force in it, halves the force needed to break it and halves hp.
        /// </summary>
        Fragile,
        /// <summary>
        /// doubles the max mp infuse of something
        /// </summary>
        MagicallyPotent,
        /// <summary>
        /// halves the max mp to infuse an item
        /// </summary>
        MagicallyWeak,
        Storage,
    }
}