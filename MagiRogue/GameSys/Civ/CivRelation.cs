using MagiRogue.Data.Enumerators;
using Newtonsoft.Json;

namespace MagiRogue.GameSys.Civ
{
    public class CivRelation
    {
        public int ParentId { get; set; }
        public int OtherCivId { get; set; }
        public RelationType Relation { get; set; }
        public bool RoadBuilt { get; set; }

        [JsonConstructor]
        public CivRelation(int parentId, int otherCivId, RelationType relation)
        {
            ParentId = parentId;
            OtherCivId = otherCivId;
            Relation = relation;
        }
    }
}