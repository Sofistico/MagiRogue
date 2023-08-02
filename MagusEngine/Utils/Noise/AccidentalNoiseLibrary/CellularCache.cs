using System;

namespace MagusEngine.Utils.Noise.AccidentalNoiseLibrary
{
    internal class CellularCache
    {
        public double X = 0.00;

        public double Y = 0.00;

        public double Z = 0.00;

        public double W = 0.00;

        public double U = 0.00;

        public double V = 0.00;

        public bool IsValid = false;

        public double[] F = new double[4];

        public double[] D = new double[4];
    }
}