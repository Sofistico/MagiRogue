using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.System.Time
{
    public sealed class TimeDefSpan
    {
        private const int DaysPerMonth = 31;
        private const int MonthsPerYear = 12;
        private const int HoursPerDay = 24;
        private const int MinutesPerHour = 60;
        private const int SecondsPerMinute = 60;
        private const int SecondsPerDay = 86400;
        private const int CentisecondsPerSecond = 100;
        private const int SecondsPerMonth = (DaysPerMonth * SecondsPerDay);
        private const int SecondsPerYear = SecondsPerMonth * MonthsPerYear;

        private long _centiseconds;

        public TimeDefSpan(long centiseconds)
        {
            _centiseconds = centiseconds;
        }

        public int Year => (int)(Seconds / SecondsPerYear);
        public int Month => ((int)(Seconds % SecondsPerYear) / SecondsPerMonth) + 1;
        public int Day => ((int)(Seconds % SecondsPerMonth) / SecondsPerDay) + 1;
        public int Hours => (int)(Minutes / MinutesPerHour);
        public int Minutes => (int)(Seconds / SecondsPerMinute);
        public long Seconds => _centiseconds / CentisecondsPerSecond;
        public long Ticks => _centiseconds;

        public void SetTick(long centiseconds)
        {
            _centiseconds = centiseconds;
        }
    }
}