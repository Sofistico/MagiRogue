namespace MagusEngine.Components.EntityComponents.Status
{
    public class SightComponent : BaseEffectComponent
    {
        public SightComponent(long tickToRemove, long tickApplied, string effectMessage)
            : base(
                new(
                    tickToRemove,
                    tickApplied,
                    effectMessage,
                    "mage_sight",
                    removeMessage: "You stop seeing"
                )
            ) { }

        public override void ExecuteEffect() { }
    }
}
