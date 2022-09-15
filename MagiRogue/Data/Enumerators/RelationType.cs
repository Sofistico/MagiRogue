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
    public enum RelationType
    {
        // relation beetween civs
        Unknow,
        Neutral,
        Friendly,
        Enemy,
        War,
        Allied,

        // relation beetween civ and a hf
        Member,
        LoyalMember,
        TraitorousMember,
        Soldier,
        Ruler,
        PatronDeity
    }
}