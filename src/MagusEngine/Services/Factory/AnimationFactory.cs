using MagusEngine.Core.Animations;

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
