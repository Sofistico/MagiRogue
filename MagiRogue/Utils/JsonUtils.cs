using Newtonsoft.Json;
using System.IO;

namespace MagiRogue.Utils
{
    public static class JsonUtils
    {
        public static T JsonDeseralize<T>(string stream) => JsonConvert.DeserializeObject<T>(File.ReadAllText(stream));
    }
}