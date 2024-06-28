using System;

// All credits go to https://github.com/TinkerWorX/AccidentalNoiseLibrary
namespace AccidentalNoiseLibrary
{
    public delegate double InterpolationDelegate(double value);

    public delegate double Noise2DDelegate(double x, double y, int seed, InterpolationDelegate interp);

    public delegate double Noise3DDelegate(double x, double y, double z, int seed, InterpolationDelegate interp);

    public delegate double Noise4DDelegate(double x, double y, double z, double w, int seed, InterpolationDelegate interp);

    public delegate double Noise6DDelegate(double x, double y, double z, double w, double u, double v, int seed, InterpolationDelegate interp);

    public static class NoiseAcc
    {
        public const int MAX_SOURCES = 20;

        internal static int FastFloor(double t)
        {
            return t > 0 ? (int)t : (int)t - 1;
        }

        internal static double ArrayDot(double[] arr, double a, double b)
        {
            return a * arr[0] + b * arr[1];
        }

        internal static double ArrayDot(double[] arr, double a, double b, double c)
        {
            return a * arr[0] + b * arr[1] + c * arr[2];
        }

        internal static double ArrayDot(double[] arr, double x, double y, double z, double w)
        {
            return x * arr[0] + y * arr[1] + z * arr[2] + w * arr[3];
        }

        internal static double ArrayDot(double[] arr, double x, double y, double z, double w, double u, double v)
        {
            return x * arr[0] + y * arr[1] + z * arr[2] + w * arr[3] + u * arr[4] + v * arr[5];
        }

        internal static void AddDistance(double[] f, double[] disp, double testdist, double testdisp)
        {
            // Compare the given distance to the ones already in f
            if (testdist >= f[3]) return;

            var index = 3;
            while (index > 0 && testdist < f[index - 1]) index--;

            for (var i = 3; i-- > index;)
            {
                f[i + 1] = f[i];
                disp[i + 1] = disp[i];
            }
            f[index] = testdist;
            disp[index] = testdisp;
        }

        // Interpolation functions
        public static double NoInterpolation(double t)
        {
            return 0;
        }

        public static double LinearInterpolation(double t)
        {
            return t;
        }

        public static double HermiteInterpolation(double t)
        {
            return t * t * (3 - 2 * t);
        }

        public static double QuinticInterpolation(double t)
        {
            return t * t * t * (t * (t * 6 - 15) + 10);
        }

        // Edge/Face/Cube/Hypercube interpolation
        internal static double Lerp(double s, double v1, double v2)
        {
            return v1 + s * (v2 - v1);
        }

        #region Hashing

        // The "new" FNV-1A hashing
        private const uint FNV_32_PRIME = 0x01000193;
        private const uint FNV_32_INIT = 2166136261;
        private const uint FNV_MASK_8 = (1 << 8) - 1;

        internal static uint FNV32Buffer(int[] uintBuffer, uint len)
        {
            //NOTE: Completely untested.
            var buffer = new byte[len];
            Buffer.BlockCopy(uintBuffer, 0, buffer, 0, buffer.Length);

            var hval = FNV_32_INIT;
            for (var i = 0; i < len;)
            {
                hval ^= buffer[i++];
                hval *= FNV_32_PRIME;
            }
            return hval;
        }

        internal static uint FNV32Buffer(double[] doubleBuffer, uint len)
        {
            //NOTE: Completely untested.
            var buffer = new byte[len];
            Buffer.BlockCopy(doubleBuffer, 0, buffer, 0, buffer.Length);

            var hval = FNV_32_INIT;
            for (var i = 0; i < len;)
            {
                hval ^= buffer[i++];
                hval *= FNV_32_PRIME;
            }
            return hval;
        }

        internal static uint FNV32Buffer(byte[] buffer, uint len)
        {
            var hval = FNV_32_INIT;
            for (var i = 0; i < len;)
            {
                hval ^= buffer[i++];
                hval *= FNV_32_PRIME;
            }
            return hval;
        }

        internal static uint FNV1A_3d(double x, double y, double z, int seed)
        {
            double[] d = { x, y, z, seed };
            return FNV32Buffer(d, sizeof(double) * 4);
        }

        internal static byte XORFoldHash(uint hash)
        {
            // Implement XOR-folding to reduce from 32 to 8-bit hash
            return (byte)(hash >> 8 ^ hash & FNV_MASK_8);
        }

        // FNV-based coordinate hashes
        internal static uint HashCoordinates(int x, int y, int seed)
        {
            int[] d = { x, y, seed };
            return XORFoldHash(FNV32Buffer(d, sizeof(int) * 3));
        }

        internal static uint HashCoordinates(int x, int y, int z, int seed)
        {
            int[] d = { x, y, z, seed };
            return XORFoldHash(FNV32Buffer(d, sizeof(int) * 4));
        }

        internal static uint HashCoordinates(int x, int y, int z, int w, int seed)
        {
            int[] d = { x, y, z, w, seed };
            return XORFoldHash(FNV32Buffer(d, sizeof(int) * 5));
        }

        internal static uint HashCoordinates(int x, int y, int z, int w, int u, int v, int seed)
        {
            int[] d = { x, y, z, w, u, v, seed };
            return XORFoldHash(FNV32Buffer(d, sizeof(int) * 7));
        }

        internal static uint HashCoordinates(double x, double y, int seed)
        {
            double[] d = { x, y, seed };
            return XORFoldHash(FNV32Buffer(d, sizeof(double) * 3));
        }

        internal static uint HashCoordinates(double x, double y, double z, int seed)
        {
            double[] d = { x, y, z, seed };
            return XORFoldHash(FNV32Buffer(d, sizeof(double) * 4));
        }

        internal static uint HashCoordinates(double x, double y, double z, double w, int seed)
        {
            double[] d = { x, y, z, w, seed };
            return XORFoldHash(FNV32Buffer(d, sizeof(double) * 5));
        }

        internal static uint HashCoordinates(double x, double y, double z, double w, double u, double v, int seed)
        {
            double[] d = { x, y, z, w, u, v, seed };
            return XORFoldHash(FNV32Buffer(d, sizeof(double) * 7));
        }

        internal delegate double WorkerNoise2(double x, double y, int ix, int iy, int seed);

        internal delegate double WorkerNoise3(double x, double y, double z, int ix, int iy, int iz, int seed);

        internal delegate double WorkerNoise4(double x, double y, double z, double w, int ix, int iy, int iz, int iw, int seed);

        internal delegate double WorkerNoise6(double x, double y, double z, double w, double u, double v, int ix, int iy, int iz, int iw, int iu, int iv, int seed);

        // Noise generators
        internal static double InternalValueNoise(
            double x, double y,
            int ix, int iy,
            int seed)
        {
            var noise = HashCoordinates(ix, iy, seed) / 255.0;
            return noise * 2.0 - 1.0;
        }

        internal static double InternalValueNoise(
            double x, double y, double z,
            int ix, int iy, int iz,
            int seed)
        {
            var noise = HashCoordinates(ix, iy, iz, seed) / 255.0;
            return noise * 2.0 - 1.0;
        }

        internal static double InternalValueNoise(
            double x, double y, double z, double w,
            int ix, int iy, int iz, int iw,
            int seed)
        {
            var noise = HashCoordinates(ix, iy, iz, iw, seed) / 255.0;
            return noise * 2.0 - 1.0;
        }

