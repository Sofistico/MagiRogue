using AccidentalNoiseLibrary.Enums;
using System;

namespace AccidentalNoiseLibrary.Implicit
{
    public sealed class ImplicitFractal : ImplicitModuleBase
    {
        private readonly ImplicitBasisFunction[] basisFunctions = new ImplicitBasisFunction[NoiseAcc.MAX_SOURCES];

        private readonly ImplicitModuleBase[] sources = new ImplicitModuleBase[NoiseAcc.MAX_SOURCES];

        private readonly double[] expArray = new double[NoiseAcc.MAX_SOURCES];

        private readonly double[,] correct = new double[NoiseAcc.MAX_SOURCES, 2];

        private int seed;

        private FractalType type;

        private int octaves;

        public ImplicitFractal(FractalType fractalType, BasisType basisType, InterpolationType interpolationType)
        {
            Octaves = 8;
            Frequency = 1.00;
            Lacunarity = 2.00;
            Type = fractalType;
            SetAllSourceTypes(basisType, interpolationType);
            ResetAllSources();
        }

        public ImplicitFractal(FractalType fractalType, BasisType basisType, InterpolationType interpolationType,
            int octaves, float frequency, int seed
            )
        {
            Octaves = octaves;
            Frequency = frequency;
            Lacunarity = 2.00;
            this.seed = seed;
            Type = fractalType;
            SetAllSourceTypes(basisType, interpolationType);
            ResetAllSources();
        }

        public override int Seed
        {
            get { return seed; }

            set
            {
                seed = value;
                for (var source = 0; source < NoiseAcc.MAX_SOURCES; source += 1)
                    sources[source].Seed = seed + source * 300;
            }
        }

        public FractalType Type
        {
            get { return type; }

            set
            {
                type = value;
                switch (type)
                {
                    case FractalType.FractionalBrownianMotion:
                        H = 1.00;
                        Gain = 0.00;
                        Offset = 0.00;
                        FractionalBrownianMotion_CalculateWeights();
                        break;

                    case FractalType.RidgedMulti:
                        H = 0.90;
                        Gain = 2.00;
                        Offset = 1.00;
                        RidgedMulti_CalculateWeights();
                        break;

                    case FractalType.Billow:
                        H = 1.00;
                        Gain = 0.00;
                        Offset = 0.00;
                        Billow_CalculateWeights();
                        break;

                    case FractalType.Multi:
                        H = 1.00;
                        Gain = 0.00;
                        Offset = 0.00;
                        Multi_CalculateWeights();
                        break;

                    case FractalType.HybridMulti:
                        H = 0.25;
                        Gain = 1.00;
                        Offset = 0.70;
                        HybridMulti_CalculateWeights();
                        break;

                    default:
                        H = 1.00;
                        Gain = 0.00;
                        Offset = 0.00;
                        FractionalBrownianMotion_CalculateWeights();
                        break;
                }
            }
        }

        public int Octaves
        {
            get { return octaves; }

            set
            {
                if (value >= NoiseAcc.MAX_SOURCES)
                    value = NoiseAcc.MAX_SOURCES - 1;
                octaves = value;
            }
        }

        public double Frequency { get; set; }

        public double Lacunarity { get; set; }

        public double Gain { get; set; }

        public double Offset { get; set; }

        public double H { get; set; }

        public void SetAllSourceTypes(BasisType newBasisType, InterpolationType newInterpolationType)
        {
            for (var i = 0; i < NoiseAcc.MAX_SOURCES; ++i)
            {
                basisFunctions[i] = new ImplicitBasisFunction(newBasisType, newInterpolationType);
            }
        }

        public void SetSourceType(int which, BasisType newBasisType, InterpolationType newInterpolationType)
        {
            if (which >= NoiseAcc.MAX_SOURCES || which < 0) return;

            basisFunctions[which].BasisType = newBasisType;
            basisFunctions[which].InterpolationType = newInterpolationType;
        }

        public void SetSourceOverride(int which, ImplicitModuleBase newSource)
        {
            if (which < 0 || which >= NoiseAcc.MAX_SOURCES) return;

            sources[which] = newSource;
        }

