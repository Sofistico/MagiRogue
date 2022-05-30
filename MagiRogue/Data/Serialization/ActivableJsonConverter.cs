using MagiRogue.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.Data.Serialization
{
    public class ActivableJsonConverter : JsonConverter<IActivable>
    {
        public override IActivable? ReadJson(JsonReader reader,
            Type objectType, IActivable? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var jObj = JObject.ReadFrom(reader);
            var activable = default(IActivable);
            UseAction action = (UseAction)Enum.Parse(typeof(UseAction), jObj.ToString());

            switch (action)
            {
                case UseAction.Sit:
                    activable = new Sit();
                    break;

                default:
                    break;
            }
            return activable;
        }

        public override void WriteJson(JsonWriter writer, IActivable? value, JsonSerializer serializer)
        {
            throw new InvalidOperationException("Use default serialization.");
        }
    }
}