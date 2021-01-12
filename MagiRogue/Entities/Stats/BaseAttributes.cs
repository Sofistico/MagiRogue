using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.Entities.Stats
{
    public class BaseAttributes
    {
        protected bool needsUpdate = true;
        protected float _statValue;

        protected readonly List<StatModifier> statModifiers;
        public readonly IReadOnlyCollection<StatModifier> StatModifiers;

        public float BaseValue;
        protected float lastBaseValue = float.MinValue;

        public virtual float StatValue
        {
            get
            {
                if (needsUpdate || lastBaseValue != BaseValue)
                {
                    lastBaseValue = BaseValue;
                    _statValue = CalculateFinalValue();
                    needsUpdate = false;
                }
                return _statValue;
            }
        }

        public BaseAttributes()
        {
            statModifiers = new List<StatModifier>();
            StatModifiers = statModifiers.AsReadOnly();
        }

        public BaseAttributes(float baseValue) : this()
        {
            BaseValue = baseValue;
            statModifiers = new List<StatModifier>();
            StatModifiers = statModifiers.AsReadOnly();
        }

        public virtual void AddModifier(StatModifier mod)
        {
            needsUpdate = true;
            statModifiers.Add(mod);
            statModifiers.Sort(CompareModifierOrder);
        }

        protected int CompareModifierOrder(StatModifier a, StatModifier b)
        {
            if (a.Order < b.Order)
                return -1;
            else if (a.Order > b.Order)
                return 1;
            return 0; // the same as return a.order == b.order
        }

        public virtual bool RemoveModifier(StatModifier mod)
        {
            if (statModifiers.Remove(mod))
            {
                needsUpdate = true;
                return true;
            }
            return false;
        }

        protected float CalculateFinalValue()
        {
            float finalValue = BaseValue;
            float sumPercentAdd = 0; // This will hold the sum of our "PercentAdd" modifiers

            for (int i = 0; i < statModifiers.Count; ++i)
            {
                StatModifier mod = statModifiers[i];

                if (mod.ModType == StatModType.Flat)
                {
                    finalValue += mod.Value;
                }
                else if (mod.ModType == StatModType.PercentAdd)
                {
                    sumPercentAdd += mod.Value; // Start adding together all modifiers of this type

                    // If we're at the end of the list OR the next modifer isn't of this type
                    if (i + 1 >= statModifiers.Count || statModifiers[i + 1].ModType != StatModType.PercentAdd)
                    {
                        finalValue *= 1 + sumPercentAdd; // Multiply the sum with the "finalValue", like we do for "PercentMult" modifiers
                        sumPercentAdd = 0; // Rests back to zero
                    }
                }
                else if (mod.ModType == StatModType.PercentMulti)
                {
                    finalValue *= 1 + mod.Value;
                }
            }

            // Rounding gets around dumb float calculation errors (like getting 12.0001f, instead of 12f)
            // 4 significant digits is usually precise enough, but feel free to change this to fit your needs
            return (float)Math.Round(finalValue, 4);
        }

        public virtual bool RemoveAllModifierFromSource(object source)
        {
            bool didRemove = false;

            for (int i = statModifiers.Count - 1; i >= 0; i--)
            {
                if (statModifiers[i].Source == source)
                {
                    needsUpdate = true;
                    didRemove = true;
                    statModifiers.RemoveAt(i);
                }
            }

            return didRemove;
        }
    }
}