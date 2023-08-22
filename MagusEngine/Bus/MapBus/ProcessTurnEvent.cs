namespace MagusEngine.Bus.MapBus
{
    public class ProcessTurnEvent
    {
        public uint Time { get; set; }
        public bool Sucess { get; set; }

        public ProcessTurnEvent(uint time, bool sucess)
        {
            Time = time;
            Sucess = sucess;
        }
    }
}
