using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using ShaiRandom.Distributions.Continuous;
using MagiRogue.Utils;
using MagiRogue.Data.Enumerators;

namespace MagiRogue.Entities
{
    public class Anatomy
    {
        #region Fields

        private Race race;

        #endregion Fields

        #region Properties

        [DataMember]
        public List<Limb> Limbs { get; set; }

        [DataMember]
        public List<Organ> Organs { get; set; }

        [DataMember]
        public Race Race
        {
            get => race;

            private set
            {
                race = value;
            }
        }

        /// <summary>
        /// It uses an aproximation of blood count equal to 75 ml/kg for an adult male
        /// </summary>
        [DataMember]
        public float BloodCount { get; set; }  // the amount of blood inside the actor, its in ml, to facilitate the calculus of blood volume, the formula is ml/kg

        [DataMember]
        public bool HasBlood { get; set; } = true;

        [JsonIgnore]
        public bool HasEnoughArms => Limbs.FindAll(l => l.TypeLimb is TypeOfLimb.Arm).Count >= 1;

        [JsonIgnore]
        public bool HasEnoughLegs => Limbs.Exists(l => l.TypeLimb is TypeOfLimb.Leg);

        [JsonIgnore]
        public bool HasEnoughWings => Limbs.FindAll(l => l.TypeLimb is TypeOfLimb.Wing).Count >= 2;

        [JsonIgnore]
        public bool CanSee => Organs.Exists(o => o.OrganType is OrganType.Visual
                                                                                                && (!o.Destroyed || o.Attached));

        /// <summary>
        /// The total lifespan of a character
        /// </summary>
        [DataMember]
        public int Lifespan { get; set; }

        /// <summary>
        /// The current age of a character
        /// </summary>
        [DataMember]
        public int CurrentAge { get; set; }

        #endregion Properties

        #region Constructor

        public Anatomy()
        {
        }

        public Anatomy(Actor actor)
        {
            CalculateBlood(actor.Weight);
        }

        #endregion Constructor

        #region Methods

        protected void CalculateBlood(float weight)
        {
            if (HasBlood)
                BloodCount = MathMagi.Round(weight * 75);
        }

        public void SetRace(Race race) => Race = race;

        public void SetLimbs(params Limb[] limbs)
        {
            foreach (Limb limb in limbs)
            {
                Limbs.Add(limb);
            }
        }

        public void ContinousBleed(float bleedAmount, InjurySeverity severity)
        {
            BloodCount = MathMagi.Round(BloodCount - bleedAmount);

            /* switch (severity)
             {
                 case InjurySeverity.Scratch:
                     break;

                 case InjurySeverity.LigthInjury:
                     break;

                 case InjurySeverity.MediumInjury:
                     break;

                 case InjurySeverity.SeriousInjury:
                     break;

                 case InjurySeverity.Crippling:
                     break;

                 case InjurySeverity.Fatal:
                     break;

                 case InjurySeverity.LimbLoss:
                     break;

                 default:
                     break;
             }*/
        }

        public void Dismember(TypeOfLimb limb, Actor actor)
        {
            List<Limb> bodyParts;
            int bodyPartIndex;
            Limb bodyPart;

            if (limb == TypeOfLimb.Head || limb == TypeOfLimb.Neck)
            {
                bodyParts = Limbs.FindAll(h => h.TypeLimb == TypeOfLimb.Head);
                if (bodyParts.Count > 1)
                {
                    bodyParts[0].Attached = false;
                    DismemberMessage(actor, bodyParts[0]);
                    Commands.CommandManager.ResolveDeath(actor);
                    return;
                }
                else
                {
                    bodyPartIndex = GoRogue.Random.GlobalRandom.DefaultRNG.NextInt(bodyParts.Count);
                    bodyPart = bodyParts[bodyPartIndex];
                    bodyPart.Attached = false;
                    DismemberMessage(actor, bodyPart);
                    Commands.CommandManager.ResolveDeath(actor);
                    return;
                }
            }
            else if (limb == TypeOfLimb.Torso)
            {
                DismemberMessage(actor, Limbs.Find(a => a.TypeLimb == TypeOfLimb.Torso));
                Commands.CommandManager.ResolveDeath(actor);
                return;
            }

            bodyParts = Limbs.FindAll(l => l.TypeLimb == limb && l.Attached);

            if (bodyParts.Count < 1)
            {
                GameLoop.UIManager.MessageLog.Add("Fix the game!");
            }

            bodyPartIndex = GoRogue.Random.GlobalRandom.DefaultRNG.NextInt(bodyParts.Count);
            bodyPart = bodyParts[bodyPartIndex];

            List<Limb> connectedParts = Limbs.FindAll(c => c.ConnectedTo == bodyPart.Id);
            int totalHpLost = 0;
            if (connectedParts.Count > 0)
            {
                foreach (Limb connectedLimb in connectedParts)
                {
                    connectedLimb.Attached = false;
                    totalHpLost += connectedLimb.LimbHp;
                }
            }

            bodyPart.Attached = false;
            totalHpLost += bodyPart.LimbHp;
            actor.Stats.Health -= totalHpLost;
            if (actor.Stats.Health <= 0)
                Commands.CommandManager.ResolveDeath(actor);

            DismemberMessage(actor, bodyPart, totalHpLost);
        }

        private static void DismemberMessage(Actor actor, Limb limb, int totalDmg = 0)
        {
            StringBuilder dismemberMessage = new StringBuilder();

            dismemberMessage.Append($"{actor.Name} lost {limb.LimbName}");

            GameLoop.UIManager.MessageLog.Add(dismemberMessage.ToString());
            if (totalDmg > 0)
                GameLoop.UIManager.MessageLog.Add($"and took {totalDmg} damage!");

            GameLoop.GetCurrentMap().Add(limb.ReturnLimbAsItem(actor));
        }

        internal void Update(Actor actor)
        {
            CalculateBlood(actor.Weight);
        }

        #endregion Methods
    }
}