        internal static double InternalValueNoise(
            double x, double y, double z, double w, double u, double v,
            int ix, int iy, int iz, int iw, int iu, int iv,
            int seed)
        {
            var noise = HashCoordinates(ix, iy, iz, iw, iu, iv, seed) / 255.0;
            return noise * 2.0 - 1.0;
        }

        internal static double InternalGradientNoise(
            double x, double y,
            int ix, int iy,
            int seed)
        {
            var hash = HashCoordinates(ix, iy, seed);

            var dx = x - ix;
            var dy = y - iy;

            return dx * NoiseLookupTable.Gradient2D[hash, 0] +
                    dy * NoiseLookupTable.Gradient2D[hash, 1];
        }

        internal static double InternalGradientNoise(
            double x, double y, double z,
            int ix, int iy, int iz,
            int seed)
        {
            var hash = HashCoordinates(ix, iy, iz, seed);

            var dx = x - ix;
            var dy = y - iy;
            var dz = z - iz;

            return dx * NoiseLookupTable.Gradient3D[hash, 0] +
                    dy * NoiseLookupTable.Gradient3D[hash, 1] +
                    dz * NoiseLookupTable.Gradient3D[hash, 2];
        }

        internal static double InternalGradientNoise(
            double x, double y, double z, double w,
            int ix, int iy, int iz, int iw,
            int seed)
        {
            var hash = HashCoordinates(ix, iy, iz, iw, seed);

            var dx = x - ix;
            var dy = y - iy;
            var dz = z - iz;
            var dw = w - iw;

            return dx * NoiseLookupTable.Gradient4D[hash, 0] +
                    dy * NoiseLookupTable.Gradient4D[hash, 1] +
                    dz * NoiseLookupTable.Gradient4D[hash, 2] +
                    dw * NoiseLookupTable.Gradient4D[hash, 3];
        }

        internal static double InternalGradientNoise(
            double x, double y, double z, double w, double u, double v,
            int ix, int iy, int iz, int iw, int iu, int iv,
            int seed)
        {
            var hash = HashCoordinates(ix, iy, iz, iw, iu, iv, seed);

            var dx = x - ix;
            var dy = y - iy;
            var dz = z - iz;
            var dw = w - iw;
            var du = u - iu;
            var dv = v - iv;

            return dx * NoiseLookupTable.Gradient6D[hash, 0] +
                    dy * NoiseLookupTable.Gradient6D[hash, 1] +
                    dz * NoiseLookupTable.Gradient6D[hash, 2] +
                    dw * NoiseLookupTable.Gradient6D[hash, 3] +
                    du * NoiseLookupTable.Gradient6D[hash, 4] +
                    dv * NoiseLookupTable.Gradient6D[hash, 5];
        }

        internal static double interpolate_X_2(
            double x, double y, double xs,
            int x0, int x1, int iy,
            int seed, WorkerNoise2 noisefunc)
        {
            var v1 = noisefunc(x, y, x0, iy, seed);
            var v2 = noisefunc(x, y, x1, iy, seed);

            return Lerp(xs, v1, v2);
        }

        internal static double interpolate_XY_2(
            double x, double y, double xs, double ys,
            int x0, int x1, int y0, int y1,
            int seed, WorkerNoise2 noisefunc)
        {
            var v1 = interpolate_X_2(x, y, xs, x0, x1, y0, seed, noisefunc);
            var v2 = interpolate_X_2(x, y, xs, x0, x1, y1, seed, noisefunc);

            return Lerp(ys, v1, v2);
        }

        internal static double interpolate_X_3(
            double x, double y, double z, double xs,
            int x0, int x1, int iy, int iz,
            int seed, WorkerNoise3 noisefunc)
        {
            var v1 = noisefunc(x, y, z, x0, iy, iz, seed);
            var v2 = noisefunc(x, y, z, x1, iy, iz, seed);

            return Lerp(xs, v1, v2);
        }

        internal static double interpolate_XY_3(
            double x, double y, double z, double xs, double ys,
            int x0, int x1, int y0, int y1, int iz,
            int seed, WorkerNoise3 noisefunc)
        {
            var v1 = interpolate_X_3(x, y, z, xs, x0, x1, y0, iz, seed, noisefunc);
            var v2 = interpolate_X_3(x, y, z, xs, x0, x1, y1, iz, seed, noisefunc);

            return Lerp(ys, v1, v2);
        }

        internal static double interpolate_XYZ_3(
            double x, double y, double z, double xs, double ys, double zs,
            int x0, int x1, int y0, int y1, int z0, int z1,
            int seed, WorkerNoise3 noisefunc)
        {
            var v1 = interpolate_XY_3(x, y, z, xs, ys, x0, x1, y0, y1, z0, seed, noisefunc);
            var v2 = interpolate_XY_3(x, y, z, xs, ys, x0, x1, y0, y1, z1, seed, noisefunc);

            return Lerp(zs, v1, v2);
        }

        internal static double interpolate_X_4(
            double x, double y, double z, double w, double xs,
            int x0, int x1, int iy, int iz, int iw,
            int seed, WorkerNoise4 noisefunc)
        {
            var v1 = noisefunc(x, y, z, w, x0, iy, iz, iw, seed);
            var v2 = noisefunc(x, y, z, w, x1, iy, iz, iw, seed);

            return Lerp(xs, v1, v2);
        }

        internal static double interpolate_XY_4(
            double x, double y, double z, double w, double xs, double ys,
            int x0, int x1, int y0, int y1, int iz, int iw,
            int seed, WorkerNoise4 noisefunc)
        {
            var v1 = interpolate_X_4(x, y, z, w, xs, x0, x1, y0, iz, iw, seed, noisefunc);
            var v2 = interpolate_X_4(x, y, z, w, xs, x0, x1, y1, iz, iw, seed, noisefunc);

            return Lerp(ys, v1, v2);
        }

        internal static double interpolate_XYZ_4(
            double x, double y, double z, double w, double xs, double ys, double zs,
            int x0, int x1, int y0, int y1, int z0, int z1, int iw,
            int seed, WorkerNoise4 noisefunc)
        {
            var v1 = interpolate_XY_4(x, y, z, w, xs, ys, x0, x1, y0, y1, z0, iw, seed, noisefunc);
            var v2 = interpolate_XY_4(x, y, z, w, xs, ys, x0, x1, y0, y1, z1, iw, seed, noisefunc);

            return Lerp(zs, v1, v2);
        }

        internal static double interpolate_XYZW_4(
            double x, double y, double z, double w, double xs, double ys, double zs, double ws,
            int x0, int x1, int y0, int y1, int z0, int z1, int w0, int w1,
            int seed, WorkerNoise4 noisefunc)
        {
            var v1 = interpolate_XYZ_4(x, y, z, w, xs, ys, zs, x0, x1, y0, y1, z0, z1, w0, seed, noisefunc);
            var v2 = interpolate_XYZ_4(x, y, z, w, xs, ys, zs, x0, x1, y0, y1, z0, z1, w1, seed, noisefunc);

            return Lerp(ws, v1, v2);
        }

        internal static double interpolate_X_6(
            double x, double y, double z, double w, double u, double v, double xs,
            int x0, int x1, int iy, int iz, int iw, int iu, int iv,
            int seed, WorkerNoise6 noisefunc)
        {
            var v1 = noisefunc(x, y, z, w, u, v, x0, iy, iz, iw, iu, iv, seed);
            var v2 = noisefunc(x, y, z, w, u, v, x1, iy, iz, iw, iu, iv, seed);

            return Lerp(xs, v1, v2);
        }

