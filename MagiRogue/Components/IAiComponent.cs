using MagiRogue.System;
using MagiRogue.UI.Windows;

namespace MagiRogue.Components
{
    public interface IAiComponent : GoRogue.Components.ParentAware.IParentAwareComponent
    {
        (bool sucess, long ticks) RunAi(Map map, MessageLogWindow messageLog);
    }
}