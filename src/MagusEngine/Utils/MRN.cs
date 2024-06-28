using GoRogue.DiceNotation;
using GoRogue.Random;

namespace MagusEngine.Utils
{
    /// <summary>
    /// MagiRogue random number generation, uses a explodind dice of 2d6 to determine randomness.
    /// </summary>
    public struct Mrn
    {
        public static int Exploding2D6Dice => ExplodingDice();
        public static int Normal2D6Dice => Dice.Roll("2d6");

        public static int Normal1D100Dice => Dice.Roll("1d100");

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

        public static bool OneIn(int chance)
        {
            return chance <= 1 || GlobalRandom.DefaultRNG.NextInt(0, chance) == 0;
        }

        public static bool XinY(int x, int y)
        {
            return x <= y || GlobalRandom.DefaultRNG.NextInt(x, y) == x;
        }

        public static int CustomDice(string diceExpression)
            => Dice.Roll(diceExpression);
    }
}