using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using MagiRogue.Entities.Materials;
using System.IO;

namespace MagiRogue.Utils
{
    public class JsonUtils
    {
        public T JsonDeseralize<T>(string stream)
        {
            string json = File.ReadAllText(stream);
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}