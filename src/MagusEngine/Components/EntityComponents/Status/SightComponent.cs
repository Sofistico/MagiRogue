namespace MagusEngine.Components.EntityComponents.Status
{
    public class SightComponent : BaseEffectComponent
    {
        public SightComponent(long tickToRemove, long tickApplied, string effectMessage)
            : base(
                new(
                    tickApplied,
                    tickToRemove,
                    effectMessage,
                    "mage_sight",
                    removeMessage: "You stop seeing"
                )
            ) { }

        public override void ExecuteEffect() { }
    }
}
