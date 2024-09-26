using MagusEngine.Core.Entities.Base;

namespace MagusEngine.Bus.MapBus
{
    public class RemoveEntitiyCurrentMap
    {
        public MagiEntity Entity { get; set; }

        public RemoveEntitiyCurrentMap(MagiEntity entity)
        {
            Entity = entity;
        }
    }
}
