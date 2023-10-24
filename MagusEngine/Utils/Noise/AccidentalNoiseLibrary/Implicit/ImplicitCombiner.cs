using MagusEngine.Utils.Noise.AccidentalNoiseLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MagusEngine.Utils.Noise.AccidentalNoiseLibrary.Implicit
{
    public sealed class ImplicitCombiner : ImplicitModuleBase
    {
        private readonly HashSet<ImplicitModuleBase> sources = new HashSet<ImplicitModuleBase>();

        public ImplicitCombiner(CombinerType type)
        {
            CombinerType = type;
        }

        public CombinerType CombinerType { get; set; }

        public void AddSource(ImplicitModuleBase module)
        {
            sources.Add(module);
        }

        public void RemoveSource(ImplicitModuleBase module)
        {
            sources.Remove(module);
        }

        public void ClearSources()
        {
            sources.Clear();
        }

        public override double Get(double x, double y)
        {
            switch (CombinerType)
            {
                case CombinerType.Add:
                    return AddGet(x, y);
                case CombinerType.Multiply:
                    return MultiplyGet(x, y);
                case CombinerType.Max:
                    return MaxGet(x, y);
                case CombinerType.Min:
                    return MinGet(x, y);
                case CombinerType.Average:
                    return AverageGet(x, y);
                default:
                    return 0.0;
            }
        }

        public override double Get(double x, double y, double z)
        {
            switch (CombinerType)
            {
                case CombinerType.Add:
                    return AddGet(x, y, z);
                case CombinerType.Multiply:
                    return MultiplyGet(x, y, z);
                case CombinerType.Max:
                    return MaxGet(x, y, z);
                case CombinerType.Min:
                    return MinGet(x, y, z);
                case CombinerType.Average:
                    return AverageGet(x, y, z);
                default:
                    return 0.0;
            }
        }

        public override double Get(double x, double y, double z, double w)
        {
            switch (CombinerType)
            {
                case CombinerType.Add:
                    return AddGet(x, y, z, w);
                case CombinerType.Multiply:
                    return MultiplyGet(x, y, z, w);
                case CombinerType.Max:
                    return MaxGet(x, y, z, w);
                case CombinerType.Min:
                    return MinGet(x, y, z, w);
                case CombinerType.Average:
                    return AverageGet(x, y, z, w);
                default:
                    return 0.0;
            }
        }

        public override double Get(double x, double y, double z, double w, double u, double v)
        {
            switch (CombinerType)
            {
                case CombinerType.Add:
                    return AddGet(x, y, z, w, u, v);
                case CombinerType.Multiply:
                    return MultiplyGet(x, y, z, w, u, v);
                case CombinerType.Max:
                    return MaxGet(x, y, z, w, u, v);
                case CombinerType.Min:
                    return MinGet(x, y, z, w, u, v);
                case CombinerType.Average:
                    return AverageGet(x, y, z, w, u, v);
                default:
                    return 0.0;
            }
        }


        private double AddGet(double x, double y)
        {
            return sources.Sum(source => source.Get(x, y));
        }

        private double AddGet(double x, double y, double z)
        {
            return sources.Sum(source => source.Get(x, y, z));
        }

        private double AddGet(double x, double y, double z, double w)
        {
            return sources.Sum(source => source.Get(x, y, z, w));
        }

        private double AddGet(double x, double y, double z, double w, double u, double v)
        {
            return sources.Sum(source => source.Get(x, y, z, w, u, v));
        }


        private double MultiplyGet(double x, double y)
        {
            return sources.Aggregate(1.00, (current, source) => current * source.Get(x, y));
        }

        private double MultiplyGet(double x, double y, double z)
        {
            return sources.Aggregate(1.00, (current, source) => current * source.Get(x, y, z));
        }

        private double MultiplyGet(double x, double y, double z, double w)
        {
            return sources.Aggregate(1.00, (current, source) => current * source.Get(x, y, z, w));
        }

        private double MultiplyGet(double x, double y, double z, double w, double u, double v)
        {
            return sources.Aggregate(1.00, (current, source) => current * source.Get(x, y, z, w, u, v));
        }


        private double MinGet(double x, double y)
        {
            return sources.Min(source => source.Get(x, y));
        }

        private double MinGet(double x, double y, double z)
        {
            return sources.Min(source => source.Get(x, y, z));
        }

        private double MinGet(double x, double y, double z, double w)
        {
            return sources.Min(source => source.Get(x, y, z, w));
        }

        private double MinGet(double x, double y, double z, double w, double u, double v)
        {
            return sources.Min(source => source.Get(x, y, z, w, u, v));
        }


        private double MaxGet(double x, double y)
        {
            return sources.Max(source => source.Get(x, y));
        }

        private double MaxGet(double x, double y, double z)
        {
            return sources.Max(source => source.Get(x, y, z));
        }

        private double MaxGet(double x, double y, double z, double w)
        {
            return sources.Max(source => source.Get(x, y, z, w));
        }

        private double MaxGet(double x, double y, double z, double w, double u, double v)
        {
            return sources.Max(source => source.Get(x, y, z, w, u, v));
        }


        private double AverageGet(double x, double y)
        {
            return sources.Average(source => source.Get(x, y));
        }

        private double AverageGet(double x, double y, double z)
        {
            return sources.Average(source => source.Get(x, y, z));
        }

        private double AverageGet(double x, double y, double z, double w)
        {
            return sources.Average(source => source.Get(x, y, z, w));
        }

        private double AverageGet(double x, double y, double z, double w, double u, double v)
        {
            return sources.Average(source => source.Get(x, y, z, w, u, v));
        }
    }
}