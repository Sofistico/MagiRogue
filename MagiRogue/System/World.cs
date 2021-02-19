using GoRogue;
using MagiRogue.Components;
using MagiRogue.Entities;
using MagiRogue.Entities.Items;
using MagiRogue.System.Tiles;
using MagiRogue.System.Time;
using Microsoft.Xna.Framework;
using SadConsole;
using System;

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
        private TileBase[] _mapTiles;
        private readonly int _maxRooms = 20;
        private readonly int _minRoomSize = 4;
        private readonly int _maxRoomSize = 10;

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
            if (testGame == false)
            {
                // Build a map
                CreateMap();

                // spawn a bunch of monsters
                CreateMonster();

                // Spawn a bunch of loot
                CreateLoot();

                // create an instance of player
                CreatePlayer();
            }
            else
            {
                CreateTestMap();

                CreatePlayer();
            }
        }

        // Create a new map using the Map class
        // and a map generator. Uses several
        // parameters to determine geometry
        private void CreateMap()
        {
            _mapTiles = new TileBase[_mapWidth * _mapHeight];
            CurrentMap = new Map(_mapWidth, _mapHeight);
            MapGenerator mapGen = new MapGenerator();
            CurrentMap = mapGen.GenerateMap(_mapWidth, _mapHeight, _maxRooms, _minRoomSize, _maxRoomSize);
        }

        private void CreateTestMap()
        {
            _mapTiles = new TileBase[_mapWidth * _mapHeight];
            CurrentMap = new Map(_mapWidth, _mapHeight);
            MapGenerator mapGen = new MapGenerator();
            CurrentMap = mapGen.GenerateTestMap(_mapWidth, _mapHeight);
        }

        // Create a player using the Player class
        // and set its starting position
        private void CreatePlayer()
        {
            //Player.Components.Add(new EntityViewSyncComponent());

            // Place the player on the first non-movement-blocking tile on the map
            for (int i = 0; i < CurrentMap.Tiles.Length; i++)
            {
                if (!CurrentMap.Tiles[i].IsBlockingMove)
                {
                    // Set the player's position to the index of the current map position
                    var pos = Helpers.GetPointFromIndex(i, CurrentMap.Width);
                    Player = new Player(Color.White, Color.Black, pos)
                    {
                        Position = pos
                    };
                    //Player.AddComponent(new Components.HealthComponent(10, 10, 0.1f));
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
                //newMonster.Components.Add(new EntityViewSyncComponent());
                while (CurrentMap.Tiles[monsterPosition].IsBlockingMove)
                {
                    // pick a random spot on the map
                    monsterPosition = rndNum.Next(0, CurrentMap.Width * CurrentMap.Height);
                }

                // Set the monster's new position
                // Note: this fancy math will be replaced by a new helper method
                // in the next revision of SadConsole
                var pos = new Point(monsterPosition % CurrentMap.Width, monsterPosition / CurrentMap.Height);
                Monster newMonster = new Monster(Color.Blue, Color.Transparent, pos)
                {
                    // plug in some magic numbers for attack and defense values

                    Name = "a common troll",
                };

                newMonster.Stats.Defense = rndNum.Next(0, 10);
                newMonster.Stats.DefenseChance = rndNum.Next(0, 50);
                newMonster.Stats.Attack = rndNum.Next(0, 10);
                newMonster.Stats.AttackChance = rndNum.Next(0, 50);
                newMonster.Stats.Speed = 1;
                newMonster.AddComponent(new MoveAndAttackAI(newMonster.Stats.ViewRadius));

                CurrentMap.Add(newMonster);
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

                // Let SadConsole know that this Item's position be tracked on the map
                //newLoot.Components.Add(new EntityViewSyncComponent());

                // Try placing the Item at lootPosition; if this fails, try random positions on the map's tile array
                while (CurrentMap.Tiles[lootPosition].IsBlockingMove)
                {
                    // pick a random spot on the map
                    lootPosition = rndNum.Next(0, CurrentMap.Width * CurrentMap.Height);
                }

                // set the loot's new position
                //newLoot.Position = new Point(lootPosition % CurrentMap.Width, lootPosition / CurrentMap.Height);
                //ironBar.Position = new Point(lootPosition / CurrentMap.Width, lootPosition % CurrentMap.Height);
                Point posNew = new Point(lootPosition % CurrentMap.Width, lootPosition / CurrentMap.Height);
                Point posIron = new Point(lootPosition / CurrentMap.Width, lootPosition % CurrentMap.Height);

                Item newLoot = new Item(Color.Gold, Color.White, "Gold bar", '=', posNew, 12.5);
                IronBar ironBar = new IronBar(posIron);

                // add the Item to the MultiSpatialMap
                CurrentMap.Add(newLoot);
                CurrentMap.Add(ironBar);
            }
        }

        public void ProcessTurn(long playerTime, bool sucess)
        {
            // TODO: define the way that entity regain energy better.
            if (sucess)
            {
                PlayerTimeNode playerTurn = new PlayerTimeNode(GetTime.TimePassed.Ticks + playerTime);
                GetTime.RegisterEntity(playerTurn);

                Player.ApplyHpRegen();
                CurrentMap.CalculateFOV(position: Player.Position, Player.Stats.ViewRadius, radiusShape: Radius.CIRCLE);

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

#if DEBUG
                GameLoop.UIManager.MessageLog.Add($"Turns: {GetTime.Turns}, Tick: {GetTime.TimePassed.Ticks}");
#endif
            }
        }

        private void ProcessAiTurn(uint entityId, long time)
        {
            // TODO: Add some basic ai handling
            Entity entity = CurrentMap.GetEntityById(entityId);

            if (entity != null)
            {
                IAiComponent ai = entity.GetGoRogueComponent<IAiComponent>();
                (bool sucess, long tick) = ai?.RunAi(CurrentMap, GameLoop.UIManager.MessageLog) ?? (false, -1);

                if (!sucess || tick < -1)
                    return;

                EntityTimeNode nextTurnNode = new EntityTimeNode(entityId, time + tick);
                GetTime.RegisterEntity(nextTurnNode);
            }
        }
    }
}