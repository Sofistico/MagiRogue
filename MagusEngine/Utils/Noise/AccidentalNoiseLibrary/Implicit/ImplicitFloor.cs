using System;

namespace MagusEngine.Utils.Noise.AccidentalNoiseLibrary.Implicit
{
    public sealed class ImplicitFloor : ImplicitModuleBase
    {
        public ImplicitFloor()
        {
            Source = new ImplicitConstant(0.00);
        }

        public ImplicitFloor(ImplicitModuleBase source)
        {
            Source = source;
        }

        public ImplicitModuleBase Source { get; set; }

        public override double Get(double x, double y)
        {
            return Math.Floor(Source.Get(x, y));
        }

        public override double Get(double x, double y, double z)
        {
            return Math.Floor(Source.Get(x, y, z));
        }

        public override double Get(double x, double y, double z, double w)
        {
            return Math.Floor(Source.Get(x, y, z, w));
        }

        public override double Get(double x, double y, double z, double w, double u, double v)
        {
            return Math.Floor(Source.Get(x, y, z, w, u, v));
        }
    }
}
