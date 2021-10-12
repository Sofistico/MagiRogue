using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.Utils
{
    public static class MathMagi
    {
        public static float ReturnPositive(float nmb)
        {
            float positive;
            if (nmb < 0)
                positive = nmb * -1;
            else
                positive = nmb;
            return positive;
        }
    }
}
