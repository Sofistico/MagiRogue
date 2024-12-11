using MagusEngine.Core.Animations;

namespace MagusEngine.Factory
{
    public sealed class AnimationFactory : GenericFactory<AnimationBase>
    {
        public AnimationFactory()
        {
            Register("Kaboom", static token => token.ToObject<KaboomAnimation>()!);
        }
    }
}
