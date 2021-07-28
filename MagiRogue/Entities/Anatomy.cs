using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using Troschuetz.Random;

namespace MagiRogue.Entities
{
    #region Enums

    public enum HumanoidBody
    {
        Head,
        Torso,
        R_arm,
        R_hand,
        L_arm,
        L_hand,
        R_leg,
        R_foot,
        L_leg,
        L_foot
    }

    public enum QuadrupleBody
    {
        Head,
        Torso,
        R_FrontalLeg,
        L_FrontalLeg,
        R_FrontalPaw,
        L_FrontalPaw,
        R_BackLeg,
        L_BackLeg,
        R_BackPaw,
        L_BackPaw,
        Tail
    }

    #endregion Enums

    public class Anatomy
    {
        #region Fields

        private Race race;
        private readonly TRandom random = new TRandom();

        #endregion Fields

        #region Properties

        [DataMember]
        public List<Limb> Limbs { get; set; }

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
        public float BloodCount { get; set; }  // the amount of blood inside the actor, its in ml, to facilitate the calculus of blood volume, the formula is ml/kg

        [DataMember]
        public bool HasBlood { get; set; }

        [JsonIgnore]
        public bool HasEnoughArms => Limbs.FindAll(l => l.TypeLimb is TypeOfLimb.Arm).Count >= 1;

        [JsonIgnore]
        public bool HasEnoughLegs => Limbs.Exists(l => l.TypeLimb is TypeOfLimb.Leg);

        [JsonIgnore]
        public bool HasEnoughWings => Limbs.FindAll(l => l.TypeLimb is TypeOfLimb.Wing).Count >= 2;

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
                BloodCount = (float)Math.Round(weight * 75);
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
            BloodCount = (float)Math.Round(BloodCount - bleedAmount, 4);

            switch (severity)
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
            }
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
                    bodyPartIndex = random.Next(bodyParts.Count);
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

            bodyParts = Limbs.FindAll(l => l.TypeLimb == limb && l.Attached == true);

            if (bodyParts.Count < 1)
            {
                GameLoop.UIManager.MessageLog.Add("Fix the game!");
            }

            bodyPartIndex = random.Next(bodyParts.Count);
            bodyPart = bodyParts[bodyPartIndex];

            List<Limb> connectedParts = Limbs.FindAll(c => c.ConnectedTo == bodyPart);
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
                GameLoop.UIManager.MessageLog.Add($"And took {totalDmg} damage!");
        }

        #endregion Methods
    }

    public enum InjurySeverity
    {
        Scratch,
        LigthInjury,
        MediumInjury,
        SeriousInjury,
        Crippling,
        Fatal,
        LimbLoss
    }
}