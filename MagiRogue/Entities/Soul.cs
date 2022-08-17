using MagiRogue.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.Entities
{
    public class Soul
    {
        public int MaxMana { get; set; }
        public double CurrentMana { get; set; }
        public int WillPower { get; set; }
        public int WildMana { get; set; }
        public double BaseManaRegen { get; set; }

        public void ApplyManaRegen(double manaRegen)
        {
            if (CurrentMana < MaxMana)
            {
                double newMana = (manaRegen + CurrentMana);
                CurrentMana = MathMagi.Round(newMana);
            }
        }

        public void InitialMana(int inteligence)
        {
            MaxMana = (WillPower / 2) + (inteligence / 2) + GameLoop.GlobalRand.NextInt(10);
            CurrentMana = MaxMana;
        }
    }
}