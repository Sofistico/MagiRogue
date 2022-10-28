using MagiRogue.Data.Enumerators;
using MagiRogue.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.Utils
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