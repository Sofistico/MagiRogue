using MagiRogue.Entities;
using System.Collections.Generic;

namespace MagiRogue.GameSys.Magic.Systems
{
    public interface IMagic
    {
        public bool CanCast(MagicManager magicSkills, Actor stats);

        public bool CastSpell(Point target, Actor caster);

        public bool CastSpell(List<Point> targetS, Actor caster);
    }
}
