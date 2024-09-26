using System;

namespace AccidentalNoiseLibrary.Implicit
{
    public sealed class ImplicitTiers : ImplicitModuleBase
    {
        public ImplicitTiers(ImplicitModuleBase source, int tiers = 0, bool smooth = true)
        {
            Source = source;
            Tiers = tiers;
            Smooth = smooth;
        }

        public ImplicitModuleBase Source { get; set; }

        public int Tiers { get; set; }

        public bool Smooth { get; set; }

        public override double Get(double x, double y)
        {
            var numsteps = Tiers;
            if (Smooth) --numsteps;
            var val = Source.Get(x, y);
            var tb = Math.Floor(val * numsteps);
            var tt = tb + 1.0;
            var t = val * numsteps - tb;
            tb /= numsteps;
            tt /= numsteps;
            var u = Smooth ? Utilities.QuinticBlend(t) : 0.0;
            return tb + u * (tt - tb);
        }

        public override double Get(double x, double y, double z)
        {
            var numsteps = Tiers;
            if (Smooth) --numsteps;
            var val = Source.Get(x, y, z);
            var tb = Math.Floor(val * numsteps);
            var tt = tb + 1.0;
            var t = val * numsteps - tb;
            tb /= numsteps;
            tt /= numsteps;
            var u = Smooth ? Utilities.QuinticBlend(t) : 0.0;
            return tb + u * (tt - tb);
        }

        public override double Get(double x, double y, double z, double w)
        {
            var numsteps = Tiers;
            if (Smooth) --numsteps;
            var val = Source.Get(x, y, z, w);
            var tb = Math.Floor(val * numsteps);
            var tt = tb + 1.0;
            var t = val * numsteps - tb;
            tb /= numsteps;
            tt /= numsteps;
            var u = Smooth ? Utilities.QuinticBlend(t) : 0.0;
            return tb + u * (tt - tb);
        }

        public override double Get(double x, double y, double z, double w, double u, double v)
        {
            var numsteps = Tiers;
            if (Smooth) --numsteps;
            var val = Source.Get(x, y, z, w, u, v);
            var tb = Math.Floor(val * numsteps);
            var tt = tb + 1.0;
            var t = val * numsteps - tb;
            tb /= numsteps;
            tt /= numsteps;
            var s = Smooth ? Utilities.QuinticBlend(t) : 0.0;
            return tb + s * (tt - tb);
        }
    }
}
