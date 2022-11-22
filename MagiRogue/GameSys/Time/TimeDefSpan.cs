namespace MagiRogue.GameSys.Time
{
    /// <summary>
    /// Definition of time, has converter to and from various dates and ticks.
    /// </summary>
    public sealed class TimeDefSpan
    {
        public const int DaysPerMonth = 31;
        public const int MonthsPerYear = 12;
        public const int MinutesPerHour = 60;
        public const int SecondsPerMinute = 60;
        public const int SecondsPerDay = 86400;
        public const int CentisecondsPerSecond = 100;
        public const int SecondsPerMonth = (DaysPerMonth * SecondsPerDay);
        public const int SecondsPerYear = SecondsPerMonth * MonthsPerYear;

        private long _centiseconds;

        public TimeDefSpan(long centiseconds)
        {
            _centiseconds = centiseconds;
        }

        public int Year => (int)(Seconds / SecondsPerYear);
        public int Session => (int)((Month / 3) + 0.5f);
        public int Month => ((int)(Seconds % SecondsPerYear) / SecondsPerMonth) + 1;
        public int Day => ((int)(Seconds % SecondsPerMonth) / SecondsPerDay) + 1;
        public int Hours => Minutes / MinutesPerHour;
        public int Minutes => (int)(Seconds / SecondsPerMinute);
        public long Seconds => _centiseconds / CentisecondsPerSecond;
        public long Ticks => _centiseconds;

        public void SetTick(long centiseconds)
        {
            _centiseconds = centiseconds;
        }
    }
}