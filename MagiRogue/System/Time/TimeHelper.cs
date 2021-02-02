using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MagiRogue.Entities;

namespace MagiRogue.System.Time
{
    public static class TimeHelper
    {
        public const int WalkTime = 100;
        public const int AttackTime = 150;
        public const int Wait = 100;
        public const int Interact = 50;
        public const int Wear = 200;
        public const int MagicalThings = 200;

        public static int GetWalkTime(Actor actor)
        {
            return (int)(WalkTime / actor.Stats.Speed);
        }

        public static int GetAttackTime(Actor actor)
        {
            return (int)(AttackTime / actor.Stats.Speed);
        }
    }
}