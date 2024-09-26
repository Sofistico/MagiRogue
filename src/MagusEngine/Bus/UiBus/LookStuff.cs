using MagusEngine.Core;
using MagusEngine.Core.Entities.Base;
using MagusEngine.Core.MapStuff;

namespace MagusEngine.Bus.UiBus
{
    public class LookStuff
    {
        public MagiEntity? Entitiy { get; set; }
        public Tile? Tile { get; set; }

        public LookStuff(MagiEntity? entitiy)
        {
            Entitiy = entitiy;
        }

        public LookStuff(Tile? tile)
        {
            Tile = tile;
        }
    }
}