        public void ResetSource(int which)
        {
            if (which < 0 || which >= NoiseAcc.MAX_SOURCES) return;

            sources[which] = basisFunctions[which];
        }

        public void ResetAllSources()
        {
            for (var c = 0; c < NoiseAcc.MAX_SOURCES; ++c)
                sources[c] = basisFunctions[c];
        }

        public ImplicitBasisFunction GetBasis(int which)
        {
            if (which < 0 || which >= NoiseAcc.MAX_SOURCES) return null;

            return basisFunctions[which];
        }

        public override double Get(double x, double y)
        {
            double v;
            switch (type)
            {
                case FractalType.FractionalBrownianMotion:
                    v = FractionalBrownianMotion_Get(x, y);
                    break;

                case FractalType.RidgedMulti:
                    v = RidgedMulti_Get(x, y);
                    break;

                case FractalType.Billow:
                    v = Billow_Get(x, y);
                    break;

                case FractalType.Multi:
                    v = Multi_Get(x, y);
                    break;

                case FractalType.HybridMulti:
                    v = HybridMulti_Get(x, y);
                    break;

                default:
                    v = FractionalBrownianMotion_Get(x, y);
                    break;
            }
            return Utilities.Clamp(v, -1.0, 1.0);
        }

        public override double Get(double x, double y, double z)
        {
            double val;
            switch (type)
            {
                case FractalType.FractionalBrownianMotion:
                    val = FractionalBrownianMotion_Get(x, y, z);
                    break;

                case FractalType.RidgedMulti:
                    val = RidgedMulti_Get(x, y, z);
                    break;

                case FractalType.Billow:
                    val = Billow_Get(x, y, z);
                    break;

                case FractalType.Multi:
                    val = Multi_Get(x, y, z);
                    break;

                case FractalType.HybridMulti:
                    val = HybridMulti_Get(x, y, z);
                    break;

                default:
                    val = FractionalBrownianMotion_Get(x, y, z);
                    break;
            }
            return Utilities.Clamp(val, -1.0, 1.0);
        }

        public override double Get(double x, double y, double z, double w)
        {
            double val;
            switch (type)
            {
                case FractalType.FractionalBrownianMotion:
                    val = FractionalBrownianMotion_Get(x, y, z, w);
                    break;

                case FractalType.RidgedMulti:
                    val = RidgedMulti_Get(x, y, z, w);
                    break;

                case FractalType.Billow:
                    val = Billow_Get(x, y, z, w);
                    break;

                case FractalType.Multi:
                    val = Multi_Get(x, y, z, w);
                    break;

                case FractalType.HybridMulti:
                    val = HybridMulti_Get(x, y, z, w);
                    break;

                default:
                    val = FractionalBrownianMotion_Get(x, y, z, w);
                    break;
            }
            return Utilities.Clamp(val, -1.0, 1.0);
        }

        public override double Get(double x, double y, double z, double w, double u, double v)
        {
            double val;
            switch (type)
            {
                case FractalType.FractionalBrownianMotion:
                    val = FractionalBrownianMotion_Get(x, y, z, w, u, v);
                    break;

                case FractalType.RidgedMulti:
                    val = RidgedMulti_Get(x, y, z, w, u, v);
                    break;

                case FractalType.Billow:
                    val = Billow_Get(x, y, z, w, u, v);
                    break;

                case FractalType.Multi:
                    val = Multi_Get(x, y, z, w, u, v);
                    break;

                case FractalType.HybridMulti:
                    val = HybridMulti_Get(x, y, z, w, u, v);
                    break;

                default:
                    val = FractionalBrownianMotion_Get(x, y, z, w, u, v);
                    break;
            }

            return Utilities.Clamp(val, -1.0, 1.0);
        }

        private void FractionalBrownianMotion_CalculateWeights()
        {
            for (var i = 0; i < NoiseAcc.MAX_SOURCES; ++i)
            {
                expArray[i] = Math.Pow(Lacunarity, -i * H);
            }

            // Calculate scale/bias pairs by guessing at minimum and maximum values and remapping to [-1,1]
            var minvalue = 0.00;
            var maxvalue = 0.00;
            for (var i = 0; i < NoiseAcc.MAX_SOURCES; ++i)
            {
                minvalue += -1.0 * expArray[i];
                maxvalue += 1.0 * expArray[i];

                const double a = -1.0;
                const double b = 1.0;
                var scale = (b - a) / (maxvalue - minvalue);
                var bias = a - minvalue * scale;
                correct[i, 0] = scale;
                correct[i, 1] = bias;
            }
        }

