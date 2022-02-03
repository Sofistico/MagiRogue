using MagiRogue.System;
using Newtonsoft.Json;
using System;

namespace MagiRogue.Data.Serialization
{
    public class RegionChunkJsonConverter : JsonConverter<RegionChunk>
    {
        public override RegionChunk? ReadJson(JsonReader reader,
            Type objectType, RegionChunk? existingValue,
            bool hasExistingValue, JsonSerializer serializer)
        {
            return serializer.Deserialize<RegionChunkTemplate>(reader);
        }

        public override void WriteJson(JsonWriter writer,
            RegionChunk? value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, (RegionChunkTemplate)value);
            writer.Flush();
        }
    }

    public class RegionChunkTemplate
    {
        public int X { get; }
        public int Y { get; }
        public Map[] LocalMaps { get; set; }
        public uint[] LocalMapsIds { get; set; }

        public RegionChunkTemplate(int x, int y, Map[] localMaps)
        {
            X = x;
            Y = y;
            LocalMaps = localMaps;
            LocalMapsIds = new uint[LocalMaps.Length];
            for (int i = 0; i < LocalMaps.Length; i++)
            {
                LocalMapsIds[i] = LocalMaps[i].MapId;
            }
        }

        public Map[] ReturnAsMap()
        {
            var map = new Map[LocalMaps.Length];
            for (int i = 0; i < LocalMaps.Length; i++)
            {
                map[i] = LocalMaps[i];
            }

            return map;
        }

        public static implicit operator RegionChunkTemplate(RegionChunk region)
        {
            Map[] maps = new Map[region.LocalMaps.Length];
            for (int i = 0; i < region.LocalMaps.Length; i++)
            {
                maps[i] = region.LocalMaps[i];
            }
            var chunk = new RegionChunkTemplate(region.X, region.Y, maps);

            return chunk;
        }

        public static implicit operator RegionChunk(RegionChunkTemplate template)
        {
            if (template is null)
                return null;
            var regio = new RegionChunk(template.X, template.Y)
            {
                LocalMaps = template.ReturnAsMap()
            };
            return regio;
        }
    }
}