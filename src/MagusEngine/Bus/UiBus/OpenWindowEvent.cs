using Arquimedes.Enumerators;

namespace MagusEngine.Bus.UiBus
{
    public class OpenWindowEvent
    {
        public WindowTag Window { get; }

        public OpenWindowEvent(WindowTag window)
        {
            Window = window;
        }
    }
}