        private void RidgedMulti_CalculateWeights()
        {
            for (var i = 0; i < NoiseAcc.MAX_SOURCES; ++i)
            {
                expArray[i] = Math.Pow(Lacunarity, -i * H);
            }

            // Calculate scale/bias pairs by guessing at minimum and maximum values and remapping to [-1,1]
            var minvalue = 0.00;
            var maxvalue = 0.00;
            for (var i = 0; i < NoiseAcc.MAX_SOURCES; ++i)
            {
                minvalue += (Offset - 1.0) * (Offset - 1.0) * expArray[i];
                maxvalue += Offset * Offset * expArray[i];

                const double a = -1.0;
                const double b = 1.0;
                var scale = (b - a) / (maxvalue - minvalue);
                var bias = a - minvalue * scale;
                correct[i, 0] = scale;
                correct[i, 1] = bias;
            }
        }

        private void Billow_CalculateWeights()
        {
            for (var i = 0; i < NoiseAcc.MAX_SOURCES; ++i)
            {
                expArray[i] = Math.Pow(Lacunarity, -i * H);
            }

            // Calculate scale/bias pairs by guessing at minimum and maximum values and remapping to [-1,1]
            var minvalue = 0.0;
            var maxvalue = 0.0;
            for (var i = 0; i < NoiseAcc.MAX_SOURCES; ++i)
            {
                minvalue += -1.0 * expArray[i];
                maxvalue += 1.0 * expArray[i];

                const double a = -1.0;
                const double b = 1.0;
                var scale = (b - a) / (maxvalue - minvalue);
                var bias = a - minvalue * scale;
                correct[i, 0] = scale;
                correct[i, 1] = bias;
            }
        }

        private void Multi_CalculateWeights()
        {
            for (var i = 0; i < NoiseAcc.MAX_SOURCES; ++i)
            {
                expArray[i] = Math.Pow(Lacunarity, -i * H);
            }

            // Calculate scale/bias pairs by guessing at minimum and maximum values and remapping to [-1,1]
            var minvalue = 1.0;
            var maxvalue = 1.0;
            for (var i = 0; i < NoiseAcc.MAX_SOURCES; ++i)
            {
                minvalue *= -1.0 * expArray[i] + 1.0;
                maxvalue *= 1.0 * expArray[i] + 1.0;

                const double a = -1.0;
                const double b = 1.0;
                var scale = (b - a) / (maxvalue - minvalue);
                var bias = a - minvalue * scale;
                correct[i, 0] = scale;
                correct[i, 1] = bias;
            }
        }

        private void HybridMulti_CalculateWeights()
        {
            for (var i = 0; i < NoiseAcc.MAX_SOURCES; ++i)
            {
                expArray[i] = Math.Pow(Lacunarity, -i * H);
            }

            // Calculate scale/bias pairs by guessing at minimum and maximum values and remapping to [-1,1]
            const double a = -1.0;
            const double b = 1.0;

            var minvalue = Offset - 1.0;
            var maxvalue = Offset + 1.0;
            var weightmin = Gain * minvalue;
            var weightmax = Gain * maxvalue;

            var scale = (b - a) / (maxvalue - minvalue);
            var bias = a - minvalue * scale;
            correct[0, 0] = scale;
            correct[0, 1] = bias;

            for (var i = 1; i < NoiseAcc.MAX_SOURCES; ++i)
            {
                if (weightmin > 1.00) weightmin = 1.00;
                if (weightmax > 1.00) weightmax = 1.00;

                var signal = (Offset - 1.0) * expArray[i];
                minvalue += signal * weightmin;
                weightmin *= Gain * signal;

                signal = (Offset + 1.0) * expArray[i];
                maxvalue += signal * weightmax;
                weightmax *= Gain * signal;

                scale = (b - a) / (maxvalue - minvalue);
                bias = a - minvalue * scale;
                correct[i, 0] = scale;
                correct[i, 1] = bias;
            }
        }

