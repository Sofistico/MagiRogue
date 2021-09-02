using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace MagiRogue.Utils
{
    public static class JsonUtils
    {
        public static T JsonDeseralize<T>(string stream) => JsonConvert.DeserializeObject<T>(File.ReadAllText(stream));

        public static List<T> JsonDeseralize<T>(string[] arrayOfStreams)
        {
            List<T> list = new();
            foreach (string stream in arrayOfStreams)
            {
                list.Add(JsonConvert.DeserializeObject<T>(File.ReadAllText(stream)));
            }

            return list;
        }

        public static T JsonDeseralize<T>(string stream, JsonConverter converter) => JsonConvert.DeserializeObject<T>(File.ReadAllText(stream), converter);

        public static T JsonDeseralize<T>(string stream, JsonSerializerSettings settings) => JsonConvert.DeserializeObject<T>(File.ReadAllText(stream), settings);
    }
}