        internal static double interpolate_XY_6(
            double x, double y, double z, double w, double u, double v,
            double xs, double ys,
            int x0, int x1, int y0, int y1, int iz, int iw, int iu, int iv,
            int seed, WorkerNoise6 noisefunc)
        {
            var v1 = interpolate_X_6(x, y, z, w, u, v, xs, x0, x1, y0, iz, iw, iu, iv, seed, noisefunc);
            var v2 = interpolate_X_6(x, y, z, w, u, v, xs, x0, x1, y1, iz, iw, iu, iv, seed, noisefunc);

            return Lerp(ys, v1, v2);
        }

        internal static double interpolate_XYZ_6(
            double x, double y, double z, double w, double u, double v,
            double xs, double ys, double zs,
            int x0, int x1, int y0, int y1, int z0, int z1, int iw, int iu, int iv,
            int seed, WorkerNoise6 noisefunc)
        {
            var v1 = interpolate_XY_6(x, y, z, w, u, v, xs, ys, x0, x1, y0, y1, z0, iw, iu, iv, seed, noisefunc);
            var v2 = interpolate_XY_6(x, y, z, w, u, v, xs, ys, x0, x1, y0, y1, z1, iw, iu, iv, seed, noisefunc);

            return Lerp(zs, v1, v2);
        }

        internal static double interpolate_XYZW_6(
            double x, double y, double z, double w, double u, double v,
            double xs, double ys, double zs, double ws,
            int x0, int x1, int y0, int y1, int z0, int z1, int w0, int w1, int iu, int iv,
            int seed, WorkerNoise6 noisefunc)
        {
            var v1 = interpolate_XYZ_6(x, y, z, w, u, v, xs, ys, zs, x0, x1, y0, y1, z0, z1, w0, iu, iv, seed, noisefunc);
            var v2 = interpolate_XYZ_6(x, y, z, w, u, v, xs, ys, zs, x0, x1, y0, y1, z0, z1, w1, iu, iv, seed, noisefunc);

            return Lerp(ws, v1, v2);
        }

        internal static double interpolate_XYZWU_6(
            double x, double y, double z, double w, double u, double v,
            double xs, double ys, double zs, double ws, double us,
            int x0, int x1, int y0, int y1, int z0, int z1, int w0, int w1, int u0, int u1, int iv,
            int seed, WorkerNoise6 noisefunc)
        {
            var v1 = interpolate_XYZW_6(x, y, z, w, u, v, xs, ys, zs, ws, x0, x1, y0, y1, z0, z1, w0, w1, u0, iv, seed, noisefunc);
            var v2 = interpolate_XYZW_6(x, y, z, w, u, v, xs, ys, zs, ws, x0, x1, y0, y1, z0, z1, w0, w1, u1, iv, seed, noisefunc);

            return Lerp(us, v1, v2);
        }

        internal static double interpolate_XYZWUV_6(
            double x, double y, double z, double w, double u, double v,
            double xs, double ys, double zs, double ws, double us, double vs,
            int x0, int x1, int y0, int y1, int z0, int z1, int w0, int w1, int u0, int u1, int v0, int v1,
            int seed, WorkerNoise6 noisefunc)
        {
            var val1 = interpolate_XYZWU_6(x, y, z, w, u, v, xs, ys, zs, ws, us, x0, x1, y0, y1, z0, z1, w0, w1, u0, u1, v0, seed, noisefunc);
            var val2 = interpolate_XYZWU_6(x, y, z, w, u, v, xs, ys, zs, ws, us, x0, x1, y0, y1, z0, z1, w0, w1, u0, u1, v1, seed, noisefunc);

            return Lerp(vs, val1, val2);
        }

        // The usable noise functions
        public static double ValueNoise(double x, double y, int seed, InterpolationDelegate interp)
        {
            var x0 = FastFloor(x);
            var y0 = FastFloor(y);

            var x1 = x0 + 1;
            var y1 = y0 + 1;

            var xs = interp(x - x0);
            var ys = interp(y - y0);

            return interpolate_XY_2(x, y, xs, ys, x0, x1, y0, y1, seed, InternalValueNoise);
        }

        public static double ValueNoise(double x, double y, double z, int seed, InterpolationDelegate interp)
        {
            var x0 = FastFloor(x);
            var y0 = FastFloor(y);
            var z0 = FastFloor(z);
            var x1 = x0 + 1;
            var y1 = y0 + 1;
            var z1 = z0 + 1;

            var xs = interp(x - x0);
            var ys = interp(y - y0);
            var zs = interp(z - z0);

            return interpolate_XYZ_3(x, y, z, xs, ys, zs, x0, x1, y0, y1, z0, z1, seed, InternalValueNoise);
        }

        public static double ValueNoise(double x, double y, double z, double w, int seed, InterpolationDelegate interp)
        {
            var x0 = FastFloor(x);
            var y0 = FastFloor(y);
            var z0 = FastFloor(z);
            var w0 = FastFloor(w);

            var x1 = x0 + 1;
            var y1 = y0 + 1;
            var z1 = z0 + 1;
            var w1 = w0 + 1;

            var xs = interp(x - x0);
            var ys = interp(y - y0);
            var zs = interp(z - z0);
            var ws = interp(w - w0);

            return interpolate_XYZW_4(x, y, z, w, xs, ys, zs, ws, x0, x1, y0, y1, z0, z1, w0, w1, seed, InternalValueNoise);
        }

        public static double ValueNoise(double x, double y, double z, double w, double u, double v, int seed, InterpolationDelegate interp)
        {
            var x0 = FastFloor(x);
            var y0 = FastFloor(y);
            var z0 = FastFloor(z);
            var w0 = FastFloor(w);
            var u0 = FastFloor(u);
            var v0 = FastFloor(v);

            var x1 = x0 + 1;
            var y1 = y0 + 1;
            var z1 = z0 + 1;
            var w1 = w0 + 1;
            var u1 = u0 + 1;
            var v1 = v0 + 1;

            var xs = interp(x - x0);
            var ys = interp(y - y0);
            var zs = interp(z - z0);
            var ws = interp(w - w0);
            var us = interp(u - u0);
            var vs = interp(v - v0);

            return interpolate_XYZWUV_6(x, y, z, w, u, v, xs, ys, zs, ws, us, vs, x0, x1, y0, y1, z0, z1, w0, w1, u0, u1, v0, v1, seed, InternalValueNoise);
        }

        public static double GradientNoise(double x, double y, int seed, InterpolationDelegate interp)
        {
            var x0 = FastFloor(x);
            var y0 = FastFloor(y);

            var x1 = x0 + 1;
            var y1 = y0 + 1;

            var xs = interp(x - x0);
            var ys = interp(y - y0);

            return interpolate_XY_2(x, y, xs, ys, x0, x1, y0, y1, seed, InternalGradientNoise);
        }

        public static double GradientNoise(double x, double y, double z, int seed, InterpolationDelegate interp)
        {
            var x0 = FastFloor(x);
            var y0 = FastFloor(y);
            var z0 = FastFloor(z);

            var x1 = x0 + 1;
            var y1 = y0 + 1;
            var z1 = z0 + 1;

            var xs = interp(x - x0);
            var ys = interp(y - y0);
            var zs = interp(z - z0);

            return interpolate_XYZ_3(x, y, z, xs, ys, zs, x0, x1, y0, y1, z0, z1, seed, InternalGradientNoise);
        }

