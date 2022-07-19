using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MagiRogue.Data.Enumerators
{
    /// <summary>
    /// What type of furniture is it?
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum FurnitureType
    {
        /// <summary>
        /// Any kind really, too generic to say
        /// </summary>
        General,
        /// <summary>
        /// Stairs down
        /// </summary>
        StairsDown,
        /// <summary>
        /// Stairs up
        /// </summary>
        StairsUp,
        /// <summary>
        /// chair
        /// </summary>
        Chair,
        /// <summary>
        /// Can be a eating table, a studying table, a hitting table
        /// </summary>
        Table,
        /// <summary>
        /// To store books and dust
        /// </summary>
        BookCase,
        /// <summary>
        /// Can be used for crafting
        /// </summary>
        Craft,
        /// <summary>
        /// Furniture that emits light
        /// </summary>
        Light
    }
}