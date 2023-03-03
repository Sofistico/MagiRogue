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
    public enum Actions
    {
        None,
        Eat,
        Sleep,
        Drink,
        Fun,
        Train,
        Pray,
        Study,
        Teach,
        Craft,
        Fight,
        Bully,
        PickUp,
        GoTo
    }
}
