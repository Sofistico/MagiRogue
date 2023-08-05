using MagusEngine.Systems.Time;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace MagusEngine.Serialization
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
        [DataMember] public long Ticks { get; set; }
        [DataMember] public List<ITimeNode> Nodes { get; set; }
        /*[DataMember] public PlayerTimeNode PlayerNode;
        [DataMember] public List<EntityTimeNode> EntityNodes;*/

        public static implicit operator TimeTemplate(TimeSystem timeMaster)
        {
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

            return new TimeTemplate()
            {
                Ticks = timeMaster.TimePassed.Ticks,
                Nodes = timeMaster.Nodes.ToList()
                //EntityNodes = new List<EntityTimeNode>(),
            };
        }

        public static implicit operator TimeSystem(TimeTemplate serialized)
        {
            // TODO: broken ass loading of entities in time!
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
            var time = new TimeSystem(serialized.Ticks);

            foreach (var item in serialized.Nodes)
            {
                time.RegisterEntity(item);
            }

            return time;
        }
    }
}