using GoRogue.Messaging;

namespace MagusEngine.Bus.ComponentBus
{
    public class ComponentListAddedHandler : ISubscriber<ComponentAddedCommand>
    {
        private Map _map;

        public ComponentListAddedHandler(Map map)
        {
            _map = map;
        }

        public void Handle(ComponentAddedCommand message)
        {
            _map.AddComponentToEntity(message.Id, message.Component);
        }
    }
}
