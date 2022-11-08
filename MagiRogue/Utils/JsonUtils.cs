using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MagiRogue.Utils
{
    public static class JsonUtils
    {
        public static T JsonDeseralize<T>(string path)
        {
            List<string> errors = new List<string>();

            var settings = new JsonSerializerSettings()
            {
                Error = delegate (object? sender, Newtonsoft.Json.Serialization.ErrorEventArgs args)
                {
                    errors.Add(args.ErrorContext.Error.Message);
                    args.ErrorContext.Handled = true;
                },
                Converters = { new IsoDateTimeConverter() }
            };

            return JsonConvert.DeserializeObject<T>(File.ReadAllText(path), settings);

            //yield return JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
            //try
            //{
            //    return JsonConvert.DeserializeObject<T>(File.ReadAllText(stream));
            //}
            //catch (Exception)
            //{
            //    return default;
            //}
        }

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