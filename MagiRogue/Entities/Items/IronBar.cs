using Microsoft.Xna.Framework;

namespace MagiRogue.Entities.Items
{
    internal class IronBar : Item
    {
        public IronBar(Point position) : base(Color.Gray, Color.Transparent, "Iron Bar", '_', position, 10)
        {
        }
    }
}