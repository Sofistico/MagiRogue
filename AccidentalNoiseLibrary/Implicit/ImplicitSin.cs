using System;

namespace AccidentalNoiseLibrary.Implicit
{
    public sealed class ImplicitSin : ImplicitModuleBase
    {
        public ImplicitSin(ImplicitModuleBase source)
        {
            Source = source;
        }

        public ImplicitModuleBase Source { get; set; }

        public override double Get(double x, double y)
        {
            return Math.Sin(Source.Get(x, y));
        }

        public override double Get(double x, double y, double z)
        {
            return Math.Sin(Source.Get(x, y, z));
        }

        public override double Get(double x, double y, double z, double w)
        {
            return Math.Sin(Source.Get(x, y, z, w));
        }

        public override double Get(double x, double y, double z, double w, double u, double v)
        {
            return Math.Sin(Source.Get(x, y, z, w, u, v));
        }
    }
}
