﻿using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Arquimedes.Enumerators
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum SpellAreaEffect
    {
        /// <summary>
        /// Targets the caster
        /// </summary>
        Self,
        /// <summary>
        /// Targets something hit by another effect of the spell
        /// </summary>
        Struck,
        /// <summary>
        /// Affects the specified target instantly
        /// </summary>
        Target,
        /// <summary>
        /// Targets the specified target as a projectile
        /// </summary>
        Projectile,
        /// <summary>
        /// It's a ray, targets the first thing specified
        /// </summary>
        Ray,
        /// <summary>
        /// Targets everything in a circle radius
        /// </summary>
        Ball,
        /// <summary>
        /// Targets everything in it's path
        /// </summary>
        Beam,
        /// <summary>
        /// Target's everything in a cone radius
        /// </summary>
        Cone,
        /// <summary>
        /// Targets everything on the level
        /// </summary>
        Level,
        /// <summary>
        /// Targets everything on the world tile
        /// </summary>
        WorldTile,
        /// <summary>
        /// Targets the world itself
        /// </summary>
        World,
    }
}
