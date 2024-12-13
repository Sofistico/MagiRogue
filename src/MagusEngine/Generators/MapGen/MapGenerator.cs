using Arquimedes.Enumerators;
using GoRogue.Random;
using MagusEngine.Core.Civ;
using MagusEngine.Core.Entities;
using MagusEngine.Core.MapStuff;
using MagusEngine.Components.TilesComponents;
using MagusEngine.Systems;
using MagusEngine.Utils;
using MagusEngine.Utils.Extensions;
using Newtonsoft.Json;
using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;
using ShaiRandom.Generators;
using System;
using System.Collections.Generic;
using System.Linq;
using MagusEngine.Services.Factory;

namespace MagusEngine.Generators.MapGen
{
    // TODO: Refactor this whole class
    public class MapGenerator
    {
        private bool oneRulerOnly;

        protected DistinctRandom randNum;
        protected ulong seed;
        protected MagiMap _map; // Temporarily store the map currently worked on
        protected List<Building> _rooms;

        // Empty constructor
        public MapGenerator()
        {
            ulong rand = GoRogue.Random.GlobalRandom.DefaultRNG.NextULong();
            randNum = new(rand);
            seed = rand;
            _rooms = new();
        }

        public MapGenerator(MagiMap map) : this()
        {
            _map = map;
        }

        #region GeneralMapGen

        /// <summary>
        /// Inserts an stair at the center of a room
        /// </summary>
        /// <param name="room"></param>
        protected void InsertStairs(Room room)
        {
            Furniture stairsDown = new(Color.White, Color.Black, '>', room.RoomRectangle.Center,
                FurnitureType.StairsDown, "wood", "Stair");

            // debug to unlock the room
            //room.ForceUnlock();

            _map.AddMagiEntity(stairsDown);
        }

        /// <summary>
        /// Prepares the map with all tiles being stone floor
        /// </summary>
        protected void PrepareForFloors()
        {
            foreach (var pos in _map.Positions())
            {
                _map.SetTerrain(TileFactory.GenericStoneFloor(pos, true));
            }
            TileFactory.ResetCachedMaterial();
        }

        /// <summary>
        /// Makes all tiles become grass tiles
        /// </summary>
        protected void PrepareForFloorsWithGrass()
        {
            foreach (var pos in _map.Positions())
            {
                _map.SetTerrain(TileFactory.GenericGrass(pos));
            }
        }

        /// <summary>
        /// Makes all tiles become the specifed TileFloor
        /// </summary>
        /// <param name="floor"></param>
        protected void PrepareForAnyFloor(Tile floor)
        {
            foreach (var pos in _map.Positions())
            {
                floor.Position = pos;
                _map.SetTerrain(floor);
            }
        }

        /// <summary>
        /// Makes the specified map tiles become the specifed TileFloor
        /// </summary>
        /// <param name="floor"></param>
        /// <param name="map"></param>
        protected static void PrepareForAnyFloor(Tile floor, MagiMap map)
        {
            map.SetTerrain(floor);
        }

        /// <summary>
        /// Connects 2 points with a road.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        protected void CarveRoad(Point endCoord, Point originCoord, Tile roadTile)
        {
            var path = _map.AStar.ShortestPath(originCoord, endCoord, Distance.Manhattan, true);
            if (path is null)
                return;
            for (int i = 0; i < path.Length; i++)
            {
                if (i == path.Length - 1)
                    break;
                var step = path.GetStep(i);
                // road can't enter inside a room.
                if (_rooms.Any(r => r.PhysicalRoom.RoomRectangle.Contains(step)))
                    return;
                var tile = roadTile.Copy();
                tile.Position = step;
                _map.SetTerrain(tile);
            }
        }

        protected void ApplyRoads(List<Building> roomsInMap, Tile tileToUse)
        {
            for (int i = 1; i < roomsInMap.Count; i++)
            {
                if (roomsInMap[i - 1].PhysicalRoom.Doors.Count == 0 || roomsInMap[i].PhysicalRoom.Doors.Count == 0)
                    continue;
                Point previousRoomDoor = roomsInMap[i - 1].PhysicalRoom.Doors[0].Position;
                Point currentRoomDoor = roomsInMap[i].PhysicalRoom.Doors[0].Position;

                CarveRoad(previousRoomDoor, currentRoomDoor, tileToUse);
            }
        }

