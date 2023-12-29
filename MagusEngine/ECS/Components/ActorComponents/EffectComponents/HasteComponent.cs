namespace MagusEngine.ECS.Components.ActorComponents.EffectComponents
{
    public class HasteComponent : BaseEffectComponent
    {
        public double HastePower { get; set; }

        public HasteComponent(double hastePower,
            int turnApplied,
            int turnToRemove,
            string effectMessage) : base(turnApplied, turnToRemove, effectMessage, "haste")
        {
            HastePower = hastePower;
        }
    }
}
