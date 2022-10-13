using MagiRogue.Data.Enumerators;
using Newtonsoft.Json;
using System;

namespace MagiRogue.GameSys.Civ
{
    public class CivRelation
    {
        public int ParentId { get; set; }
        public int CivRelatedId { get; set; }
        public RelationType Relation { get; set; }
        public bool? RoadBuilt { get; set; }

        [JsonConstructor]
        public CivRelation(int parentId, int otherCivId, RelationType relation)
        {
            ParentId = parentId;
            CivRelatedId = otherCivId;
            Relation = relation;
        }

        public bool GetIfMember()
        {
            return Relation is RelationType.Member
                || Relation is RelationType.LoyalMember
                || Relation is RelationType.TraitorousMember
                || Relation is RelationType.Ruler
                || Relation is RelationType.Soldier;
        }
    }
}