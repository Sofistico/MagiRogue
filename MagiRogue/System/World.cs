using MagiRogue.Components;
using MagiRogue.Entities;
using MagiRogue.Data;
using MagiRogue.System.Tiles;
using MagiRogue.System.Time;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MagiRogue.System
{
    /// <summary>
    /// All game state data is stored in World
    /// also creates and processes generators
    /// for map creation
    /// </summary>
    public class World
    {
        // map creation and storage data
        private const int CHUNK_SIZE = 2500;
        private const int _mapWidth = 50;
        //private const int _mapHeight = 50;
        private readonly int _maxRooms = 20;
        private readonly int _minRoomSize = 4;
        private readonly int _maxRoomSize = 10;
        /*private const int _zMaxUpLevel = 10;
        private const int _zMaxLowLevel = -10;*/
        private readonly HashSet<Point> existingChunckPositions;
        private Dictionary<Point, MapChunk> localChunks;
        private readonly Dictionary<Point, RegionChunk> worldChunks;

        /// <summary>
        /// Stores the current map
        /// </summary>
        public MapChunk CurrentChunk { get; set; }

        public IReadOnlyCollection<Point> ExistingChunkPositions { get => existingChunckPositions; }

        // Player data
        public Player Player { get; set; }

        public TimeSystem Time { get; private set; }

        private readonly Random rndNum = new Random();

        /// <summary>
        /// Creates a new game world and stores it in a
        /// publicly accessible constructor.
        /// </summary>
        public World(Player player, bool testGame = false)
        {
            localChunks = new();
            worldChunks = new();
            existingChunckPositions = new HashSet<Point>();
            Time = new TimeSystem();
            if (!testGame)
            {
                // Build a map
                CreateMap();

                CreateAnotherChunkTest();

                // spawn a bunch of monsters
                /*CreateMonster();

                // Spawn a bunch of loot
                CreateLoot();*/

                // create an instance of player
                PlacePlayer(player);

                // Set up anything that needs to be set up for the world to work
                SetUpStuff();
            }
            else
            {
                CreateTestMap();

                PlacePlayer(player);
            }
        }

        private void CreateAnotherChunkTest()
        {
            var map = new MapGenerator().GenerateStoneFloorMap();

            AddChunkToDictionary(map);
        }

        /// <summary>
        /// Sets up anything that needs to be set up after map gen and after placing entities, like the nodes turn
        /// system
        /// </summary>
        private void SetUpStuff()
        {
            foreach (NodeTile node in CurrentChunk.Map.Tiles.OfType<NodeTile>())
            {
                node.SetUpNodeTurn(this);
            }
        }

        // Create a new map using the Map class
        // and a map generator. Uses several
        // parameters to determine geometry
        private void CreateMap()
        {
            MapGenerator mapGen = new();
            var map = mapGen.GenerateTownMap(_maxRooms, _minRoomSize, _maxRoomSize);
            AddChunkToDictionary(map);
        }

        private void CreateTestMap()
        {
            MapGenerator mapGen = new();
            var map = mapGen.GenerateTestMap();
            AddChunkToDictionary(map);
        }

        // Create a player using the Player class
        // and set its starting position
        private void PlacePlayer(Player player)
        {
            // Place the player on the first non-movement-blocking tile on the map
            for (int i = 0; i < CurrentChunk.Map.Tiles.Length; i++)
            {
                if (!CurrentChunk.Map.Tiles[i].IsBlockingMove && CurrentChunk.Map.Tiles[i] is not NodeTile
                    && !CurrentChunk.Map.GetEntitiesAt<Entity>(Point.FromIndex(i, _mapWidth)).Any())
                {
                    // Set the player's position to the index of the current map position
                    var pos = Point.FromIndex(i, CurrentChunk.Map.Width);

                    Player = player;
                    Player.Position = pos;
                    Player.Description = "Here is you, you are beautiful";

                    break;
                }
            }

            // add the player to the Map's collection of Entities
            CurrentChunk.Map.Add(Player);
        }

        // Create some random monsters with random attack and defense values
        // and drop them all over the map in
        // random places.
        private void CreateMonster()
        {
            // number of monsters to create
            int numMonster = 10;

            // Create several monsters and
            // pick a random position on the map to place them.
            // check if the placement spot is blocking (e.g. a wall)
            // and if it is, try a new position
            for (int i = 0; i < numMonster; i++)
            {
                int monsterPosition = 0;
                while (CurrentChunk.Map.Tiles[monsterPosition].IsBlockingMove)
                {
                    // pick a random spot on the map
                    monsterPosition = rndNum.Next(0, CurrentChunk.Map.Width * CurrentChunk.Map.Height);
                    if (CurrentChunk.Map.Tiles[monsterPosition] is NodeTile)
                    {
                        monsterPosition = rndNum.Next(0, CurrentChunk.Map.Width * CurrentChunk.Map.Height);
                    }
                }

                // Set the monster's new position
                // Note: this fancy math will be replaced by a new helper method
                // in the next revision of SadConsole
                var pos = new Point(monsterPosition % CurrentChunk.Map.Width, monsterPosition / CurrentChunk.Map.Height);

                Stat monsterStat = new Stat()
                {
                    Protection = rndNum.Next(0, 5),
                    Defense = rndNum.Next(7, 12),
                    Strength = rndNum.Next(3, 10),
                    BaseAttack = rndNum.Next(3, 12),
                    Speed = 1,
                    ViewRadius = 7,
                    Health = 10,
                    MaxHealth = 10
                };

                // Need to refactor this so that it's simpler to create a monster, propably gonna use the example
                // of moving castle to make a static class containing blueprints on how to create the actors and items.
                Anatomy monsterAnatomy = new();
                monsterAnatomy.SetRace(new Race("Debug Race"));

                Actor debugMonster = EntityFactory.ActorCreator(
                    pos,
                    new ActorTemplate("Debug Monster", Color.Blue, Color.Transparent, 'M',
                    (int)MapLayer.ACTORS, monsterStat, monsterAnatomy, "DebugTest", 150, 60, "flesh"));

                debugMonster.AddComponent(new MoveAndAttackAI(debugMonster.Stats.ViewRadius));
                debugMonster.Inventory.Add(EntityFactory.ItemCreator(debugMonster.Position,
                    new ItemTemplate("Debug Remains", Color.Red, Color.Black, '%', 1.5f, 35, "DebugRotten", "flesh")));
                debugMonster.Anatomy.Limbs = LimbTemplate.BasicHumanoidBody(debugMonster);

                CurrentChunk.Map.Add(debugMonster);
                EntityTimeNode entityNode = new EntityTimeNode(debugMonster.ID, Time.TimePassed.Ticks + 100);
                Time.RegisterEntity(entityNode);
            }

            CurrentChunk.Map.Add(DataManager.ListOfActors[0]);
        }

        private void CreateLoot()
        {
            // number of treasure drops to create
            int numLoot = 20;

            // Produce loot up to a max of numLoot
            for (int i = 0; i < numLoot; i++)
            {
                // Create an Item with some standard attributes
                int lootPosition = 0;

                // Try placing the Item at lootPosition; if this fails, try random positions on the map's tile array
                while (CurrentChunk.Map.Tiles[lootPosition].IsBlockingMove)
                {
                    // pick a random spot on the map
                    lootPosition = rndNum.Next(0, CurrentChunk.Map.Width * CurrentChunk.Map.Height);
                    if (CurrentChunk.Map.Tiles[lootPosition] is NodeTile)
                    {
                        lootPosition = rndNum.Next(0, CurrentChunk.Map.Width * CurrentChunk.Map.Height);
                    }
                }

                // set the loot's new position
                Point posNew = new Point(lootPosition % CurrentChunk.Map.Width, lootPosition / CurrentChunk.Map.Height);

                Item newLoot = EntityFactory.ItemCreator(posNew,
                    new ItemTemplate("Gold Bar", "Gold", "White", '=', 12.5f, 15, "Here is a gold bar, pretty heavy", "gold"));

                // add the Item to the MultiSpatialMap
                CurrentChunk.Map.Add(newLoot);
            }
#if DEBUG
            Item test =
                EntityFactory.ItemCreator(new Point(10, 10), DataManager.ListOfItems.FirstOrDefault
                (i => i.Id == "test"));

            CurrentChunk.Map.Add(test);
#endif
        }

        public void ProcessTurn(long playerTime, bool sucess)
        {
            if (sucess)
            {
                if (Player.Stats.Health <= 0)
                {
                    CurrentChunk.Map.RemoveAllEntities();
                    CurrentChunk.Map.RemoveAllTiles();
                    localChunks = null;
                    Player = null;

                    GameLoop.UIManager.MainMenu.RestartGame();
                    return;
                }

                PlayerTimeNode playerTurn = new PlayerTimeNode(Time.TimePassed.Ticks + playerTime);
                Time.RegisterEntity(playerTurn);

                Player.Stats.ApplyHpRegen();
                Player.Stats.ApplyManaRegen();
                CurrentChunk.Map.PlayerFOV.Calculate(Player.Position, Player.Stats.ViewRadius);

                var node = Time.NextNode();

                while (node is not PlayerTimeNode)
                {
                    switch (node)
                    {
                        case EntityTimeNode entityTurn:
                            ProcessAiTurn(entityTurn.EntityId, Time.TimePassed.Ticks);
                            break;

                        default:
                            throw new NotSupportedException($"Unhandled time master node type: {node.GetType()}");
                    }

                    node = Time.NextNode();
                }

                GameLoop.UIManager.MapWindow.MapConsole.IsDirty = true;

#if DEBUG
                GameLoop.UIManager.MessageLog.Add($"Turns: {Time.Turns}, Tick: {Time.TimePassed.Ticks}");
#endif
            }
        }

        private void ProcessAiTurn(uint entityId, long time)
        {
            Actor entity = (Actor)CurrentChunk.Map.GetEntityById(entityId);

            if (entity != null)
            {
                IAiComponent ai = entity.GoRogueComponents.GetFirstOrDefault<IAiComponent>();
                (bool sucess, long tick) = ai?.RunAi(CurrentChunk.Map, GameLoop.UIManager.MessageLog) ?? (false, -1);
                entity.Stats.ApplyAllRegen();

                if (!sucess || tick < -1)
                    return;

                EntityTimeNode nextTurnNode = new EntityTimeNode(entityId, time + tick);
                Time.RegisterEntity(nextTurnNode);
            }
        }

        public void ChangeControlledEntity(Entity entity)
        {
            CurrentChunk.Map.ControlledEntitiy = entity;
        }

        /// <summary>
        /// Gets the grid chunk coordinate that contains the specified position.
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public Point ChunkPositionFor(Point pos) => new Point(pos.X / CHUNK_SIZE, pos.Y / CHUNK_SIZE);

        /// <summary>
        /// If the given chunk is loaded
        /// </summary>
        /// <param name="chunkLocation"></param>
        /// <returns></returns>
        public bool IsChunkLoaded(Point chunkLocation) => localChunks.ContainsKey(chunkLocation);

        /// <summary>
        /// Whether or not the chunk containing the specified position is loaded.
        /// </summary>
        /// <returns></returns>
        public bool IsLoaded(Point position) => localChunks.ContainsKey(ChunkPositionFor(position));

        /// <summary>
        /// Whether a chunk for the given chunk grid position has ever been generated.
        /// </summary>
        /// <param name="chunkPosition"></param>
        /// <returns></returns>
        public bool ChunkExists(Point chunkPosition) => existingChunckPositions.Contains(chunkPosition);

        // Whether a chunk containing data for the given position has ever been generated.
        public bool Exists(Point position) => existingChunckPositions.Contains(ChunkPositionFor(position));

        private void AddChunkToDictionary(Map map)
        {
            List<Point> keys = new List<Point>();
            if (localChunks.Keys.Count > 0)
            {
                foreach (Point key in localChunks.Keys)
                {
                    keys.Add(key);
                }

                Point newKey = PointInGrid(10, 10, keys.Last());

                if (newKey != Point.None)
                    localChunks.Add(newKey, new MapChunk(newKey.X, newKey.Y, map));
                else
                    throw new ApplicationException("Tried to add a chunk in a non-existant position");
            }
            else
            {
                var p0 = new Point(0, 0);
                localChunks.Add(p0, new MapChunk(p0.X, p0.Y, map));
                CurrentChunk = localChunks[p0];
            }
        }

        /// <summary>
        /// if the values of the grid size,the only thing that will need to be done is to change the x and y
        /// max value size. \nStarts counting from zero and disregards negative numbers.
        /// </summary>
        /// <param name="maxX"></param>
        /// <param name="maxY"></param>
        /// <param name="point"></param>
        private Point PointInGrid(int maxX, int maxY, Point point)
        {
            Point newKey;
            int x = point.X;
            int y = point.Y;

            if (x < maxX && y < 1)
                newKey = new(x + 1, 0);
            else if (x >= maxX && y < maxY)
                newKey = new(0, y + 1);
            else if (x < maxX && y > 0 && y < maxY)
                newKey = new(x + 1, y);
            else
                //TODO: add a logging system to capture these events
                return Point.None;

            return newKey;
        }
    }

    public class RegionChunk
    {
        /// <summary>
        /// The max amount of local chuncks the region chunks hold, should be 16*16 = 256 regions.
        /// </summary>
        private const int MAX_LOCAL_CHUNCKS = 16 * 16;

        public int X { get; }
        public int Y { get; }
        public Dictionary<Point, MapChunk> LocalChunks { get; set; }

        public RegionChunk(int x, int y)
        {
            X = x;
            Y = y;

            LocalChunks = new Dictionary<Point, MapChunk>(MAX_LOCAL_CHUNCKS);
        }
    }
}