using Newtonsoft.Json;
using System.IO;

namespace MagiRogue.Utils
{
    public static class JsonUtils
    {
        public static T JsonDeseralize<T>(string stream) => JsonConvert.DeserializeObject<T>(File.ReadAllText(stream));

        public static T JsonDeseralize<T>(string stream, JsonConverter converter) => JsonConvert.DeserializeObject<T>(File.ReadAllText(stream), converter);

        public static T JsonDeseralize<T>(string stream, JsonSerializerSettings settings) => JsonConvert.DeserializeObject<T>(File.ReadAllText(stream), settings);
    }
}