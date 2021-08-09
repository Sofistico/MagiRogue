using MagiRogue.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.System.Magic.PermEnchantments
{
    public interface IPermEffect
    {
        public Actor Enchanted { get; set; }
        public ISpellEffect Enchantment { get; set; }
        public int NodeCost { get; set; }

        public void Enchant(int nodesSacrificed);
    }
}