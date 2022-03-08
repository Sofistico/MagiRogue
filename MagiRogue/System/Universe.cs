using MagiRogue.Components;
using MagiRogue.Data;
using MagiRogue.Entities;
using MagiRogue.System.Civ;
using MagiRogue.System.Tiles;
using MagiRogue.System.Time;
using MagiRogue.System.Planet;
using SadRogue.Primitives;
using System;
using MagiRogue.System.Magic;
using System.Linq;
using MagiRogue.Data.Serialization;
using Newtonsoft.Json;

namespace MagiRogue.System
{
    /// <summary>
    /// All game state data is stored in the Universe
    /// also creates and processes generators
    /// for map creation
    /// </summary>
    [JsonConverter(typeof(UniverseJsonConverter))]
    public class Universe
    {
        // map creation and storage data
        private const int _mapWidth = 50;
        //private const int _mapHeight = 50;
        private readonly int maxChunks;
        private readonly int planetWidth = 257;
        private readonly int planetHeight = 257;
        private readonly int planetMaxCivs = 30;
        private readonly int _maxRooms = 20;
        private readonly int _minRoomSize = 4;
        private readonly int _maxRoomSize = 10;
        private readonly Random rndNum = new();
        /*private const int _zMaxUpLevel = 10;
        private const int _zMaxLowLevel = -10;*/

        /// <summary>
        /// The World map, contains the map data and the Planet data
        /// </summary>
        public PlanetMap WorldMap { get; set; }

        /// <summary>
        /// Stores the current map
        /// </summary>
        public Map CurrentMap { get; private set; }

        /// <summary>
        /// Stores the current chunk that is loaded
        /// </summary>
        public RegionChunk CurrentChunk { get; set; }

        /// <summary>
        /// Player data
        /// </summary>
        public Player Player { get; set; }

        public TimeSystem Time { get; private set; }
        public bool PossibleChangeMap { get; internal set; } = true;

        public SeasonType CurrentSeason { get; set; }

        /// <summary>
        /// All the maps and chunks of the game
        /// NOTE WILL BE SLOWLEY REMOVED FROM CODE!!
        /// TO BE REPLACED WITH AN ARRAY THAT CONTAINS FEWR CHUNKS!
        /// </summary>
        public RegionChunk[] AllChunks { get; set; }

        public SaveAndLoad SaveAndLoad { get; set; }

        /// <summary>
        /// Creates a new game world and stores it in a
        /// publicly accessible constructor.
        /// </summary>
        public Universe(Player player, bool testGame = false)
        {
            Time = new TimeSystem();
            CurrentSeason = SeasonType.Spring;

            if (!testGame)
            {
                WorldMap = new PlanetGenerator().CreatePlanet(planetWidth,
                    planetHeight,
                    planetMaxCivs);
                WorldMap.AssocietatedMap.IsActive = true;
                maxChunks = planetWidth * planetHeight;
                AllChunks = new RegionChunk[maxChunks];
                CurrentMap = WorldMap.AssocietatedMap;
                PlacePlayerOnWorld(player);
                SaveAndLoad = new();
                if (player is not null)
                    SaveGame(player.Name);
            }
            else
            {
                CreateTestMap();

                PlacePlayer(player);
            }
        }

        public Universe(PlanetMap worldMap,
            Map currentMap,
            Player player,
            TimeSystem time,
            bool possibleChangeMap,
            SeasonType currentSeason,
            RegionChunk[] allChunks,
            SaveAndLoad loadSave)
        {
            WorldMap = worldMap;

            if (currentMap is not null && worldMap.AssocietatedMap.MapName.Equals(currentMap.MapName))
                CurrentMap = worldMap.AssocietatedMap;
            else
                CurrentMap = currentMap;

            if (CurrentMap is not null)
                CurrentMap.IsActive = true;
            Player = player;
            Time = time;
            PossibleChangeMap = possibleChangeMap;
            CurrentSeason = currentSeason;
            AllChunks = allChunks;
            SaveAndLoad = loadSave;
        }

        private void PlacePlayerOnWorld(Player player)
        {
            if (player != null)
            {
                Civilization startTown = WorldMap.Civilizations
                    .FirstOrDefault(a => a.Tendency == CivilizationTendency.Normal);
                player.Position = startTown.ReturnAllLandTerritory(WorldMap.AssocietatedMap)[0].Position;
                player.Description = "Here is you, you are beautiful";
                Player = player;

                CurrentMap.Add(Player);
            }
        }

        public void PlacePlayerOnLoad()
        {
            if (Player != null)
            {
                Player.Position = CurrentMap.LastPlayerPosition;
                CurrentMap.Add(Player);
            }
            else
            {
                throw new Exception("Coudn't load the player, it was null!");
            }
        }

        private void CreateStoneMazeMap()
        {
            Map map = new DungeonGenerator().GenerateMazeMap(_maxRooms, _minRoomSize, _maxRoomSize);

            // spawn a bunch of monsters
            CreateMonster(map, 10);

            // Spawn a bunch of loot
            CreateLoot(map, 20);

            // Set up anything that needs to be set up for the world to work
            SetUpStuff(map);
        }

