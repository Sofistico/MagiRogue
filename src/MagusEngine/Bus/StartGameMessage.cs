using MagusEngine.Core.Entities;
using MagusEngine.Systems;

namespace MagusEngine.Bus
{
    public class StartGameMessage
    {
        public Player Player { get; set; }
        public Universe? Universe { get; set; }
        public bool TestGame { get; set; }

        public StartGameMessage(Player player, Universe universe = null)
        {
            Player = player;
            Universe = universe;
        }
    }
}
