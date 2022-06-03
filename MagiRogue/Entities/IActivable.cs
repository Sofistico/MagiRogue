using MagiRogue.Data.Serialization;
using MagiRogue.Data.Serialization.EntitySerialization;
using MagiRogue.GameSys;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SadRogue.Primitives;

namespace MagiRogue.Entities
{
    #region Interface

    [JsonConverter(typeof(ActivableJsonConverter))]
    public interface IActivable
    {
        public void Activate(Entity entity);
    }

    #endregion Interface

    #region Enum

    [JsonConverter(typeof(StringEnumConverter))]
    public enum UseAction
    {
        None,
        Sit,
        Study,
        Craft,
        Enchant,
        Rest,
        VisExtract,
        Hammer,
        Lockpick,
        Pry
    }

    #endregion Enum

    #region StructRegion

    // TODO: Take a look later of what will do with these activable types.
    public struct Sit : IActivable
    {
        public void Activate(Entity entity)
        {
            throw new System.NotImplementedException();
        }
    }

    public struct Study : IActivable
    {
        public void Activate(Entity entity)
        {
            throw new System.NotImplementedException();
        }
    }

    public struct Craft : IActivable
    {
        public void Activate(Entity entity)
        {
            throw new System.NotImplementedException();
        }
    }

    public struct Enchant : IActivable
    {
        public void Activate(Entity entity)
        {
            throw new System.NotImplementedException();
        }
    }

    public struct Rest : IActivable
    {
        public void Activate(Entity entity)
        {
            throw new System.NotImplementedException();
        }
    }

    public struct Lockpick : IActivable
    {
        public void Activate(Entity entity)
        {
            throw new System.NotImplementedException();
        }
    }

    public struct VisExtract : IActivable
    {
        public void Activate(Entity entity)
        {
            throw new System.NotImplementedException();
        }
    }

    public struct Hammer : IActivable
    {
        public void Activate(Entity entity)
        {
            throw new System.NotImplementedException();
        }
    }

    public struct Pry : IActivable
    {
        public void Activate(Entity entity)
        {
            throw new System.NotImplementedException();
        }
    }

    #endregion StructRegion
}