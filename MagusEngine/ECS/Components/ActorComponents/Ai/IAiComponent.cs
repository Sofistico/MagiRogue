using MagiRogue.GameSys;
using MagiRogue.UI.Windows;

namespace MagusEngine.ECS.Components.ActorComponents.Ai
{
    public interface IAiComponent : GoRogue.Components.ParentAware.IParentAwareComponent
    {
        (bool sucess, long ticks) RunAi(Map map, MessageLogWindow messageLog);
    }
}