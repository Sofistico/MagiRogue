using MagusEngine.ECS.Components.ActorComponents.Effects;

namespace MagusEngine.ECS.Components.ActorComponents.Status
{
    public class SightComponent : BaseEffectComponent
    {
        public SightComponent(int turnToRemove, int turnApplied, string effectMessage)
            : base(turnToRemove, turnApplied, effectMessage, "mage_sight")
        {
        }

        public override void ExecutePerTurn()
        {
        }
    }
}
