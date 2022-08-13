using MagiRogue.Data;
using MagiRogue.Data.Enumerators;
using MagiRogue.Data.Serialization;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;

namespace MagiRogue.Entities
{
    [DebuggerDisplay("{DebuggerDisplay, nq}")]
    public class Race
    {
        private readonly List<BodyPart> bodyParts;

        public string Id { get; set; }
        public string RaceName { get; set; }
        public string Description { get; set; }

        public char RaceGlyph { get; set; }
        public string RaceForeground { get; set; }
        public string RaceBackground { get; set; }

        public int ChildVolume { get; set; }
        public int MidVolume { get; set; }
        public int MaxVolume { get; set; }

        // Age related stuff
        public int LifespanMax { get; set; }
        public int LifespanMin { get; set; }
        public int? ChildAge { get; set; }
        public int? TeenAge { get; set; }
        public int AdulthoodAge { get; set; }

        // stats
        public int RaceViewRadius { get; set; }
        public int BaseStrenght { get; set; }
        public int BaseToughness { get; set; }
        public int BaseEndurance { get; set; }
        public double GeneralSpeed { get; set; }
        public int BaseInt { get; set; }
        public int BaseWillPower { get; set; }
        public int BasePrecision { get; set; }
        public double BaseManaRegen { get; set; }

        // Body
        public double RaceNormalLimbRegen { get; set; }
        public double BleedRegenaration { get; set; }
        public bool CanRegenLostLimbs { get; set; }

        public string[] BodyPlan { get; set; }

        // Civ tendencies
        // Temporary
        public bool ValidCivRace { get; set; }

        public Race()
        {
            if (BodyPlan is not null && BodyPlan.Length > 0)
            {
                bodyParts = DataManager.QueryBpsPlansInDataAndReturnBodyParts(BodyPlan);
            }
        }

        private string DebuggerDisplay()
        {
            return string.Format($"{RaceName}");
        }

        public Color ReturnForegroundColor()
        {
            MagiColorSerialization color = new MagiColorSerialization(RaceForeground);
            return color.Color;
        }

        public Color ReturnBackgroundColor()
        {
            MagiColorSerialization color = new MagiColorSerialization(RaceBackground);
            return color.Color;
        }

        public List<Limb> ReturnRaceLimbs() => bodyParts.Where(i => i is Limb).Cast<Limb>().ToList();

        public List<Organ> ReturnRaceOrgans() => bodyParts.Where(i => i is Organ).Cast<Organ>().ToList();

        public int GetRngVolume(int age)
        {
            // in the future will take in acount the variation across the height and broadness of the individual
            // so that some are bigger or smaller than others.
            int volume = 0;

            if (ChildAge.HasValue && age <= ChildAge && age < TeenAge)
                volume = ChildVolume;
            if (TeenAge.HasValue && age <= TeenAge && age < AdulthoodAge)
                volume = MidVolume;
            if (age >= AdulthoodAge)
                volume = MaxVolume;

            return volume;
        }

        public AgeGroup GetAgeGroup(int age, int lifeSpan, bool ages)
        {
            if (ChildAge.HasValue && age < ChildAge)
                return AgeGroup.Baby;
            if (ChildAge.HasValue && age >= ChildAge && age < TeenAge)
                return AgeGroup.Child;
            if (TeenAge.HasValue && age >= TeenAge && age < AdulthoodAge)
                return AgeGroup.Teen;
            if (age >= AdulthoodAge || !ages)
                return AgeGroup.Adult;
            if (age >= lifeSpan)
                return AgeGroup.Elderly;
            // if did't hit any, then it's an adult
            return AgeGroup.Adult;
        }
    }
}