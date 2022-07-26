using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.Entities
{
    public class Mind
    {
        public int Inteligence { get; internal set; }
        public int Precision { get; internal set; }

        /// <summary>
        /// Dictionary of the Abilities of an actor.
        /// Never add directly to the dictionary, use the method AddAbilityToDictionary to add new abilities
        /// </summary>
        public Dictionary<int, Ability> Abilities { get; set; }

        public Mind()
        {
            Abilities = new();
        }

        public void AddAbilityToDictionary(Ability ability)
        {
            Abilities.Add(ability.Id, ability);
        }
    }
}