        private double FractionalBrownianMotion_Get(double x, double y)
        {
            var value = 0.00;
            x *= Frequency;
            y *= Frequency;

            for (var i = 0; i < octaves; ++i)
            {
                var signal = sources[i].Get(x, y) * expArray[i];
                value += signal;
                x *= Lacunarity;
                y *= Lacunarity;
            }

            return value;
        }

        private double FractionalBrownianMotion_Get(double x, double y, double z)
        {
            var value = 0.00;
            x *= Frequency;
            y *= Frequency;
            z *= Frequency;

            for (var i = 0; i < octaves; ++i)
            {
                var signal = sources[i].Get(x, y, z) * expArray[i];
                value += signal;
                x *= Lacunarity;
                y *= Lacunarity;
                z *= Lacunarity;
            }

            return value;
        }

        private double FractionalBrownianMotion_Get(double x, double y, double z, double w)
        {
            var value = 0.00;
            x *= Frequency;
            y *= Frequency;
            z *= Frequency;
            w *= Frequency;

            for (var i = 0; i < octaves; ++i)
            {
                var signal = sources[i].Get(x, y, z, w) * expArray[i];
                value += signal;
                x *= Lacunarity;
                y *= Lacunarity;
                z *= Lacunarity;
                w *= Lacunarity;
            }

            return value * correct[octaves - 1, 0] + correct[octaves - 1, 1];
        }

        private double FractionalBrownianMotion_Get(double x, double y, double z, double w, double u, double v)
        {
            var value = 0.00;
            x *= Frequency;
            y *= Frequency;
            z *= Frequency;
            w *= Frequency;
            u *= Frequency;
            v *= Frequency;

            for (var i = 0; i < octaves; ++i)
            {
                var signal = sources[i].Get(x, y, z, w, u, v) * expArray[i];
                value += signal;
                x *= Lacunarity;
                y *= Lacunarity;
                z *= Lacunarity;
                w *= Lacunarity;
                u *= Lacunarity;
                v *= Lacunarity;
            }

            return value * correct[octaves - 1, 0] + correct[octaves - 1, 1];
        }

        private double Multi_Get(double x, double y)
        {
            var value = 1.00;
            x *= Frequency;
            y *= Frequency;

            for (var i = 0; i < octaves; ++i)
            {
                value *= sources[i].Get(x, y) * expArray[i] + 1.0;
                x *= Lacunarity;
                y *= Lacunarity;
            }

            return value * correct[octaves - 1, 0] + correct[octaves - 1, 1];
        }

        private double Multi_Get(double x, double y, double z, double w)
        {
            var value = 1.00;
            x *= Frequency;
            y *= Frequency;
            z *= Frequency;
            w *= Frequency;

            for (var i = 0; i < octaves; ++i)
            {
                value *= sources[i].Get(x, y, z, w) * expArray[i] + 1.0;
                x *= Lacunarity;
                y *= Lacunarity;
                z *= Lacunarity;
                w *= Lacunarity;
            }

            return value * correct[octaves - 1, 0] + correct[octaves - 1, 1];
        }

        private double Multi_Get(double x, double y, double z)
        {
            var value = 1.00;
            x *= Frequency;
            y *= Frequency;
            z *= Frequency;

            for (var i = 0; i < octaves; ++i)
            {
                value *= sources[i].Get(x, y, z) * expArray[i] + 1.0;
                x *= Lacunarity;
                y *= Lacunarity;
                z *= Lacunarity;
            }

            return value * correct[octaves - 1, 0] + correct[octaves - 1, 1];
        }

        private double Multi_Get(double x, double y, double z, double w, double u, double v)
        {
            var value = 1.00;
            x *= Frequency;
            y *= Frequency;
            z *= Frequency;
            w *= Frequency;
            u *= Frequency;
            v *= Frequency;

            for (var i = 0; i < octaves; ++i)
            {
                value *= sources[i].Get(x, y, z, w, u, v) * expArray[i] + 1.00;
                x *= Lacunarity;
                y *= Lacunarity;
                z *= Lacunarity;
                w *= Lacunarity;
                u *= Lacunarity;
                v *= Lacunarity;
            }

            return value * correct[octaves - 1, 0] + correct[octaves - 1, 1];
        }

