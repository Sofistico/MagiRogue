using MagiRogue.Data.Enumerators;
using MagusEngine.Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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