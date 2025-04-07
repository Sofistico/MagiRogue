using MagusEngine.Components.EntityComponents.Effects;

namespace MagusEngine.Components.EntityComponents.Status
{
    public class SightComponent : BaseEffectComponent
    {
        public SightComponent(long tickToRemove, long tickApplied, string effectMessage)
            : base(tickToRemove, tickApplied, effectMessage, "mage_sight", removeMessage: "You stop seeing")
        {
        }

        public override void ExecuteEffect()
        {
        }
    }
}
