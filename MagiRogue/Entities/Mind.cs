using MagiRogue.Data.Enumerators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.Entities
{
    public class Mind
    {
        public int Inteligence { get; set; }
        public int Precision { get; set; }

        /// <summary>
        /// Dictionary of the Abilities of an actor.
        /// Never add directly to the dictionary, use the method AddAbilityToDictionary to add new abilities
        /// </summary>
        public Dictionary<int, Ability> Abilities { get; set; }
        public Personality Personality { get; set; }

        public Mind()
        {
            Abilities = new();
        }

        public void AddAbilityToDictionary(Ability ability)
        {
            Abilities.TryAdd(ability.Id, ability);
        }

        public bool HasSpecifiedAttackAbility(WeaponType weaponType, out int abilityScore)
        {
            int possibleId = (int)Ability.ReturnAbilityEnumFromString(weaponType.ToString());
            abilityScore = 0;
            if (Abilities.ContainsKey(possibleId))
            {
                abilityScore = Abilities[possibleId].Score;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}