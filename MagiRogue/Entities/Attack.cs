using MagiRogue.Data.Enumerators;

namespace MagiRogue.Entities
{
    public class Attack
    {
        public string Name { get; set; }
        public string[] AttackVerb { get; set; }
        public AbilityName AttackAbility { get; set; }
        public int PrepareVelocity { get; set; }
        public int RecoverVelocity { get; set; }
        public DamageTypes DamageTypes { get; set; }
        public int ContactArea { get; set; }
        public int PenetrationValue { get; set; }
        public int VelocityMultiplier { get; set; }
    }
}