using Arquimedes.Enumerators;
using MagusEngine.Core.Entities.Base;

namespace MagusEngine.Utils
{
    public static class AbilityHelper
    {
        public static Ability[] GetMagicalAbilities()
        {
            var abilitiesNames = new AbilityName[]
            {
                AbilityName.MagicTheory,
                AbilityName.Gestures,
                AbilityName.Incantation,
                AbilityName.ManaShaping,
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