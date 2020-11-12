using GoRogue;
using GoRogue.MapViews;
using GoRogue.Pathing;
using MagiRogue.Entities;

namespace MagiRogue.System
{
    // Will insert Dijikstra algorith, and then A* algorithm
    public class Pathfinding
    {
        private readonly IMapView<bool> mapNodes; // Views the map by simpliyfing tilebase to a bool that something is walkable

        private readonly FastAStar fastPath;

        public Pathfinding(IMapView<bool> walkabilityMap)
        {
            mapNodes = walkabilityMap;
            fastPath = new FastAStar(walkabilityMap, Distance.CHEBYSHEV);
        }

        public bool WalkPath(Coord target, Actor mob)
        {
            if (mob != null)
            {
                Path path = fastPath.ShortestPath(mob.Position, target);
                return FollowPath(path, mob);
            }
            else
                return false;
        }

        private bool FollowPath(Path path, Actor actor)
        {
            if (path != null)
            {
                foreach (Coord steps in path.Steps)
                {
                    // I think this solves the bug of getting an entitiy out of the bordes of the map
                    //Direction.GetDirection(path.Start, path.End); // thoughts for a future upgrade.
                    actor.Position = new Coord(steps.X, steps.Y);
                    return true;
                }
                return false;
            }
            else
            {
                return false;
            }
        }
    }
}