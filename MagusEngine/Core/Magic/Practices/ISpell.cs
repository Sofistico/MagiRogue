using MagusEngine.Core.Entities;
using System.Collections.Generic;

namespace MagusEngine.Core.Magic.Practices
{
    public interface ISpell
    {
        public bool CanCast(Actor caster, bool tickProficiency = true);

        public bool CastSpell(Point target, Actor caster);

        public bool CastSpell(List<Point> targetS, Actor caster);
    }
}
