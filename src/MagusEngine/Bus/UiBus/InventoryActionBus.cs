using System;
using MagusEngine.Core.Entities;

namespace MagusEngine.Bus.UiBus
{
    public class InventoryActionBus
    {
        public Action<Item> Action { get; }
        public Actor ActorInventory { get; }

        public InventoryActionBus(Action<Item> action, Actor actorInventory)
        {
            Action = action;
            ActorInventory = actorInventory;
        }
    }
}
