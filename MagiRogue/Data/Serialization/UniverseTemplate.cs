using MagiRogue.Data.Enumerators;
using MagiRogue.Data.Serialization.MapSerialization;
using MagiRogue.Entities;
using MagiRogue.GameSys;
using MagiRogue.GameSys.Planet;
using MagiRogue.Settings;
using Newtonsoft.Json;
using System;

namespace MagiRogue.Data.Serialization
{
    public class UniverseJsonConverter : JsonConverter<Universe>
    {
        public override Universe ReadJson(JsonReader reader,
            Type objectType,
            Universe? existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            return serializer.Deserialize<UniverseTemplate>(reader);
        }

        public override void WriteJson(JsonWriter writer, Universe value, JsonSerializer serializer)
        {
            // Make it only referrence other maps, like all chunks containing only the id to a map
            serializer.NullValueHandling = NullValueHandling.Ignore;
            serializer.Serialize(writer, (UniverseTemplate)value);
        }
    }

    public class UniverseTemplate
    {
        /// <summary>
        /// The World map, contains the map data and the Planet data
        /// </summary>
        // TODO: Separate it so that it searchs for the world map in another file
        [JsonProperty(ItemConverterType = typeof(PlanetMapJsonConverter))]
        public PlanetMap WorldMap { get; set; }

        /// <summary>
        /// Stores the current map
        /// </summary>
        [JsonProperty(ItemConverterType = typeof(MapJsonConverter))]
        public Map CurrentMap { get; private set; }

        /// <summary>
        /// The current chunk of the player
        /// </summary>
        [JsonProperty(ItemConverterType = typeof(RegionChunkJsonConverter))]
        public RegionChunk CurrentChunk { get; set; }

        /// <summary>
        /// Player data
        /// </summary>
        public Actor Player { get; set; }

        public TimeTemplate Time { get; }

        public bool PossibleChangeMap { get; internal set; } = true;

        public string CurrentSeason { get; set; }

        public SaveAndLoad SaveAndLoad { get; set; }

        public uint LastIdAssigned { get; set; }

        public int ZLevel { get; set; }

        public EntityRegistry Registry { get; set; }

        public PlanetGenSettings PlanetSettings { get; set; }

        public UniverseTemplate(PlanetMap worldMap,
            Map currentMap,
            Player player,
            TimeTemplate time,
            bool possibleChangeMap,
            SeasonType currentSeason)
        {
            WorldMap = worldMap;
            if (currentMap is not null && worldMap.AssocietatedMap.MapId == currentMap.MapId)
                CurrentMap = worldMap.AssocietatedMap;
            else
                CurrentMap = currentMap!;

            Player = player;
            Time = time;
            PossibleChangeMap = possibleChangeMap;
            CurrentSeason = currentSeason.ToString();
        }

        public UniverseTemplate()
        {
            // empty one
        }

        private static SeasonType SeasonEnumToString(string v)
        {
            return v switch
            {
                "Spring" => SeasonType.Spring,
                "Summer" => SeasonType.Summer,
                "Autumn" => SeasonType.Autumn,
                "Winter" => SeasonType.Winter,
                _ => throw new Exception($"Tried to deseralize an invalid Season! Data {v}")
            };
        }

        public static implicit operator UniverseTemplate(Universe uni)
        {
            return new UniverseTemplate(
                uni.WorldMap, uni.CurrentMap, uni.Player,
                uni.Time, uni.PossibleChangeMap,
                uni.CurrentSeason)
            {
                CurrentChunk = uni.CurrentChunk,
                SaveAndLoad = uni.SaveAndLoad,
                LastIdAssigned = GameLoop.IdGen.CurrentInteger,
                ZLevel = uni.ZLevel,
                Registry = uni.Registry,
                PlanetSettings = uni.PlanetSettings,
            };
        }

        public static implicit operator Universe(UniverseTemplate uni)
        {
            Universe universe = new Universe(uni.WorldMap, uni.CurrentMap,
                Entities.Player.ReturnPlayerFromActor(uni.Player),
                uni.Time, uni.PossibleChangeMap,
                SeasonEnumToString(uni.CurrentSeason), uni.SaveAndLoad, uni.CurrentChunk, uni.Registry)
            {
                ZLevel = uni.ZLevel,
                PlanetSettings = uni.PlanetSettings,
            };
            GameLoop.SetIdGen(uni.LastIdAssigned);

            return universe;
        }
    }
}