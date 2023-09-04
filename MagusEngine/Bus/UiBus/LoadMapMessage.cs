using MagusEngine.Core.MapStuff;

namespace MagusEngine.Bus.UiBus
{
    public class LoadMapMessage
    {
        public Map Map { get; set; }

        public LoadMapMessage(Map map)
        {
            Map = map;
        }
    }
}
