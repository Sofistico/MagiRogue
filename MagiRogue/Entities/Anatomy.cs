using MagiRogue.Commands;
using MagiRogue.Data;
using MagiRogue.Data.Enumerators;
using MagiRogue.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
        public bool HasEnoughArms =>
            Limbs.FindAll(l => l.LimbFunction is BodyPartFunction.Grasp).Count >= 1;

        [JsonIgnore]
        public bool HasEnoughLegs =>
            Limbs.Exists(l => l.LimbFunction is BodyPartFunction.Stance);

        [JsonIgnore]
        public bool HasEnoughWings =>
            Limbs.FindAll(l => l.LimbFunction is BodyPartFunction.Flier).Count >= 2;

        [JsonIgnore]
        public bool HasAtLeastOneHead =>
            Limbs.Exists(i => i.LimbFunction is BodyPartFunction.Thought && i.BodyPartHp > 0);

        [JsonIgnore]
        public bool CanSee =>
            Organs.Exists(o => o.OrganType is OrganType.Visual && (!o.Destroyed));

        [JsonIgnore]
        public bool HasATorso =>
            Limbs.Exists(l => l.TypeLimb is TypeOfLimb.Torso && l.BodyPartHp > 0);

        /// <summary>
        /// The current age of a character
        /// </summary>
        [DataMember]
        public int CurrentAge { get; set; }

        //public double BloodLoss { get; set; } = 0;
        public int Lifespan { get; set; }
        public double NormalLimbRegen { get; set; } = 0.001;

        /// <summary>
        /// Value between 0 and 2, fitness level determines how well you "health" you are and how well you recover
        /// your stamina, also helps with how long you will live.
        /// </summary>
        public double FitLevel { get; set; } = 0.1;
        public bool NeedsHead { get; set; } = true;

        //public bool CanRegenLostLimbs { get; set; }

        #endregion Properties

        #region Constructor

        public Anatomy()
        {
        }

        public Anatomy(Actor actor)
        {
            actor.Weight = Limbs.Sum(w => w.BodyPartWeight) + Organs.Sum(w => w.BodyPartWeight);
            CalculateBlood(actor.Weight);
        }

        #endregion Constructor

        #region Methods

        public void CalculateBlood(double weight)
        {
            if (HasBlood)
                BloodCount = MathMagi.Round(weight * 75);
        }

        public Race GetActorRace() => DataManager.QueryRaceInData(Race);

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
                    if (wound.DamageSource is DamageType.Blunt)
                        wound.Severity = InjurySeverity.Pulped;
                    break;

                default:
                    throw new Exception($"Error with getting percentage of the damage done to the body part: {hpLostPercentage}");
            }

            if (wound.DamageSource is DamageType.Sharp || wound.DamageSource is DamageType.Point)
            {
                wound.Bleeding = (actorWounded.Weight / bpInjured.BodyPartWeight) * (int)wound.Severity;
            }

            bpInjured.CalculateWound(wound);
            if (wound.Severity is InjurySeverity.Missing && bpInjured is Limb limb)
            {
                Dismember(limb.TypeLimb, actorWounded);
            }
        }

        private void Dismember(TypeOfLimb limb, Actor actor)
        {
            List<Limb> bodyParts;
            int bodyPartIndex;
            Limb bodyPart;

            if (limb == TypeOfLimb.Head || limb == TypeOfLimb.Neck)
            {
                bodyParts = Limbs.FindAll(h => h.LimbFunction == BodyPartFunction.Thought && NeedsHead);
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
            else if (limb == TypeOfLimb.Torso)
            {
                bodyPart = Limbs.Find(i => i.TypeLimb is TypeOfLimb.Torso);
                DismemberMessage(actor, bodyPart);
                ActionManager.ResolveDeath(actor);
                return;
            }

            bodyParts = Limbs.FindAll(l => l.TypeLimb == limb && l.Attached);

            if (bodyParts.Count < 1)
            {
                GameLoop.AddMessageLog("Fix the game!");
            }

            bodyPartIndex = GoRogue.Random.GlobalRandom.DefaultRNG.NextInt(bodyParts.Count);
            bodyPart = bodyParts[bodyPartIndex];

            List<Limb> connectedParts = GetAllConnectedLimb(bodyPart);
            if (connectedParts.Count > 0)
            {
                foreach (Limb connectedLimb in connectedParts)
                {
                    // Here so that the bleeding from a lost part isn't being considered
                    Wound lostLimb = new Wound(connectedLimb.BodyPartHp, DamageType.Sharp)
                    {
                        Severity = InjurySeverity.Missing
                    };
                    connectedLimb.CalculateWound(lostLimb);
                }
            }

            DismemberMessage(actor, bodyPart);
        }

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

            GameLoop.AddMessageLog(dismemberMessage.ToString());
            //if (totalDmg > 0)
            //    GameLoop.AddMessageLog($"and took {totalDmg} damage!");

            GameLoop.GetCurrentMap().Add(limb.ReturnLimbAsItem(actor));
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

        public (int, int) GetMinMaxLifespan() => (GetActorRace().LifespanMin, GetActorRace().LifespanMax);

        public int GetRaceAdulthoodAge() => GetActorRace().AdulthoodAge;

        public void SetRandomLifespanByRace()
        {
            (int min, int max) = GetMinMaxLifespan();
            Lifespan = GoRogue.Random.GlobalRandom.DefaultRNG.NextInt(min, max);
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
        {
            CurrentAge = GoRogue.Random.GlobalRandom.DefaultRNG.NextInt(GetRaceAdulthoodAge(),
                GetRaceAdulthoodAge() + 4);
        }

        public bool EnoughBodyToLive()
        {
            if (BloodCount > 0) return false;
            if (Limbs.Count > 0) return false;
            if (CheckIfHasHeadAndNeedsOne()) return false;
            if (!HasATorso) return false;

            // if for some reason still alive, then has enough of a body to live!
            return true;
        }

        private bool CheckIfHasHeadAndNeedsOne()
        {
            return HasAtLeastOneHead && NeedsHead;
        }

        #endregion Methods
    }
}