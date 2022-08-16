using MagiRogue.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace MagiRogue.Data.Serialization.EntitySerialization
{
    public class EquipTemplate
    {
        public string ItemEquipped { get; set; }
        public string LimbEquipped { get; set; }

        public EquipTemplate(string itemEquipped, string limbEquipped)
        {
            ItemEquipped = itemEquipped;
            LimbEquipped = limbEquipped;
        }
    }
}