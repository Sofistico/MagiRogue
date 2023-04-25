using MagiRogue.Data.Enumerators;

namespace MagiRogue.Entities
{
    public class Attack
    {
        public string Name { get; set; }

        // 0 = fist person, 1 = third person
        public string[] AttackVerb { get; set; }
        public AbilityName AttackAbility { get; set; }
        public int PrepareVelocity { get; set; }
        public int RecoverVelocity { get; set; }
        public DamageTypes DamageTypes { get; set; }
        public int ContactArea { get; set; }
        public int PenetrationPercentage { get; set; }
        public int VelocityMultiplier { get; set; }

        public BodyPartFunction? LimbFunction { get; set; }
        public bool Main { get; set; }
        public bool? AttacksUsesLimbName { get; set; }
        public bool? UseAllLimbs { get; set; }

        /// <summary>
        /// The pitful kind of attack that all entities have if they don't have ANY other form of attack
        /// </summary>
        /// <returns>The attack if there is no other an entity can do!</returns>
        public static Attack PushAttack()
        {
            return new Attack()
            {
                AttackAbility = AbilityName.None,
                AttackVerb = new[] { "push", "pushes" },
                ContactArea = 100,
                DamageTypes = DamageTypes.Blunt,
                LimbFunction = BodyPartFunction.Root,
                Main = true,
                Name = "Push",
                PrepareVelocity = 5,
                RecoverVelocity = 5,
            };
        }
    }
}