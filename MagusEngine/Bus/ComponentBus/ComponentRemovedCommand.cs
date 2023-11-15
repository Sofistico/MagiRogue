namespace MagusEngine.Bus.ComponentBus
{
    public class ComponentRemovedCommand
    {
        public uint Id { get; set; }

        public ComponentRemovedCommand(uint id)
        {
            Id = id;
        }
    }
}
