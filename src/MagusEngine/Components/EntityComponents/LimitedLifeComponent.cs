namespace MagusEngine.Components.EntityComponents
{
    public class LimitedLifeComponent
    {
        public uint TicksToLive { get; set; }

        public LimitedLifeComponent(uint ticksToLive)
        {
            TicksToLive = ticksToLive;
        }
    }
}
