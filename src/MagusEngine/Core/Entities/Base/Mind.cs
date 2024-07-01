using Arquimedes.Enumerators;
using MagusEngine.Core.Entities.Interfaces;
using MagusEngine.Utils;
using System.Collections.Generic;
using System.Linq;

namespace MagusEngine.Core.Entities.Base
{
    public class Mind : IStat
    {
        public int Inteligence { get => Stats["Inteligence"]; set => Stats["Inteligence"] = value; }
        public int Precision { get => Stats["Precision"]; set => Stats["Precision"] = value; }

        /// <summary>
        /// Dictionary of the Abilities of an actor. Never add directly to the dictionary, use the
        /// method AddAbilityToDictionary to add new abilities
        /// </summary>
        public Dictionary<int, Ability> Abilities { get; set; }
        public Personality Personality { get; set; }
        public Profession Profession { get; set; }
        public Dictionary<string, int> Stats { get; set; }
        public List<IMemory> Memories { get; set; }

        public Mind()
        {
            Abilities = [];
            Personality = new Personality();
            Stats = new Dictionary<string, int>() { { "Inteligence", 0 }, { "Precision", 0 } };
            Memories = [];
        }

        public void AddAbilityToDictionary(Ability ability)
        {
            if (ability.Category is AbilityCategory.None)
                return;
            Abilities.TryAdd(ability.Id, ability);
        }

        public void AddAbilitiesToDictionary(Ability[] abilities, bool randomScore = false)
        {
            for (int i = 0; i < abilities.Length; i++)
            {
                Ability ability = abilities[i];

                if (randomScore)
                    ability.Score = Mrn.Exploding2D6Dice;

                AddAbilityToDictionary(ability);
            }
        }

        public bool HasSpecifiedAttackAbility(AbilityCategory ability, out int abilityScore)
        {
            int possibleId = (int)ability;
            abilityScore = 0;
            if (Abilities.TryGetValue(possibleId, out Ability value))
            {
                abilityScore = value.Score;
                return true;
            }
            else
            {
                return false;
            }
        }

        public int GetAbilityScore(AbilityCategory ability)
        {
            int possibleId = (int)Ability.ReturnAbilityEnumFromString(ability.ToString());
            int abilityScore = 0;
            if (Abilities.TryGetValue(possibleId, out Ability value))
            {
                abilityScore = value.Score;
            }

            return abilityScore;
        }

        public int GetAbilityScore(string ability)
        {
            return Abilities.Values.FirstOrDefault(i => i.Name?.Equals(ability) == true).Score;
        }

        public Ability? GetAbility(AbilityCategory ability)
        {
            int possibleId = (int)Ability.ReturnAbilityEnumFromString(ability.ToString());
            return Abilities.TryGetValue(possibleId, out Ability value) ? value : null;
        }

        public Ability GetAbility(string ability) => Abilities.Values.FirstOrDefault(i => i.Name?.Equals(ability) == true);

        public List<AbilityCategory> CheckForCombatAbilities()
        {
            var abilitiesNeeded = new AbilityCategory[]
            {
                AbilityCategory.Unarmed,
                AbilityCategory.Misc,
                AbilityCategory.Sword,
                AbilityCategory.Staff,
                AbilityCategory.Hammer,
                AbilityCategory.Spear,
                AbilityCategory.Axe,
            };
            return ReturnIntersectionAbilities(abilitiesNeeded);
        }

        public List<AbilityCategory> CheckForDefensiveAbilities()
        {
            var abilitiesNeeded = new AbilityCategory[]
            {
                AbilityCategory.ArmorUse,
                AbilityCategory.Dodge,
            };
            return ReturnIntersectionAbilities(abilitiesNeeded);
        }

        public List<AbilityCategory> ReturnIntersectionAbilities(IEnumerable<AbilityCategory> abilitiesNeeded)
        {
            return abilitiesNeeded.Where(i => Abilities.ContainsKey((int)i)).ToList();
        }

        public bool HasSpecifiedAttackAbility(WeaponType weaponType, out int abilityScore)
        {
            int possibleId = (int)Ability.ReturnAbilityEnumFromString(weaponType.ToString());
            abilityScore = 0;
            if (Abilities.TryGetValue(possibleId, out Ability value))
            {
                abilityScore = value.Score;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
