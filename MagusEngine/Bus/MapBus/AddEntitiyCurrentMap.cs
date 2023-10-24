using MagusEngine.Core.Entities.Base;

namespace MagusEngine.Bus.MapBus
{
    public class AddEntitiyCurrentMap
    {
        public MagiEntity Entitiy { get; set; }

        public AddEntitiyCurrentMap(MagiEntity entitiy)
        {
            Entitiy = entitiy;
        }
    }
}
