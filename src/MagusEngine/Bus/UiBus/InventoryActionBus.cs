using System;
using MagusEngine.Core.Entities;

namespace MagusEngine.Bus.UiBus
{
    public class InventoryActionBus
    {
        public Actor ActorInventory { get; }
        public Action<Item>? Action { get; }

        public InventoryActionBus(Actor actorInventory, Action<Item>? action)
        {
            ActorInventory = actorInventory;
            Action = action;
        }
    }
}
