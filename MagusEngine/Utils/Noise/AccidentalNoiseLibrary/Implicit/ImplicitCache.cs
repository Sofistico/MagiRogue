using MagusEngine.Utils.Noise.AccidentalNoiseLibrary;
using System;

namespace MagusEngine.Utils.Noise.AccidentalNoiseLibrary.Implicit
{
    public sealed class ImplicitCache : ImplicitModuleBase
    {
        private readonly Cache cache2D = new Cache();

        private readonly Cache cache3D = new Cache();

        private readonly Cache cache4D = new Cache();

        private readonly Cache cache6D = new Cache();

        public ImplicitCache(ImplicitModuleBase source)
        {
            Source = source;
        }

        public ImplicitModuleBase Source { get; set; }

        public override double Get(double x, double y)
        {
            if (!cache2D.IsValid || cache2D.X != x || cache2D.Y != y)
            {
                cache2D.X = x;
                cache2D.Y = y;
                cache2D.IsValid = true;
                cache2D.Value = Source.Get(x, y);
            }
            return cache2D.Value;
        }

        public override double Get(double x, double y, double z)
        {
            if (!cache3D.IsValid || cache3D.X != x || cache3D.Y != y || cache3D.Z != z)
            {
                cache3D.X = x;
                cache3D.Y = y;
                cache3D.Z = z;
                cache3D.IsValid = true;
                cache3D.Value = Source.Get(x, y, z);
            }
            return cache3D.Value;
        }

        public override double Get(double x, double y, double z, double w)
        {
            if (!cache4D.IsValid || cache4D.X != x || cache4D.Y != y || cache4D.Z != z || cache4D.W != w)
            {
                cache4D.X = x;
                cache4D.Y = y;
                cache4D.Z = z;
                cache4D.W = w;
                cache4D.IsValid = true;
                cache4D.Value = Source.Get(x, y, z, w);
            }
            return cache4D.Value;
        }

        public override double Get(double x, double y, double z, double w, double u, double v)
        {
            if (!cache6D.IsValid || cache6D.X != x || cache6D.Y != y || cache6D.Z != z || cache6D.W != w || cache6D.U != u || cache6D.V != v)
            {
                cache6D.X = x;
                cache6D.Y = y;
                cache6D.Z = z;
                cache6D.W = w;
                cache6D.U = u;
                cache6D.V = v;
                cache6D.IsValid = true;
                cache6D.Value = Source.Get(x, y, z, w, u, v);
            }
            return cache6D.Value;
        }
    }
}