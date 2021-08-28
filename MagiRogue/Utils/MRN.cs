using GoRogue.DiceNotation;

namespace MagiRogue.Utils
{
    /// <summary>
    /// MagiRogue random number generation, uses a explodind dice of 2d6 to determine randomness.
    /// </summary>
    public struct MRN
    {
        public static int ExplodingD6Dice => ExplodingDice();

        /// <summary>
        /// This uses a similar number
        /// </summary>
        /// <returns></returns>
        private static int ExplodingDice()
        {
            int roll1 = Dice.Roll("1d6");
            int roll2 = Dice.Roll("1d6");
            int total = 0;

            do
            {
                int loopTotal = total;
                int sumRoll1 = roll1;
                int sumRoll2 = roll2;

                if (roll1 == 6)
                {
                    roll1 = Dice.Roll("1d6 - 1");
                    sumRoll1 += roll1;
                }
                if (roll2 == 6)
                {
                    roll2 = Dice.Roll("1d6 - 1");
                    sumRoll2 += roll2;
                }

                loopTotal += sumRoll1 + sumRoll2;
                total = loopTotal;
            } while (roll1 == 6 || roll2 == 6);

            return total;
        }
    }
}