using MagiRogue.Data.Enumerators;

namespace MagiRogue.GameSys.Planet.History
{
    public class SiteRelation
    {
        public int ParentId { get; set; }
        public int OtherHfId { get; set; }
        public SiteRelationType RelationType { get; set; }

        public SiteRelation(int parentId, int otherHfId, SiteRelationType relationType)
        {
            ParentId = parentId;
            OtherHfId = otherHfId;
            RelationType = relationType;
        }
    }
}