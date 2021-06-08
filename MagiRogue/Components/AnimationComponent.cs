using SadConsole;
using GoRogue;
using SadConsole.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoRogue.GameFramework;

namespace MagiRogue.Components
{
    public class AnimationComponent : GoRogue.GameFramework.Components.IGameObjectComponent
    {
        private ColoredGlyph[] _animationFrames;
        private Timer _timer;

        public AnimationComponent(Timer animationTimer, params ColoredGlyph[] animationFrames)
        {
            _animationFrames = animationFrames;
            _timer = animationTimer;
        }

        public IGameObject Parent { get; set; }
    }
}