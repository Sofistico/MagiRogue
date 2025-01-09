using Arquimedes.Enumerators;
using Arquimedes.Settings;
using MagusEngine.Core.Entities;
using MagusEngine.Core.MapStuff;
using MagusEngine.Exceptions;
using MagusEngine.Serialization.MapConverter;
using MagusEngine.Systems;
using Newtonsoft.Json;
using SadRogue.Primitives;
using System;

namespace MagusEngine.Serialization
{
    public class UniverseJsonConverter : JsonConverter<Universe>
    {
        public override Universe ReadJson(JsonReader reader,
            Type objectType,
            Universe? existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            return serializer.Deserialize<UniverseTemplate>(reader)!;
        }

        public override void WriteJson(JsonWriter writer, Universe? value, JsonSerializer serializer)
        {
            // Make it only referrence other maps, like all chunks containing only the id to a map
            serializer.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            serializer.Serialize(writer, (UniverseTemplate?)value!);
        }
    }

    public class UniverseTemplate
    {
        [JsonRequired]
        public PlanetMap WorldMap { get; set; } = null!;

        /// <summary>
        /// Stores the current map
        /// </summary>
        [JsonProperty(ItemConverterType = typeof(MapJsonConverter))]
        public MagiMap CurrentMap { get; set; } = null!;

        /// <summary>
        /// The current chunk of the player
        /// </summary>
        [JsonProperty(ItemConverterType = typeof(RegionChunkJsonConverter))]
        public RegionChunk CurrentChunk { get; set; } = null!;

        public Actor Player { get; set; } = null!;

        public TimeTemplate Time { get; set; } = null!;

        public bool PossibleChangeMap { get; set; } = true;

        public string CurrentSeason { get; set; } = null!;

        public uint LastIdAssigned { get; set; }

        public PlanetGenSettings PlanetSettings { get; set; } = null!;

        public UniverseTemplate(PlanetMap worldMap,
            MagiMap currentMap,
            Player player,
            TimeTemplate time,
            bool possibleChangeMap,
            SeasonType currentSeason)
        {
            WorldMap = worldMap;
            CurrentMap = currentMap is not null && worldMap.AssocietatedMap.MapId == currentMap.MapId ? worldMap.AssocietatedMap : currentMap!;
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
                _ => throw new NullValueException($"Tried to deseralize an invalid Season! Data {v}")
            };
        }

        public static implicit operator UniverseTemplate(Universe uni)
        {
            return new UniverseTemplate(
                uni.WorldMap, uni!.CurrentMap!, uni.Player,
                uni.Time, uni.PossibleChangeMap,
                uni.CurrentSeason)
            {
                CurrentChunk = uni.CurrentChunk,
                LastIdAssigned = Locator.GetService<IDGenerator>().CurrentInteger,
                PlanetSettings = uni.PlanetSettings!,
            };
        }

        public static implicit operator Universe(UniverseTemplate uni)
        {
            Universe universe = new(uni.WorldMap, uni.CurrentMap,
                MagusEngine.Core.Entities.Player.ReturnPlayerFromActor(uni.Player)!,
                uni.Time, uni.PossibleChangeMap,
                SeasonEnumToString(uni.CurrentSeason), uni.CurrentChunk)
            {
                PlanetSettings = uni.PlanetSettings,
            };
            Locator.AddService<IDGenerator>(new(uni.LastIdAssigned));

            return universe;
        }
    }
}
