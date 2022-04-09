using MagiRogue.Data;
using MagiRogue.Entities;
using MagiRogue.System.Planet;
using MagiRogue.System.Tiles;
using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;
using System;
using System.Collections.Generic;
using System.Linq;
using ShaiRandom.Generators;
using MagiRogue.Utils;

namespace MagiRogue.System
{
    // TODO: Refactor this whole class
    public abstract class MapGenerator
    {
        protected DistinctRandom randNum;
        protected ulong seed;
        protected Map _map; // Temporarily store the map currently worked on
        protected List<Room> _rooms;

        // Empty constructor
        public MapGenerator()
        {
            ulong rand = GoRogue.Random.GlobalRandom.DefaultRNG.NextULong();
            randNum = new(rand);
            seed = rand;
            _rooms = new();
        }

        public MapGenerator(Map map) : this()
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
            Furniture stairsDown = new Furniture(Color.White, Color.Black, '>', room.RoomRectangle.Center,
                (int)MapLayer.FURNITURE, FurnitureType.StairsDown);

            // debug to unlock the room
            //room.ForceUnlock();

            _map.Add(stairsDown);
        }

        /// <summary>
        /// Prepares the map with all tiles being stone floor
        /// </summary>
        protected void PrepareForFloors()
        {
            foreach (var pos in _map.Positions())
            {
                _map.SetTerrain(new TileFloor(pos, "stone"));
            }
        }

        /// <summary>
        /// Makes all tiles become grass tiles
        /// </summary>
        protected void PrepareForFloorsWithGrass()
        {
            foreach (var pos in _map.Positions())
            {
                _map.SetTerrain(TileEncyclopedia.GenericGrass(pos));
            }
        }

        /// <summary>
        /// Makes all tiles become the specifed TileFloor
        /// </summary>
        /// <param name="floor"></param>
        protected void PrepareForAnyFloor(TileFloor floor)
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
        protected static void PrepareForAnyFloor(TileFloor floor, Map map)
        {
            map.SetTerrain(floor);
        }

        /// <summary>
        /// Connects 2 points with a road.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        protected void CarveRoad(Point endCoord, Point originCoord, TileFloor roadTile)
        {
            var path = _map.AStar.ShortestPath(originCoord, endCoord, Distance.Manhattan, true);
            if (path is null)
                return;
            for (int i = 0; i < path.Length; i++)
            {
                if (i == path.Length - 1)
                    break;
                var step = path.GetStep(i);
                var tile = roadTile.Copy();
                tile.Position = step;
                _map.SetTerrain(tile);
            }
        }

