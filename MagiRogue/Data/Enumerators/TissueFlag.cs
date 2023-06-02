using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.Data.Enumerators
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum TissueFlag
    {
        ThickensOnEnergyStore,
        ThickensOnStrTraining,
        Muscular,
        Structural,
        ConnectiveTissueAnchor,
        Scars,
        Artery
    }
}
