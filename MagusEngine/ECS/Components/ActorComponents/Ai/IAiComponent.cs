using MagusEngine.Core.MapStuff;

namespace MagusEngine.ECS.Components.ActorComponents.Ai
{
    public interface IAiComponent : GoRogue.Components.ParentAware.IParentAwareComponent
    {
        // message log will be by event
        (bool sucess, long ticks) RunAi(MagiMap map);
    }
}