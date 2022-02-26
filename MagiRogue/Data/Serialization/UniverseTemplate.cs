using MagiRogue.Entities;
using MagiRogue.System;
using MagiRogue.System.Planet;
using MagiRogue.System.Time;
using MagiRogue.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;

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
            /*JObject universe = JObject.Load(reader);
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
            MagiGlobalRandom rng =
                JsonConvert.DeserializeObject<MagiGlobalRandom>(universe["MagiRandom"].ToString());

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
                player, time, changeMap, season, regionChunk, rng);

            return objUni;*/

            /*while (reader.Read())
            {
                // i think i found a way here
                if (reader.TokenType == JsonToken.StartObject)
                {
                    var c = serializer.Deserialize<PlanetMap>(reader);
                }
            }*/
            return serializer.Deserialize<UniverseTemplate>(reader);
        }

        public override void WriteJson(JsonWriter writer, Universe value, JsonSerializer serializer)
        {
            // Make it only referrence other maps, like all chunks containing only the id to a map
            serializer.NullValueHandling = NullValueHandling.Ignore;
            serializer.Serialize(writer, (UniverseTemplate)value);
        }
    }

    [DataContract]
    public class UniverseTemplate
    {
        /// <summary>
        /// The World map, contains the map data and the Planet data
        /// </summary>
        // TODO: Separate it so that it searchs for the world map in another file
        [DataMember]
        [JsonProperty(ItemConverterType = typeof(PlanetMapJsonConverter))]
        public PlanetMap WorldMap { get; set; }

        /// <summary>
        /// Stores the current map
        /// </summary>
        [DataMember]
        [JsonProperty(ItemConverterType = typeof(MapJsonConverter))]
        public Map CurrentMap { get; private set; }

        /// <summary>
        /// The current chunk of the player
        /// </summary>
        [DataMember]
        [JsonProperty(ItemConverterType = typeof(RegionChunkJsonConverter))]
        public RegionChunk CurrentChunk { get; set; }

        /// <summary>
        /// Player data
        /// </summary>
        [DataMember]
        public Actor Player { get; set; }

        [DataMember]
        public TimeTemplate Time { get; private set; }

        [DataMember]
        public bool PossibleChangeMap { get; internal set; } = true;

        [DataMember]
        public string CurrentSeason { get; set; }

        /// <summary>
        /// All the maps and chunks of the game
        /// </summary>
        [JsonIgnore]
        public RegionChunk[] AllChunks { get; set; }

        [DataMember]
        public int[] AllChunksIds { get; set; }

        public UniverseTemplate(PlanetMap worldMap,
            Map currentMap,
            Player player,
            TimeTemplate time,
            bool possibleChangeMap,
            SeasonType currentSeason,
            RegionChunk[] allChunks)
        {
            WorldMap = worldMap;
            if (currentMap is not null && worldMap.AssocietatedMap.MapId == currentMap.MapId)
                CurrentMap = worldMap.AssocietatedMap;
            else
                CurrentMap = currentMap;

            Player = player;
            Time = time;
            PossibleChangeMap = possibleChangeMap;
            CurrentSeason = currentSeason.ToString();
            AllChunks = allChunks;
            if (AllChunks is not null)
            {
                AllChunksIds = new int[allChunks.Length];
                for (int i = 0; i < AllChunks.Length; i++)
                {
                    AllChunksIds[i] = i;
                }
            }
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
            UniverseTemplate universe = new UniverseTemplate(
                uni.WorldMap, uni.CurrentMap, uni.Player,
                uni.Time, uni.PossibleChangeMap,
                uni.CurrentSeason, uni.AllChunks)
            {
                CurrentChunk = uni.CurrentChunk
            };
            return universe;
        }

        public static implicit operator Universe(UniverseTemplate uni)
        {
            Universe universe = new Universe(uni.WorldMap, uni.CurrentMap,
                Entities.Player.ReturnPlayerFromActor(uni.Player),
                uni.Time, uni.PossibleChangeMap,
                SeasonEnumToString(uni.CurrentSeason), uni.AllChunks);

            return universe;
        }
    }
}