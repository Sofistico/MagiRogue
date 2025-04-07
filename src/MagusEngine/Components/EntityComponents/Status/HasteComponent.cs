using MagusEngine.Components.EntityComponents.Effects;

namespace MagusEngine.Components.EntityComponents.Status
{
    public class HasteComponent : BaseEffectComponent
    {
        public double HastePower { get; set; }

        public HasteComponent(double hastePower,
            long tickApplied,
            long tickToRemove,
            string effectMessage) : base(tickApplied, tickToRemove, effectMessage, "haste", removeMessage: "You feel yourself slowing down")
        {
            HastePower = hastePower;
        }

        public override void ExecuteEffect()
        {
        }
    }
}
