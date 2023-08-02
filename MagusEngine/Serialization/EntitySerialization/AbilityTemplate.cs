using MagiRogue.Data.Enumerators;
using MagiRogue.Entities.Core;

namespace MagusEngine.Serialization.EntitySerialization
{
    public class AbilityTemplate
    {
        public AbilityName Ability { get; set; }
        public int Score { get; set; }

        public static implicit operator AbilityTemplate(Ability ability)
        {
            var abilityTemplaye = new AbilityTemplate()
            {
                Ability = ability.ReturnAbilityEnumFromString(),
                Score = ability.Score,
            };

            return abilityTemplaye;
        }

        public static implicit operator Ability(AbilityTemplate abilityTemplate)
        {
            var ability = new Ability(abilityTemplate.Ability, abilityTemplate.Score);
            return ability;
        }
    }
}