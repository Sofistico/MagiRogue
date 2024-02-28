using MagusEngine.Core.Entities;

namespace Diviner.Animation
{
    public class ProjectileAnimation
    {
        public Projectile Projectile { get; set; }
        public int Miliseconds { get; set; }

        public ProjectileAnimation(Projectile projectile, int miliseconds)
        {
            Projectile = projectile;
            Miliseconds = miliseconds;
        }
    }
}
