using AccidentalNoiseLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace AccidentalNoiseLibrary.Implicit
{
    public class ImplicitXmlChain : ImplicitModuleBase
    {
        public static ImplicitXmlChain FromString(string text)
        {
            var chain = new ImplicitXmlChain();
            var xml = XDocument.Parse(text);

            chain.Modules = new Dictionary<string, ImplicitModuleBase>();

            foreach (var xElement in xml.Descendants("module"))
            {
                var nameAttribute = xElement.Attribute("name");
                var typeAttribute = xElement.Attribute("type");

                if (nameAttribute == null) continue;
                if (typeAttribute == null) continue;

                var name = nameAttribute.Value;
                var type = typeAttribute.Value;

                switch (type)
                {
                    case "auto_correct": chain.Modules.Add(name, AutoCorrectFromXElement(chain, xElement)); break;
                    case "bias": chain.Modules.Add(name, BiasFromXElement(chain, xElement)); break;
                    case "blend": throw new NotImplementedException("blend");
                    case "bright_contrast": throw new NotImplementedException("bright_contrast");
                    case "cache": chain.Modules.Add(name, CacheFromXElement(chain, xElement)); break;
                    case "ceiling": throw new NotImplementedException("ceiling");
                    case "clamp": chain.Modules.Add(name, ClampFromXElement(chain, xElement)); break;
                    case "combiner": chain.Modules.Add(name, CombinerFromXElement(chain, xElement)); break;
                    case "constant": chain.Modules.Add(name, ConstantFromXElement(chain, xElement)); break;
                    case "cos": throw new NotImplementedException("cos");
                    case "floor": throw new NotImplementedException("floor");
                    case "fractal": chain.Modules.Add(name, FractalFromXElement(chain, xElement)); break;
                    case "gain": throw new NotImplementedException("gain");
                    case "gradient": chain.Modules.Add(name, GradientFromXElement(chain, xElement)); break;
                    case "invert": throw new NotImplementedException("invert");
                    case "log": throw new NotImplementedException("log");
                    case "pow": throw new NotImplementedException("pow");
                    case "rotate_domain": throw new NotImplementedException("rotate_domain");
                    case "scale_domain": chain.Modules.Add(name, ScaleDomainFromXElement(chain, xElement)); break;
                    case "scale_offset": chain.Modules.Add(name, ScaleOffsetFromXElement(chain, xElement)); break;
                    case "select": chain.Modules.Add(name, SelectFromXElement(chain, xElement)); break;
                    case "sin": throw new NotImplementedException("sin");
                    case "sphere": chain.Modules.Add(name, SphereFromXElement(chain, xElement)); break;
                    case "tan": throw new NotImplementedException("tan");
                    case "tiers": throw new NotImplementedException("tiers");
                    case "translate_domain": chain.Modules.Add(name, TranslateDomainFromXElement(chain, xElement)); break;
                    default: throw new NotImplementedException(type);
                }
            }

            chain.Source = chain.Modules[xml.Descendants("chain").Single().Attribute("source").Value];

            return chain;
        }

        public static ImplicitAutoCorrect AutoCorrectFromXElement(ImplicitXmlChain chain, XElement xElement)
        {
            var sourceString = xElement.Attribute("source") != null ? xElement.Attribute("source").Value : string.Empty;
            var low = xElement.Attribute("low") != null ? xElement.Attribute("low").Value : string.Empty;
            var high = xElement.Attribute("high") != null ? xElement.Attribute("high").Value : string.Empty;

            ImplicitAutoCorrect autoCorrect;

            ImplicitModuleBase source;
            double value;

            if (!string.IsNullOrEmpty(sourceString))
            {
                if (chain.Modules.TryGetValue(sourceString, out source))
                    autoCorrect = new ImplicitAutoCorrect(source);
                else if (double.TryParse(sourceString, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                    autoCorrect = new ImplicitAutoCorrect(value);
                else
                    throw new InvalidOperationException("Invalid source value");
            }
            else
                throw new InvalidOperationException("Missing source");

            if (!string.IsNullOrEmpty(low))
            {
                if (double.TryParse(low, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                    autoCorrect.Low = value;
                else
                    throw new InvalidOperationException("Invalid low value");
            }

            if (!string.IsNullOrEmpty(high))
            {
                if (double.TryParse(high, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                    autoCorrect.High = value;
                else
                    throw new InvalidOperationException("Invalid high value");
            }

            return autoCorrect;
        }

        public static ImplicitBias BiasFromXElement(ImplicitXmlChain chain, XElement xElement)
        {
            var sourceString = xElement.Attribute("source") != null ? xElement.Attribute("source").Value : string.Empty;
            var low = xElement.Attribute("low") != null ? xElement.Attribute("low").Value : string.Empty;

            ImplicitBias autoCorrect;

            ImplicitModuleBase source;
            double value;

            if (!string.IsNullOrEmpty(sourceString))
            {
                if (chain.Modules.TryGetValue(sourceString, out source))
                    autoCorrect = new ImplicitBias(source, 0);
                else if (double.TryParse(sourceString, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                    autoCorrect = new ImplicitBias(value, 0);
                else
                    throw new InvalidOperationException("Invalid source value");
            }
            else
                throw new InvalidOperationException("Missing source");

            if (!string.IsNullOrEmpty(low))
            {
                if (chain.Modules.TryGetValue(sourceString, out source))
                    autoCorrect.Bias = source;
                else if (double.TryParse(low, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                    autoCorrect.Bias = value;
                else
                    throw new InvalidOperationException("Invalid bias value");
            }

            return autoCorrect;
        }

        public static ImplicitCache CacheFromXElement(ImplicitXmlChain chain, XElement xElement)
        {
            var sourceString = xElement.Attribute("source") != null ? xElement.Attribute("source").Value : string.Empty;

            if (string.IsNullOrEmpty(sourceString))
                throw new InvalidOperationException("Missing source");

            ImplicitModuleBase source;
            double value;

            if (chain.Modules.TryGetValue(sourceString, out source))
                return new ImplicitCache(source);

            if (double.TryParse(sourceString, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                return new ImplicitCache(value);

            throw new InvalidOperationException("Invalid source value");
        }

        public static ImplicitClamp ClampFromXElement(ImplicitXmlChain chain, XElement xElement)
        {
            var sourceString = xElement.Attribute("source") != null ? xElement.Attribute("source").Value : string.Empty;
            var low = xElement.Attribute("low") != null ? xElement.Attribute("low").Value : string.Empty;
            var high = xElement.Attribute("high") != null ? xElement.Attribute("high").Value : string.Empty;

            ImplicitClamp clamp;

            ImplicitModuleBase source;
            double value;

            if (!string.IsNullOrEmpty(sourceString))
            {
                if (chain.Modules.TryGetValue(sourceString, out source))
                    clamp = new ImplicitClamp(source);
                else if (double.TryParse(sourceString, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                    clamp = new ImplicitClamp(value);
                else
                    throw new InvalidOperationException("Invalid source value");
            }
            else
                throw new InvalidOperationException("Missing source");

            if (!string.IsNullOrEmpty(low))
            {
                if (chain.Modules.TryGetValue(low, out source))
                    clamp.Low = source;
                else if (double.TryParse(low, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                    clamp.Low = value;
                else
                    throw new InvalidOperationException("Invalid low value");
            }

            if (!string.IsNullOrEmpty(high))
            {
                if (chain.Modules.TryGetValue(high, out source))
                    clamp.High = source;
                else if (double.TryParse(high, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                    clamp.High = value;
                else
                    throw new InvalidOperationException("Invalid high value");
            }

            return clamp;
        }

        public static ImplicitConstant ConstantFromXElement(ImplicitXmlChain chain, XElement xElement)
        {
            var value = xElement.Attribute("value");
            return new ImplicitConstant(value == null ? 0.00 : double.Parse(value.Value, NumberStyles.Any, CultureInfo.InvariantCulture));
        }

        public static ImplicitCombiner CombinerFromXElement(ImplicitXmlChain chain, XElement xElement)
        {
            var combinerTypeString = xElement.Attribute("combiner_type") != null ? xElement.Attribute("combiner_type").Value : string.Empty;

            ImplicitCombiner combiner;

            switch (combinerTypeString.ToLower())
            {
                case "add":
                    combiner = new ImplicitCombiner(CombinerType.Add);
                    break;
                case "average":
                    combiner = new ImplicitCombiner(CombinerType.Average);
                    break;
                case "max":
                    combiner = new ImplicitCombiner(CombinerType.Max);
                    break;
                case "min":
                    combiner = new ImplicitCombiner(CombinerType.Min);
                    break;
                case "multiply":
                    combiner = new ImplicitCombiner(CombinerType.Multiply);
                    break;
                default: throw new InvalidOperationException("Invalid combiner_type.");
            }

            foreach (var source in xElement.Elements("source"))
            {
                combiner.AddSource(chain.Modules[source.Value]);
            }

            return combiner;
        }

        public static ImplicitFractal FractalFromXElement(ImplicitXmlChain chain, XElement xElement)
        {
            var fractalTypeAttribute = xElement.Attribute("fractal");
            if (fractalTypeAttribute == null)
                throw new InvalidOperationException("Missing fractal.");
            var basisTypeAttribute = xElement.Attribute("basis");
            if (basisTypeAttribute == null)
                throw new InvalidOperationException("Missing basis.");
            var interpolationTypeAttribute = xElement.Attribute("interpolation");
            if (interpolationTypeAttribute == null)
                throw new InvalidOperationException("Missing interpolation.");

            FractalType fractalType;
            switch (fractalTypeAttribute.Value.ToLower())
            {
                case "billow":
                    fractalType = FractalType.Billow;
                    break;
                case "fbm":
                    fractalType = FractalType.FractionalBrownianMotion;
                    break;
                case "hybrid_multi":
                    fractalType = FractalType.HybridMulti;
                    break;
                case "multi":
                    fractalType = FractalType.Multi;
                    break;
                case "ridged_multi":
                    fractalType = FractalType.RidgedMulti;
                    break;
                default: throw new InvalidOperationException("Invalid fractal.");
            }

            BasisType basisType;
            switch (basisTypeAttribute.Value.ToLower())
            {
                case "gradient":
                    basisType = BasisType.Gradient;
                    break;
                case "gradient_value":
                    basisType = BasisType.GradientValue;
                    break;
                case "simplex":
                    basisType = BasisType.Simplex;
                    break;
                case "value":
                    basisType = BasisType.Value;
                    break;
                case "white":
                    basisType = BasisType.White;
                    break;
                default: throw new InvalidOperationException("Invalid basis.");
            }

            InterpolationType interpolationType;
            switch (interpolationTypeAttribute.Value.ToLower())
            {
                case "cubic":
                    interpolationType = InterpolationType.Cubic;
                    break;
                case "linear":
                    interpolationType = InterpolationType.Linear;
                    break;
                case "none":
                    interpolationType = InterpolationType.None;
                    break;
                case "quintic":
                    interpolationType = InterpolationType.Quintic;
                    break;
                default: throw new InvalidOperationException("Invalid interpolation.");
            }

            var fractal = new ImplicitFractal(fractalType, basisType, interpolationType);

            var octavesAttribute = xElement.Attribute("octaves");
            if (octavesAttribute != null)
                fractal.Octaves = int.Parse(octavesAttribute.Value);

            var frequencyAttribute = xElement.Attribute("frequency");
            if (frequencyAttribute != null)
                fractal.Frequency = double.Parse(frequencyAttribute.Value, NumberStyles.Any, CultureInfo.InvariantCulture);

            var lacunarityAttribute = xElement.Attribute("lacunarity");
            if (lacunarityAttribute != null)
                fractal.Lacunarity = double.Parse(lacunarityAttribute.Value, NumberStyles.Any, CultureInfo.InvariantCulture);

            var gainAttribute = xElement.Attribute("gain");
            if (gainAttribute != null)
                fractal.Gain = double.Parse(gainAttribute.Value, NumberStyles.Any, CultureInfo.InvariantCulture);

            var offsetAttribute = xElement.Attribute("offset");
            if (offsetAttribute != null)
                fractal.Offset = double.Parse(offsetAttribute.Value, NumberStyles.Any, CultureInfo.InvariantCulture);

            var hAttribute = xElement.Attribute("h");
            if (hAttribute != null)
                fractal.H = double.Parse(hAttribute.Value, NumberStyles.Any, CultureInfo.InvariantCulture);

            return fractal;
        }

        public static ImplicitGradient GradientFromXElement(ImplicitXmlChain chain, XElement xElement)
        {
            var x0 = xElement.Attribute("x0");
            var x1 = xElement.Attribute("x1");
            var y0 = xElement.Attribute("y0");
            var y1 = xElement.Attribute("y1");
            var z0 = xElement.Attribute("z0");
            var z1 = xElement.Attribute("z1");
            var v0 = xElement.Attribute("v0");
            var v1 = xElement.Attribute("v1");
            var u0 = xElement.Attribute("u0");
            var u1 = xElement.Attribute("u1");
            var w0 = xElement.Attribute("w0");
            var w1 = xElement.Attribute("w1");
            return new ImplicitGradient(
                x0 == null ? 0.00 : double.Parse(x0.Value, NumberStyles.Any, CultureInfo.InvariantCulture),
                x1 == null ? 1.00 : double.Parse(x1.Value, NumberStyles.Any, CultureInfo.InvariantCulture),
                y0 == null ? 0.00 : double.Parse(y0.Value, NumberStyles.Any, CultureInfo.InvariantCulture),
                y1 == null ? 1.00 : double.Parse(y1.Value, NumberStyles.Any, CultureInfo.InvariantCulture),
                z0 == null ? 0.00 : double.Parse(z0.Value, NumberStyles.Any, CultureInfo.InvariantCulture),
                z1 == null ? 1.00 : double.Parse(z1.Value, NumberStyles.Any, CultureInfo.InvariantCulture),
                v0 == null ? 0.00 : double.Parse(v0.Value, NumberStyles.Any, CultureInfo.InvariantCulture),
                v1 == null ? 1.00 : double.Parse(v1.Value, NumberStyles.Any, CultureInfo.InvariantCulture),
                u0 == null ? 0.00 : double.Parse(u0.Value, NumberStyles.Any, CultureInfo.InvariantCulture),
                u1 == null ? 1.00 : double.Parse(u1.Value, NumberStyles.Any, CultureInfo.InvariantCulture),
                w0 == null ? 0.00 : double.Parse(w0.Value, NumberStyles.Any, CultureInfo.InvariantCulture),
                w1 == null ? 1.00 : double.Parse(w1.Value, NumberStyles.Any, CultureInfo.InvariantCulture));
        }

        public static ImplicitSphere SphereFromXElement(ImplicitXmlChain chain, XElement xElement)
        {
            var xc = xElement.Attribute("xc") != null ? xElement.Attribute("xc").Value : string.Empty;
            var yc = xElement.Attribute("yc") != null ? xElement.Attribute("yc").Value : string.Empty;
            var zc = xElement.Attribute("zc") != null ? xElement.Attribute("zc").Value : string.Empty;
            var vc = xElement.Attribute("vc") != null ? xElement.Attribute("vc").Value : string.Empty;
            var uc = xElement.Attribute("uc") != null ? xElement.Attribute("uc").Value : string.Empty;
            var wc = xElement.Attribute("wc") != null ? xElement.Attribute("wc").Value : string.Empty;
            var radius = xElement.Attribute("radius") != null ? xElement.Attribute("radius").Value : string.Empty;

            var sphere = new ImplicitSphere();
            ImplicitModuleBase source;
            double value;

            if (!string.IsNullOrEmpty(xc))
            {
                if (chain.Modules.TryGetValue(xc, out source))
                    sphere.XCenter = source;
                else if (double.TryParse(xc, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                    sphere.XCenter = value;
                else
                    throw new InvalidOperationException("Invalid xc value");
            }

            if (!string.IsNullOrEmpty(yc))
            {
                if (chain.Modules.TryGetValue(yc, out source))
                    sphere.YCenter = source;
                else if (double.TryParse(yc, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                    sphere.YCenter = value;
                else
                    throw new InvalidOperationException("Invalid yc value");
            }

            if (!string.IsNullOrEmpty(zc))
            {

                if (chain.Modules.TryGetValue(zc, out source))
                    sphere.ZCenter = source;
                else if (double.TryParse(zc, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                    sphere.ZCenter = value;
                else
                    throw new InvalidOperationException("Invalid zc value");
            }

            if (!string.IsNullOrEmpty(vc))
            {
                if (chain.Modules.TryGetValue(vc, out source))
                    sphere.VCenter = source;
                else if (double.TryParse(vc, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                    sphere.VCenter = value;
                else
                    throw new InvalidOperationException("Invalid vc value");
            }

            if (!string.IsNullOrEmpty(uc))
            {
                if (chain.Modules.TryGetValue(uc, out source))
                    sphere.UCenter = source;
                else if (double.TryParse(uc, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                    sphere.UCenter = value;
                else
                    throw new InvalidOperationException("Invalid uc value");
            }

            if (!string.IsNullOrEmpty(wc))
            {

                if (chain.Modules.TryGetValue(wc, out source))
                    sphere.WCenter = source;
                else if (double.TryParse(wc, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                    sphere.WCenter = value;
                else
                    throw new InvalidOperationException("Invalid wc value");
            }

            if (!string.IsNullOrEmpty(radius))
            {
                if (chain.Modules.TryGetValue(radius, out source))
                    sphere.Radius = source;
                else if (double.TryParse(radius, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                    sphere.Radius = value;
                else
                    throw new InvalidOperationException("Invalid radius value");
            }

            return sphere;
        }

        public static ImplicitScaleOffset ScaleOffsetFromXElement(ImplicitXmlChain chain, XElement xElement)
        {
            var sourceString = xElement.Attribute("source") != null ? xElement.Attribute("source").Value : string.Empty;
            var scaleString = xElement.Attribute("scale") != null ? xElement.Attribute("scale").Value : string.Empty;
            var offsetString = xElement.Attribute("offset") != null ? xElement.Attribute("offset").Value : string.Empty;

            ImplicitScaleOffset scaleOffset;
            ImplicitModuleBase source;
            double value;

            if (!string.IsNullOrEmpty(sourceString))
            {
                if (chain.Modules.TryGetValue(sourceString, out source))
                    scaleOffset = new ImplicitScaleOffset(source);
                else if (double.TryParse(sourceString, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                    scaleOffset = new ImplicitScaleOffset(value);
                else
                    throw new InvalidOperationException("Invalid source value");
            }
            else
                throw new InvalidOperationException("Missing source");

            if (!string.IsNullOrEmpty(scaleString))
            {
                if (chain.Modules.TryGetValue(scaleString, out source))
                    scaleOffset.Scale = source;
                else if (double.TryParse(scaleString, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                    scaleOffset.Scale = value;
                else
                    throw new InvalidOperationException("Invalid scale value");
            }

            if (!string.IsNullOrEmpty(offsetString))
            {
                if (chain.Modules.TryGetValue(offsetString, out source))
                    scaleOffset.Offset = source;
                else if (double.TryParse(offsetString, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                    scaleOffset.Offset = value;
                else
                    throw new InvalidOperationException("Invalid offset value");
            }

            return scaleOffset;

        }

        public static ImplicitScaleDomain ScaleDomainFromXElement(ImplicitXmlChain chain, XElement xElement)
        {
            var sourceString = xElement.Attribute("source") != null ? xElement.Attribute("source").Value : string.Empty;
            var xs = xElement.Attribute("xs") != null ? xElement.Attribute("xs").Value : string.Empty;
            var ys = xElement.Attribute("ys") != null ? xElement.Attribute("ys").Value : string.Empty;
            var zs = xElement.Attribute("zs") != null ? xElement.Attribute("zs").Value : string.Empty;
            var vs = xElement.Attribute("vs") != null ? xElement.Attribute("vs").Value : string.Empty;
            var us = xElement.Attribute("us") != null ? xElement.Attribute("us").Value : string.Empty;
            var ws = xElement.Attribute("ws") != null ? xElement.Attribute("ws").Value : string.Empty;

            ImplicitScaleDomain scaleDomain;
            ImplicitModuleBase source;
            double value;

            if (!string.IsNullOrEmpty(sourceString))
            {
                if (chain.Modules.TryGetValue(sourceString, out source))
                    scaleDomain = new ImplicitScaleDomain(source);
                else if (double.TryParse(sourceString, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                    scaleDomain = new ImplicitScaleDomain(value);
                else
                    throw new InvalidOperationException("Invalid source value");
            }
            else
                throw new InvalidOperationException("Missing source");

            if (!string.IsNullOrEmpty(xs))
            {
                if (chain.Modules.TryGetValue(xs, out source))
                    scaleDomain.XScale = source;
                else if (double.TryParse(xs, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                    scaleDomain.XScale = value;
                else
                    throw new InvalidOperationException("Invalid xs value");
            }

            if (!string.IsNullOrEmpty(ys))
            {
                if (chain.Modules.TryGetValue(ys, out source))
                    scaleDomain.YScale = source;
                else if (double.TryParse(ys, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                    scaleDomain.YScale = value;
                else
                    throw new InvalidOperationException("Invalid ys value");
            }

            if (!string.IsNullOrEmpty(zs))
            {

                if (chain.Modules.TryGetValue(zs, out source))
                    scaleDomain.ZScale = source;
                else if (double.TryParse(zs, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                    scaleDomain.ZScale = value;
                else
                    throw new InvalidOperationException("Invalid zs value");
            }

            if (!string.IsNullOrEmpty(vs))
            {
                if (chain.Modules.TryGetValue(vs, out source))
                    scaleDomain.VScale = source;
                else if (double.TryParse(vs, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                    scaleDomain.VScale = value;
                else
                    throw new InvalidOperationException("Invalid vs value");
            }

            if (!string.IsNullOrEmpty(us))
            {
                if (chain.Modules.TryGetValue(us, out source))
                    scaleDomain.UScale = source;
                else if (double.TryParse(us, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                    scaleDomain.UScale = value;
                else
                    throw new InvalidOperationException("Invalid us value");
            }

            if (!string.IsNullOrEmpty(ws))
            {

                if (chain.Modules.TryGetValue(ws, out source))
                    scaleDomain.WScale = source;
                else if (double.TryParse(ws, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                    scaleDomain.WScale = value;
                else
                    throw new InvalidOperationException("Invalid ws value");
            }

            return scaleDomain;
        }

        public static ImplicitTranslateDomain TranslateDomainFromXElement(ImplicitXmlChain chain, XElement xElement)
        {
            var sourceString = xElement.Attribute("source") != null ? xElement.Attribute("source").Value : string.Empty;
            var xa = xElement.Attribute("xa") != null ? xElement.Attribute("xa").Value : string.Empty;
            var ya = xElement.Attribute("ya") != null ? xElement.Attribute("ya").Value : string.Empty;
            var za = xElement.Attribute("za") != null ? xElement.Attribute("za").Value : string.Empty;
            var va = xElement.Attribute("va") != null ? xElement.Attribute("va").Value : string.Empty;
            var ua = xElement.Attribute("ua") != null ? xElement.Attribute("ua").Value : string.Empty;
            var wa = xElement.Attribute("wa") != null ? xElement.Attribute("wa").Value : string.Empty;

            ImplicitTranslateDomain translateDomain;

            ImplicitModuleBase source;
            double value;

            if (!string.IsNullOrEmpty(sourceString))
            {
                if (chain.Modules.TryGetValue(sourceString, out source))
                    translateDomain = new ImplicitTranslateDomain(source);
                else if (double.TryParse(sourceString, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                    translateDomain = new ImplicitTranslateDomain(value);
                else
                    throw new InvalidOperationException("Invalid source value");
            }
            else
                throw new InvalidOperationException("Missing source");

            if (!string.IsNullOrEmpty(xa))
            {
                if (chain.Modules.TryGetValue(xa, out source))
                    translateDomain.XAxis = source;
                else if (double.TryParse(xa, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                    translateDomain.XAxis = value;
                else
                    throw new InvalidOperationException("Invalid xa value");
            }

            if (!string.IsNullOrEmpty(ya))
            {
                if (chain.Modules.TryGetValue(ya, out source))
                    translateDomain.YAxis = source;
                else if (double.TryParse(ya, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                    translateDomain.YAxis = value;
                else
                    throw new InvalidOperationException("Invalid ya value");
            }

            if (!string.IsNullOrEmpty(za))
            {
                if (chain.Modules.TryGetValue(za, out source))
                    translateDomain.ZAxis = source;
                else if (double.TryParse(za, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                    translateDomain.ZAxis = value;
                else
                    throw new InvalidOperationException("Invalid za value");
            }

            if (!string.IsNullOrEmpty(va))
            {
                if (chain.Modules.TryGetValue(va, out source))
                    translateDomain.VAxis = source;
                else if (double.TryParse(va, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                    translateDomain.VAxis = value;
                else
                    throw new InvalidOperationException("Invalid va value");
            }

            if (!string.IsNullOrEmpty(ua))
            {
                if (chain.Modules.TryGetValue(ua, out source))
                    translateDomain.UAxis = source;
                else if (double.TryParse(ua, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                    translateDomain.UAxis = value;
                else
                    throw new InvalidOperationException("Invalid ua value");
            }

            if (!string.IsNullOrEmpty(wa))
            {
                if (chain.Modules.TryGetValue(wa, out source))
                    translateDomain.WAxis = source;
                else if (double.TryParse(wa, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                    translateDomain.WAxis = value;
                else
                    throw new InvalidOperationException("Invalid wa value");
            }

            return translateDomain;
        }

        public static ImplicitSelect SelectFromXElement(ImplicitXmlChain chain, XElement xElement)
        {
            var sourceString = xElement.Attribute("source") != null ? xElement.Attribute("source").Value : string.Empty;
            var low = xElement.Attribute("low") != null ? xElement.Attribute("low").Value : string.Empty;
            var high = xElement.Attribute("high") != null ? xElement.Attribute("high").Value : string.Empty;
            var falloff = xElement.Attribute("falloff") != null ? xElement.Attribute("falloff").Value : string.Empty;
            var threshold = xElement.Attribute("threshold") != null ? xElement.Attribute("threshold").Value : string.Empty;

            ImplicitSelect @select;

            ImplicitModuleBase source;
            double value;

            if (!string.IsNullOrEmpty(sourceString))
            {
                if (chain.Modules.TryGetValue(sourceString, out source))
                    @select = new ImplicitSelect(source);
                else if (double.TryParse(sourceString, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                    @select = new ImplicitSelect(value);
                else
                    throw new InvalidOperationException("Invalid source value");
            }
            else
                throw new InvalidOperationException("Missing source");

            if (!string.IsNullOrEmpty(low))
            {
                if (chain.Modules.TryGetValue(low, out source))
                    @select.Low = source;
                else if (double.TryParse(low, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                    @select.Low = value;
                else
                    throw new InvalidOperationException("Invalid low value");
            }

            if (!string.IsNullOrEmpty(high))
            {
                if (chain.Modules.TryGetValue(high, out source))
                    @select.High = source;
                else if (double.TryParse(high, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                    @select.High = value;
                else
                    throw new InvalidOperationException("Invalid high value");
            }

            if (!string.IsNullOrEmpty(falloff))
            {
                if (chain.Modules.TryGetValue(falloff, out source))
                    @select.Falloff = source;
                else if (double.TryParse(falloff, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                    @select.Falloff = value;
                else
                    throw new InvalidOperationException("Invalid falloff value");
            }

            if (!string.IsNullOrEmpty(threshold))
            {
                if (chain.Modules.TryGetValue(threshold, out source))
                    @select.Threshold = source;
                else if (double.TryParse(threshold, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                    @select.Threshold = value;
                else
                    throw new InvalidOperationException("Invalid threshold value");
            }

            return @select;
        }

        private ImplicitXmlChain() { }

        public Dictionary<string, ImplicitModuleBase> Modules { get; private set; }

        public ImplicitModuleBase Source { get; private set; }

        public override double Get(double x, double y)
        {
            return Source.Get(x, y);
        }

        public override double Get(double x, double y, double z)
        {
            return Source.Get(x, y, z);
        }

        public override double Get(double x, double y, double z, double w)
        {
            return Source.Get(x, y, z, w);
        }

        public override double Get(double x, double y, double z, double w, double u, double v)
        {
            return Source.Get(x, y, z, w, u, v);
        }
    }
}
