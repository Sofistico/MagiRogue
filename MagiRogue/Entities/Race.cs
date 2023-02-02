﻿using MagiRogue.Data;
using MagiRogue.Utils;
using MagiRogue.Data.Enumerators;
using MagiRogue.Data.Serialization;
using MagiRogue.Data.Serialization.EntitySerialization;
using Newtonsoft.Json;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using MagiRogue.Utils.Extensions;

namespace MagiRogue.Entities
{
    [DebuggerDisplay("{DebuggerDisplay, nq}")]
    public sealed class Race
    {
        private List<BodyPart> bodyParts;

        public string Id { get; set; }
        public string RaceName { get; set; }
        public string NamePlural { get; set; }
        public string Adjective { get; set; }
        public string Description { get; set; }

        public char RaceGlyph { get; set; }
        public string RaceForeground { get; set; }
        public string RaceBackground { get; set; }

        public int ChildVolume { get; set; }
        public int MidVolume { get; set; }
        public int MaxVolume { get; set; }

        // Age related stuff
        public int? LifespanMax { get; set; }
        public int? LifespanMin { get; set; }
        public int? ChildAge { get; set; }
        public string? ChildName { get; set; }
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

        /// <summary>
        /// The max mana that the species can be born with
        /// </summary>
        public int MaxManaRange { get; set; } = 5;

        /// <summary>
        /// The max mana that the species can be born with
        /// </summary>
        public int MinManaRange { get; set; } = 1;

        public int BaseMagicResistance { get; set; }

        // Body
        public double RaceNormalLimbRegen { get; set; }
        public double BleedRegenaration { get; set; }

        public string[] BodyPlan { get; set; }
        public string[] Tissues { get; set; }
        public int[] HeightModifier { get; set; }
        public int[] BroadnessModifier { get; set; }
        public int[] LengthModifier { get; set; }
        public List<string> Genders { get; set; }

        public double BloodMultiplier { get; set; } = 75; // defaults to 75
        public bool DeadRace { get; set; }
        public List<string> CreatureClass { get; set; }
        public List<SpecialFlag> Flags { get; set; }

        /// <summary>
        /// select various aspect of a creature and change it's body and other stuff!
        /// </summary>
        public object[]? Select { get; set; }

        public Race()
        {
            Flags = new();
        }

        public void SetBodyPlan()
        {
            if (BodyPlan is not null && BodyPlan.Length > 0)
            {
                bodyParts = DataManager.QueryBpsPlansInDataAndReturnBodyParts(BodyPlan);
            }
        }

        private string DebuggerDisplay
        {
            get
            {
                return string.Format($"Race : {ToString()}");
            }
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

        public List<Limb> ReturnRaceLimbs()
        {
            if (bodyParts is null)
                SetBodyPlan();
            return bodyParts.Where(i => i is Limb).Cast<Limb>().ToList();
        }

        public List<Organ> ReturnRaceOrgans()
        {
            if (bodyParts is null)
                SetBodyPlan();

            return bodyParts.Where(i => i is Organ).Cast<Organ>().ToList();
        }

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

        public AgeGroup GetAgeGroup(int age, bool ages)
        {
            if (ChildAge.HasValue && age < ChildAge)
                return AgeGroup.Baby;
            if (ChildAge.HasValue && age >= ChildAge && age < TeenAge)
                return AgeGroup.Child;
            if (TeenAge.HasValue && age >= TeenAge && age < AdulthoodAge)
                return AgeGroup.Teen;
            if (age >= AdulthoodAge && (!ages || age < LifespanMin))
                return AgeGroup.Adult;
            if (age >= LifespanMin && ages)
                return AgeGroup.Elderly;
            // if did't hit any, then it's an adult
            return AgeGroup.Adult;
        }

        public int GetAgeFromAgeGroup(AgeGroup group)
        {
            var rng = GameLoop.GlobalRand;
            return group switch
            {
                AgeGroup.Baby => rng.NextInt(ChildAge.Value - 1),
                AgeGroup.Child => rng.NextInt(ChildAge.Value, TeenAge.Value - 1),
                AgeGroup.Teen => rng.NextInt(TeenAge.Value, AdulthoodAge - 1),
                AgeGroup.Adult => rng.NextInt(AdulthoodAge, LifespanMin.HasValue ? LifespanMin.Value - 1 : int.MaxValue),
                AgeGroup.Elderly => rng.NextInt(LifespanMin.Value, LifespanMax.Value - 1),
                _ => rng.NextInt(AdulthoodAge, LifespanMin.HasValue ? LifespanMin.Value - 1 : int.MaxValue),
            };
        }

        public int GetRandomBroadness()
        {
            return GetRandomModifierFromList(BroadnessModifier);
        }

        public int GetRandomLength()
        {
            return GetRandomModifierFromList(LengthModifier);
        }

        public int GetRandomHeight()
        {
            return GetRandomModifierFromList(HeightModifier);
        }

        private static int GetRandomModifierFromList(int[] modifiers)
        {
            if (modifiers is null)
                return 0;
            int rng = GameLoop.GlobalRand.NextInt(modifiers.Length);
            int value = modifiers.Length > 0 ? modifiers[rng] : 0;
            return value;
        }

        public Sex ReturnRandomSex()
        {
            return Enum.Parse<Sex>(Genders.GetRandomItemFromList());
        }

        public override string ToString()
        {
            return RaceName;
        }

        public bool CanRegenarate() => Flags.Contains(SpecialFlag.Regenerator);

        public int GetAverageRacialManaRange()
            => (MaxManaRange + MinManaRange) / 2;
    }
}