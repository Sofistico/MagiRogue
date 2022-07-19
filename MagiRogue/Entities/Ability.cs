using MagiRogue.Data.Enumerators;
using System;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace MagiRogue.Entities
{
    public struct Ability
    {
        public string Name { get; }
        public int Score { get; set; }
        public string Speciality { get; set; }
        public int Id { get; }

        public Ability(AbilityName name, int abilityScore, SpecialityType speciality)
        {
            Name = ReturnEnumString(name);
            Score = abilityScore;
            Id = (int)name;
            Speciality = ReturnEnumString(speciality);
        }

        public Ability(string name, int score, string speciality)
        {
            Name = name;
            Score = score;
            Id = (int)ReturnAbilityEnumFromString(name);
            Speciality = speciality;
        }

        private static string ReturnEnumString(Enum name)
        {
            string tempName = name.ToString();
            return string.Join(" ", Regex.Split(tempName, @"(?<!^)(?=[A-Z](?![A-Z]|$))"));
        }

        private static AbilityName ReturnAbilityEnumFromString(string name)
        {
            return name switch
            {
                "Magic Lore" => AbilityName.MagicLore,
                "Swin" => AbilityName.Swin,
                _ => throw new AbilityNotFoundExepction("Cound't find the ability in the enum class"),
            };
        }
    }

    public class AbilityNotFoundExepction : ApplicationException
    {
        public AbilityNotFoundExepction()
        {
        }

        public AbilityNotFoundExepction(string message) : base(message)
        {
        }

        public AbilityNotFoundExepction(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected AbilityNotFoundExepction(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}