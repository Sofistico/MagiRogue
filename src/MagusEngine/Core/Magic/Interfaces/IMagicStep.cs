using MagusEngine.Core.Entities.Base;

namespace MagusEngine.Core.Magic.Interfaces
{
    /// <summary>
    /// Represents a step in a spell's execution.
    /// <para/>Can be anything such as clapping your hands or saying a word.
    /// <para/>TODO: Will be heavily used for rituals and other complex spells with many steps.
    /// </summary>
    public interface IMagicStep
    {
        public int Ticks { get; set; }

        public bool ExecuteStep(MagiEntity caster);
    }
}
