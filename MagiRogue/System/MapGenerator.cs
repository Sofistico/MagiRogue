﻿using MagiRogue.Data;
using MagiRogue.Entities;
using MagiRogue.System.Planet;
using MagiRogue.System.Tiles;
using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MagiRogue.System
{
    // based on tunnelling room generation algorithm
    // from RogueSharp tutorial
    // https://roguesharp.wordpress.com/2016/03/26/roguesharp-v3-tutorial-simple-room-generation/
    // TODO: Refactor this whole class
    public class MapGenerator
    {
        protected readonly Random randNum = new();
        protected Map _map; // Temporarily store the map currently worked on

        // Empty constructor
        public MapGenerator()
        {
        }

        public MapGenerator(Map map) => _map = map;

        #region GeneralMapGen

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
                int newRoomWidth = randNum.Next(minRoomSize, maxRoomSize);
                int newRoomHeigth = randNum.Next(minRoomSize, maxRoomSize);

                // sets the room's X/Y Position at a random point between the edges of the map
                int newRoomX = randNum.Next(0, _map.Width - newRoomWidth - 1);
                int newRoomY = randNum.Next(0, _map.Height - newRoomHeigth - 1);

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
                CreateRoom(room.RoomRectangle);
            }

            // carve out tunnels between all rooms
            // based on the Positions of their centers
            for (int r = 1; r < Rooms.Count; r++)
            {
                //for all remaining rooms get the center of the room and the previous room
                Point previousRoomCenter = Rooms[r - 1].RoomRectangle.Center;
                Point currentRoomCenter = Rooms[r].RoomRectangle.Center;

                // give a 50/50 chance of which 'L' shaped connecting hallway to tunnel out
                if (randNum.Next(1, 2) == 1)
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

        protected void InsertStairs(Room room)
        {
            Furniture stairsDown = new Furniture(Color.White, Color.Black, '>', room.RoomRectangle.Center,
                (int)MapLayer.FURNITURE, FurnitureType.StairsDown);

            // debug to unlock the room
            //room.ForceUnlock();

            _map.Add(stairsDown);
        }

        protected void PrepareForFloors()
        {
            foreach (var pos in _map.Positions())
            {
                _map.SetTerrain(new TileFloor(pos, "stone"));
            }
        }

        protected void PrepareForFloorsWithGrass()
        {
            foreach (var pos in _map.Positions())
            {
                _map.SetTerrain(TileEncyclopedia.ShortGrass(pos));
            }
        }

        protected void PrepareForAnyFloor(TileFloor floor)
        {
            foreach (var pos in _map.Positions())
            {
                floor.Position = pos;
                _map.SetTerrain(floor);
            }
        }

        protected void PrepareForAnyFloor(TileFloor floor, Map map)
        {
            for (int i = 0; i < map.Tiles.Length; i++)
            {
                Point pos = Point.FromIndex(i, map.Width);
                floor.Position = pos;
                map.SetTerrain(floor);
            }
        }

        protected void ConnectWithRoads(Point start, Point end)
        {
        }

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

        // Builds a room composed of walls and floors using the supplied Rectangle
        // which determines its size and position on the map
        // Walls are placed at the perimeter of the room
        // Floors are placed in the interior area of the room
        protected void CreateRoom(Rectangle room)
        {
            // Place floors in interior area
            for (int x = room.ToMonoRectangle().Left + 1; x < room.ToMonoRectangle().Right; x++)
            {
                for (int y = room.ToMonoRectangle().Top + 1; y < room.ToMonoRectangle().Bottom; y++)
                {
                    CreateStoneFloor(new Point(x, y));
                }
            }

            // Place walls at perimeter
            List<Point> perimeter = GetBorderCellLocations(room);

            foreach (Point location in perimeter)
            {
                CreateStoneWall(location);
            }
        }

        // Creates a Floor tile at the specified X/Y location
        protected void CreateStoneFloor(Point location)
        {
            TileFloor floor = new TileFloor(location);

            // a simple setterrain already does it for me
            _map.SetTerrain(floor);
        }

        // Creates a Wall tile at the specified X/Y location
        protected void CreateStoneWall(Point location)
        {
            TileWall wall = new TileWall(location);
            _map.SetTerrain(wall);
        }

        protected void CreateAnyWall(TileWall wall) => _map.SetTerrain(wall);

        protected void CreateAnyWall(TileWall wall, Map map)
        {
            map.SetTerrain(wall);
        }

        // Fills the map with walls
        protected void FloodWalls()
        {
            for (int i = 0; i < _map.Tiles.Length; i++)
            {
                TileWall wall = new TileWall(Point.FromIndex(i, _map.Width), "stone");
                _map.SetTerrain(wall);
            }
        }

        // Returns a list of points expressing the perimeter of a rectangle
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

        protected void PlaceNodes(int nodes)
        {
            for (int i = 0; i < nodes; i++)
            {
                int rnd = randNum.Next(_map.Tiles.Length);

                TileBase rndTile = _map.GetTerrainAt<TileBase>(Point.FromIndex(rnd, _map.Width));

                int rndMp = randNum.Next(1, 15);

                TileBase nodeTile = new NodeTile
                    (Color.Purple, Color.Transparent, Point.FromIndex(rnd, _map.Width), rndMp,
                    (int)NodeStrength.Normal);

                nodeTile.GoRogueComponents.Add(new Components.IllusionComponent(rndTile),
                    Components.IllusionComponent.Tag);

                _map.SetTerrain(nodeTile);
            }
        }

        // returns a collection of Points which represent
        // locations along a line
        public IEnumerable<Point> GetTileLocationsAlongLine
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

        // sets X coordinate between right and left edges of map
        // to prevent any out-of-bounds errors
        protected int ClampX(int x)
        {
            if (x < 0)
                x = 0;
            else if (x > _map.Width - 1)
                x = _map.Width - 1;
            return x;

            // OR using ternary conditional operators: return (x < 0) ? 0 : (x > _map.Width - 1) ? _map.Width - 1 : x;
        }

        // sets Y coordinate between top and bottom edges of map
        // to prevent any out-of-bounds errors
        protected int ClampY(int y)
        {
            if (y < 0)
                y = 0;
            else if (y > _map.Height - 1)
                y = _map.Height - 1;
            return y;

            // OR using ternary conditional operators: return (y < 0) ? 0 : (y > _map.Height - 1) ? _map.Height - 1 : y;
        }

        // carve a tunnel out of the map parallel to the x-axis
        protected void CreateHorizontalTunnel(int xStart, int xEnd, int yPosition)
        {
            for (int x = Math.Min(xStart, xEnd); x <= Math.Max(xStart, xEnd); x++)
            {
                CreateStoneFloor(new Point(x, yPosition));
            }
        }

        // carve a tunnel using the y-axis
        protected void CreateVerticalTunnel(int yStart, int yEnd, int xPosition)
        {
            for (int y = Math.Min(yStart, yEnd); y <= Math.Max(yStart, yEnd); y++)
            {
                CreateStoneFloor(new Point(xPosition, y));
            }
        }

        // Determines if a Point on the map is a good
        // candidate for a door.
        // Returns true if it's a good spot for a door
        // Returns false if there is a Tile that IsBlockingMove=true
        // at that location
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
            Point right = new Point(location.X + 1, location.Y);
            Point left = new Point(location.X - 1, location.Y);
            Point top = new Point(location.X, location.Y - 1);
            Point bottom = new Point(location.X, location.Y + 1);

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
                !_map.Tiles[top.ToIndex(_map.Width)].IsBlockingMove &&
                !_map.Tiles[bottom.ToIndex(_map.Width)].IsBlockingMove)
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

        //Tries to create a TileDoor object in a specified Rectangle
        //perimeter. Reads through the entire list of tiles comprising
        //the perimeter, and determines if each position is a viable
        //candidate for a door.
        //When it finds a potential position, creates a closed and
        //unlocked door.
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
                    TileDoor newDoor = new(false, false, location, "stone");
                    _map.SetTerrain(newDoor);
                    if (!acceptsMoreThanOneDoor)
                        alreadyHasDoor = true;
                    room.Doors.Add(newDoor);
                }
            }
        }

        #endregion GeneralMapGen
    }

    public class Room
    {
        public Rectangle RoomRectangle { get; set; }
        public RoomTag Tag { get; set; }
        public List<TileDoor> Doors { get; set; } = new List<TileDoor>();

        public Room(Rectangle roomRectangle, RoomTag tag)
        {
            RoomRectangle = roomRectangle;
            Tag = tag;
        }

        public Room(Rectangle roomRectangle)
        {
            RoomRectangle = roomRectangle;
            Tag = RoomTag.Generic;
        }

        public void LockDoorsRng()
        {
            int half = GoRogue.DiceNotation.Dice.Roll("1d2");

            if (half == 1)
            {
                int indexDoor = GoRogue.Random.GlobalRandom.DefaultRNG.Next(Doors.Count);

                Doors[indexDoor].Locked = true;
            }
        }

        internal void ForceUnlock()
        {
            foreach (var door in Doors)
            {
                door.Locked = false;
            }
        }
    }

    public class CityGenerator : MapGenerator
    {
        public CityGenerator() : base()
        {
            // empty one
        }

        public Map GenerateTownMap(int maxRooms, int minRoomSize, int maxRoomSize)
        {
            _map = new Map("Town of Salazar");

            PrepareForFloorsWithGrass();

            List<Room> rooms = new List<Room>();

            for (int i = 0; i < maxRooms; i++)
            {
                int newRoomWidht = randNum.Next(minRoomSize, maxRoomSize);
                int newRoomHeight = randNum.Next(minRoomSize, maxRoomSize);

                // sets the room's X/Y Position at a random point between the edges of the map
                int newRoomX = randNum.Next(0, _map.Width - newRoomWidht - 1);
                int newRoomY = randNum.Next(0, _map.Height - newRoomHeight - 1);

                Rectangle rectangle = new Rectangle(newRoomX, newRoomY, newRoomWidht, newRoomHeight);
                Room newRoom = new Room(rectangle);

                bool doesRoomIntersect = rooms.Any(room => newRoom.RoomRectangle.Intersects(room.RoomRectangle));

                if (!doesRoomIntersect)
                    rooms.Add(newRoom);
            }

            foreach (var room in rooms)
            {
                CreateRoom(room.RoomRectangle);
                CreateDoor(room);
                room.LockDoorsRng();
            }

            InsertStairs(rooms[0]);

            return _map;
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
            Map[] map = new Map[RegionChunk.MAX_LOCAL_MAPS];
            WorldTile worldTile = worldMap.AssocietatedMap.GetTileAt<WorldTile>(posGenerated);

            for (int i = 0; i < map.Length; i++)
            {
                Map completeMap = DetermineBiomeLookForTile(worldTile);
                if (completeMap is not null)
                {
                    ApplyModifierToTheMap(completeMap, worldTile);
                    FinishingTouches(completeMap, worldTile);
                    map[i] = completeMap;
                }
                else
                {
                    throw new Exception("Map was null when generating for the chunck!");
                }
            }

            return map;
        }

        /// <summary>
        /// Tweking the final result to be "better"
        /// </summary>
        /// <param name="completeMap"></param>
        /// <param name="worldTile"></param>
        private void FinishingTouches(Map completeMap, WorldTile worldTile)
        {
        }

        /// <summary>
        /// If the map has any modifer, like strong magic aura, cities, roads and particulaly civs
        /// \nAnything that changes the composition of the World Tile map.
        /// Trees also spawn here
        /// </summary>
        /// <param name="completeMap"></param>
        /// <param name="worldTile"></param>
        private void ApplyModifierToTheMap(Map completeMap, WorldTile worldTile)
        {
        }

        private Map DetermineBiomeLookForTile(WorldTile worldTile)
        {
            Map map = null;
            switch (worldTile.BiomeType)
            {
                case BiomeType.Sea:
                    map = GenericSeaMap(worldTile);
                    break;

                case BiomeType.Desert:
                    map = GenericDesertMap(worldTile);
                    break;

                case BiomeType.Savanna:
                    map = GenericSavannaMap(worldTile);
                    break;

                case BiomeType.TropicalRainforest:
                    map = GenericTropicalRainforest(worldTile);
                    break;

                case BiomeType.Grassland:
                    map = GenericGrassland(worldTile);
                    break;

                case BiomeType.Woodland:
                    map = GenericWoodLands(worldTile);
                    break;

                case BiomeType.SeasonalForest:
                    map = GenericSeasonalForests(worldTile);
                    break;

                case BiomeType.TemperateRainforest:
                    map = GenericTemperateRainForest(worldTile);
                    break;

                case BiomeType.BorealForest:
                    map = GenericBorealForest(worldTile);
                    break;

                case BiomeType.Tundra:
                    map = GenericTundra(worldTile);
                    break;

                case BiomeType.Ice:
                    map = GenericIceMap(worldTile);
                    break;

                default:
                    throw new Exception("Cound't find the biome to generate a map!");
            }
            return map;
        }

        private Map GenericIceMap(WorldTile worldTile)
        {
            Map map = new Map($"{worldTile.BiomeType}");
            for (int i = 0; i < map.Tiles.Length; i++)
            {
                Point pos = Point.FromIndex(i, map.Width);
                TileFloor tile = new TileFloor("Ice", pos, "ice", worldTile.Glyph,
                    worldTile.Foreground,
                    Color.Transparent);
                PrepareForAnyFloor(tile, map);
            }

            return map;
        }

        private Map GenericTundra(WorldTile worldTile)
        {
            Map map = new Map($"{worldTile.BiomeType}");

            return map;
        }

        private Map GenericBorealForest(WorldTile worldTile)
        {
            Map map = new Map($"{worldTile.BiomeType}");

            return map;
        }

        private Map GenericTemperateRainForest(WorldTile worldTile)
        {
            Map map = new Map($"{worldTile.BiomeType}");

            return map;
        }

        private Map GenericSeasonalForests(WorldTile worldTile)
        {
            Map map = new Map($"{worldTile.BiomeType}");

            return map;
        }

        private Map GenericWoodLands(WorldTile worldTile)
        {
            Map map = new Map($"{worldTile.BiomeType}");

            return map;
        }

        private Map GenericGrassland(WorldTile worldTile)
        {
            Map map = new Map($"{worldTile.BiomeType}");

            return map;
        }

        private Map GenericTropicalRainforest(WorldTile worldTile)
        {
            Map map = new Map($"{worldTile.BiomeType}");

            return map;
        }

        private Map GenericSavannaMap(WorldTile worldTile)
        {
            Map map = new Map($"{worldTile.BiomeType}");

            return map;
        }

        private Map GenericDesertMap(WorldTile worldTile)
        {
            Map map = new Map($"{worldTile.BiomeType}");

            return map;
        }

        private Map GenericSeaMap(WorldTile worldTile)
        {
            Map map = new Map($"{worldTile.BiomeType}");

            return map;
        }

        #endregion PlanetMapGenStuff
    }

    public enum RoomTag
    {
        Generic, Inn, Temple, Blacksmith, Clothier, Alchemist, PlayerHouse, Hovel, Abandoned
    }
}