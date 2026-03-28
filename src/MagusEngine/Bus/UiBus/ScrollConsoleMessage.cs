using Arquimedes.Enumerators;

namespace MagusEngine.Bus.UiBus
{
    public class ScrollConsoleMessage
    {
        public Point Delta { get; }
        public WindowTag Tag { get; }

        public ScrollConsoleMessage(Point delta, WindowTag tag)
        {
            Delta = delta;
            Tag = tag;
        }
    }
}
