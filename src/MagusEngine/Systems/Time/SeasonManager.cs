using System;
using Arquimedes.Enumerators;

namespace MagusEngine.Systems.Time
{
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

        public static SeasonType DetermineCurrentSeason(int monthOfYear)
        {
            if (monthOfYear <= 0 || monthOfYear > 12)
            {
                throw new Exception(
                    "An error occurred when "
                        + $"trying to determine the current season! Month of year {monthOfYear}"
                );
            }

            if (monthOfYear <= 3)
                return SeasonType.Spring;
            else if (monthOfYear > 3 && monthOfYear <= 6)
                return SeasonType.Summer;
            else if (monthOfYear > 6 && monthOfYear <= 9)
                return SeasonType.Autumn;
            else
                return SeasonType.Winter;
        }
    }
}

