﻿using MagiRogue.Commands;
using MagiRogue.Data;
using MagiRogue.Data.Enumerators;
using MagiRogue.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace MagiRogue.Entities
{
    /// <summary>
    /// Your body characteristics, what race you are, if you are an undead or alive, if you are okay and etc
    /// </summary>
    public class Anatomy
    {
        #region Fields

        private Race raceField;
        private bool basicSetupDone;

        #endregion Fields

        #region Properties

        [DataMember]
        public List<Limb> Limbs { get; set; }

        [DataMember]
        public List<Organ> Organs { get; set; }

        [DataMember]
        public string Race { get; set; }

        /// <summary>
        /// It uses an aproximation of blood count equal to 75 ml/kg for an adult male
        /// </summary>
        [DataMember]
        public double BloodCount { get; set; }  // the amount of blood inside the actor, its in ml, to facilitate the calculus of blood volume, the formula is ml/kg

        [DataMember]
        public bool HasBlood { get; set; } = true;

        [JsonIgnore]
        public bool HasEnoughArms
        {
            get
            {
                return Limbs.Any(l => l.BodyPartFunction is BodyPartFunction.Grasp && l.Working);
            }
        }

        [JsonIgnore]
        public bool HasEnoughLegs
        {
            get
            {
                return Limbs.FindAll(l => l.BodyPartFunction is BodyPartFunction.Stance && l.Working).Count >= MaxStanceLimbs;
            }
        }

        [JsonIgnore]
        public bool HasEnoughWings { get => Limbs.FindAll(l => l.BodyPartFunction is BodyPartFunction.Flier && l.Working).Count >= MaxFlierLimbs; }

        [JsonIgnore]
        public bool HasAtLeastOneHead { get => Limbs.Exists(i => i.LimbType is TypeOfLimb.Head && i.BodyPartHp > 0); }

        [JsonIgnore]
        public bool CanSee
        {
            get
            {
                return Organs.Exists(o => o.OrganType is OrganType.Visual && (!o.Destroyed || o.Working));
            }
        }

        [JsonIgnore]
        public bool HasATorso
        {
            get
            {
                return Limbs.Exists(l => l.LimbType is TypeOfLimb.UpperBody && l.BodyPartHp > 0);
            }
        }

        /// <summary>
        /// The current age of a character
        /// </summary>
        [DataMember]
        public int CurrentAge { get; set; }

        [DataMember]
        public int TickBorn { get; set; }

        /// <summary>
        /// How long the person is likely to live without death
        /// </summary>
        [DataMember]
        public int Lifespan { get; set; }

        [DataMember]
        public double NormalLimbRegen { get; set; } = 0.001;

        /// <summary>
        /// Value between 0 and 2, fitness level determines how well you "health" you are and how well you recover
        /// your stamina, also helps with how long you will live.
        /// </summary>
        [DataMember]
        public double FitLevel { get; set; } = 0.3;

        [DataMember]
        public bool NeedsHead { get; set; } = true;

        [DataMember]
        public Sex Gender { get; set; }

        [DataMember]
        public bool Ages { get; set; } = true;

        [DataMember]
        public int MaxGraspLimbs { get; set; }

        [DataMember]
        public int MaxStanceLimbs { get; set; }

        [DataMember]
        public int MaxFlierLimbs { get; set; }

        #endregion Properties

        #region Constructor

        public Anatomy()
        {
            Limbs = new();
        }

        #endregion Constructor

        #region Methods

        public void CalculateBlood(double weight)
        {
            if (HasBlood)
                BloodCount = MathMagi.Round(weight * raceField.BloodMultiplier);
        }

        public Race GetRace()
        {
            raceField ??= DataManager.QueryRaceInData(Race);
            return raceField;
        }

        public void Injury(Wound wound, BodyPart bpInjured, Actor actorWounded)
        {
            double hpLostPercentage = wound.HpLost / bpInjured.MaxBodyPartHp;

            switch (hpLostPercentage)
            {
                case > 0 and <= 0.15:
                    wound.Severity = InjurySeverity.Bruise;
                    break;

                case > 0.15 and <= 0.25:
                    wound.Severity = InjurySeverity.Minor;
                    break;

                case > 0.25 and <= 0.75:
                    wound.Severity = InjurySeverity.Inhibited;
                    break;

                case > 0.75 and < 1:
                    wound.Severity = InjurySeverity.Broken;
                    break;

                case >= 1:
                    wound.Severity = bpInjured is Limb ? InjurySeverity.Missing : InjurySeverity.Pulped;
                    if (wound.DamageSource is DamageTypes.Blunt)
                        wound.Severity = InjurySeverity.Pulped;
                    break;

                default:
                    throw new Exception($"Error with getting percentage of the damage done to the body part: {hpLostPercentage}");
            }

            if (wound.DamageSource is DamageTypes.Sharp || wound.DamageSource is DamageTypes.Point)
            {
                wound.Bleeding = ((actorWounded.Weight / bpInjured.BodyPartWeight) * (int)wound.Severity) + 0.1;
            }

            bpInjured.CalculateWound(wound);
            if (wound.Severity is InjurySeverity.Missing && bpInjured is Limb limb)
            {
                Dismember(limb, actorWounded);
            }
        }

        public void FullSetup(Actor actor, Race race, int actorAge, Sex sex, int volume)
        {
            if (!basicSetupDone)
            {
                BasicSetup(sex, actorAge, race);
            }

            SetupActorBodyProperties(actor, volume);
        }

        private void SetupActorBodyProperties(Actor actor, int volume)
        {
            actor.Volume = volume;

            if (Limbs.Count > 0)
            {
                CalculateRelativeLimbVolume(actor.Volume);
                SetMaxStandAndGraspCount();
                FindAllMaterials(actor);
                CalculateWeight(actor);
                CalculateBlood(actor.Weight);
            }
        }

        #region BasicSetups

        public void BasicSetup(Sex sex, int actorAge, Race race)
        {
            BasicSetup(race);
            Gender = sex;
            CurrentAge = actorAge;
        }

        public void BasicSetup(Race race)
        {
            if (string.IsNullOrEmpty(Race))
                Race = race.Id;
            raceField = race;
            SetRandomLifespanByRace();
            Limbs = race.ReturnRaceLimbs();
            Organs = race.ReturnRaceOrgans();
            NormalLimbRegen = race.RaceNormalLimbRegen;
            basicSetupDone = true;
        }

        public void BasicSetup()
        {
            if (!string.IsNullOrEmpty(Race))
            {
                Race race = GetRace();
                SetRandomLifespanByRace();
                Limbs = race.ReturnRaceLimbs();
                Organs = race.ReturnRaceOrgans();
                NormalLimbRegen = race.RaceNormalLimbRegen;
                basicSetupDone = true;
            }
        }

        #endregion BasicSetups

        private void SetMaxStandAndGraspCount()
        {
            foreach (Limb limb in Limbs)
            {
                switch (limb.BodyPartFunction)
                {
                    case BodyPartFunction.Grasp:
                        MaxGraspLimbs++;
                        break;

                    case BodyPartFunction.Stance:
                        MaxStanceLimbs++;
                        break;

                    case BodyPartFunction.Flier:
                        MaxFlierLimbs++;
                        break;

                    default:
                        break;
                }
            }
        }

        private void FindAllMaterials(Actor actor)
        {
            List<BodyPart> bps = new();
            bps.AddRange(Limbs);
            bps.AddRange(Organs);

            foreach (BodyPart bp in bps)
            {
                if (!actor.Body.MaterialsId.Contains(bp.MaterialId))
                    actor.Body.MaterialsId.Add(bp.MaterialId);
            }
        }

        private void CalculateRelativeLimbVolume(int volume)
        {
            List<BodyPart> bps = new();
            bps.AddRange(Limbs);
            bps.AddRange(Organs);
            foreach (BodyPart bp in bps)
            {
                bp.Volume = MathMagi.ReturnPositive((int)(volume * (bp.RelativeVolume
                    + GameLoop.GlobalRand.NextInclusiveDouble(-0.01, 0.01))));
                bp.MaxBodyPartHp = (int)((bp.Volume + 1) / (bp.BodyPartWeight + 1));
                bp.BodyPartHp = bp.MaxBodyPartHp;
            }
        }

        private void CalculateWeight(Actor actor)
        {
            actor.Weight = (Limbs.Sum(w => w.BodyPartWeight) + Organs.Sum(w => w.BodyPartWeight));
        }

        private void Dismember(Limb limb, Actor actor)
        {
            List<Limb> bodyParts;
            int bodyPartIndex;
            Limb bodyPart;

            if (limb.LimbType == TypeOfLimb.Head || limb.LimbType == TypeOfLimb.Neck)
            {
                bodyParts = Limbs.FindAll(h => h.BodyPartFunction == BodyPartFunction.Thought && NeedsHead);
                if (bodyParts.Count > 1)
                {
                    DismemberMessage(actor, bodyParts[0]);
                    Commands.ActionManager.ResolveDeath(actor);
                    return;
                }
                else
                {
                    bodyPartIndex = GoRogue.Random.GlobalRandom.DefaultRNG.NextInt(bodyParts.Count);
                    bodyPart = bodyParts[bodyPartIndex];
                    DismemberMessage(actor, bodyPart);
                    Commands.ActionManager.ResolveDeath(actor);
                    return;
                }
            }
            else if (limb.LimbType == TypeOfLimb.UpperBody || limb.LimbType == TypeOfLimb.LowerBody)
            {
                DismemberMessage(actor, limb);
                ActionManager.ResolveDeath(actor);
                return;
            }

            List<Limb> connectedParts = GetAllConnectedLimb(limb);
            if (connectedParts.Count > 0)
            {
                foreach (Limb connectedLimb in connectedParts)
                {
                    // Here so that the bleeding from a lost part isn't being considered
                    Wound lostLimb = new Wound(connectedLimb.BodyPartHp, DamageTypes.Sharp)
                    {
                        Severity = InjurySeverity.Missing
                    };
                    connectedLimb.CalculateWound(lostLimb);
                    actor.Weight -= connectedLimb.BodyPartWeight;
                }
            }

            actor.Weight -= limb.BodyPartWeight;

            DismemberMessage(actor, limb);
        }

        /// <summary>
        /// Get all conneceted limbs exclusive from the bodyPart informe in the parameter
        /// </summary>
        /// <param name="bodyPart"></param>
        /// <returns></returns>
        public List<Limb> GetAllConnectedLimb(Limb bodyPart)
        {
            List<Limb> connectedParts = new();
            List<Limb> temporaryList = Limbs.FindAll(c =>
                !string.IsNullOrEmpty(c.ConnectedTo) && c.ConnectedTo == bodyPart.Id);
            foreach (Limb limb in temporaryList)
            {
                connectedParts.Add(limb);
                var temp = GetAllConnectedLimb(limb);
                connectedParts.AddRange(temp);
            }
            return connectedParts;
        }

        /// <summary>
        /// Get all parents from an informed bodyPart, up to it's root, usually the Torso
        /// </summary>
        /// <param name="bodyPart"></param>
        /// <returns></returns>
        public List<Limb> GetAllParentConnectionLimb(Limb bodyPart)
        {
            List<Limb> limbs = new List<Limb>();
            foreach (Limb limb in Limbs)
            {
                if (!string.IsNullOrEmpty(bodyPart.ConnectedTo) && bodyPart.ConnectedTo == limb.Id)
                {
                    limbs.Add(limb);
                    var temp = GetAllParentConnectionLimb(limb);
                    limbs.AddRange(temp);
                }
            }
            return limbs;
        }

        private static void DismemberMessage(Actor actor, Limb limb)
        {
            StringBuilder dismemberMessage = new StringBuilder();

            dismemberMessage.Append($"{actor.Name} lost {limb.BodyPartName}");
            try
            {
                GameLoop.AddMessageLog(dismemberMessage.ToString());
                //if (totalDmg > 0)
                //    GameLoop.AddMessageLog($"and took {totalDmg} damage!");

                GameLoop.GetCurrentMap()?.Add(limb.ReturnLimbAsItem(actor));
            }
            catch (Exception)
            {
                return;
            }
        }

        public Limb GetRandomLimb()
        {
            int rng = GameLoop.GlobalRand.NextInt(Limbs.Count);

            return Limbs[rng];
        }

        public void UpdateBody(Actor actor)
        {
            if (BloodCount <= 0)
                ActionManager.ResolveDeath(actor);
            actor.ApplyAllRegen();
            List<Wound> wounds = GetAllWounds();
            if (wounds.Count > 0)
            {
                foreach (Wound wound in wounds)
                {
                    if (wound.Bleeding > 0)
                    {
                        BloodCount -= wound.Bleeding;

                        if (wound.Treated)
                            wound.Bleeding -= actor.GetBloodCoagulation();
                        else
                            wound.Bleeding -= (actor.GetBloodCoagulation() / (int)wound.Severity + 1);
                    }
                }
            }
        }

        /// <summary>
        /// Checks once a year for aging related stuff and etc...
        /// To be used
        /// </summary>
        public bool CheckIfDiedByAge()
        {
            if (Ages && CurrentAge >= Lifespan)
            {
                var died = Mrn.CustomDice("1d10"); // sometimes, simple is good!
                return died > 5;
            }
            return false;
        }

        public int RateOfGrowthPerYear()
        {
            var race = GetRace();
            return race.MaxVolume / race.AdulthoodAge;
        }

        public (int?, int?) GetMinMaxLifespan() => (GetRace().LifespanMin, GetRace().LifespanMax);

        public int GetRaceAdulthoodAge()
        {
            if (GetRace() is not null)
                return GetRace().AdulthoodAge;
            return 0;
        }

        public void SetRandomLifespanByRace()
        {
            (int? min, int? max) = GetMinMaxLifespan();
            if (min.HasValue && max.HasValue)
            {
                Lifespan = GoRogue.Random.GlobalRandom.DefaultRNG.NextInt(min.Value, max.Value);
            }
            else
            {
                Ages = false; // else is immortal
            }
        }

        public List<Wound> GetAllWounds()
        {
            List<Wound> list = new List<Wound>();
            List<BodyPart> bps = new List<BodyPart>();
            bps.AddRange(Limbs);
            bps.AddRange(Organs);
            foreach (BodyPart item in bps)
            {
                if (item.Wounds.Count > 0)
                    list.AddRange(item.Wounds);
            }
            return list;
        }

        public void SetCurrentAgeWithingAdulthood()
            => CurrentAge = GoRogue.Random.GlobalRandom.DefaultRNG.NextInt(GetRaceAdulthoodAge(),
                GetRaceAdulthoodAge() + 4);

        public bool EnoughBodyToLive()
        {
            if (BloodCount <= 0)
                return false;
            if (Limbs.Count <= 0)
                return false;
            if (!CheckIfHasHeadAndNeedsOne())
                return false;
            if (!HasATorso)
                return false;

            // if for some reason still alive, then has enough of a body to live!
            return true;
        }

        private bool CheckIfHasHeadAndNeedsOne()
            => HasAtLeastOneHead && NeedsHead;

        public void AgeBody()
            => CurrentAge++;

        #endregion Methods
    }
}
