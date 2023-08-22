using GoRogue.Random;
using MagusEngine.Core.Entities.Interfaces;
using MagusEngine.Utils;
using System.Collections.Generic;

namespace MagusEngine.Core.Entities.Base
{
    public class Soul : IStat
    {
        public int MaxMana { get; set; }
        public double CurrentMana { get; set; }
        public int WillPower { get => Stats["WillPower"]; set => Stats["WillPower"] = value; }

        //public int WildMana { get; set; }
        public double BaseManaRegen { get; set; }

        /// <summary>
        /// The sense of self of the entity, is a percentage. For a future working in the rituals
        /// and pact update....
        /// </summary>
        public int SenseOfSelf { get => Stats["SenseOfSelf"]; set => Stats["SenseOfSelf"] = value; }

        public Dictionary<string, int> Stats { get; set; }

        public Soul()
        {
            Stats = new Dictionary<string, int>() { { "WillPower", 0 }, { "SenseOfSelf", 100 } };
        }

        public void ApplyManaRegen(double manaRegen)
        {
            if (CurrentMana < MaxMana)
            {
                double newMana = manaRegen + CurrentMana;
                CurrentMana = MathMagi.Round(newMana);
            }
        }

        public void InitialMana(int inteligence, Race race)
        {
            int raceMaxMana = race.MaxManaRange;
            int raceMinMana = race.MinManaRange;
            int statsModifier = (int)((WillPower + 1) * 0.3 + ((inteligence + 1) * 0.2));
            MaxMana = statsModifier + GlobalRandom.DefaultRNG.NextInt(raceMinMana,
                raceMaxMana + 1);
            CurrentMana = MaxMana;
        }
    }
}