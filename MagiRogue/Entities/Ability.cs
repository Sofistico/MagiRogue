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
        public int Id { get; }

        public Ability(AbilityName name, int abilityScore)
        {
            Name = ReturnEnumString(name);
            Score = abilityScore;
            Id = (int)name;
        }

        private static string ReturnEnumString(Enum name)
        {
            string tempName = name.ToString();
            return string.Join(" ", Regex.Split(tempName, @"(?<!^)(?=[A-Z](?![A-Z]|$))"));
        }

        public AbilityName ReturnAbilityEnumFromString()
        {
            try
            {
                return Name switch
                {
                    "Magic Lore" => AbilityName.MagicLore,
                    "Swin" => AbilityName.Swin,
                    _ => throw new AbilityNotFoundExepction("Cound't find the ability in the enum class"),
                };
            }
            catch (AbilityNotFoundExepction)
            {
                return AbilityName.None;
            }
        }

        public static AbilityName ReturnAbilityEnumFromString(string name)
        {
            try
            {
                return name switch
                {
                    "Magic Lore" => AbilityName.MagicLore,
                    "Swin" => AbilityName.Swin,
                    _ => throw new AbilityNotFoundExepction("Cound't find the ability in the enum class"),
                };
            }
            catch (AbilityNotFoundExepction)
            {
                return AbilityName.None;
            }
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