using MagiRogue.Entities;
using MagiRogue.System;
using MagiRogue.System.Planet;
using MagiRogue.System.Time;
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
                => serializer.Deserialize<UniverseTemplate>(reader);

        public override void WriteJson(JsonWriter writer, Universe value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, (UniverseTemplate)value);
        }
    }

    public class UniverseTemplate
    {
        /// <summary>
        /// The World map, contains the map data and the Planet data
        /// </summary>
        public PlanetMapTemplate WorldMap { get; set; }

        /// <summary>
        /// Stores the current map
        /// </summary>
        public MapTemplate CurrentMap { get; private set; }

        /// <summary>
        /// Player data
        /// </summary>
        public Player Player { get; set; }

        public TimeTemplate Time { get; private set; }
        public bool PossibleChangeMap { get; internal set; } = true;

        public SeasonType CurrentSeason { get; set; }

        /// <summary>
        /// All the maps and chunks of the game
        /// </summary>
        public RegionChunkTemplate[] AllChunks { get; set; }

        public UniverseTemplate(PlanetMap worldMap,
            Map currentMap,
            Player player,
            TimeTemplate time,
            bool possibleChangeMap,
            SeasonType currentSeason,
            RegionChunkTemplate[] allChunks)
        {
            WorldMap = worldMap;
            CurrentMap = currentMap;
            Player = player;
            Time = time;
            PossibleChangeMap = possibleChangeMap;
            CurrentSeason = currentSeason;
            AllChunks = allChunks;
        }

        public static implicit operator UniverseTemplate(Universe uni)
        {
            UniverseTemplate universe = new UniverseTemplate(
                uni.WorldMap, uni.CurrentMap, uni.Player,
                uni.Time, uni.PossibleChangeMap,
                uni.CurrentSeason, uni.AllChunks);
            return universe;
        }

        public static implicit operator Universe(UniverseTemplate uni)
        {
            Universe universe = new Universe(uni.WorldMap, uni.CurrentMap, uni.Player,
                uni.Time, uni.PossibleChangeMap,
                uni.CurrentSeason, uni.AllChunks);

            return universe;
        }
    }
}