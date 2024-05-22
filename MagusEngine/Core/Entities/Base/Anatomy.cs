using Arquimedes.Enumerators;
using GoRogue.Random;
using MagusEngine.Actions;
using MagusEngine.Bus.UiBus;
using MagusEngine.ECS.Components.ActorComponents;
using MagusEngine.Services;
using MagusEngine.Systems;
using MagusEngine.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace MagusEngine.Core.Entities.Base
{
    /// <summary>
    /// Your body characteristics, what race you are, if you are an undead or alive, if you are okay
    /// and etc
    /// </summary>
    public class Anatomy
    {
        #region Fields

        private bool basicSetupDone;
        private List<BodyPart> bps;

        #endregion Fields

        #region Properties

        [DataMember]
        public List<Limb> Limbs { get; set; }

        [DataMember]
        public List<Organ> Organs { get; set; }

        [JsonIgnore]
        public List<BodyPart> AllBPs
        {
            get
            {
                bps ??= new();
                if (bps.Count > 0 && bps[0] == Limbs[0])
                {
                    return bps;
                }
                else
                {
                    bps.InsertRange(0, Limbs);
                    bps.InsertRange(bps.Count, Organs);
                    return bps;
                }
            }
        }

        [DataMember]
        public Race Race { get; set; }

        /// <summary>
        /// It uses an aproximation of blood count equal to 75 ml/kg for an adult male
        /// </summary>
        [DataMember]
        public double BloodCount { get; set; }  // the amount of blood inside the actor, its in ml, to facilitate the calculus of blood volume, the formula is ml/kg

        [DataMember]
        public bool HasBlood { get; set; } = true;

        [JsonIgnore]
        public bool HasAnyHands
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
        public bool HasAtLeastOneHead { get => Limbs.Exists(i => i.LimbType is LimbType.Head && i.Attached); }

        [JsonIgnore]
        public bool CanSee
        {
            get
            {
                return Organs.Exists(o => o.OrganType is OrganType.Visual && o.Working);
            }
        }

        [JsonIgnore]
        public bool HasATorso
        {
            get
            {
                return Limbs.Exists(l => l.LimbType is LimbType.UpperBody && l.Attached);
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
        /// Value between 0 and 2, fitness level determines how well you "health" you are and how
        /// well you recover your stamina, also helps with how long you will live.
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
        public string RaceId { get; set; }

        public bool HasAnyRaceAttack
        {
            get
            {
                if (Race is null) return false;
                if (Race?.Attacks?.Count < 1) return false;
                return Race.Attacks.Any(i => Limbs.Any(z => z.BodyPartFunction == i?.LimbFunction));
            }
        }

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
                BloodCount = MathMagi.Round(weight * Race.BloodMultiplier);
        }

        public void Injury(Wound wound, BodyPart bpInjured, Actor actorWounded)
        {
            double injureSeverity = MathMagi.GetPercentageBasedOnMax(wound.VolumeInjury, bpInjured.Volume) / 100;

            switch (injureSeverity)
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
                    if (wound.InitialDamageSource.Type is DamageTypes.Blunt || injureSeverity < 2)
                        wound.Severity = InjurySeverity.Pulped;
                    break;

                default:
                    throw new Exception($"Error with getting percentage of the damage done to the body part: {injureSeverity}");
            }

            if (wound.InitialDamageSource.Type is DamageTypes.Sharp
                || wound.InitialDamageSource.Type is DamageTypes.Pierce)
            {
                wound.Bleeding = (actorWounded.Weight / bpInjured.BodyPartWeight * (int)wound.Severity)
                    + wound.GetBaseBleedingRate();
            }

            bpInjured.AddWound(wound);
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
            SetupActorNeedsIfAny(actor);
        }

        private static void SetupActorNeedsIfAny(Actor actor)
        {
            var flags = actor.Flags;
            var needs = NeedCollection.WithCommonNeeds();
            actor.AddComponents(needs);
            if (flags.Contains(SpecialFlag.NoSleep))
            {
                var sleep = needs.FirstOrDefault(i => i.ActionToFulfillNeed is ActionsEnum.Sleep);
                needs.Remove(sleep);
            }
            if (flags.Contains(SpecialFlag.NoEat))
            {
                var eat = needs.FirstOrDefault(i => i.ActionToFulfillNeed is ActionsEnum.Eat);
                needs.Remove(eat);
            }
            if (flags.Contains(SpecialFlag.NoDrink))
            {
                var drink = needs.FirstOrDefault(i => i.ActionToFulfillNeed is ActionsEnum.Drink);
                needs.Remove(drink);
            }
        }

        private void SetupActorBodyProperties(Actor actor, int volume)
        {
            actor.Volume = volume;

            if (Race?.Select is not null)
            {
                Race.CustomBPSelect(actor);
            }

            Limbs = Race.ReturnRaceLimbs();
            Organs = Race.ReturnRaceOrgans();
            NormalLimbRegen = Race.RaceNormalLimbRegen;

            if (Limbs.Count > 0)
            {
                ConfigureLimbs(actor.Volume);
                SetMaxStandAndGraspCount();
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

        public void BasicSetup(Race? race = null)
        {
            try
            {
                if (race is not null)
                {
                    Race ??= race;
                    RaceId = race.Id;
                }
                if (Race is null && !string.IsNullOrEmpty(RaceId))
                    Race = DataManager.QueryRaceInData(RaceId);
                if (Race is null)
                    throw new ApplicationException("Something went wrong, the race is null!");
                SetRandomLifespanByRace();
                basicSetupDone = true;
            }
            catch (Exception ex)
            {
                Locator.GetService<MagiLog>().Log(ex.Message);
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

        private void ConfigureLimbs(int volume)
        {
            foreach (BodyPart bp in AllBPs)
            {
                bp.Volume = MathMagi.ReturnPositive((int)(volume * (bp.RelativeVolume
                    + GlobalRandom.DefaultRNG.NextInclusiveDouble(-0.01, 0.01))));

                CalculateTissueVolume(bp);
            }

            foreach (var organ in Organs)
            {
                if (!string.IsNullOrEmpty(organ.InsideOf))
                {
                    var limb = AllBPs.Find(i => i.Id.Equals(organ.InsideOf))
                        ?? throw new ApplicationException($"Something went really wrong! Cound't find the limb for organ {organ.Id} with insides {organ.InsideOf}");
                    limb.Insides.Add(organ);
                }
            }
        }

        private static void CalculateTissueVolume(BodyPart bp)
        {
            int totalVolume = bp.Volume;
            int totalThickness = bp.Tissues.Sum(t => t.RelativeThickness);

            foreach (var tissue in bp.Tissues)
            {
                tissue.Volume = (int)MathMagi.FastRound((double)totalVolume * tissue.RelativeThickness / totalThickness);
            }
        }

        private void CalculateWeight(Actor actor)
        {
            actor.Weight = Limbs.Sum(w => w.BodyPartWeight) + Organs.Sum(w => w.BodyPartWeight);
        }

        private void Dismember(Limb limb, Actor actor)
        {
            List<Limb> bodyParts;
            int bodyPartIndex;
            Limb bodyPart;

            if (limb.LimbType == LimbType.Head || limb.LimbType == LimbType.Neck)
            {
                bodyParts = Limbs.FindAll(h => h.BodyPartFunction == BodyPartFunction.Thought && NeedsHead);
                if (bodyParts.Count > 1)
                {
                    DismemberMessage(actor, bodyParts[0]);
                    ActionManager.ResolveDeath(actor);
                    return;
                }
                else
                {
                    bodyPartIndex = GoRogue.Random.GlobalRandom.DefaultRNG.NextInt(bodyParts.Count);
                    bodyPart = bodyParts[bodyPartIndex];
                    DismemberMessage(actor, bodyPart);
                    ActionManager.ResolveDeath(actor);
                    return;
                }
            }
            else if (limb.LimbType == LimbType.UpperBody || limb.LimbType == LimbType.LowerBody)
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
                    Wound lostLimb = new(DataManager.QueryDamageInData("sharp"), connectedLimb.Tissues)
                    {
                        Severity = InjurySeverity.Missing
                    };
                    connectedLimb.AddWound(lostLimb);
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

            dismemberMessage.Append(actor.Name).Append(" lost ").Append(limb.BodyPartName);
            try
            {
                Locator.GetService<MessageBusService>().SendMessage<AddMessageLog>(new(dismemberMessage.ToString()));

                //if (totalDmg > 0)
                //    GameLoop.AddMessageLog($"and took {totalDmg} damage!");

                Find.CurrentMap?.AddMagiEntity(limb.ReturnLimbAsItem(actor));
            }
            catch (Exception)
            {
                return;
            }
        }

        public Limb GetRandomLimb()
        {
            int rng = GlobalRandom.DefaultRNG.NextInt(Limbs.Count);

            return Limbs[rng];
        }

        /// <summary>
        /// Checks once a year for aging related stuff and etc... To be used
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
            var race = Race;
            return race.MaxVolume / race.AdulthoodAge;
        }

        public (int?, int?) GetMinMaxLifespan() => (Race.LifespanMin, Race.LifespanMax);

        public int GetRaceAdulthoodAge()
        {
            if (Race is not null)
                return Race.AdulthoodAge;
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
            foreach (BodyPart item in AllBPs)
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
            if (Limbs.Count == 0)
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

        public Food WhatToEat()
        {
            if (Race.Flags.Contains(SpecialFlag.NoEat))
                return Food.None;
            if (Race.Flags.Contains(SpecialFlag.Grazer))
                return Food.Herbivore;
            if (Race.Flags.Contains(SpecialFlag.Predator))
                return Food.Carnivore;

            return Food.Omnivere;
        }

        public string Pronoum()
        {
            if (Gender == Sex.Male)
                return "him";
            if (Gender == Sex.Female)
                return "her";
            return "it";
        }

        public string PronoumPossesive()
        {
            if (Gender == Sex.Male)
                return "his";
            if (Gender == Sex.Female)
                return "hers";
            return "it's";
        }

        #endregion Methods
    }
}
