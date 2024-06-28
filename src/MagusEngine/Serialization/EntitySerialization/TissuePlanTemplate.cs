using Arquimedes.Enumerators;
using Arquimedes.Interfaces;
using MagusEngine.Core.Entities.Base;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace MagusEngine.Serialization.EntitySerialization
{
    public class TissuePlanTemplate : IJsonKey
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
