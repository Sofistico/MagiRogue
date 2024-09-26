using MagusEngine.Core.MapStuff;
using MagusEngine.Systems;
using System.Collections.Generic;

namespace MagusEngine.Core.Civ
{
    public class Building
    {
        public string Name { get; set; } // maybe pick the name of the owner? like Tuarval Smithy or somesuch!
        public Room PhysicalRoom { get; set; }
        public List<Reaction> Produces { get; set; }
        public bool Workshop => Produces.Count > 0;

        public Building()
        {
            Produces = new List<Reaction>();
        }

        public Building(Room physicalRoom)
        {
            PhysicalRoom = physicalRoom;
            Produces = DataManager.GetProductsByTag(PhysicalRoom.Tag);
        }
    }
}
