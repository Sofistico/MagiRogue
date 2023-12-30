namespace MagusEngine.Bus.MapBus
{
    public class ProcessTurnEvent
    {
        public long Time { get; set; }
        public bool Sucess { get; set; }
        public bool CanAct { get; set; }

        public ProcessTurnEvent(long time, bool sucess, bool canAct = true)
        {
            Time = time;
            Sucess = sucess;
            CanAct = canAct;
        }
    }
}