        public static double GradientNoise(double x, double y, double z, double w, int seed, InterpolationDelegate interp)
        {
            var x0 = FastFloor(x);
            var y0 = FastFloor(y);
            var z0 = FastFloor(z);
            var w0 = FastFloor(w);

            var x1 = x0 + 1;
            var y1 = y0 + 1;
            var z1 = z0 + 1;
            var w1 = w0 + 1;

            var xs = interp(x - x0);
            var ys = interp(y - y0);
            var zs = interp(z - z0);
            var ws = interp(w - w0);

            return interpolate_XYZW_4(x, y, z, w, xs, ys, zs, ws, x0, x1, y0, y1, z0, z1, w0, w1, seed, InternalGradientNoise);
        }

        public static double GradientNoise(double x, double y, double z, double w, double u, double v, int seed, InterpolationDelegate interp)
        {
            var x0 = FastFloor(x);
            var y0 = FastFloor(y);
            var z0 = FastFloor(z);
            var w0 = FastFloor(w);
            var u0 = FastFloor(u);
            var v0 = FastFloor(v);

            var x1 = x0 + 1;
            var y1 = y0 + 1;
            var z1 = z0 + 1;
            var w1 = w0 + 1;
            var u1 = u0 + 1;
            var v1 = v0 + 1;

            var xs = interp(x - x0);
            var ys = interp(y - y0);
            var zs = interp(z - z0);
            var ws = interp(w - w0);
            var us = interp(u - u0);
            var vs = interp(v - v0);

            return interpolate_XYZWUV_6(x, y, z, w, u, v, xs, ys, zs, ws, us, vs, x0, x1, y0, y1, z0, z1, w0, w1, u0, u1, v0, v1, seed, InternalGradientNoise);
        }

        public static double GradientValueNoise(double x, double y, int seed, InterpolationDelegate interp)
        {
            return ValueNoise(x, y, seed, interp) + GradientNoise(x, y, seed, interp);
        }

        public static double GradientValueNoise(double x, double y, double z, int seed, InterpolationDelegate interp)
        {
            return ValueNoise(x, y, z, seed, interp) + GradientNoise(x, y, z, seed, interp);
        }

        public static double GradientValueNoise(double x, double y, double z, double w, int seed, InterpolationDelegate interp)
        {
            return ValueNoise(x, y, z, w, seed, interp) + GradientNoise(x, y, z, w, seed, interp);
        }

        public static double GradientValueNoise(double x, double y, double z, double w, double u, double v, int seed, InterpolationDelegate interp)
        {
            return ValueNoise(x, y, z, w, u, v, seed, interp) + GradientNoise(x, y, z, w, u, v, seed, interp);
        }

        public static double WhiteNoise(double x, double y, int seed, InterpolationDelegate interp)
        {
            return NoiseLookupTable.WhiteNoise[(byte)HashCoordinates(x, y, seed)];
        }

        public static double WhiteNoise(double x, double y, double z, int seed, InterpolationDelegate interp)
        {
            return NoiseLookupTable.WhiteNoise[(byte)HashCoordinates(x, y, z, seed)];
        }

        public static double WhiteNoise(double x, double y, double z, double w, int seed, InterpolationDelegate interp)
        {
            return NoiseLookupTable.WhiteNoise[(byte)HashCoordinates(x, y, z, w, seed)];
        }

        public static double WhiteNoise(double x, double y, double z, double w, double u, double v, int seed, InterpolationDelegate interp)
        {
            return NoiseLookupTable.WhiteNoise[(byte)HashCoordinates(x, y, z, w, u, v, seed)];
        }

        public static void CellularFunction(double x, double y, int seed, double[] f, double[] disp)
        {
            var xInt = FastFloor(x);
            var yInt = FastFloor(y);

            for (var c = 0; c < 4; ++c)
            {
                f[c] = 99999.0;
                disp[c] = 0.0;
            }

            for (var ycur = yInt - 3; ycur <= yInt + 3; ++ycur)
            {
                for (var xcur = xInt - 3; xcur <= xInt + 3; ++xcur)
                {
                    var xpos = xcur + InternalValueNoise(x, y, xcur, ycur, seed);
                    var ypos = ycur + InternalValueNoise(x, y, xcur, ycur, seed + 1);
                    var xdist = xpos - x;
                    var ydist = ypos - y;
                    var dist = xdist * xdist + ydist * ydist;
                    var xval = FastFloor(xpos);
                    var yval = FastFloor(ypos);
                    var dsp = InternalValueNoise(x, y, xval, yval, seed + 3);
                    AddDistance(f, disp, dist, dsp);
                }
            }
        }

        public static void CellularFunction(double x, double y, double z, int seed, double[] f, double[] disp)
        {
            var xInt = FastFloor(x);
            var yInt = FastFloor(y);
            var zInt = FastFloor(z);

            for (var c = 0; c < 4; ++c)
            {
                f[c] = 99999.0;
                disp[c] = 0.0;
            }

            for (var zcur = zInt - 2; zcur <= zInt + 2; ++zcur)
            {
                for (var ycur = yInt - 2; ycur <= yInt + 2; ++ycur)
                {
                    for (var xcur = xInt - 2; xcur <= xInt + 2; ++xcur)
                    {
                        var xpos = xcur + InternalValueNoise(x, y, z, xcur, ycur, zcur, seed);
                        var ypos = ycur + InternalValueNoise(x, y, z, xcur, ycur, zcur, seed + 1);
                        var zpos = zcur + InternalValueNoise(x, y, z, xcur, ycur, zcur, seed + 2);
                        var xdist = xpos - x;
                        var ydist = ypos - y;
                        var zdist = zpos - z;
                        var dist = xdist * xdist + ydist * ydist + zdist * zdist;
                        var xval = FastFloor(xpos);
                        var yval = FastFloor(ypos);
                        var zval = FastFloor(zpos);
                        var dsp = InternalValueNoise(x, y, z, xval, yval, zval, seed + 3);
                        AddDistance(f, disp, dist, dsp);
                    }
                }
            }
        }

        public static void CellularFunction(double x, double y, double z, double w, int seed, double[] f, double[] disp)
        {
            var xInt = FastFloor(x);
            var yInt = FastFloor(y);
            var zInt = FastFloor(z);
            var wInt = FastFloor(w);

            for (var c = 0; c < 4; ++c)
            {
                f[c] = 99999.0;
                disp[c] = 0.0;
            }

            for (var wcur = wInt - 2; wcur <= wInt + 2; ++wcur)
            {
                for (var zcur = zInt - 2; zcur <= zInt + 2; ++zcur)
                {
                    for (var ycur = yInt - 2; ycur <= yInt + 2; ++ycur)
                    {
                        for (var xcur = xInt - 2; xcur <= xInt + 2; ++xcur)
                        {
                            var xpos = xcur + InternalValueNoise(x, y, z, w, xcur, ycur, zcur, wcur, seed);
                            var ypos = ycur + InternalValueNoise(x, y, z, w, xcur, ycur, zcur, wcur, seed + 1);
                            var zpos = zcur + InternalValueNoise(x, y, z, w, xcur, ycur, zcur, wcur, seed + 2);
                            var wpos = wcur + InternalValueNoise(x, y, z, w, xcur, ycur, zcur, wcur, seed + 3);
                            var xdist = xpos - x;
                            var ydist = ypos - y;
                            var zdist = zpos - z;
                            var wdist = wpos - w;
                            var dist = xdist * xdist + ydist * ydist + zdist * zdist + wdist * wdist;
                            var xval = FastFloor(xpos);
                            var yval = FastFloor(ypos);
                            var zval = FastFloor(zpos);
                            var wval = FastFloor(wpos);
                            var dsp = InternalValueNoise(x, y, z, w, xval, yval, zval, wval, seed + 3);
                            AddDistance(f, disp, dist, dsp);
                        }
                    }
                }
            }
        }

