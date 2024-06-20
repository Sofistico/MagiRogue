using MagusEngine.Core.Entities.Base;

namespace MagusEngine.Core.Magic.Interfaces
{
    public interface IMagicComponent
    {
        public bool ExecuteComponent(MagiEntity caster);
    }
}
