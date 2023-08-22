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
            Abilities = new();
            Personality = new Personality();
            Stats = new Dictionary<string, int>() { { "Inteligence", 0 }, { "Precision", 0 } };
            Memories = new List<IMemory>();
        }

        public void AddAbilityToDictionary(Ability ability)
        {
            if (ability.ReturnAbilityEnumFromString() is AbilityName.None)
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

        public bool HasSpecifiedAttackAbility(AbilityName ability, out int abilityScore)
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

        public int GetAbility(AbilityName ability)
        {
            int possibleId = (int)Ability.ReturnAbilityEnumFromString(ability.ToString());
            int abilityScore = 0;
            if (Abilities.TryGetValue(possibleId, out Ability value))
            {
                abilityScore = value.Score;
            }

            return abilityScore;
        }

        public Ability ReturnAbilityFromName(AbilityName ability)
        {
            int possibleId = (int)Ability.ReturnAbilityEnumFromString(ability.ToString());
            return Abilities.TryGetValue(possibleId, out Ability value) ? value : new Ability();
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
