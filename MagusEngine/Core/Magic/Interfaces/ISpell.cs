using MagusEngine.Core.Entities;
using MagusEngine.Core.Entities.Base;
using System.Collections.Generic;

namespace MagusEngine.Core.Magic.Interfaces
{
    public interface ISpell
    {
        public List<IMagicStep> Steps { get; set; }

        public bool CanCast(Actor caster, bool tickProficiency = true);

        public bool CastSpell(Point target, Actor caster);

        public bool CastSpell(List<Point> target, Actor caster);

        public SpellEntity GetSpellEntity(MagiEntity caster);
    }
}
