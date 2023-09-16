namespace MagusEngine.Bus.ComponentBus
{
    public class ComponentAddedCommand<T>
    {
        public uint Id { get; set; }
        public T Component { get; set; }

        public ComponentAddedCommand(uint id, T component)
        {
            Id = id;
            Component = component;
        }
    }
}
