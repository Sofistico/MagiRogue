using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arquimedes.Enumerators
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Tech
    {
        // default
        None,

        // magic stuff
        Gate,
        SpellCasting,
        CounterMagic,
        MagicTheory,
        MagicalReagents,
        Alchemy,
        Enchanting,
        SoulMagic,
        CrudeMedicinalMagic,
        MedicinalMagic,
        DamagingMagic,
        Summoning,
        Teleport,
        Rituals,
        BasicNodeUse,

        // tech stuff
        Gunpowder,
        SteamPower,
        Electricity,
        BasicMetalsmithing,
        Metalsmithing,
        Woodcrafting,
        Glassmaking,
        Engineering,
        Steelmaking,
        BlastFurnace,

        // science or mathematical discoveries
        BasicMathematics,
        AdvancedMathematics,
        PhysicsConstants,

        ChemicalProperties,

        BasicAstronomy,
        BasicSpaceTravel,

        FolkMedicine,
        AlchemicalMedicine,
        BasicAnatomy,
        WoundCleaning,
        AdvancedAnatomy,
        Prosthetics,
    }
}