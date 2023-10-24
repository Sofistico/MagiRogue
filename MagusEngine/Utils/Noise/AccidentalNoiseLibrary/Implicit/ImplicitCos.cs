using System;

namespace MagusEngine.Utils.Noise.AccidentalNoiseLibrary.Implicit
{
    public sealed class ImplicitCos : ImplicitModuleBase
    {
        public ImplicitCos(ImplicitModuleBase source)
        {
            Source = source;
        }

        public ImplicitModuleBase Source { get; set; }

        public override double Get(double x, double y)
        {
            return Math.Cos(Source.Get(x, y));
        }

        public override double Get(double x, double y, double z)
        {
            return Math.Cos(Source.Get(x, y, z));
        }

        public override double Get(double x, double y, double z, double w)
        {
            return Math.Cos(Source.Get(x, y, z, w));
        }

        public override double Get(double x, double y, double z, double w, double u, double v)
        {
            return Math.Cos(Source.Get(x, y, z, w, u, v));
        }
    }
}
