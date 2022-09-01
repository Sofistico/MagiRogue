using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.Data.Enumerators
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Tech
    {
        // magic stuff
        None,
        Gate,
        Magic,
        CounterMagic,
        MagicTheory,
        MagicalReagents,
        Alchemy,
        Enchanting,
        CostOfMagic,
        SoulMagic,
        MedicinalMagic,
        DamagingMagic,
        Rifts,
        Summoning,
        Teleport,
        Rituals,

        // tech stuff
        Gunpowder,
        SteamPower,
        Electricity,
        Metalsmithing,
        Woodcrafting,
        Glassmaking,
        Engineering
    }
}