﻿using System.Collections.Generic;
using Arquimedes.Enumerators;
using Arquimedes.Interfaces;
using MagusEngine.Core.Entities;
using MagusEngine.Core.Entities.Base;
using MagusEngine.Systems;
using Newtonsoft.Json;

namespace MagusEngine.Core.Civ
{
    // will need to rethink this object better!
    public class Reaction : IJsonKey
    {
        private readonly Dictionary<string, Item> cachedItems = [];

        public string Id { get; set; }
        public string Time { get; set; }
        public bool Oversight { get; set; }

        /// <summary>
        /// The items that is produced
        /// </summary>
        public List<string> ItemsId { get; set; }

        /// <summary>
        /// The selected item to produce
        /// </summary>
        public string SelectedItemId { get; set; }

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
        public List<MaterialType> Material { get; set; } = [];

        public List<string> Query { get; set; }

        [JsonIgnore]
        /// <summary> The total volume that the <see cref="Material"/ of the item will be needed!> </summary>
        public int VolumeOfMaterialNeededToProduce
        {
            get
            {
                if (!cachedItems.TryGetValue(SelectedItemId, out var cachedItem) == true)
                {
                    cachedItem = DataManager.QueryItemInData(SelectedItemId);
                    cachedItems.Add(SelectedItemId, cachedItem);
                }

                return cachedItem.Volume * Amount;
            }
        }

        /// <summary>
        /// What quality the product needs to be made
        /// </summary>
        public List<Quality> Quality { get; set; } = [];
        public List<RoomTag> RoomTag { get; set; } = [];

        public Reaction() { }

        public Reaction(string itemId, int amount, List<MaterialType> material)
        {
            ItemsId = [itemId];
            SelectedItemId = itemId;
            Amount = amount;
            Material = material;
        }

        public Item ReturnProductItem(string itemId, Material materialToUse)
        {
            SelectedItemId = itemId;
            if (!cachedItems.TryGetValue(itemId, out var cachedItem))
            {
                cachedItem = DataManager.QueryItemInData(SelectedItemId, materialToUse);
            }
            return cachedItem;
        }
    }
}
