using Arquimedes.Enumerators;
using MagusEngine.Core.Entities.Base;

namespace MagusEngine.Serialization.EntitySerialization
{
    public class AbilityTemplate
    {
        public string Name { get; set; }
        public AbilityCategory Category { get; set; }
        public int Score { get; set; }

        public static implicit operator AbilityTemplate(Ability ability)
        {
            return new AbilityTemplate()
            {
                Category = ability.Category,
                Score = ability.Score,
                Name = ability.Name
            };
        }

        public static implicit operator Ability(AbilityTemplate abilityTemplate)
        {
            return new Ability(abilityTemplate.Name, abilityTemplate.Category, abilityTemplate.Score);
        }
    }
}