        public static void CellularFunction(double x, double y, double z, double w, double u, double v, int seed, double[] f, double[] disp)
        {
            var xInt = FastFloor(x);
            var yInt = FastFloor(y);
            var zInt = FastFloor(z);
            var wInt = FastFloor(w);
            var uInt = FastFloor(u);
            var vInt = FastFloor(v);

            for (var c = 0; c < 4; ++c)
            {
                f[c] = 99999.0;
                disp[c] = 0.0;
            }

            for (var vcur = vInt - 1; vcur <= vInt + 1; ++vcur)
            {
                for (var ucur = uInt - 1; ucur <= uInt + 1; ++ucur)
                {
                    for (var wcur = wInt - 2; wcur <= wInt + 2; ++wcur)
                    {
                        for (var zcur = zInt - 2; zcur <= zInt + 2; ++zcur)
                        {
                            for (var ycur = yInt - 2; ycur <= yInt + 2; ++ycur)
                            {
                                for (var xcur = xInt - 2; xcur <= xInt + 2; ++xcur)
                                {
                                    var xpos = xcur + InternalValueNoise(x, y, z, w, u, v, xcur, ycur, zcur, wcur, ucur, vcur, seed);
                                    var ypos = ycur + InternalValueNoise(x, y, z, w, u, v, xcur, ycur, zcur, wcur, ucur, vcur, seed + 1);
                                    var zpos = zcur + InternalValueNoise(x, y, z, w, u, v, xcur, ycur, zcur, wcur, ucur, vcur, seed + 2);
                                    var wpos = wcur + InternalValueNoise(x, y, z, w, u, v, xcur, ycur, zcur, wcur, ucur, vcur, seed + 3);
                                    var upos = ucur + InternalValueNoise(x, y, z, w, u, v, xcur, ycur, zcur, wcur, ucur, vcur, seed + 4);
                                    var vpos = vcur + InternalValueNoise(x, y, z, w, u, v, xcur, ycur, zcur, wcur, ucur, vcur, seed + 5);
                                    var xdist = xpos - x;
                                    var ydist = ypos - y;
                                    var zdist = zpos - z;
                                    var wdist = wpos - w;
                                    var udist = upos - u;
                                    var vdist = vpos - v;
                                    var dist = xdist * xdist + ydist * ydist + zdist * zdist + wdist * wdist + udist * udist + vdist * vdist;
                                    var xval = FastFloor(xpos);
                                    var yval = FastFloor(ypos);
                                    var zval = FastFloor(zpos);
                                    var wval = FastFloor(wpos);
                                    var uval = FastFloor(upos);
                                    var vval = FastFloor(vpos);
                                    var dsp = InternalValueNoise(x, y, z, w, u, v, xval, yval, zval, wval, uval, vval, seed + 6);
                                    AddDistance(f, disp, dist, dsp);
                                }
                            }
                        }
                    }
                }
            }
        }

        private const double F2 = 0.36602540378443864676372317075294;
        private const double G2 = 0.21132486540518711774542560974902;
        private const double F3 = 1.0 / 3.0;
        private const double G3 = 1.0 / 6.0;

        private static readonly int[,] Simplex = {
                                            {0, 1, 2, 3}, {0, 1, 3, 2}, {0, 0, 0, 0}, {0, 2, 3, 1},
                                            {0, 0, 0, 0}, {0, 0, 0, 0}, {0, 0, 0, 0}, {1, 2, 3, 0},
                                            {0, 2, 1, 3}, {0, 0, 0, 0}, {0, 3, 1, 2}, {0, 3, 2, 1},
                                            {0, 0, 0, 0}, {0, 0, 0, 0}, {0, 0, 0, 0}, {1, 3, 2, 0},
                                            {0, 0, 0, 0}, {0, 0, 0, 0}, {0, 0, 0, 0}, {0, 0, 0, 0},
                                            {0, 0, 0, 0}, {0, 0, 0, 0}, {0, 0, 0, 0}, {0, 0, 0, 0},
                                            {1, 2, 0, 3}, {0, 0, 0, 0}, {1, 3, 0, 2}, {0, 0, 0, 0},
                                            {0, 0, 0, 0}, {0, 0, 0, 0}, {2, 3, 0, 1}, {2, 3, 1, 0},
                                            {1, 0, 2, 3}, {1, 0, 3, 2}, {0, 0, 0, 0}, {0, 0, 0, 0},
                                            {0, 0, 0, 0}, {2, 0, 3, 1}, {0, 0, 0, 0}, {2, 1, 3, 0},
                                            {0, 0, 0, 0}, {0, 0, 0, 0}, {0, 0, 0, 0}, {0, 0, 0, 0},
                                            {0, 0, 0, 0}, {0, 0, 0, 0}, {0, 0, 0, 0}, {0, 0, 0, 0},
                                            {2, 0, 1, 3}, {0, 0, 0, 0}, {0, 0, 0, 0}, {0, 0, 0, 0},
                                            {3, 0, 1, 2}, {3, 0, 2, 1}, {0, 0, 0, 0}, {3, 1, 2, 0},
                                            {2, 1, 0, 3}, {0, 0, 0, 0}, {0, 0, 0, 0}, {0, 0, 0, 0},
                                            {3, 1, 0, 2}, {0, 0, 0, 0}, {3, 2, 0, 1}, {3, 2, 1, 0}
                                        };

        internal struct VectorOrdering4
        {
            internal double Coordinates;

            internal int X;

            internal int Y;

            internal int Z;

            internal int W;

            internal VectorOrdering4(double v, int x, int y, int z, int w)
            {
                Coordinates = v;
                X = x;
                Y = y;
                Z = z;
                W = w;
            }
        }

        internal static int VectorOrdering4Compare(VectorOrdering4 v1, VectorOrdering4 v2)
        {
            return v1.Coordinates.CompareTo(v2.Coordinates);
        }

        internal struct VectorOrdering
        {
            internal double Value;

            internal int Axis;

            internal VectorOrdering(double v, int a)
            {
                Value = v;
                Axis = a;
            }
        }

        internal static int VectorOrderingCompare(VectorOrdering v1, VectorOrdering v2)
        {
            return v1.Value.CompareTo(v2.Value);
        }

        internal static void SortBy4(double[] l1, int[] l2)
        {
            var a = new VectorOrdering[4];
            for (var c = 0; c < 4; c += 1)
            {
                a[c].Value = l1[c];
                a[c].Axis = l2[c];
            }

            Array.Sort(a, VectorOrderingCompare);

            for (var c = 0; c < 4; c += 1)
            {
                l2[c] = a[c].Axis;
            }
        }

