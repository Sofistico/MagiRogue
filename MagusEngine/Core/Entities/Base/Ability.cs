﻿using Arquimedes.Enumerators;
using MagusEngine.Generators;
using System;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace MagusEngine.Core.Entities.Base
{
    public struct Ability
    {
        public string Name { get; }
        public int Score { get; set; }
        public int Id { get; }
        public int XpTotal { get; set; }
        public readonly int XpRequiredForNextLevel { get => (Score + 1) * 1000 * 2; }

        public Ability(AbilityName name, int abilityScore)
        {
            Name = ReturnEnumString(name);
            Score = abilityScore;
            Id = (int)name;
            XpTotal = abilityScore * 1000;
        }

        public Ability(string name, int abilityScore)
        {
            Name = name;
            Score = abilityScore;
            Id = SequentialIdGenerator.AbilityId;
            XpTotal = abilityScore * 1000;
        }

        private static string ReturnEnumString(Enum name)
        {
            string tempName = name.ToString();
            return string.Join(" ", Regex.Split(tempName, "(?<!^)(?=[A-Z](?![A-Z]|$))", RegexOptions.None, TimeSpan.FromSeconds(10)));
        }

        public readonly AbilityName ReturnAbilityEnumFromString()
        {
            try
            {
                string test = Name.Replace(" ", "");
                AbilityName ability = Enum.Parse<AbilityName>(test);
                return ability;
            }
            catch (AbilityNotFoundExepction)
            {
                return AbilityName.None;
            }
        }

        public static AbilityName ReturnAbilityEnumFromString(string name)
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
                AbilityName ability = Enum.Parse<AbilityName>(test);
                return ability;
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