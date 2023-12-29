namespace MagusEngine.ECS.Components.ActorComponents.EffectComponents
{
    public class SightComponent : BaseEffectComponent
    {
        public SightComponent(int turnToRemove, int turnApplied, string effectMessage)
            : base(turnToRemove, turnApplied, effectMessage, "mage_sight")
        {
        }
    }
}
