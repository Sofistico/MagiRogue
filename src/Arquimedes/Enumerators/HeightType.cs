﻿using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Arquimedes.Enumerators
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum HeightType
    {
        Error,
        DeepWater,
        ShallowWater,
        Shore,
        Sand,
        Grass,
        Forest,
        Mountain,
        Snow,
        River,
        HighMountain
    }
}