        internal static void SortBy6(double[] l1, int[] l2)
        {
            var a = new VectorOrdering[6];
            for (var c = 0; c < 6; c += 1)
            {
                a[c].Value = l1[c];
                a[c].Axis = l2[c];
            }

            Array.Sort(a, VectorOrderingCompare);

            for (var c = 0; c < 6; c += 1)
            {
                l2[c] = a[c].Axis;
            }
        }

        public static double SimplexNoise(double x, double y, int seed, InterpolationDelegate interp)
        {
            double s = (x + y) * F2;
            int i = FastFloor(x + s);
            int j = FastFloor(y + s);

            double t = (i + j) * G2;
            double X0 = i - t;
            double Y0 = j - t;
            double x0 = x - X0;
            double y0 = y - Y0;

            int i1, j1;
            if (x0 > y0)
            {
                i1 = 1;
                j1 = 0;
            }
            else
            {
                i1 = 0;
                j1 = 1;
            }

            double x1 = x0 - i1 + G2;
            double y1 = y0 - j1 + G2;
            double x2 = x0 - 1.0 + 2.0 * G2;
            double y2 = y0 - 1.0 + 2.0 * G2;

            // Hash the triangle coordinates to index the gradient table
            uint h0 = HashCoordinates(i, j, seed);
            uint h1 = HashCoordinates(i + i1, j + j1, seed);
            uint h2 = HashCoordinates(i + 1, j + 1, seed);

            // Now, index the tables
            double[] g0 = { NoiseLookupTable.Gradient2D[h0, 0], NoiseLookupTable.Gradient2D[h0, 1] };
            double[] g1 = { NoiseLookupTable.Gradient2D[h1, 0], NoiseLookupTable.Gradient2D[h1, 1] };
            double[] g2 = { NoiseLookupTable.Gradient2D[h2, 0], NoiseLookupTable.Gradient2D[h2, 1] };

            double n0, n1, n2;
            // Calculate the contributions from the 3 corners
            double t0 = 0.5 - x0 * x0 - y0 * y0;
            if (t0 < 0) n0 = 0;
            else
            {
                t0 *= t0;
                n0 = t0 * t0 * ArrayDot(g0, x0, y0);
            }

            double t1 = 0.5 - x1 * x1 - y1 * y1;
            if (t1 < 0) n1 = 0;
            else
            {
                t1 *= t1;
                n1 = t1 * t1 * ArrayDot(g1, x1, y1);
            }

            double t2 = 0.5 - x2 * x2 - y2 * y2;
            if (t2 < 0) n2 = 0;
            else
            {
                t2 *= t2;
                n2 = t2 * t2 * ArrayDot(g2, x2, y2);
            }

            // Add contributions together
            return 70.0 * (n0 + n1 + n2) * 1.42188695 + 0.001054489;
        }

        public static double SimplexNoise(double x, double y, double z, int seed, InterpolationDelegate interp)
        {
            //static Double F3 = 1.0/3.0;
            //static Double G3 = 1.0/6.0;
            double n0, n1, n2, n3;

            double s = (x + y + z) * F3;
            int i = FastFloor(x + s);
            int j = FastFloor(y + s);
            int k = FastFloor(z + s);

            double t = (i + j + k) * G3;
            double X0 = i - t;
            double Y0 = j - t;
            double Z0 = k - t;

            double x0 = x - X0;
            double y0 = y - Y0;
            double z0 = z - Z0;

            int i1, j1, k1;
            int i2, j2, k2;

            if (x0 >= y0)
            {
                if (y0 >= z0)
                {
                    i1 = 1;
                    j1 = 0;
                    k1 = 0;
                    i2 = 1;
                    j2 = 1;
                    k2 = 0;
                }
                else if (x0 >= z0)
                {
                    i1 = 1;
                    j1 = 0;
                    k1 = 0;
                    i2 = 1;
                    j2 = 0;
                    k2 = 1;
                }
                else
                {
                    i1 = 0;
                    j1 = 0;
                    k1 = 1;
                    i2 = 1;
                    j2 = 0;
                    k2 = 1;
                }
            }
            else
            {
                if (y0 < z0)
                {
                    i1 = 0;
                    j1 = 0;
                    k1 = 1;
                    i2 = 0;
                    j2 = 1;
                    k2 = 1;
                }
                else if (x0 < z0)
                {
                    i1 = 0;
                    j1 = 1;
                    k1 = 0;
                    i2 = 0;
                    j2 = 1;
                    k2 = 1;
                }
                else
                {
                    i1 = 0;
                    j1 = 1;
                    k1 = 0;
                    i2 = 1;
                    j2 = 1;
                    k2 = 0;
                }
            }

            var x1 = x0 - i1 + G3;
            var y1 = y0 - j1 + G3;
            var z1 = z0 - k1 + G3;
            var x2 = x0 - i2 + 2.0 * G3;
            var y2 = y0 - j2 + 2.0 * G3;
            var z2 = z0 - k2 + 2.0 * G3;
            var x3 = x0 - 1.0 + 3.0 * G3;
            var y3 = y0 - 1.0 + 3.0 * G3;
            var z3 = z0 - 1.0 + 3.0 * G3;

            var h0 = HashCoordinates(i, j, k, seed);
            var h1 = HashCoordinates(i + i1, j + j1, k + k1, seed);
            var h2 = HashCoordinates(i + i2, j + j2, k + k2, seed);
            var h3 = HashCoordinates(i + 1, j + 1, k + 1, seed);

            double[] g0 = {
                              NoiseLookupTable.Gradient3D[h0, 0], NoiseLookupTable.Gradient3D[h0, 1],
                              NoiseLookupTable.Gradient3D[h0, 2]
                          };
            double[] g1 = {
                              NoiseLookupTable.Gradient3D[h1, 0], NoiseLookupTable.Gradient3D[h1, 1],
                              NoiseLookupTable.Gradient3D[h1, 2]
                          };
            double[] g2 = {
                              NoiseLookupTable.Gradient3D[h2, 0], NoiseLookupTable.Gradient3D[h2, 1],
                              NoiseLookupTable.Gradient3D[h2, 2]
                          };
            double[] g3 = {
                              NoiseLookupTable.Gradient3D[h3, 0], NoiseLookupTable.Gradient3D[h3, 1],
                              NoiseLookupTable.Gradient3D[h3, 2]
                          };

            var t0 = 0.6 - x0 * x0 - y0 * y0 - z0 * z0;
            if (t0 < 0.0) n0 = 0.0;
            else
            {
                t0 *= t0;
                n0 = t0 * t0 * ArrayDot(g0, x0, y0, z0);
            }

            var t1 = 0.6 - x1 * x1 - y1 * y1 - z1 * z1;
            if (t1 < 0.0) n1 = 0.0;
            else
            {
                t1 *= t1;
                n1 = t1 * t1 * ArrayDot(g1, x1, y1, z1);
            }

            var t2 = 0.6 - x2 * x2 - y2 * y2 - z2 * z2;
            if (t2 < 0) n2 = 0.0;
            else
            {
                t2 *= t2;
                n2 = t2 * t2 * ArrayDot(g2, x2, y2, z2);
            }

            var t3 = 0.6 - x3 * x3 - y3 * y3 - z3 * z3;
            if (t3 < 0) n3 = 0.0;
            else
            {
                t3 *= t3;
                n3 = t3 * t3 * ArrayDot(g3, x3, y3, z3);
            }

            return 32.0 * (n0 + n1 + n2 + n3) * 1.25086885 + 0.0003194984;
        }

