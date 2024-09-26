using MagusEngine.Bus.UiBus;
using MagusEngine.Core.Entities.Base;
using MagusEngine.Core.MapStuff;
using MagusEngine.Services;
using MagusEngine.Systems.Time;
using MagusEngine.Utils;

namespace MagusEngine.ECS.Components.EntityComponents.Ai
{
    public class BasicAi : IAiComponent
    {
        private readonly MagiEntity _entity;
        public object? Parent { get; set; }

        public BasicAi(MagiEntity entity)
        {
            _entity = entity;
        }

        public virtual (bool sucess, long ticks) RunAi(MagiMap map)
        {
            bool rng = Mrn.OneIn(10);
            if (rng)
            {
                Locator.GetService<MessageBusService>().SendMessage<AddMessageLog>(new($"The {_entity.Name} waits doing nothing..."));
                return (true, TimeHelper.Wait);
            }
            else
            {
                return (false, TimeHelper.AiFailed);
            }
        }
    }
}