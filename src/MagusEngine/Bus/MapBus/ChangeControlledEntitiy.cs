using MagusEngine.Core.Entities.Base;

namespace MagusEngine.Bus.MapBus
{
    public class ChangeControlledEntitiy
    {
        public MagiEntity ControlledEntitiy { get; set; }

        public ChangeControlledEntitiy(MagiEntity controlledEntitiy)
        {
            ControlledEntitiy = controlledEntitiy;
        }
    }
}
