using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Arquimedes.Enumerators;
using Arquimedes.Settings;
using Arquimedes.Utils;
using GoRogue.Messaging;
using MagusEngine.Bus;
using MagusEngine.Bus.MapBus;
using MagusEngine.Bus.UiBus;
using MagusEngine.Components.EntityComponents.Ai;
using MagusEngine.Core.Entities;
using MagusEngine.Core.Entities.Base;
using MagusEngine.Core.MapStuff;
using MagusEngine.Exceptions;
using MagusEngine.Generators;
using MagusEngine.Generators.MapGen;
using MagusEngine.Serialization;
using MagusEngine.Services;
using MagusEngine.Systems.Time;
using MagusEngine.Systems.Time.Nodes;
using Newtonsoft.Json;
using SadRogue.Primitives;

namespace MagusEngine.Systems
{
    /// <summary>
    /// All game state data is stored in the Universe also creates and processes generators for map creation
    /// </summary>
    // TODO: Break down the universe in more discrete systems
    [JsonConverter(typeof(UniverseJsonConverter))]
    public sealed class Universe :
        ISubscriber<ChangeControlledActorMap>,
        ISubscriber<ChangeControlledEntitiy>,
        ISubscriber<AddEntitiyCurrentMap>,
        ISubscriber<ProcessTurnEvent>,
        ISubscriber<RemoveEntitiyCurrentMap>
    {
        private readonly MessageBusService _messageBus;
        private readonly SavingService _savingService;
        private readonly IDGenerator _idGenerator;

        /// <summary>
        /// The World map, contains the map data and the Planet data
        /// </summary>
        public PlanetMap WorldMap { get; set; } = null!;

        /// <summary>
        /// Stores the current map
        /// </summary>
        public MagiMap? CurrentMap { get; private set; }

        /// <summary>
        /// Stores the current chunk that is loaded
        /// </summary>
        public RegionChunk CurrentChunk { get; set; } = null!;

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

        public PlanetGenSettings PlanetSettings { get; set; } = null!;

        private Universe()
        {
            _messageBus = Locator.GetService<MessageBusService>();
            _messageBus.RegisterAllSubscriber(this);
            _savingService = Locator.GetService<SavingService>();
            _idGenerator = Locator.GetService<IDGenerator>();
            Player = null!;
            Time = null!;
        }

        /// <summary>
        /// Creates a new game world and stores it in a publicly accessible constructor.
        /// </summary>
        public Universe(Player player, bool testGame = false) : this()
        {
            Time = new TimeSystem();
            CurrentSeason = SeasonType.Spring;
            PlanetSettings = JsonUtils.JsonDeseralize<PlanetGenSettings>(Path
                .Combine(AppDomain.CurrentDomain.BaseDirectory,
                "Settings",
                "planet_gen_setting.json"))!;

            Player = player ?? throw new NullValueException(nameof(player));

            if (!testGame)
            {
                WorldMap = new PlanetGenerator().CreatePlanet(PlanetSettings!.PlanetWidth,
                    PlanetSettings!.PlanetHeight,
                    PlanetSettings!.PlanetMaxInitialCivs);
                Time = WorldMap.GetTimePassed();
                WorldMap.PlacePlayerOnWorld(player);
                CurrentMap = WorldMap.AssocietatedMap;
                SaveGame(player.Name);
            }
            else
            {
                CreateTestMap();
            }
        }

        [JsonConstructor]
        public Universe(PlanetMap worldMap,
            MagiMap currentMap,
            Player player,
            TimeSystem time,
            bool possibleChangeMap,
            SeasonType currentSeason,
            RegionChunk currentChunk) : this()
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
        }

