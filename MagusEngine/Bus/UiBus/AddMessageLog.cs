namespace MagusEngine.Bus.UiBus
{
    public class AddMessageLog
    {
        public string Message { get; set; }
        public bool IsPlayer { get; set; }

        public AddMessageLog(string message, bool playerSees = true)
        {
            Message = message;
            IsPlayer = playerSees;
        }
    }
}
