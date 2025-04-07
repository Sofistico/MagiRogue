namespace MagusEngine.Components.EntityComponents.Effects
{
    public class LimitedLifeComponent : BaseEffectComponent
    {
        public LimitedLifeComponent(long tickApplied, long tickToRemove, string effectMessage = "", string tag = "limited_time") : base(tickApplied, tickToRemove, effectMessage, tag, execution: ExecutionType.OnEnd)
        {
        }

        public override void ExecuteEffect()
        {
            throw new System.NotImplementedException();
        }
    }
}

