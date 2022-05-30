using MagiRogue.Data.Serialization;
using MagiRogue.GameSys;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SadRogue.Primitives;

namespace MagiRogue.Entities
{
    [JsonConverter(typeof(ActivableJsonConverter))]
    public interface IActivable
    {
        public void Activate();
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum UseAction
    {
        None,
        Sit,
    }

    // TODO: Take a look later of what will do with these activable types.
    public struct Sit : IActivable
    {
        public void Activate()
        {
            throw new System.NotImplementedException();
        }
    }
}