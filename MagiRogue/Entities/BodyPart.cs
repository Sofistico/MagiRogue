using MagiRogue.Data.Enumerators;
using MagiRogue.Data.Serialization;
using MagiRogue.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.Entities
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class BodyPart
    {
        private double bodyPartHp;

        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public double BodyPartHp
        {
            get
            {
                if (bodyPartHp > MaxBodyPartHp)
                {
                    return MaxBodyPartHp;
                }
                else
                    return bodyPartHp;
            }

            set
            {
                if (value > MaxBodyPartHp)
                {
                    bodyPartHp = MaxBodyPartHp;
                }
                else
                    bodyPartHp = value;
            }
        }

        [DataMember]
        public double MaxBodyPartHp { get; set; }

        [DataMember]
        public double BodyPartWeight
        {
            get
            {
                double finalResult = MathMagi.GetWeightWithDensity(BodyPartMaterial.Density, Volume / 1000);
                return finalResult;
            }
        }

        /// <summary>
        /// The size of the bodypart in cm³
        /// </summary>
        [DataMember]
        public int Volume { get; set; }

        /// <summary>
        /// The relative size of the being body part.
        /// So that a giant doens't have the arm the size of a normal human
        /// </summary>
        public int RelativeVolume { get; set; }

        /// <summary>
        /// Marks if the BP is right, left, or center, this is the property.
        /// </summary>
        [DataMember]
        public BodyPartOrientation Orientation { get; set; }

        [DataMember]
        public string BodyPartName { get; set; }

        [DataMember]
        public MaterialTemplate BodyPartMaterial { get; set; }

        [DataMember]
        public string MaterialId { get; set; }

        /// <summary>
        /// Determines how effective the races regen is...
        /// Values are between 0 and 1, where 0 is no regen for the limb and 1 is full regen.
        /// </summary>
        public double RateOfHeal { get; set; }
        public bool CanHeal { get; set; }
        public BodyPartFunction LimbFunction { get; set; }
        public bool Working { get; set; } = true;
        public List<Wound> Wounds { get; set; }

        public BodyPart()
        {
            Wounds = new();
        }

        public void ApplyHeal(double rateOfHeal)
        {
            if (BodyPartHp < MaxBodyPartHp)
            {
                double newHp = rateOfHeal + BodyPartHp;
                BodyPartHp = MathMagi.Round(newHp);
            }
        }

        private string DebuggerDisplay
        {
            get
            {
                return string.Format($"{nameof(BodyPart)} : {BodyPartName}");
            }
        }

        public virtual void CalculateWounds()
        {
            foreach (Wound wound in Wounds)
            {
                if (wound.Severity is InjurySeverity.Missing
                    || wound.Severity is InjurySeverity.Pulped
                    || wound.Severity is InjurySeverity.Broken)
                    Working = false;

                BodyPartHp -= wound.HpLost;
            }
        }
    }
}