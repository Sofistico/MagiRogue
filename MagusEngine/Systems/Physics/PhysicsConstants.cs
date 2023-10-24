namespace MagusEngine.Systems.Physics
{
    public static class PhysicsConstants
    {
        /// <summary>
        /// In meters per second, m/s²
        /// </summary>
        public const double EarthGravity = 9.807;

        /// <summary>
        /// Represents the speed that something will reach when eventually going to the ground.
        /// \n Uses the V=g*t, where g is Gravity and t is time falling.
        /// \n This method only considers the use case of something falling from a zero inicial velocity.
        /// </summary>
        /// <param name="ticksFalling">the amount of ticks something will be falling</param>
        /// <returns>Returns the speed in m/s</returns>
        public static double CalculateFallSpeed(int ticksFalling)
        {
            return EarthGravity * ticksFalling;
        }
    }
}