        /// <summary>
        /// Makes the perimeter of a map filled with walls
        /// </summary>
        protected void PrepareForOuterWalls()
        {
            foreach (var pos in _map.Positions())
            {
                if (pos.X == 0 || pos.Y == 0 || pos.X == _map.Width - 1 || pos.Y == _map.Height - 1)
                {
                    _map.SetTerrain(TileFactory.GenericStoneWall(pos, true));
                }
            }
            TileFactory.ResetCachedMaterial();
        }

        /// <summary>
        /// Builds a room composed of walls and floors using the supplied Rectangle which determines
        /// its size and position on the map Walls are placed at the perimeter of the room Floors
        /// are placed in the interior area of the room
        /// </summary>
        /// <param name="room"></param>
        protected void CreateRoom(Room room, Tile wall, Tile floor)
        {
            Rectangle rectangle = room.RoomRectangle;
            // Place floors in interior area
            for (int x = 1; x < rectangle.Width; x++)
            {
                for (int y = 1; y < rectangle.Height; y++)
                {
                    CreateAnyFloor(new Point(x, y), floor);
                }
            }

            // Place walls at perimeter
            foreach (Point location in Utils.Extensions.MagiPointExtensions.GetBorderCellLocations(rectangle))
            {
                CreateAnyWall(location, wall);
            }
        }

        protected void CreateAnyWall(Point location, Tile wall)
        {
            var copyWall = wall.Copy();
            copyWall.Position = location;
            _map.SetTerrain(copyWall);
        }

        protected void CreateAnyFloor(Point point, Tile floor)
        {
            var copyFloor = floor.Copy();
            copyFloor.Position = point;
            _map.SetTerrain(copyFloor);
        }

        /// <summary>
        /// Creates a Floor tile at the specified X/Y location
        /// </summary>
        /// <param name="location"></param>
        protected void CreateStoneFloor(Point location)
        {
            Tile floor = TileFactory.GenericStoneFloor(location);

            // a simple setterrain already does it for me
            _map.SetTerrain(floor);
        }

        /// <summary>
        /// Create any type of wall
        /// </summary>
        /// <param name="wall"></param>
        protected void CreateAnyWall(Tile wall) => _map.SetTerrain(wall);

        /// <summary>
        /// Create any type of wall at the specifed map
        /// </summary>
        /// <param name="wall"></param>
        /// <param name="map"></param>
        protected static void CreateAnyWall(Tile wall, MagiMap map)
        {
            map.SetTerrain(wall);
        }

        /// <summary>
        /// Fills the map with walls
        /// </summary>
        protected void FloodWalls()
        {
            for (int i = 0; i < _map.Terrain.Count; i++)
            {
                Tile wall = TileFactory.GenericStoneWall(Point.FromIndex(i, _map.Width), true);
                _map.SetTerrain(wall);
            }
            TileFactory.ResetCachedMaterial();
        }

        /// <summary>
        /// Place magic nodes at random location of the map
        /// </summary>
        /// <param name="nodes"></param>
        protected void PlaceNodes(int nodes)
        {
            for (int i = 0; i < nodes; i++)
            {
                int rnd = randNum.NextInt(_map.Terrain.Count);

                Tile? rndTile = _map.GetTerrainAt<Tile>(Point.FromIndex(rnd, _map.Width));

                int rndMp = randNum.NextInt(1, 15);

                //var node = new NodeTile
                //    (Color.Purple, Color.Transparent, Point.FromIndex(rnd, _map.Width), rndMp,
                //    (int)NodeStrength.Normal);

                //nodeTile.GoRogueComponents.Add(new Components.IllusionComponent(rndTile),
                //    Components.IllusionComponent.Tag);

                //_map.SetTerrain(nodeTile);
            }
        }

