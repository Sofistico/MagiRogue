﻿using MagusEngine.Core.Entities.Base;
using SadConsole;
using SadConsole.Components;

namespace MagusEngine.ECS.Components.ActorComponents
{
    public class AnimationComponent : GoRogue.Components.ParentAware.ParentAwareComponentBase<MagiEntity>
    {
        private readonly ColoredGlyph[] _animationFrames;
        private readonly Timer _timer;
        protected int animationIndex;
        public bool Animating { get; protected set; }

        public AnimationComponent(Timer animationTimer, params ColoredGlyph[] animationFrames)
        {
            _animationFrames = animationFrames;
            _timer = animationTimer;
            _timer.Repeat = true;
        }

        public void Start() => Animating = true;

        public void Stop()
        {
            Animating = false;
            var parent = Parent;
            parent.SadCell.AppearanceSingle.Appearance.Glyph = _animationFrames[0].Glyph;
            animationIndex = 0;
        }

        public void Animate()
        {
            if (!Animating)
                return;

            if (animationIndex >= _animationFrames.Length)
            {
                Stop();
            }
            else
            {
                var parent = Parent;
                parent.SadCell.AppearanceSingle.Appearance.CopyAppearanceFrom(_animationFrames[animationIndex]);
                animationIndex++;
            }
        }
    }
}