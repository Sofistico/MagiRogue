using MagiRogue.Components;
using MagiRogue.Entities;
using MagiRogue.Entities.Data;
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

        // Player data
        public Player Player { get; set; }

        public TimeSystem GetTime => CurrentMap.Time;

        private readonly Random rndNum = new Random();

        /// <summary>
        /// Creates a new game world and stores it in a
        /// publicly accessible constructor.
        /// </summary>
        public World(bool testGame = false)
        {
            if (!testGame)
            {
                // Build a map
                CreateMap();

                // spawn a bunch of monsters
                CreateMonster();

                // Spawn a bunch of loot
                CreateLoot();

                // create an instance of player
                CreatePlayer();

                // Set up anything that needs to be set up for the world to work
                SetUpStuff();
            }
            else
            {
                CreateTestMap();

                CreatePlayer();
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
            CurrentMap = mapGen.GenerateMap(_mapWidth, _mapHeight, _maxRooms, _minRoomSize, _maxRoomSize);
        }

        private void CreateTestMap()
        {
            CurrentMap = new Map(_mapWidth, _mapHeight);
            MapGenerator mapGen = new MapGenerator();
            CurrentMap = mapGen.GenerateTestMap(_mapWidth, _mapHeight);
        }

        // Create a player using the Player class
        // and set its starting position
        private void CreatePlayer()
        {
            // Place the player on the first non-movement-blocking tile on the map
            for (int i = 0; i < CurrentMap.Tiles.Length; i++)
            {
                if (!CurrentMap.Tiles[i].IsBlockingMove && CurrentMap.Tiles[i] is not NodeTile)
                {
                    // Set the player's position to the index of the current map position
                    var pos = Point.FromIndex(i, CurrentMap.Width);

                    Player = new Player(Color.White, Color.Black, pos)
                    {
                        Position = pos,
                        Description = "Here is you, you are beautiful"
                    };
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
                    Defense = rndNum.Next(0, 10),
                    DefenseChance = rndNum.Next(0, 50),
                    Attack = rndNum.Next(0, 10),
                    AttackChance = rndNum.Next(0, 50),
                    Speed = 1,
                    ViewRadius = 7,
                    Health = 10,
                    MaxHealth = 10
                };

                // Need to refactor this so that it's simpler to create a monster, propably gonna use the example
                // of moving castle to make a static class containing blueprints on how to create the actors and items.
                Anatomy monsterAnatomy = new Anatomy();
                monsterAnatomy.SetRace(new Race("Debug Race"));

                Actor debugMonster = EntityFactory.ActorCreator(
                    pos,
                    new ActorTemplate("Debug Monster", Color.Blue, Color.Transparent, 'M',
                    (int)MapLayer.ACTORS, monsterStat, monsterAnatomy, "DebugTest", 150, 60, "flesh"));

                debugMonster.AddComponent(new MoveAndAttackAI(debugMonster.Stats.ViewRadius));
                debugMonster.Inventory.Add(EntityFactory.ItemCreator(debugMonster.Position,
                    new ItemTemplate("Debug Remains", Color.Red, Color.Black, '%', 1.5f, 35, "DebugRotten")));
                debugMonster.Anatomy.Limbs = LimbTemplate.BasicHumanoidBody(debugMonster);

                CurrentMap.Add(debugMonster);
            }
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
                    new ItemTemplate("Gold Bar", Color.Gold, Color.White, '=', 12.5f, 15, "Here is a gold bar, pretty heavy"));

                // add the Item to the MultiSpatialMap
                CurrentMap.Add(newLoot);
            }

            IList<ItemTemplate> itemTest = Utils.JsonUtils.JsonDeseralize<List<ItemTemplate>>(Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Entities", "Items", "Bars.json"));

            Item test = EntityFactory.ItemCreator(new Point(10, 10), itemTest.FirstOrDefault(i => i.Id == "test"));

            CurrentMap.Add(test);
        }

        public void ProcessTurn(long playerTime, bool sucess)
        {
            if (sucess)
            {
                PlayerTimeNode playerTurn = new PlayerTimeNode(GetTime.TimePassed.Ticks + playerTime);
                GetTime.RegisterEntity(playerTurn);

                Player.Stats.ApplyHpRegen();
                Player.Stats.ApplyManaRegen();
                CurrentMap.PlayerFOV.Calculate(Player.Position, Player.Stats.ViewRadius);

                if (Player.Stats.Health <= 0)
                {
                    CurrentMap.RemoveAllEntities();
                    CurrentMap = null;
                    Player = null;

                    GameLoop.UIManager.MainMenu.RestartGame();
                    return;
                }

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