        public static double SimplexNoise(double x, double y, double z, double w, int seed, InterpolationDelegate interp)
        {
            double F4 = (Math.Sqrt(5.0) - 1.0) / 4.0;
            double G4 = (5.0 - Math.Sqrt(5.0)) / 20.0;
            double n0, n1, n2, n3, n4; // Noise contributions from the five corners
            // Skew the (x,y,z,w) space to determine which cell of 24 simplices we're in
            double s = (x + y + z + w) * F4; // Factor for 4D skewing
            int i = FastFloor(x + s);
            int j = FastFloor(y + s);
            int k = FastFloor(z + s);
            int l = FastFloor(w + s);
            double t = (i + j + k + l) * G4; // Factor for 4D unskewing
            double X0 = i - t; // Unskew the cell origin back to (x,y,z,w) space
            double Y0 = j - t;
            double Z0 = k - t;
            double W0 = l - t;
            double x0 = x - X0; // The x,y,z,w distances from the cell origin
            double y0 = y - Y0;
            double z0 = z - Z0;
            double w0 = w - W0;
            // For the 4D case, the simplex is a 4D shape I won't even try to describe.
            // To find out which of the 24 possible simplices we're in, we need to
            // determine the magnitude ordering of x0, y0, z0 and w0.
            // The method below is a good way of finding the ordering of x,y,z,w and
            // then find the correct traversal order for the simplex we’re in.
            // First, six pair-wise comparisons are performed between each possible pair
            // of the four coordinates, and the results are used to add up binary bits
            // for an integer index.
            int c1 = x0 > y0 ? 32 : 0;
            int c2 = x0 > z0 ? 16 : 0;
            int c3 = y0 > z0 ? 8 : 0;
            int c4 = x0 > w0 ? 4 : 0;
            int c5 = y0 > w0 ? 2 : 0;
            int c6 = z0 > w0 ? 1 : 0;
            int c = c1 + c2 + c3 + c4 + c5 + c6;
            int i1, j1, k1, l1; // The integer offsets for the second simplex corner
            int i2, j2, k2, l2; // The integer offsets for the third simplex corner
            int i3, j3, k3, l3; // The integer offsets for the fourth simplex corner
            // simplex[c] is a 4-vector with the numbers 0, 1, 2 and 3 in some order.
            // Many values of c will never occur, since e.g. x>y>z>w makes x<z, y<w and x<w
            // impossible. Only the 24 indices which have non-zero entries make any sense.
            // We use a thresholding to set the coordinates in turn from the largest magnitude.
            // The number 3 in the "simplex" array is at the position of the largest coordinate.
            i1 = Simplex[c, 0] >= 3 ? 1 : 0;
            j1 = Simplex[c, 1] >= 3 ? 1 : 0;
            k1 = Simplex[c, 2] >= 3 ? 1 : 0;
            l1 = Simplex[c, 3] >= 3 ? 1 : 0;
            // The number 2 in the "simplex" array is at the second largest coordinate.
            i2 = Simplex[c, 0] >= 2 ? 1 : 0;
            j2 = Simplex[c, 1] >= 2 ? 1 : 0;
            k2 = Simplex[c, 2] >= 2 ? 1 : 0;
            l2 = Simplex[c, 3] >= 2 ? 1 : 0;
            // The number 1 in the "simplex" array is at the second smallest coordinate.
            i3 = Simplex[c, 0] >= 1 ? 1 : 0;
            j3 = Simplex[c, 1] >= 1 ? 1 : 0;
            k3 = Simplex[c, 2] >= 1 ? 1 : 0;
            l3 = Simplex[c, 3] >= 1 ? 1 : 0;
            // The fifth corner has all coordinate offsets = 1, so no need to look that up.
            double x1 = x0 - i1 + G4; // Offsets for second corner in (x,y,z,w) coords
            double y1 = y0 - j1 + G4;
            double z1 = z0 - k1 + G4;
            double w1 = w0 - l1 + G4;
            double x2 = x0 - i2 + 2.0 * G4; // Offsets for third corner in (x,y,z,w) coords
            double y2 = y0 - j2 + 2.0 * G4;
            double z2 = z0 - k2 + 2.0 * G4;
            double w2 = w0 - l2 + 2.0 * G4;
            double x3 = x0 - i3 + 3.0 * G4; // Offsets for fourth corner in (x,y,z,w) coords
            double y3 = y0 - j3 + 3.0 * G4;
            double z3 = z0 - k3 + 3.0 * G4;
            double w3 = w0 - l3 + 3.0 * G4;
            double x4 = x0 - 1.0 + 4.0 * G4; // Offsets for last corner in (x,y,z,w) coords
            double y4 = y0 - 1.0 + 4.0 * G4;
            double z4 = z0 - 1.0 + 4.0 * G4;
            double w4 = w0 - 1.0 + 4.0 * G4;
            // Work out the hashed gradient indices of the five simplex corners
            uint h0 = HashCoordinates(i, j, k, l, seed);
            uint h1 = HashCoordinates(i + i1, j + j1, k + k1, l + l1, seed);
            uint h2 = HashCoordinates(i + i2, j + j2, k + k2, l + l2, seed);
            uint h3 = HashCoordinates(i + i3, j + j3, k + k3, l + l3, seed);
            uint h4 = HashCoordinates(i + 1, j + 1, k + 1, l + 1, seed);

            double[] g0 = {
                              NoiseLookupTable.Gradient4D[h0, 0], NoiseLookupTable.Gradient4D[h0, 1],
                              NoiseLookupTable.Gradient4D[h0, 2], NoiseLookupTable.Gradient4D[h0, 3]
                          };
            double[] g1 = {
                              NoiseLookupTable.Gradient4D[h1, 0], NoiseLookupTable.Gradient4D[h1, 1],
                              NoiseLookupTable.Gradient4D[h1, 2], NoiseLookupTable.Gradient4D[h1, 3]
                          };
            double[] g2 = {
                              NoiseLookupTable.Gradient4D[h2, 0], NoiseLookupTable.Gradient4D[h2, 1],
                              NoiseLookupTable.Gradient4D[h2, 2], NoiseLookupTable.Gradient4D[h2, 3]
                          };
            double[] g3 = {
                              NoiseLookupTable.Gradient4D[h3, 0], NoiseLookupTable.Gradient4D[h3, 1],
                              NoiseLookupTable.Gradient4D[h3, 2], NoiseLookupTable.Gradient4D[h3, 3]
                          };
            double[] g4 = {
                              NoiseLookupTable.Gradient4D[h4, 0], NoiseLookupTable.Gradient4D[h4, 1],
                              NoiseLookupTable.Gradient4D[h4, 2], NoiseLookupTable.Gradient4D[h4, 3]
                          };

            // Calculate the contribution from the five corners
            double t0 = 0.6 - x0 * x0 - y0 * y0 - z0 * z0 - w0 * w0;
            if (t0 < 0) n0 = 0.0;
            else
            {
                t0 *= t0;
                n0 = t0 * t0 * ArrayDot(g0, x0, y0, z0, w0);
            }
            double t1 = 0.6 - x1 * x1 - y1 * y1 - z1 * z1 - w1 * w1;
            if (t1 < 0) n1 = 0.0;
            else
            {
                t1 *= t1;
                n1 = t1 * t1 * ArrayDot(g1, x1, y1, z1, w1);
            }
            double t2 = 0.6 - x2 * x2 - y2 * y2 - z2 * z2 - w2 * w2;
            if (t2 < 0) n2 = 0.0;
            else
            {
                t2 *= t2;
                n2 = t2 * t2 * ArrayDot(g2, x2, y2, z2, w2);
            }
            double t3 = 0.6 - x3 * x3 - y3 * y3 - z3 * z3 - w3 * w3;
            if (t3 < 0) n3 = 0.0;
            else
            {
                t3 *= t3;
                n3 = t3 * t3 * ArrayDot(g3, x3, y3, z3, w3);
            }
            double t4 = 0.6 - x4 * x4 - y4 * y4 - z4 * z4 - w4 * w4;
            if (t4 < 0) n4 = 0.0;
            else
            {
                t4 *= t4;
                n4 = t4 * t4 * ArrayDot(g4, x4, y4, z4, w4);
            }
            // Sum up and scale the result to cover the range [-1,1]
            return 27.0 * (n0 + n1 + n2 + n3 + n4);
        }

