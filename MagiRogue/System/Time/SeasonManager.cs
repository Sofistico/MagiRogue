using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Threading.Tasks;

namespace MagiRogue.System.Time
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum SeasonType
    {
        Spring = 1,
        Summer = 2,
        Autumn = 3,
        Winter = 4
    }

    public class SeasonManager
    {
        public int SeasonLenght { get; set; }
        public SeasonType CurrentSeason { get; set; }

        public SeasonManager()
        {
            // Empty constructor
        }

        public SeasonManager(SeasonType currentSeason, int seasonLenght)
        {
            CurrentSeason = currentSeason;
            SeasonLenght = seasonLenght;
        }

        public SeasonType DetermineCurrentSeason(int monthOfYear)
        {
            if (monthOfYear <= 0 || monthOfYear > 12)
                throw new Exception("An error occurred when " +
                    $"trying to determine the current season! Month of year {monthOfYear}");

            if (monthOfYear <= 3)
                return SeasonType.Spring;
            else if (monthOfYear > 3 && monthOfYear <= 6)
                return SeasonType.Summer;
            else if (monthOfYear > 6 && monthOfYear <= 9)
                return SeasonType.Autumn;
            else if (monthOfYear > 9 && monthOfYear <= 12)
                return SeasonType.Winter;
            else
                throw new Exception
                    ("An error occured in the if else statement to determine current season");
        }
    }
}