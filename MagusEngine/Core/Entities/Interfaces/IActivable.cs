using MagusEngine.Core.Entities.Base;
using MagusEngine.Serialization.EntitySerialization;
using Newtonsoft.Json;

namespace MagusEngine.Core.Entities.Interfaces
{
    #region Interface

    [JsonConverter(typeof(ActivableJsonConverter))]
    public interface IActivable
    {
        public void Activate(MagiEntity entity);
    }

    #endregion Interface

    #region StructRegion

    // TODO: Take a look later of what will do with these activable types.
    public struct Sit : IActivable
    {
        public void Activate(MagiEntity entity)
        {
            throw new NotImplementedException();
        }
    }

    public struct Study : IActivable
    {
        public void Activate(MagiEntity entity)
        {
            throw new NotImplementedException();
        }
    }

    public struct Craft : IActivable
    {
        public void Activate(MagiEntity entity)
        {
            throw new NotImplementedException();
        }
    }

    public struct Enchant : IActivable
    {
        public void Activate(MagiEntity entity)
        {
            throw new NotImplementedException();
        }
    }

    public struct Rest : IActivable
    {
        public void Activate(MagiEntity entity)
        {
            throw new NotImplementedException();
        }
    }

    public struct Lockpick : IActivable
    {
        public void Activate(MagiEntity entity)
        {
            throw new NotImplementedException();
        }
    }

    public struct VisExtract : IActivable
    {
        public void Activate(MagiEntity entity)
        {
            throw new NotImplementedException();
        }
    }

    public struct Hammer : IActivable
    {
        public void Activate(MagiEntity entity)
        {
            throw new NotImplementedException();
        }
    }

    public struct Pry : IActivable
    {
        public void Activate(MagiEntity entity)
        {
            throw new NotImplementedException();
        }
    }

    public struct Distill : IActivable
    {
        public void Activate(MagiEntity entity)
        {
            throw new NotImplementedException();
        }
    }

    public struct Alchemy : IActivable
    {
        public void Activate(MagiEntity entity)
        {
            throw new NotImplementedException();
        }
    }

    public struct Store : IActivable
    {
        public void Activate(MagiEntity entity)
        {
            throw new NotImplementedException();
        }
    }

    public struct Unlight : IActivable
    {
        public void Activate(MagiEntity entity)
        {
            throw new NotImplementedException();
        }
    }

    #endregion StructRegion
}