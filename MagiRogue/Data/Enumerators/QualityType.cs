using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MagiRogue.Data.Enumerators
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum QualityType
    {
        None,
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