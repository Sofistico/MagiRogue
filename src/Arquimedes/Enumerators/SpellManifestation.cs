using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Arquimedes.Enumerators
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum SpellManifestation
    {
        /// <summary>
        /// The magic manifests at the target location pretty much instantaneously
        /// </summary>
        Instantaneous,
        /// <summary>
        /// The magic manifests as a projectile that must travel to it's target
        /// </summary>
        Projectile,
        /// <summary>
        /// The magic manifests as a ray that must travel to it's target
        /// </summary>
        Ray,
    }
}
