using System;

namespace AccidentalNoiseLibrary.Implicit
{
    public sealed class ImplicitBrightContrast : ImplicitModuleBase
    {
        public ImplicitBrightContrast(ImplicitModuleBase source, double brightness = 0.00, double contrastThreshold = 0.00, double contrastFactor = 1.00)
        {
            Source = source;
            Brightness = new ImplicitConstant(brightness);
            ContrastThreshold = new ImplicitConstant(contrastThreshold);
            ContrastFactor = new ImplicitConstant(contrastFactor);
        }

        private ImplicitModuleBase Source { get; set; }

        private ImplicitModuleBase Brightness { get; set; }

        private ImplicitModuleBase ContrastThreshold { get; set; }

        private ImplicitModuleBase ContrastFactor { get; set; }

        public override double Get(double x, double y)
        {
            var value = Source.Get(x, y);
            // Apply brightness
            value += Brightness.Get(x, y);

            // Subtract contrastThreshold, scale by contrastFactor, add contrastThreshold
            var threshold = ContrastThreshold.Get(x, y);
            value -= threshold;
            value *= ContrastFactor.Get(x, y);
            value += threshold;
            return value;
        }

        public override double Get(double x, double y, double z)
        {
            var value = Source.Get(x, y, z);
            // Apply brightness
            value += Brightness.Get(x, y, z);

            // Subtract contrastThreshold, scale by contrastFactor, add contrastThreshold
            var threshold = ContrastThreshold.Get(x, y, z);
            value -= threshold;
            value *= ContrastFactor.Get(x, y, z);
            value += threshold;
            return value;
        }

        public override double Get(double x, double y, double z, double w)
        {
            var value = Source.Get(x, y, z, w);
            // Apply brightness
            value += Brightness.Get(x, y, z, w);

            // Subtract contrastThreshold, scale by contrastFactor, add contrastThreshold
            var threshold = ContrastThreshold.Get(x, y, z, w);
            value -= threshold;
            value *= ContrastFactor.Get(x, y, z, w);
            value += threshold;
            return value;
        }

        public override double Get(double x, double y, double z, double w, double u, double v)
        {
            var value = Source.Get(x, y, z, w, u, v);
            // Apply brightness
            value += Brightness.Get(x, y, z, w, u, v);

            // Subtract contrastThreshold, scale by contrastFactor, add contrastThreshold
            var threshold = ContrastThreshold.Get(x, y, z, w, u, v);
            value -= threshold;
            value *= ContrastFactor.Get(x, y, z, w, u, v);
            value += threshold;
            return value;
        }
    }
}
