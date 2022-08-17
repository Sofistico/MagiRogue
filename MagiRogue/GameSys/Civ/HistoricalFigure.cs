using MagiRogue.Data.Enumerators;
using MagiRogue.Entities;

namespace MagiRogue.GameSys.Civ
{
    /// <summary>
    /// A class that models a historical figure of the world, if it's still alive, there should be
    /// an associetaded actor to represent it.
    /// </summary>
    public class HistoricalFigure
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Sex HFGender { get; set; }
        public string Race { get; set; }
        public Legend[] Legends { get; set; }
        public int YearBorn { get; set; }
        public int? YearDeath { get; set; }
        public bool IsAlive { get; set; }
        public Actor? AssocietedActor { get; set; }

        public HistoricalFigure(string name,
            Sex hFGender,
            Legend[] legends,
            int yearBorn,
            int? yearDeath,
            bool isAlive,
            Actor? associetedActor = null)
        {
            Name = name;
            HFGender = hFGender;
            Legends = legends;
            YearBorn = yearBorn;
            YearDeath = yearDeath;
            IsAlive = isAlive;
            AssocietedActor = associetedActor;
            Description = associetedActor.Description;
            Race = associetedActor.GetAnatomy().GetRace().RaceName;
        }

        public HistoricalFigure(string name,
            string desc,
            string race,
            int born,
            int? died,
            bool alive,
            Sex hFGender)
        {
            Name = name;
            Description = desc;
            Race = race;
            YearBorn = born;
            YearDeath = died;
            IsAlive = alive;
            HFGender = hFGender;
        }
    }
}