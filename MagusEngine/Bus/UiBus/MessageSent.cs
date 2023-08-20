namespace MagusEngine.Bus.UiBus
{
    public class MessageSent
    {
        public string Message { get; set; }
        public bool IsPlayer { get; set; }

        public MessageSent(string message, bool playerSees = true)
        {
            Message = message;
            IsPlayer = playerSees;
        }
    }
}
