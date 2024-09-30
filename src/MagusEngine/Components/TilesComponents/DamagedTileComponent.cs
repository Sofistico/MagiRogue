namespace MagusEngine.Components.TilesComponents
{
    public class DamagedTileComponent
    {
        public const string Tag = "dmg_tile";
        public int Damage { get; }

        public DamagedTileComponent(int damage)
        {
            Damage = damage;
        }
    }
}
