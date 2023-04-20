using MagiRogue.Data.Enumerators;

namespace MagiRogue.Entities
{
    public class BodyPartAttack : Attack
    {
        public LimbType LimbType { get; set; }
        public bool Main { get; set; }
        public bool AttacksUsesLimbName { get; set; }
    }
}
