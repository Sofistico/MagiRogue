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
    public enum RoomTag
    {
        Generic,
        Inn,
        Temple,
        Blacksmith,
        Clothier,
        Alchemist,
        Hovel,
        Abandoned,
        House,
        Throne,
        Kitchen,
        GenericWorkshop,
        Dinner,
    }
}