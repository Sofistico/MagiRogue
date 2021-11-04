using MagiRogue.Components;
using MagiRogue.Data;
using MagiRogue.Entities;
using MagiRogue.System.Tiles;
using MagiRogue.System.Time;
using MagiRogue.System.WorldGen;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;
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
        private const int _mapWidth = 50;
        //private const int _mapHeight = 50;
        private readonly int _maxRooms = 20;
        private readonly int _minRoomSize = 4;
        private readonly int _maxRoomSize = 10;
        private readonly Random rndNum = new();
        /*private const int _zMaxUpLevel = 10;
        private const int _zMaxLowLevel = -10;*/

        public PlanetMap PlanetMap { get; set; }

        /// <summary>
        /// Stores the current map
        /// </summary>
        public Map CurrentMap { get; private set; }

        public List<Map> AllMaps { get; set; }

        // Player data
        public Player Player { get; set; }

        public TimeSystem Time { get; private set; }

        /// <summary>
        /// Creates a new game world and stores it in a
        /// publicly accessible constructor.
        /// </summary>
        public World(Player player, bool testGame = false)
        {
            AllMaps = new();
            Time = new TimeSystem();
            PlanetMap = new PlanetGenerator().CreatePlanet(500, 500);
            CurrentMap = PlanetMap.AssocietatedMap;
            /*if (!testGame)
            {
                // Build a map
                CreateTownMap();

                CreateStoneFloorMap();

                CreateStoneMazeMap();

                // create an instance of player
                PlacePlayer(player);
            }
            else
            {
                CreateTestMap();

                PlacePlayer(player);
            }*/
        }

        private void CreateStoneMazeMap()
        {
            Map map = new MapGenerator().GenerateMazeMap(_maxRooms, _minRoomSize, _maxRoomSize);

            // spawn a bunch of monsters
            CreateMonster(map, 10);

            // Spawn a bunch of loot
            CreateLoot(map, 20);

            // Set up anything that needs to be set up for the world to work
            SetUpStuff(map);
        }

        private void CreateStoneFloorMap()
        {
            var map = new MapGenerator().GenerateStoneFloorMap();

            AddMapToList(map);
        }

        private void AddMapToList(Map map) => AllMaps.Add(map);

        public void ChangePlayerMap(Map mapToGo, Point pos)
        {
            ChangeActorMap(Player, mapToGo, pos);
            CurrentMap = mapToGo;
            GameLoop.UIManager.MapWindow.LoadMap(CurrentMap);
        }

        public void ChangeActorMap(Entity entity, Map mapToGo, Point pos)
        {
            mapToGo.Add(entity);
            entity.Position = pos;
            if (!AllMaps.Contains(mapToGo))
                AddMapToList(mapToGo);
        }

        /// <summary>
        /// Sets up anything that needs to be set up after map gen and after placing entities, like the nodes turn
        /// system
        /// </summary>
        private void SetUpStuff(Map map)
        {
            foreach (NodeTile node in map.Tiles.OfType<NodeTile>())
            {
                node.SetUpNodeTurn(this);
            }
        }

        // Create a new map using the Map class
        // and a map generator. Uses several
        // parameters to determine geometry
        private void CreateTownMap()
        {
            MapGenerator mapGen = new();
            var map = mapGen.GenerateTownMap(_maxRooms, _minRoomSize, _maxRoomSize);
            CurrentMap = map;
            AddMapToList(map);
        }

        private void CreateTestMap()
        {
            MapGenerator mapGen = new();
            var map = mapGen.GenerateTestMap();
            CurrentMap = map;
            AddMapToList(map);
        }

        // Create a player using the Player class
        // and set its starting position
        private void PlacePlayer(Player player)
        {
            // Place the player on the first non-movement-blocking tile on the map
            for (int i = 0; i < CurrentMap.Tiles.Length; i++)
            {
                if (!CurrentMap.Tiles[i].IsBlockingMove && CurrentMap.Tiles[i] is not NodeTile
                    && !CurrentMap.GetEntitiesAt<Entity>(Point.FromIndex(i, _mapWidth)).Any())
                {
                    // Set the player's position to the index of the current map position
                    var pos = Point.FromIndex(i, CurrentMap.Width);

                    Player = player;
                    Player.Position = pos;
                    Player.Description = "Here is you, you are beautiful";

                    break;
                }
            }

            // add the player to the Map's collection of Entities
            CurrentMap.Add(Player);
        }

        // Create some random monsters with random attack and defense values
        // and drop them all over the map in
        // random places.
        private void CreateMonster(Map map, int numMonster)
        {
            // Create several monsters and
            // pick a random position on the map to place them.
            // check if the placement spot is blocking (e.g. a wall)
            // and if it is, try a new position
            for (int i = 0; i < numMonster; i++)
            {
                int monsterPosition = 0;
                while (map.Tiles[monsterPosition].IsBlockingMove)
                {
                    // pick a random spot on the map
                    monsterPosition = rndNum.Next(0, map.Width * map.Height);
                    if (map.Tiles[monsterPosition] is NodeTile)
                    {
                        monsterPosition = rndNum.Next(0, map.Width * map.Height);
                    }
                }

                // Set the monster's new position
                // Note: this fancy math will be replaced by a new helper method
                // in the next revision of SadConsole
                var pos = new Point(monsterPosition % map.Width, monsterPosition / map.Height);

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

                map.Add(debugMonster);
                EntityTimeNode entityNode = new EntityTimeNode(debugMonster.ID, Time.TimePassed.Ticks + 100);
                Time.RegisterEntity(entityNode);
            }

            map.Add(DataManager.ListOfActors[0]);
        }

        private void CreateLoot(Map map, int numLoot)
        {
            // Produce loot up to a max of numLoot
            for (int i = 0; i < numLoot; i++)
            {
                // Create an Item with some standard attributes
                int lootPosition = 0;

                // Try placing the Item at lootPosition; if this fails, try random positions on the map's tile array
                while (map.Tiles[lootPosition].IsBlockingMove)
                {
                    // pick a random spot on the map
                    lootPosition = rndNum.Next(0, map.Width * map.Height);
                    if (map.Tiles[lootPosition] is NodeTile)
                    {
                        lootPosition = rndNum.Next(0, map.Width * map.Height);
                    }
                }

                // set the loot's new position
                Point posNew = new Point(lootPosition % map.Width, lootPosition / map.Height);

                Item newLoot = EntityFactory.ItemCreator(posNew,
                    new ItemTemplate("Gold Bar", "Gold", "White", '=', 12.5f, 15, "Here is a gold bar, pretty heavy", "gold"));

                // add the Item to the MultiSpatialMap
                map.Add(newLoot);
            }
#if DEBUG
            Item test =
                EntityFactory.ItemCreator(new Point(10, 10), DataManager.ListOfItems.FirstOrDefault
                (i => i.Id == "test"));

            map.Add(test);
#endif
        }

        public void ProcessTurn(long playerTime, bool sucess)
        {
            if (sucess)
            {
                if (Player.Stats.Health <= 0)
                {
                    RestartGame();
                    return;
                }

                PlayerTimeNode playerTurn = new PlayerTimeNode(Time.TimePassed.Ticks + playerTime);
                Time.RegisterEntity(playerTurn);

                Player.Stats.ApplyHpRegen();
                Player.Stats.ApplyManaRegen();
                CurrentMap.PlayerFOV.Calculate(Player.Position, Player.Stats.ViewRadius);

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

        private void RestartGame()
        {
            CurrentMap.RemoveAllEntities();
            CurrentMap.RemoveAllTiles();
            CurrentMap.GoRogueComponents.GetFirstOrDefault<FOVHandler>().DisposeMap();
            for (int i = 0; i < AllMaps.Count; i++)
            {
                AllMaps[i].RemoveAllEntities();
                AllMaps[i].RemoveAllTiles();
                AllMaps[i].GoRogueComponents.GetFirstOrDefault<FOVHandler>().DisposeMap();
            }
            CurrentMap = null;
            Player = null;
            AllMaps.Clear();
            AllMaps = null;

            GameLoop.UIManager.MainMenu.RestartGame();
        }

        private void ProcessAiTurn(uint entityId, long time)
        {
            Actor entity = (Actor)CurrentMap.GetEntityById(entityId);

            if (entity != null)
            {
                IAiComponent ai = entity.GoRogueComponents.GetFirstOrDefault<IAiComponent>();
                (bool sucess, long tick) = ai?.RunAi(CurrentMap, GameLoop.UIManager.MessageLog) ?? (false, -1);
                entity.Stats.ApplyAllRegen();

                if (!sucess || tick < -1)
                    return;

                EntityTimeNode nextTurnNode = new EntityTimeNode(entityId, time + tick);
                Time.RegisterEntity(nextTurnNode);
            }
        }

        public void ChangeControlledEntity(Entity entity)
        {
            CurrentMap.ControlledEntitiy = entity;
        }
    }

    // For a future world update.
    public class RegionChunk
    {
        /// <summary>
        /// The max amount of local maps the region chunks hold, should be 3*3 = 9 maps.
        /// </summary>
        private const int MAX_LOCAL_CHUNCKS = 3 * 3;

        public int X { get; }
        public int Y { get; }
        public Map[] LocalMaps { get; set; }

        public RegionChunk(int x, int y)
        {
            X = x;
            Y = y;
            LocalMaps = new Map[MAX_LOCAL_CHUNCKS];
        }
    }

    public class AddMapEventArgs : EventArgs
    {
        public Map NewMap { get; }

        public AddMapEventArgs(Map newMap) => NewMap = newMap;
    }
}