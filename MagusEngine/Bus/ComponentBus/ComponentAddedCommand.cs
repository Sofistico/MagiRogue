namespace MagusEngine.Bus.ComponentBus
{
    public class ComponentAddedCommand
    {
        public uint Id { get; set; }
        public object Component { get; set; }

        public ComponentAddedCommand(uint id, object component)
        {
            Id = id;
            Component = component;
        }
    }
}