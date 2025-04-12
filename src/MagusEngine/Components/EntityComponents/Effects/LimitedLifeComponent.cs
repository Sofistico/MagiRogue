using MagusEngine.Bus.MapBus;
using MagusEngine.Exceptions;
using MagusEngine.Services;

namespace MagusEngine.Components.EntityComponents.Effects
{
    public class LimitedLifeComponent : BaseEffectComponent
    {
        public LimitedLifeComponent(
            long tickApplied,
            long tickToRemove,
            string effectMessage = "",
            string tag = "limited_time"
        )
            : base(tickApplied, tickToRemove, effectMessage, tag, execution: ExecutionType.OnEnd)
        { }

        public override void ExecuteEffect()
        {
            try
            {
                if (Parent is null)
                    throw new NullValueException(nameof(Parent));
                Locator.GetService<MessageBusService>().SendMessage<RemoveEntitiyCurrentMap>(new(Parent));
            }
            catch (System.Exception ex)
            {
                MagiLog.Log(ex, "Something went wrong with the LimitedLifeComponent");
                throw;
            }
        }
    }
}
