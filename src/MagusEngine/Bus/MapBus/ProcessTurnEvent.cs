namespace MagusEngine.Bus.MapBus
{
    public class ProcessTurnEvent
    {
        public long Time { get; set; }
        public bool Sucess { get; set; }

        public ProcessTurnEvent(long time, bool sucess)
        {
            Time = time;
            Sucess = sucess;
        }
    }
}
