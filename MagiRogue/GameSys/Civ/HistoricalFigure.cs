using MagiRogue.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public string[] Feats { get; set; }
        public int YearBorn { get; set; }
        public int? YearDeath { get; set; }
        public bool IsAlive { get; set; }
        public Actor? AssocietedActor { get; set; }

        public HistoricalFigure(string name,
            string description,
            string[] feats,
            int yearBorn,
            int? yearDeath,
            bool isAlive,
            Actor? associetedActor = null)
        {
            Name = name;
            Description = description;
            Feats = feats;
            YearBorn = yearBorn;
            YearDeath = yearDeath;
            IsAlive = isAlive;
            AssocietedActor = associetedActor;
        }

        public HistoricalFigure(string name, string desc, int born, int? died, bool alive)
        {
            Name = name;
            Description = desc;
            YearBorn = born;
            YearDeath = died;
            IsAlive = alive;
        }
    }
}