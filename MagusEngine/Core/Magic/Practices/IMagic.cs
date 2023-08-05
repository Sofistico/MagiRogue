using MagiRogue.Entities;
using System.Collections.Generic;

namespace MagusEngine.Core.Magic.Practices
{
    public interface IMagic
    {
        public bool CanCast(MagicManager magicSkills, Actor stats);

        public bool CastSpell(Point target, Actor caster);

        public bool CastSpell(List<Point> targetS, Actor caster);
    }
}
