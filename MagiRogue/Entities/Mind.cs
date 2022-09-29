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
        public Profession Profession { get; set; }

        public Mind()
        {
            Abilities = new();
            Personality = new Personality();
        }

        public void AddAbilityToDictionary(Ability ability)
        {
            if (ability.ReturnAbilityEnumFromString() is AbilityName.None)
                return;
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

        public int GetAbility(AbilityName ability)
        {
            int possibleId = (int)Ability.ReturnAbilityEnumFromString(ability.ToString());
            int abilityScore = 0;
            if (Abilities.ContainsKey(possibleId))
            {
                abilityScore = Abilities[possibleId].Score;
            }

            return abilityScore;
        }

        public Ability ReturnAbilityFromName(AbilityName ability)
        {
            int possibleId = (int)Ability.ReturnAbilityEnumFromString(ability.ToString());
            if (Abilities.ContainsKey(possibleId))
            {
                return Abilities[possibleId];
            }
            else
                return new Ability();
        }

        public List<AbilityName> CheckForCombatAbilities()
        {
            var abilitiesNeeded = new AbilityName[]
            {
                AbilityName.Unarmed,
                AbilityName.Misc,
                AbilityName.Sword,
                AbilityName.Staff,
                AbilityName.Hammer,
                AbilityName.Spear,
                AbilityName.Axe,
            };
            return ReturnIntersectionAbilities(abilitiesNeeded);
        }

        public List<AbilityName> CheckForDefensiveAbilities()
        {
            var abilitiesNeeded = new AbilityName[]
            {
                AbilityName.ArmorUse,
                AbilityName.Dodge,
            };
            return ReturnIntersectionAbilities(abilitiesNeeded);
        }

        public List<AbilityName> ReturnIntersectionAbilities(IEnumerable<AbilityName> abilitiesNeeded)
        {
            return abilitiesNeeded.Where(i => Abilities.ContainsKey((int)i)).ToList();
        }
    }
}