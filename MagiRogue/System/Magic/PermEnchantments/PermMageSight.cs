using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.System.Magic.PermEnchantments
{
    /// <summary>
    /// Isn't really permanent, must be renewed yearly
    /// </summary>
    public class PermMageSight : Effects.MageSightEffect
    {
        public int NodeCost { get; set; }

        public PermMageSight(int nodeCost) : base(Time.TimeHelper.Year)
        {
        }
    }
}