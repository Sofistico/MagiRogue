﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.Entities
{
    #region enum

    public enum HumanoidRace
    {
        HUMAN,
        ELF,
        DWARF,
        HALFELF,
        GNOME,
        LIZARD,
        GOBLIN,
        OGRE,
        NYMPH,
    }

    public enum MonsterRace
    {
        GENERICBEAST,
        DRAGON,
        DOG,
        CAT,
        BEAR,
        DONKEY,
        HORSE
    }

    #endregion enum

    public class Race : IRace
    {
        public bool IsHumanoid;
        public Dictionary<HumanoidBody, Limb> Limbs;

        public Race()
        {
        }

        private Race(HumanoidRace humanoidRace)
        {
        }

        private Race(MonsterRace monsterRace)
        {
        }

        public IRace GetRace(Actor actor) => actor.Anatomy.Race;

        public void SetBeastRace(Actor actor, MonsterRace monsterRace)
        {
            actor.Anatomy.Race = new Race(monsterRace);
            IsHumanoid = false;
        }

        public void SetHumanoidRace(Actor actor, HumanoidRace humanoidRace)
        {
            actor.Anatomy.Race = new Race(humanoidRace);
            IsHumanoid = true;
            Limbs = new Dictionary<HumanoidBody, Limb>()
            {
                {HumanoidBody.Head, new Limb(TypeOfLimb.Head,10, 10, 10, $"{actor.Name}'s Head")},
                {HumanoidBody.L_arm, new Limb(TypeOfLimb.L_arm, 5, 5, 5, $"{actor.Name}'s Left arm") },
                {HumanoidBody.L_hand, new Limb(TypeOfLimb.L_hand, 2, 2,2, $"{actor.Name}'s Left hand" ) }
            };
        }
    }
}