using Arquimedes.Enumerators;

namespace MagusEngine.Bus.UiBus
{
    public class CloseWindowMessage
    {
        public WindowTag Tag { get; set; }

        public CloseWindowMessage(WindowTag tag)
        {
            Tag = tag;
        }
    }
}
