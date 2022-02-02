using GoRogue.DiceNotation;
using Troschuetz.Random;

namespace MagiRogue.Utils
{
    /// <summary>
    /// MagiRogue random number generation, uses a explodind dice of 2d6 to determine randomness.
    /// </summary>
    public struct Mrn
    {
        public static int Exploding2D6Dice => ExplodingDice();

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

    public class MagiGlobalRandom
    {
        private TRandom rng;
        public int Seed { get; }

        public MagiGlobalRandom(int seed)
        {
            rng = new TRandom(seed);
            Seed = seed;
        }

        public MagiGlobalRandom()
        {
            Seed = GoRogue.Random.GlobalRandom.DefaultRNG.Next();
            rng = new TRandom(Seed);
        }

        public TRandom Random() => rng;
    }
}