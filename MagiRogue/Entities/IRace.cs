using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.Entities
{
    public interface IRace
    {
        IRace GetRace(Actor actor);

        void SetHumanoidRace(Actor actor, HumanoidRace humanoidRace);

        void SetBeastRace(Actor actor, MonsterRace monsterRace);
    }
}