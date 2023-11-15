using Arquimedes.Enumerators;

namespace MagusEngine.Bus.UiBus
{
    public class AddMessageLog
    {
        public string Message { get; set; }
        public bool PlayerCanSee { get; set; }
        public PointOfView Person { get; set; }

        public AddMessageLog(string message, bool playerSees = true, PointOfView firstOrThirdPerson = PointOfView.First)
        {
            Message = message;
            PlayerCanSee = playerSees;
            Person = firstOrThirdPerson;
        }
    }
}
