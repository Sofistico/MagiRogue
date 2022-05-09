namespace MagiRogue.GameSys.Time
{
    public interface ITimeSystem
    {
        void RegisterEntity(ITimeNode node);

        void DeRegisterEntity(ITimeNode node);

        ITimeNode NextNode();
    }
}