        public static double NewSimplexNoise4D(double x, double y, double z, double w, int seed, InterpolationDelegate interp)
        {
            var f4 = (Math.Sqrt(5.0) - 1.0) / 4.0;
            var sideLength = 2.0 / (4.0 * f4 + 1.0);
            var a = Math.Sqrt(sideLength * sideLength - sideLength / 2.0 * (sideLength / 2.0));
            var cornerToFace = Math.Sqrt(a * a + a / 2.0 * (a / 2.0));
            var cornerToFaceSquared = cornerToFace * cornerToFace;

            var valueScaler = Math.Pow(3.0, -0.5);

            var g4 = f4 / (1.0 + 4.0 * f4);
            valueScaler *= Math.Pow(3.0, -3.5) * 100.0 + 13.0;

            double[] loc = { x, y, z, w };
            double s = 0;
            for (var c = 0; c < 4; ++c)
                s += loc[c];
            s *= f4;

            var skewLoc = new[] { FastFloor(x + s), FastFloor(y + s), FastFloor(z + s), FastFloor(w + s) };
            var intLoc = new[] { FastFloor(x + s), FastFloor(y + s), FastFloor(z + s), FastFloor(w + s) };
            var unskew = 0.00;
            for (var c = 0; c < 4; ++c)
                unskew += skewLoc[c];
            unskew *= g4;
            var cellDist = new[]
            {
                loc[0] - skewLoc[0] + unskew, loc[1] - skewLoc[1] + unskew,
                loc[2] - skewLoc[2] + unskew, loc[3] - skewLoc[3] + unskew
            };
            var distOrder = new[] { 0, 1, 2, 3 };
            SortBy4(cellDist, distOrder);

            var newDistOrder = new[] { -1, distOrder[0], distOrder[1], distOrder[2], distOrder[3] };

            var n = 0.00;
            var skewOffset = 0.00;

            for (var c = 0; c < 5; ++c)
            {
                var i = newDistOrder[c];
                if (i != -1)
                    intLoc[i] += 1;

                var u = new double[4];
                for (var d = 0; d < 4; ++d)
                {
                    u[d] = cellDist[d] - (intLoc[d] - skewLoc[d]) + skewOffset;
                }

                var t = cornerToFaceSquared;

                for (var d = 0; d < 4; ++d)
                {
                    t -= u[d] * u[d];
                }

                if (t > 0.0)
                {
                    var h = HashCoordinates(intLoc[0], intLoc[1], intLoc[2], intLoc[3], seed);
                    var gr = 0.00;
                    for (var d = 0; d < 4; ++d)
                    {
                        gr += NoiseLookupTable.Gradient4D[h, d] * u[d];
                    }

                    n += gr * t * t * t * t;
                }
                skewOffset += g4;
            }
            n *= valueScaler;
            return n;
        }

        public static double SimplexNoise(double x, double y, double z, double w, double u, double v, int seed, InterpolationDelegate interp)
        {
            // Skew
            var f4 = (Math.Sqrt(7.0) - 1.0) / 6.0;

            // Unskew
            var g4 = f4 / (1.0 + 6.0 * f4);

            var sideLength = Math.Sqrt(6.0) / (6.0 * f4 + 1.0);
            var a = Math.Sqrt(sideLength * sideLength - sideLength / 2.0 * (sideLength / 2.0));
            var cornerFace = Math.Sqrt(a * a + a / 2.0 * (a / 2.0));

            var cornerFaceSqrd = cornerFace * cornerFace;

            var valueScaler = Math.Pow(5.0, -0.5);
            valueScaler *= Math.Pow(5.0, -3.5) * 100 + 13;

            var loc = new[] { x, y, z, w, u, v };
            var s = 0.00;
            for (var c = 0; c < 6; ++c)
                s += loc[c];
            s *= f4;

            var skewLoc = new[]{
                FastFloor(x + s), FastFloor(y + s), FastFloor(z + s),
                FastFloor(w + s), FastFloor(u + s), FastFloor(v + s)
            };
            var intLoc = new[]{
                FastFloor(x + s), FastFloor(y + s), FastFloor(z + s),
                FastFloor(w + s), FastFloor(u + s), FastFloor(v + s)
            };
            var unskew = 0.0;
            for (var c = 0; c < 6; ++c)
                unskew += skewLoc[c];
            unskew *= g4;

            var cellDist = new[]
            {
                loc[0] - skewLoc[0] + unskew, loc[1] - skewLoc[1] + unskew,
                loc[2] - skewLoc[2] + unskew, loc[3] - skewLoc[3] + unskew,
                loc[4] - skewLoc[4] + unskew, loc[5] - skewLoc[5] + unskew
            };
            var distOrder = new[] { 0, 1, 2, 3, 4, 5 };
            SortBy6(cellDist, distOrder);

            var newDistOrder = new[]
            {
                -1, distOrder[0], distOrder[1], distOrder[2], distOrder[3], distOrder[4], distOrder[5]
            };

            var n = 0.00;
            var skewOffset = 0.00;

            for (var c = 0; c < 7; ++c)
            {
                var i = newDistOrder[c];
                if (i != -1)
                    intLoc[i] += 1;

                var uu = new double[6];
                for (var d = 0; d < 6; ++d)
                {
                    uu[d] = cellDist[d] - (intLoc[d] - skewLoc[d]) + skewOffset;
                }

                var t = cornerFaceSqrd;

                for (var d = 0; d < 6; ++d)
                {
                    t -= uu[d] * uu[d];
                }

                if (t > 0.0)
                {
                    var h = HashCoordinates(intLoc[0], intLoc[1], intLoc[2], intLoc[3], intLoc[4], intLoc[5], seed);
                    var gr = 0.00;

                    gr += NoiseLookupTable.Gradient6D[h, 0] * uu[0];
                    gr += NoiseLookupTable.Gradient6D[h, 1] * uu[1];
                    gr += NoiseLookupTable.Gradient6D[h, 2] * uu[2];
                    gr += NoiseLookupTable.Gradient6D[h, 3] * uu[3];
                    gr += NoiseLookupTable.Gradient6D[h, 4] * uu[4];
                    gr += NoiseLookupTable.Gradient6D[h, 5] * uu[5];

                    n += gr * t * t * t * t;
                }
                skewOffset += g4;
            }
            n *= valueScaler;
            return n;
        }

        #endregion Hashing
    }
}