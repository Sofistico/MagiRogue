using Arquimedes.Enumerators;
using MagusEngine.Systems;

namespace MagusEngine.Core.Entities.Base
{
    public class Attack
    {
        private DamageType? damageType;
        public string Name { get; set; }

        /// <summary>
        /// 0 = fist person, 1 = third person
        /// </summary>
        public string[] AttackVerb { get; set; }
        public AbilityCategory AttackAbility { get; set; }
        public int PrepareVelocity { get; set; }
        public int RecoverVelocity { get; set; }
        public string DamageTypeId { get; set; } = null!;

        public DamageType? DamageType
        {
            get
            {
                damageType ??= DataManager.QueryDamageInData(DamageTypeId);
                return damageType;
            }
        }

        /// <summary>
        /// Determines the surface area hit by the weapon.
        /// </summary>
        public int ContactArea { get; set; }
        public double PenetrationPercentage { get; set; }
        public int VelocityMultiplier { get; set; }

        public BodyPartFunction? LimbFunction { get; set; }
        public bool Main { get; set; }
        public bool? AttacksUsesLimbName { get; set; }

        /// <summary>
        /// The pitful kind of attack that all entities have if they don't have ANY other form of attack
        /// </summary>
        /// <returns>The attack if there is no other an entity can do!</returns>
        public static Attack PushAttack()
        {
            return new Attack()
            {
                AttackAbility = AbilityCategory.None,
                AttackVerb = new[] { "push", "pushes" },
                ContactArea = 100,
                DamageTypeId = "blunt",
                LimbFunction = BodyPartFunction.Root,
                Main = true,
                Name = "Push",
                PrepareVelocity = 5,
                RecoverVelocity = 5,
            };
        }
    }
}