        public void ChangePlayerMap(Map mapToGo, Point pos, Map previousMap)
        {
            CurrentMap.LastPlayerPosition = new Point(Player.Position.X, Player.Position.Y);
            ChangeActorMap(Player, mapToGo, pos, previousMap);
            UpdateIfNeedTheMap(mapToGo);
            CurrentMap = mapToGo;
            previousMap.ControlledEntitiy = null;
            GameLoop.UIManager.MapWindow.LoadMap(CurrentMap);
            GameLoop.UIManager.MapWindow.CenterOnActor(Player);
        }

        private static void UpdateIfNeedTheMap(Map mapToGo)
        {
            if (mapToGo.NeedsUpdate)
            {
                // do something
            }
            else
                return; // Do nothing
        }

        public void ChangeActorMap(Entity entity, Map mapToGo, Point pos, Map previousMap)
        {
            previousMap.Remove(entity);
            entity.Position = pos;
            mapToGo.Add(entity);
        }

        public void SaveGame(string saveName)
        {
            SaveAndLoad.SaveGameToFolder(this, saveName);
        }

        /// <summary>
        /// Sets up anything that needs to be set up after map gen
        /// and after placing entities, like the nodes turn
        /// system
        /// </summary>
        private void SetUpStuff(Map map)
        {
            foreach (NodeTile node in map.Tiles.OfType<NodeTile>())
            {
                node.SetUpNodeTurn(this);
            }
        }

        private void CreateTestMap()
        {
            GeneralMapGenerator generalMapGenerator = new();
            Map map = generalMapGenerator.GenerateTestMap();
            CurrentMap = map;
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
                    new ItemTemplate("Debug Remains",
                    Color.Red.PackedValue,
                    Color.Black.PackedValue, '%', 1.5f, 35, "flesh", new MagicManager())
                    {
                        Description = "DebugRotten"
                    }));
                debugMonster.Anatomy.Limbs = EntityFactory.BasicHumanoidBody();

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
                    DeleteSave();
                    RestartGame();
                    return;
                }

                PlayerTimeNode playerTurn = new PlayerTimeNode(Time.TimePassed.Ticks + playerTime);
                Time.RegisterEntity(playerTurn);

                Player.Stats.ApplyHpRegen();
                Player.Stats.ApplyManaRegen();
                CurrentMap.PlayerFOV.Calculate(Player.Position, Player.Stats.ViewRadius);

                var node = Time.NextNode();

                // put here terrrain effect

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

        private void DeleteSave()
        {
            SaveAndLoad.DeleteSave(Player.Name);
        }

        private void RestartGame()
        {
            CurrentMap.RemoveAllEntities();
            CurrentMap.RemoveAllTiles();
            CurrentMap.GoRogueComponents.GetFirstOrDefault<FOVHandler>().DisposeMap();
            int chunckLenght = AllChunks.Length;
            for (int i = 0; i < chunckLenght; i++)
            {
                Map[] maps = AllChunks[i].LocalMaps;
                int mapsLenght = maps.Length;
                for (int z = 0; z < mapsLenght; z++)
                {
                    maps[z].RemoveAllEntities();
                    maps[z].RemoveAllTiles();
                    maps[z].GoRogueComponents.GetFirstOrDefault<FOVHandler>().DisposeMap();
                }
            }
            CurrentMap = null;
            Player = null;
            AllChunks = null;
            CurrentChunk = null;
            WorldMap = null;

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
            GameLoop.UIManager.MapWindow.CenterOnActor(entity);
        }

        public RegionChunk GenerateChunck(Point posGenerated)
        {
            RegionChunk newChunck = new RegionChunk(posGenerated);

            WildernessGenerator genMap = new WildernessGenerator();
            newChunck.LocalMaps = genMap.GenerateMapWithWorldParam(WorldMap, posGenerated);

            for (int i = 0; i < newChunck.LocalMaps.Length; i++)
            {
                newChunck.LocalMaps[i].SetId(GameLoop.IdGen.UseID());
            }

            //AllChunks[Point.ToIndex(posGenerated.X, posGenerated.Y, planetWidth)] = newChunck;

            return newChunck;
        }

        public RegionChunk GetChunckByPos(Point playerPoint)
        {
            var chunk = SaveAndLoad.GetChunkAtIndex(Point.ToIndex(playerPoint.X, playerPoint.Y, planetWidth), planetWidth);

            return chunk;
        }

        public bool MapIsWorld()
        {
            if (CurrentMap == WorldMap.AssocietatedMap)
                return true;
            else
                return false;
        }

        public bool MapIsWorld(Map map)
        {
            if (map == WorldMap.AssocietatedMap)
                return true;
            else
                return false;
        }

        public void ForceChangeCurrentMap(Map map) => CurrentMap = map;
    }
}