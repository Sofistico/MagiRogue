using GoRogue.Messaging;

namespace MagiRogue.Bus.ComponentBus
{
    public class ComponentListChanged : ISubscriber<ComponentAdded>
    {
        public void Handle(ComponentAdded message)
        {
            GameLoop.GetCurrentMap()?.AddComponentToEntity(message.Id, message.Component);
        }
    }
}
