﻿using MagiRogue.Utils;
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

        //public int WildMana { get; set; }
        public double BaseManaRegen { get; set; }

        public Soul()
        {
        }

        public void ApplyManaRegen(double manaRegen)
        {
            if (CurrentMana < MaxMana)
            {
                double newMana = (manaRegen + CurrentMana);
                CurrentMana = MathMagi.Round(newMana);
            }
        }

        public void InitialMana(int inteligence, Race race)
        {
            int raceMaxMana = race.MaxManaRange;
            int raceMinMana = race.MinManaRange;
            int statsModifier = ((WillPower + 1) / 2) + ((inteligence + 1) / 2);
            MaxMana = statsModifier + GameLoop.GlobalRand.NextInt(raceMinMana,
                raceMaxMana + 1);
            CurrentMana = MaxMana;
        }
    }
}