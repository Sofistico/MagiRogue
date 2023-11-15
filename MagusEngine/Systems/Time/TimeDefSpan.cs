namespace MagusEngine.Systems.Time
{
    /// <summary>
    /// Definition of time, has converter to and from various dates and ticks.
    /// </summary>
    public sealed class TimeDefSpan
    {
        private long _centiseconds;

        public const int DaysPerMonth = 31;
        public const int MonthsPerYear = 12;
        public const int MinutesPerHour = 60;
        public const int SecondsPerHour = SecondsPerMinute * MinutesPerHour;
        public const int SecondsPerMinute = 60;
        public const int SecondsPerDay = 86400;
        public const int CentisecondsPerSecond = 100;
        public const int SecondsPerMonth = DaysPerMonth * SecondsPerDay;
        public const int SecondsPerYear = SecondsPerMonth * MonthsPerYear;

        public int Year { get; set; }
        public int Session => (int)((Month / 3) + 0.5f);
        public int Month => ((int)(Seconds % SecondsPerYear) / SecondsPerMonth) + 1;
        public int Day => ((int)(Seconds % SecondsPerMonth) / SecondsPerDay) + 1;
        public int Hours => Minutes / MinutesPerHour;
        public int Minutes => (int)(Seconds / SecondsPerMinute);
        public long Seconds => _centiseconds / CentisecondsPerSecond;
        public long Ticks => _centiseconds;

        public TimeDefSpan(long centiseconds)
        {
            _centiseconds = centiseconds;
        }

        public TimeDefSpan(int startYear)
        {
            Year = startYear;
        }

        public void SetTick(long centiseconds)
        {
            _centiseconds = centiseconds;
        }
    }
}
