using System;

namespace AccidentalNoiseLibrary.Implicit
{
    public sealed class ImplicitConstant : ImplicitModuleBase
    {
        public ImplicitConstant()
        {
            Value = 0.00;
        }

        public ImplicitConstant(double value)
        {
            Value = value;
        }

        public double Value { get; set; }

        public override double Get(double x, double y)
        {
            return Value;
        }

        public override double Get(double x, double y, double z)
        {
            return Value;
        }

        public override double Get(double x, double y, double z, double w)
        {
            return Value;
        }

        public override double Get(double x, double y, double z, double w, double u, double v)
        {
            return Value;
        }
    }
}
