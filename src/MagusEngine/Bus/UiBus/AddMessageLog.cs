using Arquimedes.Enumerators;
using MagusEngine.Core.Entities.Base;
using MagusEngine.Systems;
using MagusEngine.Utils.Extensions;

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

        public AddMessageLog(string message, MagiEntity? entity, PointOfView firstOrThirdPerson = PointOfView.First)
        {
            Message = message;
            if (Find.ControlledEntity is not null && entity is not null)
                PlayerCanSee = Find.ControlledEntity.CanSee(entity!.Position);
            Person = firstOrThirdPerson;
        }

        public AddMessageLog(string message, Point playerPoint, Point actionPoint, int playerFieldOfView, PointOfView firstOrThirdPerson = PointOfView.First)
        {
            Message = message;
            PlayerCanSee = playerPoint.GetDistance(actionPoint) <= playerFieldOfView;
            Person = firstOrThirdPerson;
        }
    }
}
