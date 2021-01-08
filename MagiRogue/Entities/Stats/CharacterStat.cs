using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.Entities.Stats
{
    public class CharacterStat
    {
        private bool needsUpdate = true;
        private float _statValue;

        private readonly List<StatModifier> statModifiers;

        public float BaseValue;

        public float StatValue
        {
            get
            {
                if (needsUpdate)
                {
                    _statValue = CalculateFinalValue();
                    needsUpdate = false;
                }
                return _statValue;
            }
        }

        public CharacterStat(float baseValue)
        {
            BaseValue = baseValue;
            statModifiers = new List<StatModifier>();
        }

        public void AddModifier(StatModifier mod)
        {
            needsUpdate = true;
            statModifiers.Add(mod);
            statModifiers.Sort(CompareModifierOrder);
        }

        private int CompareModifierOrder(StatModifier a, StatModifier b)
        {
            if (a.Order < b.Order)
                return -1;
            else if (a.Order > b.Order)
                return 1;
            return 0; // the same as return a.order == b.order
        }

        public bool RemoveModifier(StatModifier mod)
        {
            needsUpdate = true;
            return statModifiers.Remove(mod);
        }

        private float CalculateFinalValue()
        {
            float finalValue = BaseValue;

            for (int i = 0; i < statModifiers.Count; ++i)
            {
                StatModifier mod = statModifiers[i];

                if (mod.ModType == StatModType.Flat)
                {
                    finalValue += mod.Value;
                }
                else if (mod.ModType == StatModType.Percent)
                {
                    finalValue *= 1 + mod.Value;
                }
            }

            // Rounding gets around dumb float calculation errors (like getting 12.0001f, instead of 12f)
            // 4 significant digits is usually precise enough, but feel free to change this to fit your needs
            return (float)Math.Round(finalValue, 4);
        }
    }
}