        protected void ApplyRoads(List<Room> roomsInMap, TileFloor tileToUse)
        {
            for (int i = 1; i < roomsInMap.Count; i++)
            {
                if (roomsInMap[i - 1].Doors.Count <= 0 || roomsInMap[i].Doors.Count <= 0)
                    continue;
                Point previousRoomDoor = roomsInMap[i - 1].Doors.First().Position;
                Point currentRoomDoor = roomsInMap[i].Doors.First().Position;

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
                    _map.SetTerrain(new TileWall(pos, "stone"));
                }
            }
        }

        /// <summary>
        /// Builds a room composed of walls and floors using the supplied Rectangle
        /// which determines its size and position on the map
        /// Walls are placed at the perimeter of the room
        /// Floors are placed in the interior area of the room
        /// </summary>
        /// <param name="room"></param>
        protected void CreateRoom(Rectangle room, TileWall wall, TileFloor floor)
        {
            // Place floors in interior area
            for (int x = room.ToMonoRectangle().Left + 1; x < room.ToMonoRectangle().Right; x++)
            {
                for (int y = room.ToMonoRectangle().Top + 1; y < room.ToMonoRectangle().Bottom; y++)
                {
                    CreateAnyFloor(new Point(x, y), floor);
                }
            }

            // Place walls at perimeter
            List<Point> perimeter = GetBorderCellLocations(room);

            foreach (Point location in perimeter)
            {
                CreateAnyWall(location, wall);
            }
        }

        private void CreateAnyWall(Point location, TileWall wall)
        {
            var copyWall = wall.Copy();
            copyWall.Position = location;
            _map.SetTerrain(copyWall);
        }

        private void CreateAnyFloor(Point point, TileFloor floor)
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
            TileFloor floor = new TileFloor(location);

            // a simple setterrain already does it for me
            _map.SetTerrain(floor);
        }

        /// <summary>
        /// Creates a Wall tile at the specified X/Y location
        /// </summary>
        /// <param name="location"></param>
        protected void CreateStoneWall(Point location)
        {
            TileWall wall = new TileWall(location);
            _map.SetTerrain(wall);
        }

        /// <summary>
        /// Create any type of wall
        /// </summary>
        /// <param name="wall"></param>
        protected void CreateAnyWall(TileWall wall) => _map.SetTerrain(wall);

        /// <summary>
        /// Create any type of wall at the specifed map
        /// </summary>
        /// <param name="wall"></param>
        /// <param name="map"></param>
        protected static void CreateAnyWall(TileWall wall, Map map)
        {
            map.SetTerrain(wall);
        }

        /// <summary>
        /// Fills the map with walls
        /// </summary>
        protected void FloodWalls()
        {
            for (int i = 0; i < _map.Tiles.Length; i++)
            {
                TileWall wall = new TileWall(Point.FromIndex(i, _map.Width), "stone");
                _map.SetTerrain(wall);
            }
        }

        /// <summary>
        /// Returns a list of points expressing the perimeter of a rectangle
        /// </summary>
        /// <param name="room"></param>
        /// <returns></returns>
        protected List<Point> GetBorderCellLocations(Rectangle room)
        {
            //establish room boundaries
            int xMin = room.ToMonoRectangle().Left;
            int xMax = room.ToMonoRectangle().Right;
            int yMin = room.ToMonoRectangle().Bottom;
            int yMax = room.ToMonoRectangle().Top;

            // build a list of room border cells using a series of
            // straight lines
            List<Point> borderCells = GetTileLocationsAlongLine(xMin, yMin, xMax, yMin).ToList();
            borderCells.AddRange(GetTileLocationsAlongLine(xMin, yMin, xMin, yMax));
            borderCells.AddRange(GetTileLocationsAlongLine(xMin, yMax, xMax, yMax));
            borderCells.AddRange(GetTileLocationsAlongLine(xMax, yMin, xMax, yMax));

            return borderCells;
        }

        /// <summary>
        /// Place magic nodes at random location of the map
        /// </summary>
        /// <param name="nodes"></param>
        protected void PlaceNodes(int nodes)
        {
            for (int i = 0; i < nodes; i++)
            {
                int rnd = randNum.NextInt(_map.Tiles.Length);

                TileBase rndTile = _map.GetTerrainAt<TileBase>(Point.FromIndex(rnd, _map.Width));

                int rndMp = randNum.NextInt(1, 15);

                TileBase nodeTile = new NodeTile
                    (Color.Purple, Color.Transparent, Point.FromIndex(rnd, _map.Width), rndMp,
                    (int)NodeStrength.Normal);

                nodeTile.GoRogueComponents.Add(new Components.IllusionComponent(rndTile),
                    Components.IllusionComponent.Tag);

                _map.SetTerrain(nodeTile);
            }
        }

        /// <summary>
        /// returns a collection of Points which represent
        /// locations along a line
        /// </summary>
        /// <param name="xOrigin"></param>
        /// <param name="yOrigin"></param>
        /// <param name="xDestination"></param>
        /// <param name="yDestination"></param>
        /// <returns></returns>
        protected IEnumerable<Point> GetTileLocationsAlongLine
            (int xOrigin, int yOrigin, int xDestination, int yDestination)
        {
            // prevent line from overflowing
            // boundaries of the map
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
        /// sets X coordinate between right and left edges of map
        /// to prevent any out-of-bounds errors
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        protected int ClampX(int x)
        {
            if (x < 0)
                x = 0;
            else if (x > _map.Width - 1)
                x = _map.Width - 1;
            return x;

            // OR using ternary conditional operators: return (x < 0) ? 0 : (x > _map.Width - 1) ? _map.Width - 1 : x;
        }

        /// <summary>
        /// sets Y coordinate between top and bottom edges of map
        /// to prevent any out-of-bounds errors
        /// </summary>
        /// <param name="y"></param>
        /// <returns></returns>
        protected int ClampY(int y)
        {
            if (y < 0)
                y = 0;
            else if (y > _map.Height - 1)
                y = _map.Height - 1;
            return y;

            // OR using ternary conditional operators: return (y < 0) ? 0 : (y > _map.Height - 1) ? _map.Height - 1 : y;
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
        /// Determines if a Point on the map is a good
        /// candidate for a door.
        /// Returns true if it's a good spot for a door
        /// Returns false if there is a Tile that IsBlockingMove=true
        /// at that location
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

            // first make sure that isn't trying to take a door
            // off the limits of the map
            if (_map.CheckForIndexOutOfBounds(location))
                return false;

            //store references to all neighbouring cells
            GetAllNeighourTiles(location, out Point right,
                out Point left, out Point top, out Point bottom);

            if (_map.CheckForIndexOutOfBounds(top) || _map.CheckForIndexOutOfBounds(bottom)
                || _map.CheckForIndexOutOfBounds(left) || _map.CheckForIndexOutOfBounds(right))
                return false;

            // check to see if there is a door already in the target
            // location, or above/below/right/left of the target location
            // If it detects a door there, return false.
            if (_map.GetTileAt<TileDoor>(location.X, location.Y) != null ||
                _map.GetTileAt<TileDoor>(right.X, right.Y) != null ||
                _map.GetTileAt<TileDoor>(left.X, left.Y) != null ||
                _map.GetTileAt<TileDoor>(top.X, top.Y) != null ||
                _map.GetTileAt<TileDoor>(bottom.X, bottom.Y) != null)
            {
                return false;
            }

            //if all the prior checks are okay, make sure that the door is placed along a horizontal wall
            if (!_map.Tiles[right.ToIndex(_map.Width)].IsBlockingMove &&
                !_map.Tiles[left.ToIndex(_map.Width)].IsBlockingMove &&
                _map.Tiles[top.ToIndex(_map.Width)].IsBlockingMove &&
                _map.Tiles[bottom.ToIndex(_map.Width)].IsBlockingMove)
            {
                return true;
            }

            //or make sure that the door is placed along a vertical wall
            if (_map.Tiles[right.ToIndex(_map.Width)].IsBlockingMove &&
                _map.Tiles[left.ToIndex(_map.Width)].IsBlockingMove &&
                !_map.Tiles[top.ToIndex(_map.Width)].IsBlockingMove &&
                !_map.Tiles[bottom.ToIndex(_map.Width)].IsBlockingMove)
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
        /// Tries to create a TileDoor object in a specified Rectangle
        /// perimeter. Reads through the entire list of tiles comprising
        /// the perimeter, and determines if each position is a viable
        /// candidate for a door.
        /// When it finds a potential position, creates a closed and
        /// unlocked door.
        /// </summary>
        /// <param name="room"></param>
        /// <param name="acceptsMoreThanOneDoor"></param>
        protected void CreateDoor(Room room, bool acceptsMoreThanOneDoor = false)
        {
            List<Point> borderCells = GetBorderCellLocations(room.RoomRectangle);
            bool alreadyHasDoor = false;

            //go through every border cell and look for potential door candidates
            foreach (Point location in borderCells)
            {
                //int locationIndex = location.ToIndex(_map.Width);
                if (IsPotentialDoor(location) && !alreadyHasDoor)
                {
                    // Create a new door that is closed and unlocked.
                    TileDoor newDoor = new(false, false, location, "wood");
                    _map.SetTerrain(newDoor);
                    if (!acceptsMoreThanOneDoor)
                        alreadyHasDoor = true;
                    room.Doors.Add(newDoor);
                }
            }
        }

        /// <summary>
        /// Places trees inside the map, in a random like manner
        /// Sadly only places one type of tree
        /// </summary>
        /// <param name="map"></param>
        /// <param name="treeToPlace"></param>
        /// <exception cref="NotImplementedException"></exception>
        protected void PlaceTrees(Map map, TileWall treeToPlace)
        {
            for (int i = 0; i < map.Tiles.Length; i++)
            {
                TileWall tree = treeToPlace.Copy();
                Point pos = Point.FromIndex(i, map.Width);
                int rng = randNum.NextInt(0, 15);
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
        protected void PlaceGenericTrees(Map map)
        {
            PlaceTrees(map, TileEncyclopedia.GenericTree());
        }

        /// <summary>
        /// Place generic trees over the map
        /// </summary>
        /// <param name="map"></param>
        protected void PlaceGenericTrees()
        {
            PlaceTrees(_map, TileEncyclopedia.GenericTree());
        }

        /// <summary>
        /// Places a list of trees inside the map, varying randomly
        /// </summary>
        /// <param name="map"></param>
        /// <param name="treesToPlace"></param>
        /// <exception cref="NotImplementedException"></exception>
        protected void PlaceTrees(Map map, List<TileWall> treesToPlace)
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
        protected void PlaceVegetations(Map map, TileFloor vegetationToPlace)
        {
            //first make sure that the vegetation won't be attackable and it's a vegetation
            for (int i = 0; i < map.Tiles.Length; i++)
            {
                /*TileFloor vegetal = new
                    TileFloor(vegetationToPlace.Name, vegetationToPlace.Position,
                    vegetationToPlace.MaterialOfTile.Id, vegetationToPlace.Glyph,
                    vegetationToPlace.Foreground, vegetationToPlace.Background);*/
                TileFloor vegetal = vegetationToPlace.Copy();
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
        protected void PlaceVegetations(Map map, List<TileFloor> vegetationsToPlace)
        {
            //first make sure that the vegetation won't be attackable and it's a vegetation
            for (int i = 0; i < vegetationsToPlace.Count; i++)
            {
                TileFloor vegetal = vegetationsToPlace[randNum.NextInt(vegetationsToPlace.Count + 1)];

                PlaceVegetations(map, vegetal);
            }
        }

        /// <summary>
        /// Flood the specifide map with water
        /// </summary>
        /// <param name="tile"></param>
        /// <param name="map"></param>
        protected static void FloodWithWaterMap(WaterTile tile, Map map)
        {
            map.SetTerrain(tile);
        }

        protected void FloodWithWaterMap(WaterTile tile)
        {
            _map.SetTerrain(tile);
        }

        /// <summary>
        /// Puts a tile in a random mode in the specified map
        /// </summary>
        /// <param name="map"></param>
        /// <param name="template"></param>
        protected void PutRngFloorTileThere(Map map, TileFloor template)
        {
            for (int i = 0; i < map.Tiles.Length; i++)
            {
                int rng = randNum.NextInt(0, 15);
                Point pos = map.Tiles[i].Position;
                if (rng == 13)
                {
                    TileFloor newTile = new TileFloor(template.Name, pos,
                        template.MaterialOfTile.Id, template.Glyph,
                        template.Foreground, template.Background);
                    map.SetTerrain(newTile);
                }
            }
        }

        /// <summary>
        /// Puts a tile in a random mode in the map
        /// </summary>
        /// <param name="template"></param>
        protected void PutRngFloorTileThere(TileFloor template)
        {
            for (int i = 0; i < _map.Tiles.Length; i++)
            {
                int rng = randNum.NextInt(0, 15);
                Point pos = _map.Tiles[i].Position;
                if (rng == 13)
                {
                    TileFloor newTile = new TileFloor(template.Name, pos,
                        template.MaterialOfTile.Id, template.Glyph,
                        template.Foreground, template.Background);
                    _map.SetTerrain(newTile);
                }
            }
        }

        protected static bool CheckIfPointIsCorrect(Simple2DGrid grid, Point point)
        {
            return point.X < grid.Width && point.X >= 0 && point.Y < grid.Height && point.Y >= 0;
        }

        public static void ConnectMapsInsideChunk(Map[] maps)
        {
            int len = maps.Length;
            // perfect for flood fill!
            Stack<Point> pointedList = new Stack<Point>();
            int desiredSize = 3;
            Simple2DGrid grid = new Simple2DGrid(len / desiredSize, len / desiredSize, new());
            pointedList.Push(new SadRogue.Primitives.Point(0, 0));
            while (pointedList.Count > 0)
            {
                pointedList.TryPop(out Point currentPoint);
                if (CheckIfPointIsCorrect(grid, currentPoint))
                {
                    int idx = Point.ToIndex(currentPoint.X, currentPoint.Y, grid.Width);
                    Map map = maps[idx];

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

        private static void ConnectPointsWithMaps(Map[] maps, Stack<Point> pointedList, Simple2DGrid grid,
            Point currentPoint, Map map, Point nextPoint)
        {
            int endIdx = Point.ToIndex(nextPoint.X, nextPoint.Y, grid.Width);
            Direction di = Direction.GetDirection(currentPoint, nextPoint);
            pointedList.Push(nextPoint);
            map.MapZoneConnections.Add(di, maps[endIdx]);
        }

        #endregion GeneralMapGen

        #region MapGenAlgorithms

        #region BSPGen

        protected List<Room> BspMapFunction(Map map, int roomMaxSize, int roomMinSize, int maxRooms)
        {
            List<Room> rooms = new List<Room>();

            Rectangle rec = new Rectangle(map.Positions().First(), map.Positions().Last());

            List<Rectangle> mapPartitions =
                BisectRecursiveRandom(rec, roomMaxSize).ToList();

            for (int i = 0; i <= maxRooms; i++)
            {
                if (mapPartitions.Count <= 0) break;
                Rectangle area = mapPartitions[randNum.NextInt(mapPartitions.Count)];
                mapPartitions.Remove(area);

                if (area.Width <= roomMinSize && area.Height <= roomMinSize)
                    continue;

                int newRoomWidht = randNum.NextInt(roomMinSize, area.Width - 1);
                int newRoomHeight = randNum.NextInt(roomMinSize, area.Height - 1);

                int newRoomX = randNum.NextInt(1, map.Width - newRoomWidht);
                int newRoomY = randNum.NextInt(1, map.Height - newRoomHeight);

                Rectangle rectangle = new Rectangle(newRoomX, newRoomY, newRoomWidht, newRoomHeight);
                //Rectangle rectangle = new Rectangle(area.X, area.Y, area.Width, area.Height);
                Room room = new Room(rectangle);

                if (!rooms.Any(r => r.RoomRectangle.Intersects(room.RoomRectangle)))
                {
                    rooms.Add(room);
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
    }

    public class CityGenerator : MapGenerator
    {
        public CityGenerator() : base()
        {
            // empty one
        }

        public void GenerateTownFromMapBSP(Map map, int maxRooms,
            int minRoomSize, int maxRoomSize, string townName)
        {
            _map = map;

            List<Room> rooms = BspMapFunction(map, maxRoomSize, minRoomSize, maxRooms);

            map.MapName = townName;

            foreach (var room in rooms)
            {
                CreateRoom(room.RoomRectangle,
                    TileEncyclopedia.GenericStoneWall(), TileEncyclopedia.GenericStoneFloor());
                CreateDoor(room);
                room.LockedDoorsRng();
                _rooms.Add(room);
            }

            ApplyRoads(rooms, TileEncyclopedia.GenericDirtRoad(Point.None));
        }
    }

    public class WildernessGenerator : MapGenerator
    {
        #region PlanetMapGenStuff

        public WildernessGenerator()
        {
            //Empty const
        }

        public Map[] GenerateMapWithWorldParam(PlanetMap worldMap, Point posGenerated)
        {
            Map[] maps = new Map[RegionChunk.MAX_LOCAL_MAPS];
            WorldTile worldTile = worldMap.AssocietatedMap.GetTileAt<WorldTile>(posGenerated);

            for (int i = 0; i < maps.Length; i++)
            {
                Map completeMap = DetermineBiomeLookForTile(worldTile);
                if (completeMap is not null)
                {
                    ApplyModifierToTheMap(completeMap, worldTile, (uint)i);
                    FinishingTouches(completeMap, worldTile);
                    maps[i] = completeMap;
                }
                else
                {
                    throw new Exception("Map was null when generating for the chunck!");
                }
            }

            ConnectMapsInsideChunk(maps);

            return maps;
        }

        /// <summary>
        /// Tweking the final result to be "better"
        /// </summary>
        /// <param name="completeMap"></param>
        /// <param name="worldTile"></param>
        private static void FinishingTouches(Map completeMap, WorldTile worldTile)
        {
        }

        /// <summary>
        /// If the map has any modifer, like strong magic aura, cities, roads and particulaly civs
        /// \nAnything that changes the composition of the World Tile map.
        /// Trees also spawn here
        /// </summary>
        /// <param name="completeMap"></param>
        /// <param name="worldTile"></param>
        /// <param name="magicI">Serves to add unique seeds to the map</param>
        private void ApplyModifierToTheMap(Map completeMap, WorldTile worldTile, ulong magicI)
        {
            completeMap.SetSeed((uint)seed, (uint)worldTile.Position.X,
                (uint)worldTile.Position.Y, (uint)magicI);
            // investigate later if it's making it possible to properly reproduce a map
            randNum = new((ulong)completeMap.Seed);

            switch (worldTile.BiomeType)
            {
                case BiomeType.Sea:
                    return; // No modifer to apply here

                case BiomeType.Desert:
                    PlaceVegetations(completeMap,
                        new TileFloor("Cactus", Point.None,
                        "grass", 198, Color.Green, Color.Black));
                    break;

                case BiomeType.Savanna:
                    PlaceGenericTrees(completeMap);
                    break;

                case BiomeType.TropicalRainforest:
                    PlaceGenericTrees(completeMap);
                    break;

                case BiomeType.Grassland:
                    PlaceVegetations(completeMap,
                        new TileFloor("Shrub", Point.None,
                            "grass", '"', Color.Green, Color.Black));
                    break;

                case BiomeType.Woodland:
                    PlaceGenericTrees(completeMap);
                    break;

                case BiomeType.SeasonalForest:
                    PlaceGenericTrees(completeMap);

                    break;

                case BiomeType.TemperateRainforest:
                    PlaceGenericTrees(completeMap);

                    break;

                case BiomeType.BorealForest:
                    PlaceGenericTrees(completeMap);
                    break;

                case BiomeType.Tundra:
                    PlaceGenericTrees(completeMap);
                    break;

                case BiomeType.Ice:
                    break; // null as well

                case BiomeType.Mountain:
                    PutRngFloorTileThere(completeMap, TileEncyclopedia.GenericGrass(Point.None));
                    break;

                default:
                    break;
            }
            if (worldTile.CivInfluence is not null)
            {
                CityGenerator city = new();
                city.GenerateTownFromMapBSP(completeMap,
                    randNum.NextInt(6, 23), randNum.NextInt(4, 7), randNum.NextInt(8, 12), "Test Town");
            }
            if (worldTile.Rivers.Count > 0)
            {
            }
            if (worldTile.Road is not null)
            {
            }
            if (worldTile.MagicalAuraStrength > 7)
            {
            }
            /*switch (worldTile.SpecialLandType)
            {
                case SpecialLandType.None:
                    // nothing here
                    break;

                case SpecialLandType.MagicLand:
                    // funky shit full of interisting stuff
                    break;

                default:
                    break;
            }*/
        }

        private static Map DetermineBiomeLookForTile(WorldTile worldTile)
        {
            Map map = worldTile.BiomeType switch
            {
                BiomeType.Sea => GenericSeaMap(worldTile),
                BiomeType.Desert => GenericDesertMap(worldTile),
                BiomeType.Savanna => GenericSavannaMap(worldTile),
                BiomeType.TropicalRainforest => GenericTropicalRainforest(worldTile),
                BiomeType.Grassland => GenericGrassland(worldTile),
                BiomeType.Woodland => GenericWoodLands(worldTile),
                BiomeType.SeasonalForest => GenericSeasonalForests(worldTile),
                BiomeType.TemperateRainforest => GenericTemperateRainForest(worldTile),
                BiomeType.BorealForest => GenericBorealForest(worldTile),
                BiomeType.Tundra => GenericTundra(worldTile),
                BiomeType.Ice => GenericIceMap(worldTile),
                BiomeType.Mountain => GenericMountainMap(worldTile),
                _ => throw new Exception("Cound't find the biome to generate a map!"),
            };
            return map;
        }

        #region BiomeMaps

        private static Map GenericMountainMap(WorldTile worldTile)
        {
            Map map = new Map($"{worldTile.BiomeType}");

            for (int i = 0; i < map.Tiles.Length; i++)
            {
                Point pos = Point.FromIndex(i, map.Width);
                TileFloor tile = new TileFloor(pos);
                PrepareForAnyFloor(tile, map);
            }
            return map;
        }

        private static Map GenericIceMap(WorldTile worldTile)
        {
            Map map = new Map($"{worldTile.BiomeType}");
            for (int i = 0; i < map.Tiles.Length; i++)
            {
                Point pos = Point.FromIndex(i, map.Width);
                TileFloor tile = (TileFloor)DataManager.QueryTileInData("Snow");
                tile.Position = pos;
                PrepareForAnyFloor(tile, map);
            }

            return map;
        }

        private static Map GenericTundra(WorldTile worldTile)
        {
            Map map = new Map($"{worldTile.BiomeType}");
            for (int i = 0; i < map.Tiles.Length; i++)
            {
                Point pos = Point.FromIndex(i, map.Width);
                TileFloor tile = TileEncyclopedia.GenericGrass(pos);
                PrepareForAnyFloor(tile, map);
            }

            return map;
        }

        private static Map GenericBorealForest(WorldTile worldTile)
        {
            Map map = new Map($"{worldTile.BiomeType}");

            for (int i = 0; i < map.Tiles.Length; i++)
            {
                Point pos = Point.FromIndex(i, map.Width);
                TileFloor tile = TileEncyclopedia.GenericGrass(pos);
                PrepareForAnyFloor(tile, map);
            }

            return map;
        }

        private static Map GenericTemperateRainForest(WorldTile worldTile)
        {
            Map map = new Map($"{worldTile.BiomeType}");

            for (int i = 0; i < map.Tiles.Length; i++)
            {
                Point pos = Point.FromIndex(i, map.Width);
                TileFloor tile = TileEncyclopedia.GenericGrass(pos);
                PrepareForAnyFloor(tile, map);
            }

            return map;
        }

        private static Map GenericSeasonalForests(WorldTile worldTile)
        {
            Map map = new Map($"{worldTile.BiomeType}");

            for (int i = 0; i < map.Tiles.Length; i++)
            {
                Point pos = Point.FromIndex(i, map.Width);
                TileFloor tile = TileEncyclopedia.GenericGrass(pos);
                PrepareForAnyFloor(tile, map);
            }

            return map;
        }

        private static Map GenericWoodLands(WorldTile worldTile)
        {
            Map map = new Map($"{worldTile.BiomeType}");

            for (int i = 0; i < map.Tiles.Length; i++)
            {
                Point pos = Point.FromIndex(i, map.Width);
                TileFloor tile = TileEncyclopedia.GenericGrass(pos);
                PrepareForAnyFloor(tile, map);
            }

            return map;
        }

        private static Map GenericGrassland(WorldTile worldTile)
        {
            Map map = new Map($"{worldTile.BiomeType}");

            for (int i = 0; i < map.Tiles.Length; i++)
            {
                Point pos = Point.FromIndex(i, map.Width);
                TileFloor tile = TileEncyclopedia.GenericGrass(pos);
                PrepareForAnyFloor(tile, map);
            }

            return map;
        }

        private static Map GenericTropicalRainforest(WorldTile worldTile)
        {
            Map map = new Map($"{worldTile.BiomeType}");

            for (int i = 0; i < map.Tiles.Length; i++)
            {
                Point pos = Point.FromIndex(i, map.Width);
                TileFloor tile = TileEncyclopedia.GenericGrass(pos);
                PrepareForAnyFloor(tile, map);
            }

            return map;
        }

        private static Map GenericSavannaMap(WorldTile worldTile)
        {
            Map map = new Map($"{worldTile.BiomeType}");

            for (int i = 0; i < map.Tiles.Length; i++)
            {
                Point pos = Point.FromIndex(i, map.Width);
                TileFloor tile = TileEncyclopedia.GenericGrass(pos);
                PrepareForAnyFloor(tile, map);
            }

            return map;
        }

        private static Map GenericDesertMap(WorldTile worldTile)
        {
            Map map = new Map($"{worldTile.BiomeType}");

            for (int i = 0; i < map.Tiles.Length; i++)
            {
                Point pos = Point.FromIndex(i, map.Width);
                TileFloor tile = new TileFloor("Sand", pos, "sand", worldTile.Glyph,
                    worldTile.Foreground, Color.Transparent);
                PrepareForAnyFloor(tile, map);
            }

            return map;
        }

        private static Map GenericSeaMap(WorldTile worldTile)
        {
            Map map = new Map($"{worldTile.BiomeType}");

            for (int i = 0; i < map.Tiles.Length; i++)
            {
                Point pos = Point.FromIndex(i, map.Width);
                WaterTile tile = WaterTile.NormalSeaWater(pos);
                FloodWithWaterMap(tile, map);
            }

            return map;
        }

        #endregion BiomeMaps

        #endregion PlanetMapGenStuff
    }

    // based on tunnelling room generation algorithm
    // from RogueSharp tutorial
    // https://roguesharp.wordpress.com/2016/03/26/roguesharp-v3-tutorial-simple-room-generation/
    public class DungeonGenerator : MapGenerator
    {
        public DungeonGenerator()
        {
            // empty
        }

        public Map GenerateMazeMap(int maxRooms, int minRoomSize, int maxRoomSize)
        {
            // Create an empty map of size (mapWidht * mapHeight)
            _map = new Map("Maze");

            // store a list of the rooms created so far
            List<Room> Rooms = new();

            // create up to (maxRooms) rooms on the map
            // and make sure the rooms do not overlap with each other
            for (int i = 0; i < maxRooms; i++)
            {
                // set the room's (width, height) as a random size between (minRoomSize, maxRoomSize)
                int newRoomWidth = randNum.NextInt(minRoomSize, maxRoomSize);
                int newRoomHeigth = randNum.NextInt(minRoomSize, maxRoomSize);

                // sets the room's X/Y Position at a random point between the edges of the map
                int newRoomX = randNum.NextInt(0, _map.Width - newRoomWidth - 1);
                int newRoomY = randNum.NextInt(0, _map.Height - newRoomHeigth - 1);

                // create a Rectangle representing the room's perimeter
                Rectangle roomRectangle = new Rectangle(newRoomX, newRoomY, newRoomWidth, newRoomHeigth);
                Room newRoom = new(roomRectangle);

                // Does the new room intersect with other rooms already generated?
                bool doesRoomIntersect = Rooms.Any(room => newRoom.RoomRectangle.Intersects(room.RoomRectangle));

                if (!doesRoomIntersect)
                    Rooms.Add(newRoom);
            }

            // This is a dungeon, so begin by flooding the map with walls.
            FloodWalls();

            // carve out rooms for every room in the Rooms list
            foreach (var room in Rooms)
            {
                CreateRoom(room.RoomRectangle,
                    TileEncyclopedia.GenericStoneWall(), TileEncyclopedia.GenericStoneFloor());
            }

            // carve out tunnels between all rooms
            // based on the Positions of their centers
            for (int r = 1; r < Rooms.Count; r++)
            {
                //for all remaining rooms get the center of the room and the previous room
                Point previousRoomCenter = Rooms[r - 1].RoomRectangle.Center;
                Point currentRoomCenter = Rooms[r].RoomRectangle.Center;

                // give a 50/50 chance of which 'L' shaped connecting hallway to tunnel out
                if (randNum.NextInt(1, 2) == 1)
                {
                    CreateHorizontalTunnel(previousRoomCenter.X, currentRoomCenter.X, previousRoomCenter.Y);
                    CreateVerticalTunnel(previousRoomCenter.Y, currentRoomCenter.Y, previousRoomCenter.X);
                }
                else
                {
                    CreateVerticalTunnel(previousRoomCenter.Y, currentRoomCenter.Y, previousRoomCenter.X);
                    CreateHorizontalTunnel(previousRoomCenter.X, currentRoomCenter.X, previousRoomCenter.Y);
                }
            }

            // Create doors now that the tunnels have been carved out
            foreach (var room in Rooms)
                CreateDoor(room);

            PlaceNodes(10);

            // spit out the final map
            return _map;
        }
    }

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