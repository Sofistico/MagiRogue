using System;

namespace AccidentalNoiseLibrary.Implicit
{
    public sealed class ImplicitPow : ImplicitModuleBase
    {
        public ImplicitPow(ImplicitModuleBase source, double power)
        {
            Source = source;
            Power = new ImplicitConstant(power);
        }

        public ImplicitPow(ImplicitModuleBase source, ImplicitModuleBase power)
        {
            Source = source;
            Power = power;
        }

        public ImplicitModuleBase Source { get; set; }

        public ImplicitModuleBase Power { get; set; }

        public override double Get(double x, double y)
        {
            return Math.Pow(Source.Get(x, y), Power.Get(x, y));
        }

        public override double Get(double x, double y, double z)
        {
            return Math.Pow(Source.Get(x, y, z), Power.Get(x, y, z));
        }

        public override double Get(double x, double y, double z, double w)
        {
            return Math.Pow(Source.Get(x, y, z, w), Power.Get(x, y, z, w));
        }

        public override double Get(double x, double y, double z, double w, double u, double v)
        {
            return Math.Pow(Source.Get(x, y, z, w, u, v), Power.Get(x, y, z, w, u, v));
        }
    }
}