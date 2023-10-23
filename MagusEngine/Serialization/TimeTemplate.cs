using MagusEngine.Systems.Time;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MagusEngine.Serialization
{
    public class TimeJsonConverter : JsonConverter<TimeSystem>
    {
        public override void WriteJson(JsonWriter writer, TimeSystem value, JsonSerializer serializer) =>
            serializer.Serialize(writer, (TimeTemplate)value);

        public override TimeSystem ReadJson(JsonReader reader,
            Type objectType,
            TimeSystem existingValue,
            bool hasExistingValue,
            JsonSerializer serializer) =>
            serializer.Deserialize<TimeTemplate>(reader);
    }

    public class TimeTemplate
    {
        public long Ticks { get; set; }
        public List<ITimeNode> Nodes { get; set; }

        public static implicit operator TimeTemplate(TimeSystem timeMaster)
        {
            return new TimeTemplate()
            {
                Ticks = timeMaster.TimePassed.Ticks,
                Nodes = timeMaster.Nodes.ToList()
            };
        }

        public static implicit operator TimeSystem(TimeTemplate serialized)
        {
            if (serialized == null) 
                return new();
            var time = new TimeSystem(serialized.Ticks);

            foreach (var item in serialized.Nodes)
            {
                time.RegisterEntity(item);
            }

            return time;
        }
    }
}
