using Arquimedes.Enumerators;

namespace MagusEngine.Core.Entities.Base
{
    public class MemoryEntity<T> : IMemory
    {
        public Point LastSeen { get; set; }
        public MemoryType MemoryType { get; set; }
        public T? ObjToRemember { get; set; }
        public bool Valid { get; set; }

        public MemoryEntity()
        {
        }

        public MemoryEntity(Point lastSeen,
            MemoryType memoryType,
            T? objToRemember)
        {
            LastSeen = lastSeen;
            MemoryType = memoryType;
            ObjToRemember = objToRemember;
            Valid = true;
        }
    }

    public interface IMemory
    {
        public Point LastSeen { get; set; }
        public MemoryType MemoryType { get; set; }
        public bool Valid { get; set; }
    }
}
