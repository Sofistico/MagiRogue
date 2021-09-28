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

        private readonly int _mapWidth = 50;
        private readonly int _mapHeight = 50;
        private readonly int _maxRooms = 20;
        private readonly int _minRoomSize = 4;
        private readonly int _maxRoomSize = 10;
        private const int _zMaxUpLevel = 10;
        private const int _zMaxLowLevel = -10;

        /// <summary>
        /// Stores the current map
        /// </summary>
        public Map CurrentMap { get; set; }

        public List<Map> AllMaps { get; set; }

        // Player data
        public Player Player { get; set; }

        public TimeSystem GetTime => CurrentMap.Time;

        private readonly Random rndNum = new Random();

        /// <summary>
        /// Creates a new game world and stores it in a
        /// publicly accessible constructor.
        /// </summary>
        public World(Player player, bool testGame = false)
        {
            AllMaps = new List<Map>();
            if (!testGame)
            {
                // Build a map
                CreateMap();

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

        /// <summary>
        /// Sets up anything that needs to be set up after map gen and after placing entities, like the nodes turn
        /// system
        /// </summary>
        private void SetUpStuff()
        {
            foreach (NodeTile node in CurrentMap.Tiles.Where(t => t is NodeTile))
            {
                node.SetUpNodeTurn(this);
            }
        }

        // Create a new map using the Map class
        // and a map generator. Uses several
        // parameters to determine geometry
        private void CreateMap()
        {
            CurrentMap = new Map(_mapWidth, _mapHeight);
            MapGenerator mapGen = new MapGenerator();
            CurrentMap = mapGen.GenerateTownMap(_mapWidth, _mapHeight, _maxRooms, _minRoomSize, _maxRoomSize);
            AllMaps.Add(CurrentMap);
        }

        private void CreateTestMap()
        {
            CurrentMap = new Map(_mapWidth, _mapHeight);
            MapGenerator mapGen = new MapGenerator();
            CurrentMap = mapGen.GenerateTestMap(_mapWidth, _mapHeight);
            AllMaps.Add(CurrentMap);
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
                while (CurrentMap.Tiles[monsterPosition].IsBlockingMove)
                {
                    // pick a random spot on the map
                    monsterPosition = rndNum.Next(0, CurrentMap.Width * CurrentMap.Height);
                    if (CurrentMap.Tiles[monsterPosition] is NodeTile)
                    {
                        monsterPosition = rndNum.Next(0, CurrentMap.Width * CurrentMap.Height);
                    }
                }

                // Set the monster's new position
                // Note: this fancy math will be replaced by a new helper method
                // in the next revision of SadConsole
                var pos = new Point(monsterPosition % CurrentMap.Width, monsterPosition / CurrentMap.Height);

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

                CurrentMap.Add(debugMonster);
            }

            CurrentMap.Add(DataManager.ListOfActors[0]);
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
                while (CurrentMap.Tiles[lootPosition].IsBlockingMove)
                {
                    // pick a random spot on the map
                    lootPosition = rndNum.Next(0, CurrentMap.Width * CurrentMap.Height);
                    if (CurrentMap.Tiles[lootPosition] is NodeTile)
                    {
                        lootPosition = rndNum.Next(0, CurrentMap.Width * CurrentMap.Height);
                    }
                }

                // set the loot's new position
                Point posNew = new Point(lootPosition % CurrentMap.Width, lootPosition / CurrentMap.Height);

                Item newLoot = EntityFactory.ItemCreator(posNew,
                    new ItemTemplate("Gold Bar", "Gold", "White", '=', 12.5f, 15, "Here is a gold bar, pretty heavy", "gold"));

                string oj = Newtonsoft.Json.JsonConvert.SerializeObject(newLoot);

                // add the Item to the MultiSpatialMap
                CurrentMap.Add(newLoot);
            }
#if DEBUG
            Item test =
                EntityFactory.ItemCreator(new Point(10, 10), DataManager.ListOfItems.FirstOrDefault
                (i => i.Id == "test"));

            CurrentMap.Add(test);
#endif
        }

        public void ProcessTurn(long playerTime, bool sucess)
        {
            if (sucess)
            {
                if (Player.Stats.Health <= 0)
                {
                    CurrentMap.RemoveAllEntities();
                    CurrentMap.RemoveAllTiles();
                    CurrentMap = null;
                    Player = null;

                    GameLoop.UIManager.MainMenu.RestartGame();
                    return;
                }

                PlayerTimeNode playerTurn = new PlayerTimeNode(GetTime.TimePassed.Ticks + playerTime);
                GetTime.RegisterEntity(playerTurn);

                Player.Stats.ApplyHpRegen();
                Player.Stats.ApplyManaRegen();
                CurrentMap.PlayerFOV.Calculate(Player.Position, Player.Stats.ViewRadius);

                var node = GetTime.NextNode();

                while (node is not PlayerTimeNode)
                {
                    switch (node)
                    {
                        case EntityTimeNode entityTurn:
                            ProcessAiTurn(entityTurn.EntityId, GetTime.TimePassed.Ticks);
                            break;

                        default:
                            throw new NotSupportedException($"Unhandled time master node type: {node.GetType()}");
                    }

                    node = GetTime.NextNode();
                }

                GameLoop.UIManager.MapWindow.MapConsole.IsDirty = true;

#if DEBUG
                GameLoop.UIManager.MessageLog.Add($"Turns: {GetTime.Turns}, Tick: {GetTime.TimePassed.Ticks}");
#endif
            }
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
                GetTime.RegisterEntity(nextTurnNode);
            }
        }

        public void ChangeControlledEntity(Entity entity)
        {
            CurrentMap.ControlledEntitiy = entity;
        }
    }
}