using Newtonsoft.Json;

namespace MagiRogue.WebClient.Shared.Data
{
    public static class JsonShared
    {
        public static string ToJson(object? obj) => JsonConvert.SerializeObject(obj);

        public static Stream GenerateStreamFromJson(string str)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(str);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}
