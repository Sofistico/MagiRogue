using MagusEngine.Core.Entities.Base;
using MagusEngine.Core.MapStuff;

namespace MagusEngine.Bus
{
    public class ChangeControlledActorMap
    {
        public MagiEntity Entity { get; set; }
        public Map Map { get; set; }
        public Point PosInMap { get; set; }

        public ChangeControlledActorMap(MagiEntity actor, Map map, Point posInMap)
        {
            Entity = actor;
            Map = map;
            PosInMap = posInMap;
        }
    }
}
