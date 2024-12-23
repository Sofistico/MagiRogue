using MagusEngine.Core.Entities.Base;
using SadConsole;

namespace MagusEngine.Components.EntityComponents
{
    public class RepeatAnimationComponent : GoRogue.Components.ParentAware.ParentAwareComponentBase<MagiEntity>
    {
        private readonly ColoredGlyph[] _animationFrames;
        private int _animationIndex;

        public bool Animating { get; protected set; }

        public RepeatAnimationComponent(params ColoredGlyph[] animationFrames)
        {
            _animationFrames = animationFrames;
        }

        public void Start()
        {
            Animating = true;
        }

        public void Stop()
        {
            Animating = false;
            Parent!.SadCell!.AppearanceSingle!.Appearance.Glyph = _animationFrames[0].Glyph;
            _animationIndex = 0;
        }

        public void Animate()
        {
            if (!Animating)
                return;

            if (_animationIndex >= _animationFrames.Length)
            {
                Stop();
            }
            else
            {
                Parent!.SadCell.AppearanceSingle!.Appearance.CopyAppearanceFrom(_animationFrames[_animationIndex]);
                _animationIndex++;
            }
        }
    }
}
