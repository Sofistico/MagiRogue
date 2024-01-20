using MagusEngine.ECS.Components.ActorComponents.Effects;

namespace MagusEngine.ECS.Components.ActorComponents.Status
{
    public class SightComponent : BaseEffectComponent
    {
        public SightComponent(long tickToRemove, long tickApplied, string effectMessage)
            : base(tickToRemove, tickApplied, effectMessage, "mage_sight")
        {
        }

        public override void ExecutePerTurn()
        {
        }
    }
}
