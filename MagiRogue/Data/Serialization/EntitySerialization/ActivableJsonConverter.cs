using MagiRogue.Data.Enumerators;
using MagiRogue.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Reflection.PortableExecutable;

namespace MagiRogue.Data.Serialization.EntitySerialization
{
    public class ActivableJsonConverter : JsonConverter<IActivable>
    {
        public override IActivable? ReadJson(JsonReader reader,
            Type objectType, IActivable? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var jObj = JObject.ReadFrom(reader);
            IActivable activable;
            UseAction action = (UseAction)Enum.Parse(typeof(UseAction), jObj.ToString());

            activable = action switch
            {
                UseAction.None => throw new NotSupportedException($"UseAction None in not supported!"),
                UseAction.Sit => new Sit(),
                UseAction.Study => new Study(),
                UseAction.Craft => new Craft(),
                UseAction.Enchant => new Enchant(),
                UseAction.Rest => new Rest(),
                UseAction.VisExtract => new VisExtract(),
                UseAction.Hammer => new Hammer(),
                UseAction.Lockpick => new Lockpick(),
                UseAction.Pry => new Pry(),
                UseAction.Distill => new Distill(),
                UseAction.Alchemy => new Alchemy(),
                UseAction.Unlight => new Unlight(),
                UseAction.Store => new Store(),
                _ => throw new NotSupportedException($"Tried to add an use action that doensn't exists! Action used: {action} "),
            };
            return activable;
        }

        public override void WriteJson(JsonWriter writer, IActivable? value, JsonSerializer serializer)
        {
            //throw new InvalidOperationException("Use default serialization.");
            // as we call on my language: GAMBIARRA
            UseAction action = (UseAction)Enum.Parse(typeof(UseAction), value.ToString().Split('.')[2]);
            serializer.Serialize(writer, action);
        }
    }
}