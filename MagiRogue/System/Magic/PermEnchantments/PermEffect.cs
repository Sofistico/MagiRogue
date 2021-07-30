using MagiRogue.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.System.Magic.PermEnchantments
{
    public class PermEffect : IPermEffect
    {
        private const int _totalTime = Time.TimeHelper.Year;

        public Actor Enchanted { get; set; }
        public ISpellEffect Enchantment { get; set; }
        public int NodeCost { get; set; }

        /// <summary>
        /// Defines an enchantment that will be applied yearly, you can't have more than one type at the same time
        /// </summary>
        /// <param name="enchanted">The actor that will be enchanted</param>
        /// <param name="enchantment">The enchantment that will be applied</param>
        /// <param name="nodeCost">How many node will cost for the enchant</param>
        public PermEffect(Actor enchanted, ISpellEffect enchantment, int nodeCost)
        {
            Enchanted = enchanted;
            Enchantment = enchantment;

            NodeCost = nodeCost;
        }

        public void Enchant()
        {
            Enchantment.ApplyEffect(Enchanted.Position, Enchanted, new SpellBase());
        }
    }
}