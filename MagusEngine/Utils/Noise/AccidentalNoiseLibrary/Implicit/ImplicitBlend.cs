using MagusEngine.Utils.Noise.AccidentalNoiseLibrary;
using System;

namespace MagusEngine.Utils.Noise.AccidentalNoiseLibrary.Implicit
{
    public sealed class ImplicitBlend : ImplicitModuleBase
    {
        public ImplicitBlend(ImplicitModuleBase source, double low = 0.00, double high = 0.00)
        {
            Source = source;
            Low = new ImplicitConstant(low);
            High = new ImplicitConstant(high);
        }

        public ImplicitModuleBase Source { get; set; }

        public ImplicitModuleBase Low { get; set; }

        public ImplicitModuleBase High { get; set; }

        public override double Get(double x, double y)
        {
            var v1 = Low.Get(x, y);
            var v2 = High.Get(x, y);
            var blend = (Source.Get(x, y) + 1.0) * 0.5;
            return Utilities.Lerp(blend, v1, v2);
        }

        public override double Get(double x, double y, double z)
        {
            var v1 = Low.Get(x, y, z);
            var v2 = High.Get(x, y, z);
            var blend = Source.Get(x, y, z);
            return Utilities.Lerp(blend, v1, v2);
        }

        public override double Get(double x, double y, double z, double w)
        {
            var v1 = Low.Get(x, y, z, w);
            var v2 = High.Get(x, y, z, w);
            var blend = Source.Get(x, y, z, w);
            return Utilities.Lerp(blend, v1, v2);
        }

        public override double Get(double x, double y, double z, double w, double u, double v)
        {
            var v1 = Low.Get(x, y, z, w, u, v);
            var v2 = High.Get(x, y, z, w, u, v);
            var blend = Source.Get(x, y, z, w, u, v);
            return Utilities.Lerp(blend, v1, v2);
        }
    }
}
