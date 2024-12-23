using MagusEngine.Core.Entities;

namespace MagusEngine.Core.Magic.Interfaces
{
    public interface IPermEffect
    {
        public int SolidManaCost { get; set; }

        public ISpellEffect Enchantment { get; set; }
        public Actor Caster { get; set; }

        public void Enchant(int solidManaSacrificed);
    }
}

