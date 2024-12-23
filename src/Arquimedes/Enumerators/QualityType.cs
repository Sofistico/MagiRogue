﻿using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Arquimedes.Enumerators
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum QualityType
    {
        None,
        /// <summary>
        /// How well made the item is!
        /// </summary>
        ItemQuality,
        Forge,
        Smelt,
        Anvil,
        GeneralCraft,
        WoodCraft,
        Enchant,
        VisExtract,
        FineWorking,
        Weaving,
        Lockpick,
        Hammer,
        Pry,
        Art,
        Bake,
        Food,
        Alchemy,
        Distill,
        Mix,
        BatchPotion,
        Potion
    }
}