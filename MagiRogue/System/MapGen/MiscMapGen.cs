namespace MagiRogue.System.MapGen
{
    public class MiscMapGen : MapGenerator
    {
        public MiscMapGen()
        {
            // empty
        }

        public Map GenerateTestMap()
        {
            _map = new Map("Test Map");

            PrepareForFloors();
            PrepareForOuterWalls();

            return _map;
        }

        public Map GenerateStoneFloorMap()
        {
            _map = new Map("Test Stone Map");

            PrepareForFloors();

            return _map;
        }
    }
}