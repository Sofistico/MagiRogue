﻿using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.Data.Enumerators
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum MaterialType
    {
        None,
        Stone,
        Meat,
        Bone,
        Metal,
        Glass,
        Wood,
        Plant,
        Scale,
        Skin,
        Fat,
        Fuel,
    }
}