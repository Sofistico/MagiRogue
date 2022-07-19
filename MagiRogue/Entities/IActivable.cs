using MagiRogue.Data.Serialization.EntitySerialization;
using Newtonsoft.Json;

namespace MagiRogue.Entities
{
    #region Interface

    [JsonConverter(typeof(ActivableJsonConverter))]
    public interface IActivable
    {
        public void Activate(Entity entity);
    }

    #endregion Interface

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

    public struct Distill : IActivable
    {
        public void Activate(Entity entity)
        {
            throw new System.NotImplementedException();
        }
    }

    public struct Alchemy : IActivable
    {
        public void Activate(Entity entity)
        {
            throw new System.NotImplementedException();
        }
    }

    public struct Store : IActivable
    {
        public void Activate(Entity entity)
        {
            throw new System.NotImplementedException();
        }
    }

    public struct Unlight : IActivable
    {
        public void Activate(Entity entity)
        {
            throw new System.NotImplementedException();
        }
    }

    #endregion StructRegion
}