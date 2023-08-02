using System;

namespace MagusEngine.Utils.Noise.AccidentalNoiseLibrary.Implicit
{
    public sealed class ImplicitInvert : ImplicitModuleBase
    {
        public ImplicitInvert(ImplicitModuleBase source)
        {
            Source = source;
        }

        public ImplicitModuleBase Source { set; get; }

        public override double Get(double x, double y)
        {
            return -Source.Get(x, y);
        }

        public override double Get(double x, double y, double z)
        {
            return -Source.Get(x, y, z);
        }

        public override double Get(double x, double y, double z, double w)
        {
            return -Source.Get(x, y, z, w);
        }

        public override double Get(double x, double y, double z, double w, double u, double v)
        {
            return -Source.Get(x, y, z, w, u, v);
        }
    }
}
