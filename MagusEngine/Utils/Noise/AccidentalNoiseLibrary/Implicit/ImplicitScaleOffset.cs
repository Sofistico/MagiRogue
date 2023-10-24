using System;

namespace MagusEngine.Utils.Noise.AccidentalNoiseLibrary.Implicit
{
    public sealed class ImplicitScaleOffset : ImplicitModuleBase
    {
        public ImplicitScaleOffset(ImplicitModuleBase source, double scale = 1.00, double offset = 0.00)
        {
            Source = source;
            Scale = new ImplicitConstant(scale);
            Offset = new ImplicitConstant(offset);
        }

        public ImplicitModuleBase Source { get; set; }

        public ImplicitModuleBase Scale { get; set; }

        public ImplicitModuleBase Offset { get; set; }

        public override double Get(double x, double y)
        {
            return Source.Get(x, y) * Scale.Get(x, y) + Offset.Get(x, y);
        }

        public override double Get(double x, double y, double z)
        {
            return Source.Get(x, y, z) * Scale.Get(x, y, z) + Offset.Get(x, y, z);
        }

        public override double Get(double x, double y, double z, double w)
        {
            return Source.Get(x, y, z, w) * Scale.Get(x, y, z, w) + Offset.Get(x, y, z, w);
        }

        public override double Get(double x, double y, double z, double w, double u, double v)
        {
            return Source.Get(x, y, z, w, u, v) * Scale.Get(x, y, z, w, u, v) + Offset.Get(x, y, z, w, u, v);
        }
    }
}
