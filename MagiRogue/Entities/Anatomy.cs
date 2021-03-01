using System;
using System.Collections.Generic;

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

        private float bloodCount; // the amount of blood inside the actor, its in ml, to facilitate the calculus of blood volume, the formula is ml/kg
        private int size; // the size in meters of the actor
        private float weight; // the weight of the being in kg
        private int temperature; // the temperature of the creature, don't know if i will use or not
        private bool hasBlood;
        private Race race;

        #endregion Fields

        #region Properties

        public List<Limb> Limbs { get; set; }

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
        public float BloodCount
        {
            get => bloodCount;
            set
            {
                if (value <= (float)Math.Round((float)weight * 75, 2))
                    bloodCount = value;
                else
                    return;
            }
        }

        /// <summary>
        /// The size of the actor in centimeters
        /// </summary>
        public int Size { get { return size; } set { size = value; } }
        /// <summary>
        /// The weight of the actor in kg
        /// </summary>
        public float Weight { get { return weight; } set { weight = value; } }
        /// <summary>
        /// The temperature of the actor in celsius
        /// </summary>
        public int Temperature { get { return temperature; } set { temperature = value; } }

        public bool HasBlood { get { return hasBlood; } set { hasBlood = value; } }

        #endregion Properties

        #region Constructor

        public Anatomy()
        {
            //CalculateBlood();
        }

        #endregion Constructor

        #region Methods

        protected void CalculateBlood()
        {
            if (hasBlood)
                bloodCount = (float)Math.Round(Weight * 75);
        }

        public void SetRace(Race race) => Race = race;

        public void SetLimbs(params Limb[] limbs)
        {
            foreach (Limb limb in limbs)
            {
                Limbs.Add(limb);
            }
        }

        #endregion Methods
    }
}