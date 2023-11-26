using Arquimedes.Enumerators;
using MagusEngine.Serialization.EntitySerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagusEngine.Core.Entities.StarterScenarios
{
    public class Scenario
    {
        #region Properties

        public string Id { get; set; }
        public string ScenarioName { get; set; }
        public string Description { get; set; }

        #region stats

        public AgeGroup AgeGroup { get; set; }

        // body
        public int Strenght { get; set; }
        public int Toughness { get; set; }
        public int Endurance { get; set; }

        // mind
        public int Inteligence { get; set; }
        public int Precision { get; set; }

        // soul
        public int WillPower { get; set; }

        // max stuff
        public int MaxMana { get; set; }
        public int MaxStamina { get; set; }

        // regen
        public double StaminaRegen { get; set; }
        public double ManaRegen { get; set; }
        public double LimbRegen { get; set; }

        // fit
        public double FitLevel { get; set; }

        #endregion stats

        #region Skills

        public List<AbilityTemplate> Abilities { get; set; }

        #endregion Skills

        #region itens

        public List<string[]> Inventory { get; set; }

        public List<string[]> Equipment { get; set; }

        #endregion itens

        public string[] RacesAllowed { get; set; }

        public string[] SpellsKnow { get; set; }

        #endregion Properties

        public Scenario()
        {
        }

        public override string ToString()
        {
            return ScenarioName;
        }
    }
}