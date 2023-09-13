using MagusEngine.Bus.UiBus;
using MagusEngine.Services;
using MagusEngine.Systems;

namespace MagusEngine.ECS.Components.ActorComponents
{
    public class TestComponent
    {
        public TestComponent()
        {
            Locator.GetService<MessageBusService>().SendMessage<AddMessageLog>(new($"It worked, here are your x and y value: {Find.Universe.Player.Position.X} {Find.Universe.Player.Position.Y}"));
        }
    }
}