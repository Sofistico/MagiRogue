using System;
using Arquimedes.Enumerators;
using MagusEngine.Core.Animations;
using MagusEngine.Core.Animations.Interfaces;
using MagusEngine.Services.Factory.Base;

namespace MagusEngine.Services.Factory
{
    public sealed class AnimationFactory : GenericFactory<IAnimation>
    {
        public AnimationFactory()
        {
            _ = Register("Kaboom", static token => 
            {
                return new KaboomAnimation()
                {
                    Id = token["Id"]!.ToString()!,
                    AnimationType = Enum.Parse<AnimationType>(token["AnimationType"]!.ToString()!),
                    Colors = token["Colors"]!.ToObject<string[]>()!,
                    Glyphs = token["Glyphs"]!.ToObject<char[]>()!,
                    GlyphChangeDistance = (int)token["GlyphChangeDistance"]!,
                    LingeringTicks = (int)token["LingeringTicks"]!,
                    Radius = (int)token["Radius"]!,
                };
            });
        }

    }
}
