﻿using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MagiRogue.Data.Enumerators
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OrganType
    {
        None,
        Heart,
        Brain,
        Digestive,
        Intestine,
        Mouth,
        Tongue,
        Nose,
        Teeth,
        Filtering,
        Breather,
        Protective,
        Visual,
        Auditory,
        Nerve
    }
}