using MagusEngine.Core.Animations;
using MagusEngine.Services.Factory.Base;

namespace MagusEngine.Services.Factory
{
    public sealed class AnimationFactory : GenericFactory<AnimationBase>
    {
        public AnimationFactory()
        {
            Register("Kaboom", static token => token.ToObject<KaboomAnimation>()!);
        }
    }
}