        /// <summary>
        /// returns a collection of Points which represent locations along a line
        /// </summary>
        /// <param name="xOrigin"></param>
        /// <param name="yOrigin"></param>
        /// <param name="xDestination"></param>
        /// <param name="yDestination"></param>
        /// <returns></returns>
        protected IEnumerable<Point> GetTileLocationsAlongLine
            (int xOrigin, int yOrigin, int xDestination, int yDestination)
        {
            // prevent line from overflowing boundaries of the map
            xOrigin = ClampX(xOrigin);
            yOrigin = ClampY(yOrigin);
            xDestination = ClampX(xDestination);
            yDestination = ClampY(yDestination);

            int dx = Math.Abs(xDestination - xOrigin);
            int dy = Math.Abs(yDestination - yOrigin);

            int sx = xOrigin < xDestination ? 1 : -1;
            int sy = yOrigin < yDestination ? 1 : -1;
            int err = dx - dy;

            while (true)
            {
                yield return new Point(xOrigin, yOrigin);
                if (xOrigin == xDestination && yOrigin == yDestination)
                    break;

                int e2 = 2 * err;
                if (e2 > -dy)
                {
                    err -= dy;
                    xOrigin += sx;
                }
                if (e2 < dx)
                {
                    err += dx;
                    yOrigin += sy;
                }
            }
        }

        /// <summary>
        /// sets X coordinate between right and left edges of map to prevent any out-of-bounds errors
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        protected int ClampX(int x)
        {
            if (x < 0)
                return 0;
            else if (x > _map.Width - 1)
                return _map.Width - 1;
            return x;

            // OR using ternary conditional operators: return (x < 0) ? 0 : (x > _map.Width - 1) ?
            // _map.Width - 1 : x;
        }

        /// <summary>
        /// sets Y coordinate between top and bottom edges of map to prevent any out-of-bounds errors
        /// </summary>
        /// <param name="y"></param>
        /// <returns></returns>
        protected int ClampY(int y)
        {
            if (y < 0)
                return 0;
            else if (y > _map.Height - 1)
                return _map.Height - 1;
            return y;

            // OR using ternary conditional operators: return (y < 0) ? 0 : (y > _map.Height - 1) ?
            // _map.Height - 1 : y;
        }

        /// <summary>
        /// carve a tunnel out of the map parallel to the x-axis
        /// </summary>
        /// <param name="xStart"></param>
        /// <param name="xEnd"></param>
        /// <param name="yPosition"></param>
        protected void CreateHorizontalTunnel(int xStart, int xEnd, int yPosition)
        {
            for (int x = Math.Min(xStart, xEnd); x <= Math.Max(xStart, xEnd); x++)
            {
                CreateStoneFloor(new Point(x, yPosition));
            }
        }

        /// <summary>
        /// carve a tunnel using the y-axis
        /// </summary>
        /// <param name="yStart"></param>
        /// <param name="yEnd"></param>
        /// <param name="xPosition"></param>
        protected void CreateVerticalTunnel(int yStart, int yEnd, int xPosition)
        {
            for (int y = Math.Min(yStart, yEnd); y <= Math.Max(yStart, yEnd); y++)
            {
                CreateStoneFloor(new Point(xPosition, y));
            }
        }

