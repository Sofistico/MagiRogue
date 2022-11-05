using MagiRogue.Data.Enumerators;
using MagiRogue.Data.Serialization;
using MagiRogue.Data.Serialization.EntitySerialization;
using System.Collections.Generic;

namespace MagiRogue.GameSys.Civ
{
    public class Product
    {
        /// <summary>
        /// The item that is produced
        /// </summary>
        public string ItemId { get; set; }

        /// <summary>
        /// The amount of the item that is produced!
        /// </summary>
        public int Amount { get; set; }

        /// <summary>
        /// The type of material that will be needed or the type of material that the item will be made off!
        /// </summary>
        public List<MaterialType> Material { get; set; }

        /// <summary>
        /// The total volume that the <see cref="Material"/ of the item will be needed!>
        /// </summary>
        public int VolumeOfMaterialNeededToProduce { get; set; }

        public Product()
        {
        }

        public Product(string itemId,
            int amount,
            int volumeOfMaterials,
            List<MaterialType> material)
        {
            ItemId = itemId;
            Amount = amount;
            Material = material;
            VolumeOfMaterialNeededToProduce = volumeOfMaterials;
        }

        public ItemTemplate ReturnProductItem()
            => Data.DataManager.QueryItemInData(ItemId);
    }
}