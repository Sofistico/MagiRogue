using MagiRogue.Data.Enumerators;

namespace MagusEngine.Core.WorldStuff.History
{
    public class SiteRelation
    {
        public int ParentId { get; set; }
        public int OtherSiteId { get; set; }
        public SiteRelationTypes RelationType { get; set; }

        public SiteRelation(int parentId, int otherSiteId, SiteRelationTypes relationType)
        {
            ParentId = parentId;
            OtherSiteId = otherSiteId;
            RelationType = relationType;
        }
    }
}