        public void PlacePlayerOnLoad()
        {
            if (Player != null)
            {
                // since we are just loading from memory, better make sure!
                CurrentMap!.NeedsUpdate = true;
                Player.Position = CurrentMap.LastPlayerPosition;
                UpdateIfNeedTheMap(CurrentMap);
                CurrentMap.AddMagiEntity(Player);
            }
            else
            {
                throw new NullValueException("Coudn't load the player, it was null!", null);
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
            _messageBus.SendMessage<LoadMapMessage>(new(CurrentMap));
            _messageBus.SendMessage<ChangeCenteredActor>(new(Player));
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
            CurrentMap?.AddMagiEntity(entity);
            _messageBus.SendMessage(new AddTurnNode(new EntityTimeNode(entity.ID, 0)));
        }

        public static void ChangeActorMap(MagiEntity entity, MagiMap mapToGo, Point pos, MagiMap? previousMap)
        {
            previousMap?.RemoveMagiEntity(entity);
            entity.Position = pos;
            mapToGo.AddMagiEntity(entity);
        }

        public void SaveGame(string saveName)
        {
            _savingService.SaveGameToFolder(this, saveName);
        }

        private void CreateTestMap()
        {
            MiscMapGen generalMapGenerator = new();
            MagiMap map = generalMapGenerator.GenerateTestMap();
            CurrentMap = map;
            CurrentChunk = new RegionChunk(0, 0);
            CurrentChunk.LocalMaps[0] = map;
            PlacePlayer();
        }

        // Create a player using the Player class and set its starting position
        private void PlacePlayer()
        {
            // Place the player on the first non-movement-blocking tile on the map
            for (int i = 0; i < CurrentMap?.Terrain.Count; i++)
            {
                if (CurrentMap.Terrain[i]?.IsWalkable == true
                    && !CurrentMap.GetEntitiesAt<MagiEntity>(Point.FromIndex(i, CurrentMap.Width)).Any())
                {
                    // Set the player's position to the index of the current map position
                    var pos = Point.FromIndex(i, CurrentMap.Width);

                    Player.Position = pos;
                    break;
                }
            }

            // add the player to the Map's collection of Entities
            CurrentMap?.AddMagiEntity(Player);
        }

        public MagiMap GetWorldMap()
        {
            return WorldMap.AssocietatedMap;
        }

        private void ProcessTurn(long playerTime, bool sucess)
        {
            if (sucess)
            {
                bool playerIsAlive = ProcessPlayerTurn(playerTime);

                if (!playerIsAlive)
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
                            ProcessAiTurn(entityTurn.Id);
                            break;

                        case TickActionNode componentTurn:
                            ProcessComponentTurn(componentTurn);
                            break;

                        default:
                            throw new NotSupportedException($"Unhandled time node type: {turnNode?.GetType()}");
                    }
                    turnNode = Time.NextNode();
                    if (tries++ > maxTries)
                    {
                        _messageBus.SendMessage<AddMessageLog>(new("Something went really wrong with the processing of the AI"));
                        break;
                    }
                    _messageBus.SendMessage<MapConsoleIsDirty>();
                }
#if DEBUG
                _messageBus.SendMessage<AddMessageLog>(new($"Turns: {Time.Turns}, Tick: {Time.TimePassed.Ticks}"));
#endif
                // makes sure that any entity that exists but has no AI, or the AI failed, get's a turn.
                var ids = GetEntitiesIds();
                RegisterInTime(ids);
            }
        }

        private void ProcessComponentTurn(TickActionNode componentTurn)
        {
            var nextInvoke = componentTurn.Action.Invoke();
            if (nextInvoke > 0)
            {
                var tickActionNode = new TickActionNode(nextInvoke, componentTurn.Id, componentTurn.Action);
                _messageBus.SendMessage(new AddTurnNode(tickActionNode));
            }
        }

        private IEnumerable<Actor> GetEntitiesIds()
        {
            return CurrentChunk?.TotalPopulation() ?? [];
        }

        private void RegisterInTime(IEnumerable<Actor> population)
        {
            // called only once, to properly register the entity
            if (population is null)
                return;
            foreach (Actor actor in population)
            {
                _messageBus.SendMessage(new AddTurnNode(new EntityTimeNode(actor.ID, 0)));
            }
        }

        /// <summary>
        /// The player turn handler
        /// </summary>
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
            PlayerTimeNode playerTurn = new(playerTime, Player.ID);
            _messageBus.SendMessage<AddTurnNode>(new(playerTurn));
            Player.UpdateBody();
            CurrentMap?.PlayerFOV.Calculate(Player.Position, Player.GetViewRadius());

            return true;
        }

        private void KillPlayer()
        {
            var figure = Find.GetFigureById(Player.HistoryId);
            figure?.KillIt(WorldMap.WorldHistory.Year, Find.PlayerDeathReason);
        }

        private void RestartGame()
        {
            CurrentMap?.DestroyMap();
            int chunckLenght = (CurrentChunk?.LocalMaps.Length) ?? 0;
            for (int i = 0; i < chunckLenght; i++)
            {
                MagiMap? maps = CurrentChunk?.LocalMaps[i];
                maps?.DestroyMap();
            }
            CurrentMap = null!;
            Player = null!;
            CurrentChunk = null!;
            WorldMap = null!;

            // event
            _messageBus.SendMessage<RestartGame>(new());
        }

        private void ProcessAiTurn(uint entityId)
        {
            Actor? entity = (Actor?)CurrentMap?.GetEntityById(entityId);

            if (entity?.State.HasFlag(ActorState.Uncontrolled) == false)
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
                entity.UpdateBody();

                if (sucesses >= 0 && totalTicks <= 0)
                {
                    totalTicks = TimeHelper.Wait;
                    MagiLog.Log($"The {entity.Name} with id {entityId} has reported no success on AI");
                }

                EntityTimeNode nextTurnNode = new(entityId, totalTicks);
                _messageBus.SendMessage<AddTurnNode>(new(nextTurnNode));
            }
        }

        private void ChangeControlledEntity(MagiEntity? entity)
        {
            CurrentMap!.ControlledEntitiy = entity;
            // event
            _messageBus.SendMessage<ChangeCenteredActor>(new(entity!));
        }

        public RegionChunk GenerateChunck(Point posGenerated)
        {
            RegionChunk newChunck = new(posGenerated);

            WildernessGenerator genMap = new();
            newChunck.LocalMaps = genMap.GenerateMapWithWorldParam(WorldMap, posGenerated);

            for (int i = 0; i < newChunck.LocalMaps.Length; i++)
            {
                newChunck.LocalMaps[i].SetId(_idGenerator.UseID());
            }

            return newChunck;
        }

        public RegionChunk? GetChunckByPos(Point playerPoint)
        {
            return _savingService.GetChunkAtIndex(playerPoint, PlanetSettings!.PlanetWidth);
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
            _messageBus.UnRegisterAllSubscriber(this);
        }
    }
}
