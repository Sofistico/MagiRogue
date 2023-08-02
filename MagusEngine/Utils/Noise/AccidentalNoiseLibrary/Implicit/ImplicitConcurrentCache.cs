using MagusEngine.Utils.Noise.AccidentalNoiseLibrary;
using System;
using System.Threading;

namespace MagusEngine.Utils.Noise.AccidentalNoiseLibrary.Implicit
{
    public sealed class ImplicitConcurrentCache : ImplicitModuleBase
    {
        private readonly ThreadLocal<Cache> cache2D = new ThreadLocal<Cache>(() => new Cache());

        private readonly ThreadLocal<Cache> cache3D = new ThreadLocal<Cache>(() => new Cache());

        private readonly ThreadLocal<Cache> cache4D = new ThreadLocal<Cache>(() => new Cache());

        private readonly ThreadLocal<Cache> cache6D = new ThreadLocal<Cache>(() => new Cache());

        public ImplicitConcurrentCache(ImplicitModuleBase source)
        {
            Source = source;
        }

        public ImplicitModuleBase Source { get; set; }

        public override double Get(double x, double y)
        {
            if (!cache2D.Value.IsValid || cache2D.Value.X != x || cache2D.Value.Y != y)
            {
                cache2D.Value.X = x;
                cache2D.Value.Y = y;
                cache2D.Value.IsValid = true;
                cache2D.Value.Value = Source.Get(x, y);
            }
            return cache2D.Value.Value;
        }

        public override double Get(double x, double y, double z)
        {
            if (!cache3D.Value.IsValid || cache3D.Value.X != x || cache3D.Value.Y != y || cache3D.Value.Z != z)
            {
                cache3D.Value.X = x;
                cache3D.Value.Y = y;
                cache3D.Value.Z = z;
                cache3D.Value.IsValid = true;
                cache3D.Value.Value = Source.Get(x, y, z);
            }
            return cache3D.Value.Value;
        }

        public override double Get(double x, double y, double z, double w)
        {
            if (!cache4D.Value.IsValid || cache4D.Value.X != x || cache4D.Value.Y != y || cache4D.Value.Z != z || cache4D.Value.W != w)
            {
                cache4D.Value.X = x;
                cache4D.Value.Y = y;
                cache4D.Value.Z = z;
                cache4D.Value.W = w;
                cache4D.Value.IsValid = true;
                cache4D.Value.Value = Source.Get(x, y, z, w);
            }
            return cache4D.Value.Value;
        }

        public override double Get(double x, double y, double z, double w, double u, double v)
        {
            if (!cache6D.Value.IsValid || cache6D.Value.X != x || cache6D.Value.Y != y || cache6D.Value.Z != z || cache6D.Value.W != w || cache6D.Value.U != u || cache6D.Value.V != v)
            {
                cache6D.Value.X = x;
                cache6D.Value.Y = y;
                cache6D.Value.Z = z;
                cache6D.Value.W = w;
                cache6D.Value.U = u;
                cache6D.Value.V = v;
                cache6D.Value.IsValid = true;
                cache6D.Value.Value = Source.Get(x, y, z, w, u, v);
            }
            return cache6D.Value.Value;
        }
    }
}