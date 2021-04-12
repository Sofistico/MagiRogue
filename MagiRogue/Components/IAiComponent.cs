using GoRogue.GameFramework.Components;
using MagiRogue.System;
using MagiRogue.UI.Windows;

namespace MagiRogue.Components
{
    public interface IAiComponent : IGameObjectComponent
    {
        (bool sucess, long ticks) RunAi(Map map, MessageLogWindow messageLog);
    }
}