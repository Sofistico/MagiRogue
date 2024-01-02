using MagusEngine.Core.MapStuff;

namespace MagusEngine.Bus
{
    public class ChangeControlledActorMap
    {
        public MagiMap Map { get; set; }
        public Point PosInMap { get; set; }
        public MagiMap? PreviousMap { get; set; }

        public ChangeControlledActorMap(MagiMap map, Point posInMap, MagiMap? previousMap = null)
        {
            //Entity = actor;
            Map = map;
            PosInMap = posInMap;
            PreviousMap = previousMap;
        }
    }
}
