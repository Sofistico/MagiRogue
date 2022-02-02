using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
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

    public class JsonExtensions
    {
        public static string SerializeObjectNoCache<T>(T obj, JsonSerializerSettings settings = null)
        {
            settings ??= new JsonSerializerSettings();
            bool reset = (settings.ContractResolver == null);
            if (reset)
                // To reduce memory footprint, do not cache contract information in the global contract resolver.
                settings.ContractResolver = new DefaultContractResolver();
            try
            {
                return JsonConvert.SerializeObject(obj, settings);
            }
            finally
            {
                if (reset)
                    settings.ContractResolver = null;
            }
        }
    }
}