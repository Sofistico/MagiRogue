using MagusEngine.Core.Entities.Base;

namespace MagusEngine.Bus.UiBus
{
    public class ChangeCenteredActor
    {
        public MagiEntity Entity { get; set; }

        public ChangeCenteredActor(MagiEntity actor)
        {
            Entity = actor;
        }
    }
}
