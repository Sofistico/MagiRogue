using System;

namespace MagusEngine.Utils.Noise.AccidentalNoiseLibrary
{
    public class CellularGenerator
    {
        private int seed;

        private readonly CellularCache cache2D = new CellularCache();

        private readonly CellularCache cache3D = new CellularCache();

        private readonly CellularCache cache4D = new CellularCache();

        private readonly CellularCache cache6D = new CellularCache();

        public CellularGenerator()
        {
            seed = 10000;
        }

        public int Seed
        {
            get { return seed; }
            set
            {
                if (value == seed)
                    return;
                seed = value;
                cache2D.IsValid = false;
                cache3D.IsValid = false;
                cache4D.IsValid = false;
                cache6D.IsValid = false;
            }
        }

        internal CellularCache Get(double x, double y)
        {
            if (!cache2D.IsValid || x != cache2D.X || y != cache2D.Y)
            {
                Noise.CellularFunction(x, y, seed, cache2D.F, cache2D.D);
                cache2D.X = x;
                cache2D.Y = y;
                cache2D.IsValid = true;
            }
            return cache2D;
        }

        internal CellularCache Get(double x, double y, double z)
        {
            if (!cache3D.IsValid || x != cache3D.X || y != cache3D.Y || z != cache3D.Z)
            {
                Noise.CellularFunction(x, y, z, seed, cache3D.F, cache3D.D);
                cache3D.X = x;
                cache3D.Y = y;
                cache3D.Z = z;
                cache3D.IsValid = true;
            }
            return cache3D;
        }

        internal CellularCache Get(double x, double y, double z, double w)
        {
            if (!cache4D.IsValid || x != cache4D.X || y != cache4D.Y || z != cache4D.Z || w != cache4D.W)
            {
                Noise.CellularFunction(x, y, z, w, seed, cache4D.F, cache4D.D);
                cache4D.X = x;
                cache4D.Y = y;
                cache4D.Z = z;
                cache4D.W = w;
                cache4D.IsValid = true;
            }
            return cache4D;
        }

        internal CellularCache Get(double x, double y, double z, double w, double u, double v)
        {
            if (!cache6D.IsValid || x != cache6D.X || y != cache6D.Y || z != cache6D.Z || w != cache6D.W || u != cache6D.U || v != cache6D.V)
            {
                Noise.CellularFunction(x, y, z, w, u, v, seed, cache6D.F, cache6D.D);
                cache6D.X = x;
                cache6D.Y = y;
                cache6D.Z = z;
                cache6D.W = w;
                cache6D.U = u;
                cache6D.V = v;
                cache6D.IsValid = true;
            }

            return cache6D;
        }
    }
}
