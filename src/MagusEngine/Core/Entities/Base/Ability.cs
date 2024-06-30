using Arquimedes.Enumerators;
using MagusEngine.Generators;
using System;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace MagusEngine.Core.Entities.Base
{
    public struct Ability
    {
        public string Name { get; }
        public AbilityCategory Category { get; set; }
        public int Score { get; set; }
        public int Id { get; }
        public int XpTotal { get; set; }
        public readonly int XpRequiredForNextLevel { get => (Score + 1) * 1000 * 2; }

        public Ability(string name, AbilityCategory category, int abilityScore)
        {
            Name = name;
            Category = category;
            Score = abilityScore;
            Id = SequentialIdGenerator.AbilityId;
            XpTotal = abilityScore * 1000;
        }

        public Ability(AbilityCategory name, int abilityScore) : this(ReturnEnumString(name), name, abilityScore)
        {
        }

        private static string ReturnEnumString(Enum name)
        {
            string tempName = name.ToString();
            return string.Join(" ", Regex.Split(tempName, "(?<!^)(?=[A-Z](?![A-Z]|$))", RegexOptions.None, TimeSpan.FromSeconds(10)));
        }

        public static AbilityCategory ReturnAbilityEnumFromString(string name)
        {
            //try
            //{
            //    return name switch
            //    {
            //        "Magic Lore" => AbilityName.MagicLore,
            //        "Swin" => AbilityName.Swin,
            //        _ => throw new AbilityNotFoundExepction("Cound't find the ability in the enum class"),
            //    };
            //}
            //catch (AbilityNotFoundExepction)
            //{
            //    return AbilityName.None;
            //}
            try
            {
                string test = name.Replace(" ", "");
                return Enum.Parse<AbilityCategory>(test);
            }
            catch (AbilityNotFoundExepction)
            {
                return AbilityCategory.None;
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
