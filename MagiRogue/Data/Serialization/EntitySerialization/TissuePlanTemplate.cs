using MagiRogue.Data.Enumerators;
using MagiRogue.Entities.Core;
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
        public List<TissueLayeringTemplate> TissueLayering { get; set; }
    }

    public class TissueLayeringTemplate
    {
        public SelectContext Select { get; set; }
        public string[] Tissues { get; set; }
        public string[] BodyParts { get; set; }
    }
}
