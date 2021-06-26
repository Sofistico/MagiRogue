using System;
using System.Collections.Generic;
using Troschuetz.Random;
using System.Text;
using System.Runtime.Serialization;

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

        private int temperature; // the temperature of the creature, don't know if i will use or not
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
        /// <summary>
        /// The temperature of the actor in celsius
        /// </summary>
        public int Temperature { get { return temperature; } set { temperature = value; } }

        [DataMember]
        public bool HasBlood { get; set; }

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

        public void Dismember(TypeOfLimb limb, Actor actor)
        {
            List<Limb> bodyParts;
            int bodyPartIndex;
            Limb bodyPart;

            if (limb == TypeOfLimb.Head)
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
                    return;
                }
            }
            else if (limb == TypeOfLimb.Torso)
            {
                DismemberMessage(actor, Limbs.Find(a => a.TypeLimb == TypeOfLimb.Torso));
                Commands.CommandManager.ResolveDeath(actor);
                return;
            }

            bodyParts = Limbs.FindAll(l => l.TypeLimb == limb);

            bodyPartIndex = random.Next(bodyParts.Count);
            bodyPart = bodyParts[bodyPartIndex];

            bodyPart.Attached = false;
            DismemberMessage(actor, bodyPart);
        }

        private static void DismemberMessage(Actor actor, Limb limb)
        {
            StringBuilder dismemberMessage = new StringBuilder();

            dismemberMessage.Append($"{actor.Name} lost {limb.LimbName}");

            GameLoop.UIManager.MessageLog.Add(dismemberMessage.ToString());
        }

        #endregion Methods
    }
}