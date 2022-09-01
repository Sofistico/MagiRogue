using MagiRogue.Data.Enumerators;
using MagiRogue.Entities;
using System;
using System.Collections.Generic;

namespace MagiRogue.GameSys.Planet.History
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
        public List<Legend> Legends { get; set; }
        public int YearBorn { get; set; }
        public int? YearDeath { get; set; }
        public bool IsAlive { get; set; }

        public Entity? AssocietedEntity { get; set; }
        public List<int> RelatedCivs { get; set; } = new();
        public List<int> RelatedSettlements { get; set; } = new();
        public List<int> RelatedHFs { get; set; } = new();
        public Mind Mind { get; set; } = new();
        public Soul Soul { get; set; } = new();
        public Body Body { get; set; } = new();

        public HistoricalFigure(string name,
            Sex hFGender,
            List<Legend> legends,
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
            AssocietedEntity = associetedActor;
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

        public HistoricalFigure(string name, string description, Sex hFGender, string race, bool isAlive)
        {
            Name = name;
            Description = description;
            HFGender = hFGender;
            Race = race;
            IsAlive = isAlive;
        }

        public void GenerateRandomPersonality()
        {
            Personality personality = Mind.Personality;
            // reflection because i'm lazy
            var properties = typeof(Personality).GetProperties();
            foreach (var property in properties)
            {
                property.SetValue(personality, GameLoop.GlobalRand.NextInt(-50, 50 + 1));
            }
        }

        public void GenerateRandomSkills()
        {
            var enums = Enum.GetValues(typeof(AbilityName));

            foreach (AbilityName e in enums)
            {
            }
        }

        public void DefineProfession()
        {
        }

        public void AddLegend(Legend legend)
        {
            Legends.Add(legend);
        }
    }
}