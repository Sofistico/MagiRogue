namespace MagiRogue.Bus.ComponentBus
{
    public class ComponentAdded
    {
        public uint Id { get; set; }
        public object Component { get; set; }

        public ComponentAdded(uint id, object component)
        {
            Id = id;
            Component = component;
        }
    }
}