using Arquimedes.Enumerators;
using MagusEngine.Core.Entities.Base;
using MagusEngine.Serialization.EntitySerialization;
using MagusEngine.Systems;
using System.Collections.Generic;

namespace MagusEngine.Core.Civ
{
    // will need to rethink this object better!
    public class Reaction
    {
        private ItemTemplate cachedItem;

        public string Id { get; set; }

        /// <summary>
        /// The item that is produced
        /// </summary>
        public string ItemId { get; set; }

        /// <summary>
        /// Reaction name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The amount of the item that is produced!
        /// </summary>
        public int Amount { get; set; }

        /// <summary>
        /// The type of material that will be needed or the type of material that the item will be
        /// made off!
        /// </summary>
        public List<MaterialType> Material { get; set; } = new();

        /// <summary> The total volume that the <see cref="Material"/ of the item will be needed!> </summary>
        public int VolumeOfMaterialNeededToProduce
        {
            get
            {
                cachedItem ??= DataManager.QueryItemInData(ItemId);
                return cachedItem.Volume * Amount;
            }
        }

        /// <summary>
        /// What quality the product needs to be made
        /// </summary>
        public List<Quality> Quality { get; set; } = new();
        public List<RoomTag> RoomTag { get; set; } = new();

        public Reaction()
        {
        }

        public Reaction(string itemId,
            int amount,
            List<MaterialType> material)
        {
            ItemId = itemId;
            Amount = amount;
            Material = material;
        }

        public ItemTemplate ReturnProductItem()
        {
            cachedItem ??= DataManager.QueryItemInData(ItemId);
            return cachedItem;
        }
    }
}