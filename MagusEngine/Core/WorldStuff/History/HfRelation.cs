using MagiRogue.Data.Enumerators;

namespace MagusEngine.Core.WorldStuff.History
{
    public class HfRelation
    {
        public int ParentId { get; set; }
        public int OtherHfId { get; set; }

        public HfRelationType RelationType { get; set; }

        public HfRelation(int parentId, int otherHfId, HfRelationType relationType)
        {
            ParentId = parentId;
            OtherHfId = otherHfId;
            RelationType = relationType;
        }
    }
}