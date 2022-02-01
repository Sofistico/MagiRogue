using MagiRogue.Entities;
using MagiRogue.System;
using MagiRogue.System.Planet;
using MagiRogue.System.Time;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;

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
            JObject universe = JObject.Load(reader);
            UniverseTemplate objUni;
            MapTemplate currentMap;

            PlanetMapTemplate planet = JsonConvert.DeserializeObject<PlanetMap>
                (universe["WorldMap"].ToString());
            int id = (int)universe["CurrentMap"]["MapId"];
            if (id == planet.AssocietatedMap.MapId)
            {
                currentMap = planet.AssocietatedMap;
            }
            else
            {
                 currentMap = JsonConvert.DeserializeObject<Map>
                    (universe["CurrentMap"].ToString());
            }


            Player player = Player.ReturnPlayerFromActor
                (JsonConvert.DeserializeObject<ActorTemplate>(universe["Player"].ToString()));

            TimeTemplate time =
                JsonConvert.DeserializeObject<TimeSystem>(universe["Time"].ToString());
            bool changeMap = (bool)universe["PossibleChangeMap"];
            SeasonType season = SeasonEnumToString(universe["CurrentSeason"].ToString());
            JToken[] chunks = universe["AllChunks"].ToArray();
            RegionChunkTemplate[] regionChunk = new RegionChunkTemplate[chunks.Length];

            for (int i = 0; i < chunks.Length; i++)
            {
                if (!string.IsNullOrWhiteSpace(chunks[i].ToString()))
                {
                    RegionChunkTemplate region =
                        JsonConvert.DeserializeObject<RegionChunk>(chunks[i].ToString());
                    regionChunk[i] = region;
                }
            }

            objUni = new UniverseTemplate(planet, currentMap,
                player, time, changeMap, season, regionChunk);

            return objUni;
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

        public override void WriteJson(JsonWriter writer, Universe value, JsonSerializer serializer)
        {
            // Make it only referrence other maps, like all chunks containing only the id to a map
            serializer.NullValueHandling = NullValueHandling.Ignore;
            serializer.Formatting = Formatting.Indented;
            serializer.Serialize(writer, (UniverseTemplate)value);
        }
    }

    public class UniverseTemplate
    {
        /// <summary>
        /// The World map, contains the map data and the Planet data
        /// </summary>
        // TODO: Separate it so that it searchs for the world map in another file
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
            if (currentMap is not null && worldMap.AssocietatedMap.MapId == currentMap.MapId)
                CurrentMap = worldMap.AssocietatedMap;
            else
                CurrentMap = currentMap;

            Player = player;
            Time = time;
            PossibleChangeMap = possibleChangeMap;
            CurrentSeason = currentSeason;
            AllChunks = allChunks;
        }

        public UniverseTemplate()
        {
            // empty one
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