namespace MagusEngine.Bus.UiBus
{
    public class AddMessageLog
    {
        public string Message { get; set; }
        public bool PlayerCanSee { get; set; }

        public AddMessageLog(string message, bool playerSees = true)
        {
            Message = message;
            PlayerCanSee = playerSees;
        }
    }
}
