using MagiRogue.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.Data.Serialization.EntitySerialization
{
    public class AbilityTemplate
    {
        public string Name { get; set; }
        public int Score { get; set; }
        public string Speciality { get; set; }

        public static implicit operator AbilityTemplate(Ability ability)
        {
            var abilityTemplaye = new AbilityTemplate()
            {
                Name = ability.Name,
                Score = ability.Score,
                Speciality = ability.Speciality
            };

            return abilityTemplaye;
        }

        public static implicit operator Ability(AbilityTemplate abilityTemplate)
        {
            var ability = new Ability(abilityTemplate.Name, abilityTemplate.Score, abilityTemplate.Speciality);
            return ability;
        }
    }
}