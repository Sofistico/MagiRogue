using MagiRogue.Data.Serialization;
using MagiRogue.GameSys;
using Newtonsoft.Json;
using SadRogue.Primitives;

namespace MagiRogue.Entities
{
    [JsonConverter(typeof(ActivableJsonConverter))]
    public interface IActivable
    {
        public void Activate();
    }

    public enum ActivableType
    {
        None,
        Confortable,
        Sit,
    }

    public struct Sit : IActivable
    {
        public void Activate()
        {
            throw new System.NotImplementedException();
        }
    }

    public struct Confortable : IActivable
    {
        public void Activate()
        {
            throw new System.NotImplementedException();
        }
    }
}