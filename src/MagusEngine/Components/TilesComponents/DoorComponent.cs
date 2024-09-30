namespace MagusEngine.Components.TilesComponents
{
    public class DoorComponent
    {
        public bool IsOpen { get; set; }
        public bool Locked { get; set; }
        public int LockPower { get; set; }
        public char OpenGlyph { get; set; } = '-';
        public char ClosedGlyph { get; set; } = '+';

        //closes a door
        public void Close()
        {
            IsOpen = false;
        }

        public void Lock(int lockPower = 0)
        {
            LockPower = lockPower;
            Locked = true;
        }

        // opens a door
        public void Open(int unlockPower = 0)
        {
            if (Locked && unlockPower < LockPower)
                return;
            IsOpen = true;
            Locked = false;
        }
    }
}
