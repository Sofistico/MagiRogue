using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.Data.Serialization
{
    public class EquipTemplate
    {
        public ItemTemplate ItemEquipped { get; set; }
        public LimbTemplate LimbEquipped { get; set; }

        public EquipTemplate(ItemTemplate itemEquipped, LimbTemplate limbEquipped)
        {
            ItemEquipped = itemEquipped;
            LimbEquipped = limbEquipped;
        }
    }
}