using Xunit;

namespace MagiRogue.Test.Utils
{
    public class MrnTest
    {
        [Fact]
        public void MrnExplode()
        {
            int explo = ForceExplosion();

            Assert.Equal(14, explo);
        }

        private static int ForceExplosion()
        {
            int roll1 = 6;
            int roll2 = 6;
            int total = 0;

            do
            {
                int loopTotal = total;
                int sumRoll1 = roll1;
                int sumRoll2 = roll2;

                if (roll1 == 6)
                {
                    roll1 = 2;
                    sumRoll1 += roll1 - 1;
                }
                if (roll2 == 6)
                {
                    roll2 = 2;
                    sumRoll2 += roll2 - 1;
                }
                loopTotal += sumRoll1 + sumRoll2;

                total = loopTotal;
            } while (roll1 == 6 || roll2 == 6);

            return total;
        }
    }
}
