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
    public enum QualityType
    {
        None,
        Forge,
        Smelt,
        Anvil,
        WoodCraft,
        Enchant,
        VisExtract,
        FineWorking,
        Weaving,
        Lockpick,
        Hammer,
        Pry,
    }
}