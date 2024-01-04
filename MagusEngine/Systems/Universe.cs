using Arquimedes.Enumerators;
using Arquimedes.Settings;
using Arquimedes.Utils;
using GoRogue.Messaging;
using MagusEngine.Bus;
using MagusEngine.Bus.MapBus;
using MagusEngine.Bus.UiBus;
using MagusEngine.Core.Civ;
using MagusEngine.Core.Entities;
using MagusEngine.Core.Entities.Base;
using MagusEngine.Core.MapStuff;
using MagusEngine.ECS.Components.ActorComponents.Ai;
using MagusEngine.Generators;
using MagusEngine.Generators.MapGen;
using MagusEngine.Serialization;
using MagusEngine.Services;
using MagusEngine.Systems.Time;
using Newtonsoft.Json;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MagusEngine.Systems
{
    /// <summary>
    /// All game state data is stored in the Universe also creates and processes generators for map creation
    /// </summary>
    [JsonConverter(typeof(UniverseJsonConverter))]
    public sealed class Universe :
        ISubscriber<ChangeControlledActorMap>,
        ISubscriber<ChangeControlledEntitiy>,
        ISubscriber<AddEntitiyCurrentMap>,
        ISubscriber<ProcessTurnEvent>,
        ISubscriber<RemoveEntitiyCurrentMap>
    {
        /// <summary>
        /// The World map, contains the map data and the Planet data
        /// </summary>
        public PlanetMap WorldMap { get; set; }

        /// <summary>
        /// Stores the current map
        /// </summary>
        public MagiMap? CurrentMap { get; private set; }

        /// <summary>
        /// Stores the current chunk that is loaded
        /// </summary>
        public RegionChunk CurrentChunk { get; set; }

        /// <summary>
        /// Player data
        /// </summary>
        public Player Player { get; set; }

        /// <summary>
        /// Keeps track of where the player goes, even if it's inside of a chunk inside a map!
        /// </summary>
        public Point AbsolutePlayerPos { get; set; }

        public TimeSystem Time { get; }
        public bool PossibleChangeMap { get; set; } = true;

        public SeasonType CurrentSeason { get; set; }

        public int ZLevel { get; set; }

        public PlanetGenSettings? PlanetSettings { get; set; }

        /// <summary>
        /// Creates a new game world and stores it in a publicly accessible constructor.
        /// </summary>
        public Universe(Player player, bool testGame = false)
        {
            Time = new TimeSystem();
            CurrentSeason = SeasonType.Spring;
            PlanetSettings = JsonUtils.JsonDeseralize<PlanetGenSettings>(Path
                .Combine(AppDomain.CurrentDomain.BaseDirectory,
                "Settings",
                "planet_gen_setting.json"));

            if (!testGame)
            {
                WorldMap = new PlanetGenerator().CreatePlanet(PlanetSettings!.PlanetWidth,
                    PlanetSettings!.PlanetHeight,
                    PlanetSettings!.PlanetMaxInitialCivs);
                Time = WorldMap.GetTimePassed();
                WorldMap.AssocietatedMap.IsActive = true;
                CurrentMap = WorldMap.AssocietatedMap;
                PlacePlayerOnWorld(player);
                if (player is not null)
                    SaveGame(player.Name);
            }
            else
            {
                CreateTestMap();

                PlacePlayer(player);
            }
            Locator.GetService<MessageBusService>().RegisterAllSubscriber(this);
        }

        [JsonConstructor]
        public Universe(PlanetMap worldMap,
            MagiMap currentMap,
            Player player,
            TimeSystem time,
            bool possibleChangeMap,
            SeasonType currentSeason,
            RegionChunk currentChunk)
        {
            WorldMap = worldMap;
            CurrentChunk = currentChunk;

            if (currentMap is not null && worldMap?.AssocietatedMap?.MapName?.Equals(currentMap.MapName) == true)
            {
                CurrentMap = worldMap.AssocietatedMap;
            }
            else if (currentMap is not null && currentChunk.LocalMaps.All(i => i is not null)
                && currentChunk.LocalMaps.Any(i => i.MapId == currentMap.MapId))
            {
                CurrentMap = Array.Find(currentChunk.LocalMaps, i => i.MapId == currentMap.MapId)!;
            }
            else
            {
                CurrentMap = currentMap!;
            }

            if (CurrentMap is not null)
                CurrentMap.IsActive = true;
            Player = player;
            Time = time;
            PossibleChangeMap = possibleChangeMap;
            CurrentSeason = currentSeason;
            Locator.GetService<MessageBusService>()?.RegisterAllSubscriber(this);
        }

        private void PlacePlayerOnWorld(Player player)
        {
            if (player != null)
            {
                try
                {
                    Civilization? startTown = WorldMap.Civilizations
                        .Find(a => a.Tendency == CivilizationTendency.Neutral);
                    if (startTown is not null)
                        player.Position = startTown.ReturnAllLandTerritory(WorldMap.AssocietatedMap)[0].Position;
                    else
                        throw new ApplicationException("There was an error on trying to find the start town to place the player!");
                    Player = player;

                    CurrentMap.AddMagiEntity(Player);
                }
                catch (ApplicationException ex)
                {
                    Locator.GetService<MagiLog>().Log(ex.Message);
                    throw;
                }
                catch (Exception e)
                {
                    Locator.GetService<MagiLog>().Log(e.Message);
                    throw;
                }
            }
        }

        public void PlacePlayerOnLoad()
        {
            if (Player != null)
            {
                // since we are just loading from memory, better make sure!
                CurrentMap.NeedsUpdate = true;
                Player.Position = CurrentMap.LastPlayerPosition;
                UpdateIfNeedTheMap(CurrentMap);
                CurrentMap.AddMagiEntity(Player);
            }
            else
            {
                throw new Exception("Coudn't load the player, it was null!");
            }
        }

        private void ChangePlayerMap(MagiMap mapToGo, Point pos, MagiMap? previousMap)
        {
            if (CurrentMap is not null)
            {
                CurrentMap.LastPlayerPosition = new Point(Player.Position.X, Player.Position.Y);
            }

            ChangeActorMap(Player, mapToGo, pos, previousMap);
            UpdateIfNeedTheMap(mapToGo);
            CurrentMap = mapToGo;

            if (previousMap is not null)
            {
                previousMap.NeedsUpdate = true;
                previousMap.ControlledEntitiy = null!;
            }

            // transform into event
            Locator.GetService<MessageBusService>().SendMessage<LoadMapMessage>(new(CurrentMap));
            Locator.GetService<MessageBusService>().SendMessage<ChangeCenteredActor>(new(Player));
        }

        private void UpdateIfNeedTheMap(MagiMap mapToGo)
        {
            if (mapToGo != WorldMap.AssocietatedMap && !CurrentChunk.MapsAreConnected())
                MapGenerator.ConnectMapsInsideChunk(CurrentChunk.LocalMaps);
            if (mapToGo.NeedsUpdate && mapToGo != WorldMap.AssocietatedMap)
            {
                // if the map is now updated, then no need to change anything!
                mapToGo.NeedsUpdate = false;
                mapToGo.UpdateRooms();
                //mapToGo.UpdatePathfinding();
                var ids = GetEntitiesIds();
                RegisterInTime(ids);
            }
        }

        public void AddEntityToCurrentMap(MagiEntity entity)
        {
            CurrentMap.AddMagiEntity(entity);
            AddEntityToTime(entity);
        }

        public static void ChangeActorMap(MagiEntity entity, MagiMap mapToGo, Point pos, MagiMap? previousMap)
        {
            previousMap?.RemoveMagiEntity(entity);
            entity.Position = pos;
            mapToGo.AddMagiEntity(entity);
        }

        public void SaveGame(string saveName)
        {
            Locator.GetService<SavingService>().SaveGameToFolder(this, saveName);
        }

        /*/// <summary>
        /// Sets up anything that needs to be set up after map gen and after placing entities, like
        /// the nodes turn system </summary>
        private void SetUpStuff(Map map)
        {
            foreach (NodeTile node in map.Tiles.OfType<NodeTile>())
            {
                node.SetUpNodeTurn(this);
            }
        }*/

        private void CreateTestMap()
        {
            MiscMapGen generalMapGenerator = new();
            MagiMap map = generalMapGenerator.GenerateTestMap();
            CurrentMap = map;
            CurrentChunk = new RegionChunk(0, 0);
            CurrentChunk.LocalMaps[0] = map;
        }

        // Create a player using the Player class and set its starting position
        private void PlacePlayer(Player player)
        {
            // Place the player on the first non-movement-blocking tile on the map
            for (int i = 0; i < CurrentMap.Terrain.Count; i++)
            {
                if (CurrentMap.Terrain[i]?.IsWalkable == true
                    && !CurrentMap.GetEntitiesAt<MagiEntity>(Point.FromIndex(i, CurrentMap.Width)).Any())
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
            CurrentMap.AddMagiEntity(Player);
        }

        public MagiMap GetWorldMap()
        {
            return WorldMap.AssocietatedMap;
        }

        private void AddEntityToTime(MagiEntity entity, int time = 0)
        {
            // register to next turn
            if (!Time.Nodes.Cast<EntityTimeNode>().Any(i => i.EntityId.Equals(entity.ID)))
                Time.RegisterEntity(new EntityTimeNode(entity.ID, Time.GetTimePassed(time)));
        }

        private void ProcessTurn(long playerTime, bool sucess, bool playerCanAct)
        {
            if (sucess)
            {
                bool playerActionWorked = ProcessPlayerTurn(playerTime);

                if (!playerActionWorked)
                    return;

                // here the player has done it's turn, so let's go to the next one
                var turnNode = Time.NextNode();
                // put here terrrain effect
                const int maxTries = 1000;
                int tries = 0;
                while (turnNode is not PlayerTimeNode)
                {
                    switch (turnNode)
                    {
                        case EntityTimeNode entityTurn:
                            bool canDoAnotherAction = ProcessEffects(entityTurn.EntityId);
                            if (canDoAnotherAction)
                                ProcessAiTurn(entityTurn.EntityId);
                            break;

                        default:
                            throw new NotSupportedException($"Unhandled time node type: {turnNode.GetType()}");
                    }

                    turnNode = Time.NextNode();
                    if (tries++ > maxTries)
                    {
                        Locator.GetService<MessageBusService>()
                            .SendMessage<AddMessageLog>(new("Something went really wrong with the processing of the AI"));
                        break;
                    }
                }
#if DEBUG
                // events
                Locator.GetService<MessageBusService>()
                    .SendMessage<AddMessageLog>(new($"Turns: {Time.Turns}, Tick: {Time.TimePassed.Ticks}"));
#endif
                // makes sure that any entity that exists but has no AI, or the AI failed, get's a turn.
                var ids = GetEntitiesIds();
                RegisterInTime(ids);
            }
        }

        private IEnumerable<Actor> GetEntitiesIds()
        {
            return CurrentChunk?.TotalPopulation() ?? Enumerable.Empty<Actor>();
        }

        private void RegisterInTime(IEnumerable<Actor> population)
        {
            // called only once, to properly register the entity
            if (population is null)
                return;
            foreach (Actor actor in population)
            {
                AddEntityToTime(actor);
            }
        }

        /// <summary>
        /// The player turn handler
        /// </summary>
        /// <param name="playerTime"></param>
        /// <returns>returns true if the player made it's action sucessfully, false if otherwise</returns>
        private bool ProcessPlayerTurn(long playerTime)
        {
            if (Player.CheckIfDed())
            {
                KillPlayer();
                RestartGame();
                return false;
            }

            Player.ProcessNeeds();
            PlayerTimeNode playerTurn = new(Time.GetTimePassed(playerTime));
            Time.RegisterEntity(playerTurn);
            Player.GetAnatomy().UpdateBody(Player);
            CurrentMap.PlayerFOV.Calculate(Player.Position, Player.GetViewRadius());

            return true;
        }

        private void KillPlayer()
        {
            var figure = Find.GetFigureById(Player.HistoryId);
            figure.KillIt(WorldMap.WorldHistory.Year, Find.PlayerDeathReason);
        }

        private void RestartGame()
        {
            CurrentMap.DestroyMap();
            int chunckLenght = (CurrentChunk?.LocalMaps.Length) ?? 0;
            for (int i = 0; i < chunckLenght; i++)
            {
                MagiMap maps = CurrentChunk?.LocalMaps[i];
                maps?.DestroyMap();
            }
            CurrentMap = null!;
            Player = null!;
            CurrentChunk = null!;
            WorldMap = null!;

            // event
            //GameLoop.UIManager.MainMenu.RestartGame();
            Locator.GetService<MessageBusService>().SendMessage<RestartGame>(new());
        }

        private void ProcessAiTurn(uint entityId)
        {
            Actor entity = (Actor)CurrentMap.GetEntityById(entityId);

            if (entity != null)
            {
                var ais = entity.GetComponents<IAiComponent>();
                // sucess and failure will have to have some sort of aggregate function made later!
                int sucesses = 0;
                long totalTicks = 0;
                foreach (var ai in ais)
                {
                    (bool sucess, long tick) = ai?.RunAi(CurrentMap)
                        ?? (false, -1);
                    if (sucess)
                    {
                        sucesses++;
                        totalTicks += tick;
                    }
                }

                entity.ProcessNeeds();
                entity.GetAnatomy().UpdateBody(entity);

                if (sucesses > 0 && totalTicks < -1)
                    return;

                EntityTimeNode nextTurnNode = new(entityId, Time.GetTimePassed(totalTicks));
                Time.RegisterEntity(nextTurnNode);
            }
        }

        private void ChangeControlledEntity(MagiEntity entity)
        {
            CurrentMap.ControlledEntitiy = entity;
            // event
            Locator.GetService<MessageBusService>().SendMessage<ChangeCenteredActor>(new(entity));
        }

        public RegionChunk GenerateChunck(Point posGenerated)
        {
            RegionChunk newChunck = new(posGenerated);

            WildernessGenerator genMap = new();
            newChunck.LocalMaps = genMap.GenerateMapWithWorldParam(WorldMap, posGenerated);

            for (int i = 0; i < newChunck.LocalMaps.Length; i++)
            {
                newChunck.LocalMaps[i].SetId(Locator.GetService<IDGenerator>().UseID());
            }

            return newChunck;
        }

        public RegionChunk? GetChunckByPos(Point playerPoint)
        {
            return Locator.GetService<SavingService>().GetChunkAtIndex(playerPoint, PlanetSettings.PlanetWidth);
        }

        public static MagiMap? GetMapById(int id)
        {
            return SavingService.LoadMapById(id);
        }

        public bool MapIsWorld()
        {
            return CurrentMap == WorldMap.AssocietatedMap;
        }

        public bool MapIsWorld(MagiMap map)
        {
            return map == WorldMap.AssocietatedMap;
        }

        public void ForceChangeCurrentMap(MagiMap map) => CurrentMap = map;

        public void Handle(ChangeControlledActorMap message)
        {
            ChangePlayerMap(message.Map, message.PosInMap, CurrentMap);
        }

        public void Handle(AddEntitiyCurrentMap message)
        {
            CurrentMap?.AddMagiEntity(message.Entitiy);
        }

        public void Handle(ProcessTurnEvent message)
        {
            ProcessTurn(message.Time, message.Sucess);
        }

        public void Handle(RemoveEntitiyCurrentMap message)
        {
            CurrentMap?.RemoveMagiEntity(message.Entity);
        }

        public void Handle(ChangeControlledEntitiy message)
        {
            ChangeControlledEntity(message.ControlledEntitiy);
        }

        ~Universe()
        {
            Locator.GetService<MessageBusService>().UnRegisterAllSubscriber(this);
        }
    }
}
