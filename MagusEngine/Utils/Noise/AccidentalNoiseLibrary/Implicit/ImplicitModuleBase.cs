using System;

namespace MagusEngine.Utils.Noise.AccidentalNoiseLibrary.Implicit
{
    public abstract class ImplicitModuleBase
    {
        public virtual int Seed { get; set; }

        public virtual double Get(double x, double y) { return 0.00; }

        public virtual double Get(double x, double y, double z) { return 0.00; }

        public virtual double Get(double x, double y, double z, double w) { return 0.00; }

        public virtual double Get(double x, double y, double z, double w, double u, double v) { return 0.00; }

        public static implicit operator ImplicitModuleBase(double value)
        {
            return new ImplicitConstant(value);
        }
    }
}