        /// <summary>
        /// Determines if a Point on the map is a good candidate for a door. Returns true if it's a
        /// good spot for a door Returns false if there is a Tile that IsBlockingMove=true at that location
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        protected bool IsPotentialDoor(Point location)
        {
            //if the target location is not walkable
            //then it's a wall and not a good place for a door
            //int locationIndex = location.ToIndex(_map.Width);
            /*if (_map.Tiles[locationIndex] is null)
                && _map.Tiles[locationIndex] is TileWall)
            {
                return false;
            }*/

            // first make sure that isn't trying to take a door off the limits of the map
            if (_map.CheckForIndexOutOfBounds(location))
                return false;

            //store references to all neighbouring cells
            GetAllNeighourTiles(location, out Point right,
                out Point left, out Point top, out Point bottom);

            if (_map.CheckForIndexOutOfBounds(top) || _map.CheckForIndexOutOfBounds(bottom)
                || _map.CheckForIndexOutOfBounds(left) || _map.CheckForIndexOutOfBounds(right))
            {
                return false;
            }

            // check to see if there is a door already in the target location, or
            // above/below/right/left of the target location If it detects a door there, return false.
            if (_map.GetTileAt<DoorComponent>(location.X, location.Y) != null ||
                _map.GetTileAt<DoorComponent>(right.X, right.Y) != null ||
                _map.GetTileAt<DoorComponent>(left.X, left.Y) != null ||
                _map.GetTileAt<DoorComponent>(top.X, top.Y) != null ||
                _map.GetTileAt<DoorComponent>(bottom.X, bottom.Y) != null)
            {
                return false;
            }

            //if all the prior checks are okay, make sure that the door is placed along a horizontal wall
            if (!_map.Terrain[right.ToIndex(_map.Width)].IsWalkable &&
                !_map.Terrain[left.ToIndex(_map.Width)].IsWalkable &&
                _map.Terrain[top.ToIndex(_map.Width)].IsWalkable &&
                _map.Terrain[bottom.ToIndex(_map.Width)].IsWalkable)
            {
                return true;
            }

            //or make sure that the door is placed along a vertical wall
            if (_map.Terrain[right.ToIndex(_map.Width)].IsWalkable &&
                _map.Terrain[left.ToIndex(_map.Width)].IsWalkable &&
                !_map.Terrain[top.ToIndex(_map.Width)].IsWalkable &&
                !_map.Terrain[bottom.ToIndex(_map.Width)].IsWalkable)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// store references to all neighbouring cells and return it
        /// </summary>
        /// <param name="location"></param>
        /// <param name="right"></param>
        /// <param name="left"></param>
        /// <param name="top"></param>
        /// <param name="bottom"></param>
        protected static void GetAllNeighourTiles(Point location,
            out Point right, out Point left, out Point top, out Point bottom)
        {
            right = new Point(location.X + 1, location.Y);
            left = new Point(location.X - 1, location.Y);
            top = new Point(location.X, location.Y - 1);
            bottom = new Point(location.X, location.Y + 1);
        }

        /// <summary>
        /// Tries to create a TileDoor object in a specified Rectangle perimeter. Reads through the
        /// entire list of tiles comprising the perimeter, and determines if each position is a
        /// viable candidate for a door. When it finds a potential position, creates a closed and
        /// unlocked door.
        /// </summary>
        /// <param name="room"></param>
        /// <param name="acceptsMoreThanOneDoor"></param>
        protected void CreateDoor(Room room, bool acceptsMoreThanOneDoor = false)
        {
            List<Point> borderCells = Utils.Extensions.MagiPointExtensions.GetBorderCellLocations(room.RoomRectangle);
            bool alreadyHasDoor = false;

            //go through every border cell and look for potential door candidates
            foreach (Point location in borderCells)
            {
                if (alreadyHasDoor)
                    break;
                //int locationIndex = location.ToIndex(_map.Width);
                if (IsPotentialDoor(location) && !alreadyHasDoor)
                {
                    // Create a new door that is closed and unlocked.
                    Tile? newDoor = TileFactory.CreateTile(location, TileType.Door, "wood");
                    _map.SetTerrain(newDoor!);
                    if (!acceptsMoreThanOneDoor)
                        alreadyHasDoor = true;
                    room.Doors.Add(newDoor);
                    room.DoorsPoint.Add(newDoor.Position);
                }
            }
        }

        /// <summary>
        /// Places trees inside the map, in a random like manner Sadly only places one type of tree
        /// </summary>
        /// <param name="map"></param>
        /// <param name="treeToPlace"></param>
        /// <exception cref="NotImplementedException"></exception>
        protected static void PlaceTrees(MagiMap map, Tile treeToPlace)
        {
            for (int i = 0; i < map.Terrain.Count; i++)
            {
                Tile tree = treeToPlace.Copy();
                Point pos = Point.FromIndex(i, map.Width);
                int rng = GlobalRandom.DefaultRNG.NextInt(0, 15);
                if (rng == 13)
                {
                    tree.Position = pos;
                    map.SetTerrain(tree);
                }
            }
        }

        /// <summary>
        /// Place generic trees over the specified map
        /// </summary>
        /// <param name="map"></param>
        protected static void PlaceGenericTrees(MagiMap map)
        {
            PlaceTrees(map, TileFactory.GenericTree());
        }

        /// <summary>
        /// Place generic trees over the map
        /// </summary>
        /// <param name="map"></param>
        protected void PlaceGenericTrees()
        {
            PlaceTrees(_map, TileFactory.GenericTree());
        }

        /// <summary>
        /// Places a list of trees inside the map, varying randomly
        /// </summary>
        /// <param name="map"></param>
        /// <param name="treesToPlace"></param>
        /// <exception cref="NotImplementedException"></exception>
        protected void PlaceTrees(MagiMap map, List<Tile> treesToPlace)
        {
            // need to remake
            throw new NotImplementedException();
        }

        /// <summary>
        /// Places vegetation along the floor of the map, normamilly walkable
        /// </summary>
        /// <param name="map"></param>
        /// <param name="vegetationToPlace"></param>
        /// <exception cref="NotImplementedException"></exception>
        protected void PlaceVegetations(MagiMap map, Tile vegetationToPlace)
        {
            //first make sure that the vegetation won't be attackable and it's a vegetation
            for (int i = 0; i < map.Terrain.Count; i++)
            {
                /*TileFloor vegetal = new
                    TileFloor(vegetationToPlace.Name, vegetationToPlace.Position,
                    vegetationToPlace.MaterialOfTile.Id, vegetationToPlace.Glyph,
                    vegetationToPlace.Foreground, vegetationToPlace.Background);*/
                Tile vegetal = vegetationToPlace.Copy();
                Point pos = Point.FromIndex(i, map.Width);
                int rng = randNum.NextInt(0, 15);
                if (rng == 13)
                {
                    vegetal.Position = pos;
                    map.SetTerrain(vegetal);
                }
            }
        }

        /// <summary>
        /// Places a list of vegetation along the floor of the map
        /// </summary>
        /// <param name="map"></param>
        /// <param name="vegetationsToPlace"></param>
        /// <exception cref="NotImplementedException"></exception>
        protected void PlaceVegetations(MagiMap map, List<Tile> vegetationsToPlace)
        {
            //first make sure that the vegetation won't be attackable and it's a vegetation
            for (int i = 0; i < vegetationsToPlace.Count; i++)
            {
                Tile vegetal = vegetationsToPlace[randNum.NextInt(vegetationsToPlace.Count + 1)];

                PlaceVegetations(map, vegetal);
            }
        }

        /// <summary>
        /// Flood the specifide map with water
        /// </summary>
        /// <param name="tile"></param>
        /// <param name="map"></param>
        protected static void FloodWithWaterMap(Tile tile, MagiMap map)
        {
            map.SetTerrain(tile);
        }

        protected void FloodWithWaterMap(Tile tile)
        {
            _map.SetTerrain(tile);
        }

        /// <summary>
        /// Puts a tile in a random mode in the specified map
        /// </summary>
        /// <param name="map"></param>
        /// <param name="template"></param>
        protected void PutRngFloorTileThere(MagiMap map, Tile template)
        {
            for (int i = 0; i < map.Terrain.Count; i++)
            {
                int rng = randNum.NextInt(0, 15);
                Point pos = map.Terrain[i].Position;
                if (rng == 13)
                {
                    Tile newTile = template.Copy(pos);

                    map.SetTerrain(newTile);
                }
            }
        }

        /// <summary>
        /// Puts a tile in a random mode in the map
        /// </summary>
        /// <param name="template"></param>
        protected void PutRngFloorTileThere(Tile template)
        {
            for (int i = 0; i < _map.Terrain.Count; i++)
            {
                int rng = randNum.NextInt(0, 15);
                Point pos = _map.Terrain[i].Position;
                if (rng == 13)
                {
                    var newTile = template.Copy(pos);
                    _map.SetTerrain(newTile);
                }
            }
        }

        protected static bool CheckIfPointIsCorrect(Simple2DGrid grid, Point point)
        {
            return point.X < grid.Width && point.X >= 0 && point.Y < grid.Height && point.Y >= 0;
        }

        public static void ConnectMapsInsideChunk(MagiMap[] maps)
        {
            int len = maps.Length;
            // perfect for flood fill!
            Stack<Point> pointedList = new();
            const int desiredSize = 3;
            Simple2DGrid grid = new(len / desiredSize, len / desiredSize, new());
            pointedList.Push(new Point(0, 0));
            while (pointedList.Count > 0)
            {
                pointedList.TryPop(out Point currentPoint);
                if (CheckIfPointIsCorrect(grid, currentPoint))
                {
                    int idx = Point.ToIndex(currentPoint.X, currentPoint.Y, grid.Width);
                    MagiMap map = maps[idx];

                    if (!grid.Points.Contains(currentPoint))
                    {
                        GetAllNeighourTiles(currentPoint, out Point right, out Point left,
                            out Point top,
                            out Point bottom);
                        if (CheckIfPointIsCorrect(grid, right))
                        {
                            ConnectPointsWithMaps(maps, pointedList, grid, currentPoint, map, right);
                        }
                        if (CheckIfPointIsCorrect(grid, left))
                        {
                            ConnectPointsWithMaps(maps, pointedList, grid, currentPoint, map, left);
                        }
                        if (CheckIfPointIsCorrect(grid, top))
                        {
                            ConnectPointsWithMaps(maps, pointedList, grid, currentPoint, map, top);
                        }
                        if (CheckIfPointIsCorrect(grid, bottom))
                        {
                            ConnectPointsWithMaps(maps, pointedList, grid, currentPoint, map, bottom);
                        }

                        grid.Points.Add(currentPoint);
                        if (grid.Points.Count == len)
                            break;
                    }
                }
            }
        }

        private static void ConnectPointsWithMaps(MagiMap[] maps, Stack<Point> pointedList, Simple2DGrid grid,
            Point currentPoint, MagiMap map, Point nextPoint)
        {
            int endIdx = Point.ToIndex(nextPoint.X, nextPoint.Y, grid.Width);
            Direction di = Direction.GetDirection(currentPoint, nextPoint);
            pointedList.Push(nextPoint);
            map.MapZoneConnections.Add(di, maps[endIdx]);
        }

        protected static void FillMapWithGrass(MagiMap map)
        {
            foreach (var p in map.Terrain.Positions())
            {
                var item = (Tile)map.Terrain[p];
                if (item.IsWalkable)
                    TileFactory.AddVegetation(item, DataManager.QueryPlantInData("grass"));
            }
        }

        protected void MakeRoomsUseful(MagiMap completeMap)
        {
            List<Room>? mapRooms = completeMap.Rooms;
            if (mapRooms is null)
                return;
            // make so that the list of rooms come from the worldTile.CivInfluence.Sites
            foreach (Room room in mapRooms)
            {
                MakeRoomUseful(room, completeMap);
            }
        }

        protected static void MakeRoomWithTagUseful(Room room, MagiMap map)
        {
            // make so that the list of rooms come from the worldTile.CivInfluence.Sites
            if (room.RoomPoints.Length >= 6)
            {
                GiveRoomFurniture(room, map);
            }
        }

        protected void MakeRoomUseful(Room room, MagiMap map)
        {
            // make so that the list of rooms come from the worldTile.CivInfluence.Sites
            if (room.RoomPoints.Length >= 6)
            {
                int rng = GoRogue.Random
                    .GlobalRandom.DefaultRNG.NextInt(Enum.GetValues(typeof(RoomTag)).Length);
                var tag = (RoomTag)rng;
                if (tag != RoomTag.Throne)
                {
                    room.Tag = tag;
                }
                else if (tag == RoomTag.Throne && !oneRulerOnly)
                {
                    room.Tag = tag;
                    oneRulerOnly = true;
                }

                GiveRoomFurniture(room, map);
            }
        }

        protected static void GiveRoomFurniture(Room room, MagiMap map)
        {
            switch (room.Tag)
            {
                case RoomTag.Generic:
                    AddFurnitureAtRandomPos(DataManager.QueryFurnitureInData("wood_table"), room, map);
                    AddFurnitureAtRandomPos(DataManager.QueryFurnitureInData("wood_chair"), room, map);
                    break;

                case RoomTag.Inn:
                    AddFurnituresAtRandomPos(DataManager.QueryFurnitureInData("wood_table"), room, map, 4);
                    AddFurnituresAtRandomPos(DataManager.QueryFurnitureInData("wood_chair"), room, map, 3);
                    AddFurnituresAtRandomPos(DataManager.QueryFurnitureInData("keg"), room, map, 3);
                    break;

                case RoomTag.Temple:
                    AddFurnitureAtRandomPos(DataManager.QueryFurnitureInData("religious_altar"), room, map);
                    AddFurnituresAtRandomPos(DataManager.QueryFurnitureInData("wood_chair"), room, map, 3);

                    break;

                case RoomTag.Blacksmith:
                    AddFurnitureAtRandomPos(DataManager.QueryFurnitureInData("stone_forge"), room, map);
                    AddFurnitureAtRandomPos(DataManager.QueryFurnitureInData("anvil"), room, map);
                    AddFurnitureAtRandomPos(DataManager.QueryFurnitureInData("sack"), room, map);
                    break;

                case RoomTag.Clothier:
                    AddFurnitureAtRandomPos(DataManager.QueryFurnitureInData("wood_loom"), room, map);
                    AddFurnitureAtRandomPos(DataManager.QueryFurnitureInData("wood_chair"), room, map);
                    AddFurnitureAtRandomPos(DataManager.QueryFurnitureInData("wood_table"), room, map);
                    break;

                case RoomTag.Alchemist:
                    AddFurnitureAtRandomPos(DataManager.QueryFurnitureInData("alembic"), room, map);
                    AddFurnitureAtRandomPos(DataManager.QueryFurnitureInData("crucible"), room, map);
                    AddFurnitureAtRandomPos(DataManager.QueryFurnitureInData("stone_forge"), room, map);
                    AddFurnitureAtRandomPos(DataManager.QueryFurnitureInData("magical_distilator"), room, map);
                    break;

                case RoomTag.Hovel:
                    AddFurnituresAtRandomPos(DataManager.QueryFurnitureInData("wood_bed"), room, map, 5);
                    break;

                case RoomTag.Abandoned:
                    AddFurnituresAtRandomPos(DataManager.QueryFurnitureInData("broken_misc"), room, map, 5);
                    break;

                case RoomTag.House:
                    AddFurnitureAtRandomPos(DataManager.QueryFurnitureInData("wood_table"), room, map);
                    AddFurnitureAtRandomPos(DataManager.QueryFurnitureInData("wood_chair"), room, map);
                    AddFurnitureAtRandomPos(DataManager.QueryFurnitureInData("wood_chest"), room, map);
                    AddFurnitureAtRandomPos(DataManager.QueryFurnitureInData("wood_bed"), room, map);
                    break;

                case RoomTag.Throne:
                    AddFurnitureAtRandomPos(DataManager.QueryFurnitureInData("wood_table"), room, map);
                    AddFurnitureAtRandomPos(DataManager.QueryFurnitureInData("wood_chair"), room, map);
                    AddFurnitureAtRandomPos(DataManager.QueryFurnitureInData("wood_chest"), room, map);
                    AddFurnitureAtRandomPos(DataManager.QueryFurnitureInData("wood_throne"), room, map);
                    break;

                case RoomTag.Kitchen:
                    AddFurnituresAtRandomPos(DataManager.QueryFurnitureInData("wood_table"), room, map, 2);
                    AddFurnitureAtRandomPos(DataManager.QueryFurnitureInData("wood_chair"), room, map);
                    AddFurnituresAtRandomPos(DataManager.QueryFurnitureInData("oven"), room, map, 2);

                    break;

                case RoomTag.GenericWorkshop:
                    AddFurnitureAtRandomPos(DataManager.QueryFurnitureInData("wood_chair"), room, map);
                    AddFurnitureAtRandomPos(DataManager.QueryFurnitureInData("crafting_table"), room, map);
                    break;

                case RoomTag.Dinner:
                    AddFurnituresAtRandomPos(DataManager.QueryFurnitureInData("wood_table"), room, map, 2);
                    AddFurnituresAtRandomPos(DataManager.QueryFurnitureInData("wood_chair"), room, map, 4);

                    break;

                case RoomTag.DungeonKeeper:
                    AddFurnituresAtRandomPos(DataManager.QueryFurnitureInData("wood_table"), room, map, 2);
                    AddFurnituresAtRandomPos(DataManager.QueryFurnitureInData("wood_chair"), room, map, 4);

                    break;

                default:
                    throw new ApplicationException($"Type of room not defined! room: {JsonConvert.SerializeObject(room)}");
            }
        }

        private static void AddFurnituresAtRandomPos(Furniture furniture, Room room, MagiMap map, int quantity)
        {
            for (int i = 0; i < quantity; i++)
            {
                AddFurnitureAtRandomPos(furniture.Copy(), room, map);
            }
        }

        private static void AddFurnitureAtRandomPos(Furniture furniture, Room room, MagiMap map)
        {
            Point pos = Point.None;
            int tries = 0;
            while (!map.IsTileWalkable(pos) || map.EntityIsThere(pos))
            {
                pos = room.ReturnRandomPosRoom();
                if (tries++ >= 100)
                    return; // not found a place, can stop looking
            }
            furniture.Position = pos;
            map.AddMagiEntity(furniture);
        }

        #endregion GeneralMapGen

        #region MapGenAlgorithms

        #region BSPGen

        /// <summary>
        /// Generates a bsp map
        /// </summary>
        /// <param name="map"></param>
        /// <param name="roomMaxSize"></param>
        /// <param name="roomMinSize"></param>
        /// <param name="maxRooms"></param>
        /// <returns></returns>
        protected List<Building> BspMapFunction(MagiMap map, int roomMaxSize, int roomMinSize, int maxRooms)
        {
            _map = map;
            List<Building> rooms = new();

            Rectangle rec = new(map.Positions().First(),
                map.Positions().Last());

            List<Rectangle> mapPartitions =
                BisectRecursiveRandom(rec, roomMaxSize).ToList();

            for (int i = 0; i <= maxRooms; i++)
            {
                if (mapPartitions.Count == 0) break;
                Rectangle area = mapPartitions[randNum.NextInt(mapPartitions.Count)];
                mapPartitions.Remove(area);

                if (area.Width <= roomMinSize || area.Height <= roomMinSize)
                    continue;

                int newRoomWidht = randNum.NextInt(roomMinSize, area.Width - 1);
                int newRoomHeight = randNum.NextInt(roomMinSize, area.Height - 1);

                int newRoomX = randNum.NextInt(1, map.Width - newRoomWidht);
                int newRoomY = randNum.NextInt(1, map.Height - newRoomHeight);

                Rectangle rectangle = new Rectangle(newRoomX, newRoomY, newRoomWidht, newRoomHeight);
                //Rectangle rectangle = new Rectangle(area.X, area.Y, area.Width, area.Height);
                Room room = new Room(rectangle);

                if (!rooms.Any(r => r.PhysicalRoom.RoomRectangle.Intersects(room.RoomRectangle)))
                {
                    rooms.Add(new Building(room));
                }
            }

            return rooms;
        }

        /// <summary>
        /// Bisects the rectangle in position
        /// </summary>
        /// <returns>An IEnumerable with Top and Bottom Rectangles in indexes 0 and 1, respectively</returns>
        protected IEnumerable<Rectangle> BisectRandomHorizontal(Rectangle toBisect)
        {
            int startX = toBisect.MinExtentX;
            int stopY = toBisect.MaxExtentY;
            int startY = toBisect.MinExtentY;
            int stopX = toBisect.MaxExtentX;
            int bisection = randNum.NextInt(startY, stopY);

            yield return new Rectangle(new Point(startX, startY), new Point(stopX, bisection));
            yield return new Rectangle(new Point(startX, bisection + 1), new Point(stopX, stopY));
        }

        /// <summary>
        /// Bisects the rectangle into top and bottom halves
        /// </summary>
        /// <returns>An IEnumerable with Top and Bottom Rectangles in indexes 0 and 1, respectively</returns>
        protected IEnumerable<Rectangle> BisectRandomVertical(Rectangle toBisect)
        {
            int startX = toBisect.MinExtentX;
            int stopY = toBisect.MaxExtentY;
            int startY = toBisect.MinExtentY;
            int stopX = toBisect.MaxExtentX;
            int bisection = randNum.NextInt(startX, stopX);

            yield return new Rectangle(new Point(startX, startY), new Point(bisection, stopY));
            yield return new Rectangle(new Point(bisection + 1, startY), new Point(stopX, stopY));
        }

        protected IEnumerable<Rectangle> BisectRec(Rectangle bisectRec)
        {
            if (bisectRec.Width > bisectRec.Height)
                return BisectRandomVertical(bisectRec);
            else
                return BisectRandomHorizontal(bisectRec);
        }

        protected IEnumerable<Rectangle> BisectRecursiveRandom(Rectangle bisectRec,
            int minimumDimension)
        {
            foreach (Rectangle child in BisectRec(bisectRec))
            {
                if (child.Height < minimumDimension * 2
                    && child.Width < minimumDimension * 2)
                    yield return child;
                else
                    foreach (var grandChild in
                        BisectRecursiveRandom(child, minimumDimension))
                    {
                        yield return grandChild;
                    }
            }
        }

        #endregion BSPGen

        #endregion MapGenAlgorithms

        #region Utils

        // quite clever 🐴
        protected static Tile? QueryTilesForTrait(Trait trait, TileType type = TileType.Floor)
        {
            return TileFactory.CreateTile(Point.None, type, trait);
        }

        #endregion Utils
    }
}
