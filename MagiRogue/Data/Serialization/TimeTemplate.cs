using MagiRogue.GameSys.Time;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MagiRogue.Data.Serialization
{
    public class TimeJsonConverter : JsonConverter<TimeSystem>
    {
        public override void WriteJson(JsonWriter writer,
            TimeSystem value, JsonSerializer serializer) =>
            serializer.Serialize(writer, (TimeTemplate)value);

        public override TimeSystem ReadJson(JsonReader reader,
            Type objectType, TimeSystem existingValue,
            bool hasExistingValue, JsonSerializer serializer)
            => serializer.Deserialize<TimeTemplate>(reader);
    }

    [DataContract]
    public class TimeTemplate
    {
        [DataMember] public long Ticks;
        /*[DataMember] public PlayerTimeNode PlayerNode;
        [DataMember] public List<EntityTimeNode> EntityNodes;*/

        public static implicit operator TimeTemplate(TimeSystem timeMaster)
        {
            var serialized = new TimeTemplate()
            {
                Ticks = timeMaster.TimePassed.Ticks,
                //EntityNodes = new List<EntityTimeNode>(),
            };

            /*foreach (var node in timeMaster.Nodes)
            {
                switch (node)
                {
                    case PlayerTimeNode w:
                        serialized.PlayerNode = w;
                        break;

                    case EntityTimeNode e:
                        serialized.EntityNodes.Add(e);
                        break;

                    default:
                        throw new NotSupportedException
                            ($"Unsupported time master node type: {node.GetType()}");
                }
            }*/

            return serialized;
        }

        public static implicit operator TimeSystem(TimeTemplate serialized)
        {
            var timeMaster = new TimeSystem(serialized.Ticks);
            /*if (serialized.PlayerNode != null)
            {
                timeMaster.RegisterEntity(serialized.PlayerNode);
            }

            if (serialized.EntityNodes != null)
            {
                foreach (var node in serialized.EntityNodes)
                {
                    timeMaster.RegisterEntity(node);
                }
            }*/

            return timeMaster;
        }
    }
}