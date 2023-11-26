using Arquimedes.Enumerators;
using MagusEngine.Core.Entities.Base;

namespace MagusEngine.Utils
{
    public static class AbilityHelper
    {
        public static Ability[] GetMagicalAbilities()
        {
            var abilitiesNames = new AbilityCategory[]
            {
                AbilityCategory.MagicTheory,
                AbilityCategory.Gestures,
                AbilityCategory.Incantation,
                AbilityCategory.MagicShaping,
            };
            Ability[] abilities = new Ability[abilitiesNames.Length];
            for (int i = 0; i < abilitiesNames.Length; i++)
            {
                abilities[i] = new Ability(abilitiesNames[i], 0);
            }

            return abilities;
        }
    }
}