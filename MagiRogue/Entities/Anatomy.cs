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
            Organs.Exists(o => o.OrganType is OrganType.Visual && (!o.Destroyed || o.Working));

        [JsonIgnore]
        public bool HasATorso =>
            Limbs.Exists(l => l.TypeLimb is TypeOfLimb.Torso && l.BodyPartHp > 0);

        /// <summary>
        /// The current age of a character
        /// </summary>
        [DataMember]
        public int CurrentAge { get; set; }

        public double BloodLoss { get; set; } = 0;
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

        public void SetLimbs(params Limb[] limbs)
        {
            foreach (Limb limb in limbs)
            {
                Limbs.Add(limb);
            }
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

            BloodLoss += wound.Bleeding;
            bpInjured.Wounds.Add(wound);
            bpInjured.CalculateWounds();
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
                    //bodyParts[0].Working = false;
                    DismemberMessage(actor, bodyParts[0]);
                    //Injury(new Wound(bodyParts[0].Volume,
                    //    bodyParts[0].BodyPartHp,
                    //    InjurySeverity.Missing),
                    //    bodyParts[0],
                    //    actor);
                    Commands.ActionManager.ResolveDeath(actor);
                    return;
                }
                else
                {
                    bodyPartIndex = GoRogue.Random.GlobalRandom.DefaultRNG.NextInt(bodyParts.Count);
                    bodyPart = bodyParts[bodyPartIndex];
                    //bodyPart.Working = false;
                    //Injury(new Wound(bodyPart.Volume,
                    //    bodyPart.BodyPartHp,
                    //    InjurySeverity.Missing),
                    //    bodyPart,
                    //    actor);
                    DismemberMessage(actor, bodyPart);
                    Commands.ActionManager.ResolveDeath(actor);
                    return;
                }
            }
            else if (limb == TypeOfLimb.Torso)
            {
                bodyPart = Limbs.Find(i => i.TypeLimb is TypeOfLimb.Torso);
                //Injury(new Wound(bodyPart.Volume,
                //    bodyPart.BodyPartHp,
                //    InjurySeverity.Missing),
                //    bodyPart,
                //    actor);
                DismemberMessage(actor, bodyPart);
                ActionManager.ResolveDeath(actor);
                return;
            }

            bodyParts = Limbs.FindAll(l => l.TypeLimb == limb && l.Working);

            if (bodyParts.Count < 1)
            {
                GameLoop.AddMessageLog("Fix the game!");
            }

            bodyPartIndex = GoRogue.Random.GlobalRandom.DefaultRNG.NextInt(bodyParts.Count);
            bodyPart = bodyParts[bodyPartIndex];

            List<Limb> connectedParts = GetAllConnectedBP(bodyPart);
            if (connectedParts.Count > 0)
            {
                foreach (Limb connectedLimb in connectedParts)
                {
                    //connectedLimb.Working = false;
                    //Injury(new Wound(connectedLimb.Volume,
                    //    connectedLimb.BodyPartHp,
                    //    InjurySeverity.Missing),
                    //    connectedLimb,
                    //    actor);
                }
            }

            //bodyPart.Working = false;
            //Injury(new Wound(bodyPart.Volume,
            //    bodyPart.BodyPartHp,
            //    InjurySeverity.Missing),
            //    bodyPart,
            //    actor);

            DismemberMessage(actor, bodyPart);
        }

        public List<Limb> GetAllConnectedBP(Limb bodyPart)
        {
            return Limbs.FindAll(c => c.ConnectedTo == bodyPart.Id);
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
            if (BloodLoss > 0)
                BloodCount -= BloodLoss;
            if (BloodCount <= 0)
                ActionManager.ResolveDeath(actor);
            actor.ApplyAllRegen();
            BloodLoss -= actor.GetBloodCoagulation();
        }

        public (int, int) GetMinMaxLifespan() => (GetActorRace().LifespanMin, GetActorRace().LifespanMax);

        public int GetRaceAdulthoodAge() => GetActorRace().AdulthoodAge;

        public void SetRandomLifespanByRace()
        {
            (int min, int max) = GetMinMaxLifespan();
            Lifespan = GoRogue.Random.GlobalRandom.DefaultRNG.NextInt(min, max);
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