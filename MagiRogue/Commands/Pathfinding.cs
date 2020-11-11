using MagiRogue.System.Tiles;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using GoRogue.Pathing;
using GoRogue.MapViews;
using GoRogue;
using MagiRogue.Entities;

namespace MagiRogue.Commands
{
    // Will insert Dijikstra algorith, and then A* algorithm
    public class Pathfinding
    {
        private readonly IMapView<bool> mapNodes; // Views the map by simpliyfing tilebase to a bool that something is walkable

        private FastAStar fastPath;

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
                    GameLoop.CommandManager.MoveActorBy(actor, steps);
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