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
    public enum ComparatorEnum
    {
        None,
        NotEqual,
        Equal,
        EqualOrMore,
        More,
        Less,
        LessOrEqual
    }
}
