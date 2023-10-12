using MagusEngine.Core.MapStuff;

namespace MagusEngine.Bus.UiBus
{
    public class LoadMapMessage
    {
        public MagiMap Map { get; set; }

        public LoadMapMessage(MagiMap map)
        {
            Map = map;
        }
    }
}
