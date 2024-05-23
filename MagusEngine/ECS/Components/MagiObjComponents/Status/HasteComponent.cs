using MagusEngine.ECS.Components.MagiObjComponents.Effects;

namespace MagusEngine.ECS.Components.MagiObjComponents.Status
{
    public class HasteComponent : BaseEffectComponent
    {
        public double HastePower { get; set; }

        public HasteComponent(double hastePower,
            long tickApplied,
            long tickToRemove,
            string effectMessage) : base(tickApplied, tickToRemove, effectMessage, "haste")
        {
            HastePower = hastePower;
        }

        public override void ExecutePerTurn()
        {
        }
    }
}
