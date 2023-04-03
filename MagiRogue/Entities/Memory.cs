using MagiRogue.Data.Enumerators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.Entities
{
    public class Memory
    {
        public Point LastSeen { get; set; }
        public MemoryType MemoryType { get; set; }
        public object? ObjToRemember { get; set; }

        public Memory()
        {
        }

        public Memory(Point lastSeen,
            MemoryType memoryType,
            object? objToRemember)
        {
            LastSeen = lastSeen;
            MemoryType = memoryType;
            ObjToRemember = objToRemember;
        }
    }
}