        private double Billow_Get(double x, double y)
        {
            var value = 0.00;
            x *= Frequency;
            y *= Frequency;

            for (var i = 0; i < octaves; ++i)
            {
                var signal = sources[i].Get(x, y);
                signal = 2.0 * Math.Abs(signal) - 1.0;
                value += signal * expArray[i];

                x *= Lacunarity;
                y *= Lacunarity;
            }

            value += 0.5;
            return value * correct[octaves - 1, 0] + correct[octaves - 1, 1];
        }

        private double Billow_Get(double x, double y, double z, double w)
        {
            var value = 0.00;
            x *= Frequency;
            y *= Frequency;
            z *= Frequency;
            w *= Frequency;

            for (var i = 0; i < octaves; ++i)
            {
                var signal = sources[i].Get(x, y, z, w);
                signal = 2.0 * Math.Abs(signal) - 1.0;
                value += signal * expArray[i];

                x *= Lacunarity;
                y *= Lacunarity;
                z *= Lacunarity;
                w *= Lacunarity;
            }

            value += 0.5;
            return value * correct[octaves - 1, 0] + correct[octaves - 1, 1];
        }

        private double Billow_Get(double x, double y, double z)
        {
            var value = 0.00;
            x *= Frequency;
            y *= Frequency;
            z *= Frequency;

            for (var i = 0; i < octaves; ++i)
            {
                var signal = sources[i].Get(x, y, z);
                signal = 2.0 * Math.Abs(signal) - 1.0;
                value += signal * expArray[i];

                x *= Lacunarity;
                y *= Lacunarity;
                z *= Lacunarity;
            }

            value += 0.5;
            return value * correct[octaves - 1, 0] + correct[octaves - 1, 1];
        }

        private double Billow_Get(double x, double y, double z, double w, double u, double v)
        {
            var value = 0.00;
            x *= Frequency;
            y *= Frequency;
            z *= Frequency;
            w *= Frequency;
            u *= Frequency;
            v *= Frequency;

            for (var i = 0; i < octaves; ++i)
            {
                var signal = sources[i].Get(x, y, z, w, u, v);
                signal = 2.0 * Math.Abs(signal) - 1.0;
                value += signal * expArray[i];

                x *= Lacunarity;
                y *= Lacunarity;
                z *= Lacunarity;
                w *= Lacunarity;
                u *= Lacunarity;
                v *= Lacunarity;
            }

            value += 0.5;
            return value * correct[octaves - 1, 0] + correct[octaves - 1, 1];
        }

        private double RidgedMulti_Get(double x, double y)
        {
            var result = 0.00;
            x *= Frequency;
            y *= Frequency;

            for (var i = 0; i < octaves; ++i)
            {
                var signal = sources[i].Get(x, y);
                signal = Offset - Math.Abs(signal);
                signal *= signal;
                result += signal * expArray[i];

                x *= Lacunarity;
                y *= Lacunarity;
            }

            return result * correct[octaves - 1, 0] + correct[octaves - 1, 1];
        }

        private double RidgedMulti_Get(double x, double y, double z, double w)
        {
            var result = 0.00;
            x *= Frequency;
            y *= Frequency;
            z *= Frequency;
            w *= Frequency;

            for (var i = 0; i < octaves; ++i)
            {
                var signal = sources[i].Get(x, y, z, w);
                signal = Offset - Math.Abs(signal);
                signal *= signal;
                result += signal * expArray[i];

                x *= Lacunarity;
                y *= Lacunarity;
                z *= Lacunarity;
                w *= Lacunarity;
            }

            return result * correct[octaves - 1, 0] + correct[octaves - 1, 1];
        }

        private double RidgedMulti_Get(double x, double y, double z)
        {
            var result = 0.00;
            x *= Frequency;
            y *= Frequency;
            z *= Frequency;

            for (var i = 0; i < octaves; ++i)
            {
                var signal = sources[i].Get(x, y, z);
                signal = Offset - Math.Abs(signal);
                signal *= signal;
                result += signal * expArray[i];

                x *= Lacunarity;
                y *= Lacunarity;
                z *= Lacunarity;
            }

            return result * correct[octaves - 1, 0] + correct[octaves - 1, 1];
        }

