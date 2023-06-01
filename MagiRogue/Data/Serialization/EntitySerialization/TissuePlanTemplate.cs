using MagiRogue.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.Data.Serialization
{
    public class TissuePlanTemplate
    {
        [JsonRequired]
        public string Id { get; set; }

        public List<Tissue> Tissues { get; set; }
    }
}
