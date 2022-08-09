using MagiRogue.Entities;

namespace MagiRogue.Data.Serialization.EntitySerialization
{
    public class AbilityTemplate
    {
        public string Name { get; set; }
        public int Score { get; set; }

        public static implicit operator AbilityTemplate(Ability ability)
        {
            var abilityTemplaye = new AbilityTemplate()
            {
                Name = ability.Name,
                Score = ability.Score,
            };

            return abilityTemplaye;
        }

        public static implicit operator Ability(AbilityTemplate abilityTemplate)
        {
            var ability = new Ability(abilityTemplate.Name, abilityTemplate.Score);
            return ability;
        }
    }
}