        private double RidgedMulti_Get(double x, double y, double z, double w, double u, double v)
        {
            var result = 0.00;
            x *= Frequency;
            y *= Frequency;
            z *= Frequency;
            w *= Frequency;
            u *= Frequency;
            v *= Frequency;

            for (var i = 0; i < octaves; ++i)
            {
                var signal = sources[i].Get(x, y, z, w, u, v);
                signal = Offset - Math.Abs(signal);
                signal *= signal;
                result += signal * expArray[i];

                x *= Lacunarity;
                y *= Lacunarity;
                z *= Lacunarity;
                w *= Lacunarity;
                u *= Lacunarity;
                v *= Lacunarity;
            }

            return result * correct[octaves - 1, 0] + correct[octaves - 1, 1];
        }

        private double HybridMulti_Get(double x, double y)
        {
            x *= Frequency;
            y *= Frequency;

            var value = sources[0].Get(x, y) + Offset;
            var weight = Gain * value;
            x *= Lacunarity;
            y *= Lacunarity;

            for (var i = 1; i < octaves; ++i)
            {
                if (weight > 1.0) weight = 1.0;
                var signal = (sources[i].Get(x, y) + Offset) * expArray[i];
                value += weight * signal;
                weight *= Gain * signal;
                x *= Lacunarity;
                y *= Lacunarity;
            }

            return value * correct[octaves - 1, 0] + correct[octaves - 1, 1];
        }

        private double HybridMulti_Get(double x, double y, double z)
        {
            x *= Frequency;
            y *= Frequency;
            z *= Frequency;

            var value = sources[0].Get(x, y, z) + Offset;
            var weight = Gain * value;
            x *= Lacunarity;
            y *= Lacunarity;
            z *= Lacunarity;

            for (var i = 1; i < octaves; ++i)
            {
                if (weight > 1.0) weight = 1.0;
                var signal = (sources[i].Get(x, y, z) + Offset) * expArray[i];
                value += weight * signal;
                weight *= Gain * signal;
                x *= Lacunarity;
                y *= Lacunarity;
                z *= Lacunarity;
            }

            return value * correct[octaves - 1, 0] + correct[octaves - 1, 1];
        }

        private double HybridMulti_Get(double x, double y, double z, double w)
        {
            x *= Frequency;
            y *= Frequency;
            z *= Frequency;
            w *= Frequency;

            var value = sources[0].Get(x, y, z, w) + Offset;
            var weight = Gain * value;
            x *= Lacunarity;
            y *= Lacunarity;
            z *= Lacunarity;
            w *= Lacunarity;

            for (var i = 1; i < octaves; ++i)
            {
                if (weight > 1.0) weight = 1.0;
                var signal = (sources[i].Get(x, y, z, w) + Offset) * expArray[i];
                value += weight * signal;
                weight *= Gain * signal;
                x *= Lacunarity;
                y *= Lacunarity;
                z *= Lacunarity;
                w *= Lacunarity;
            }

            return value * correct[octaves - 1, 0] + correct[octaves - 1, 1];
        }

        private double HybridMulti_Get(double x, double y, double z, double w, double u, double v)
        {
            x *= Frequency;
            y *= Frequency;
            z *= Frequency;
            w *= Frequency;
            u *= Frequency;
            v *= Frequency;

            var value = sources[0].Get(x, y, z, w, u, v) + Offset;
            var weight = Gain * value;
            x *= Lacunarity;
            y *= Lacunarity;
            z *= Lacunarity;
            w *= Lacunarity;
            u *= Lacunarity;
            v *= Lacunarity;

            for (var i = 1; i < octaves; ++i)
            {
                if (weight > 1.0) weight = 1.0;
                var signal = (sources[i].Get(x, y, z, w, u, v) + Offset) * expArray[i];
                value += weight * signal;
                weight *= Gain * signal;
                x *= Lacunarity;
                y *= Lacunarity;
                z *= Lacunarity;
                w *= Lacunarity;
                u *= Lacunarity;
                v *= Lacunarity;
            }

            return value * correct[octaves - 1, 0] + correct[octaves - 1, 1];
        }
    }
}