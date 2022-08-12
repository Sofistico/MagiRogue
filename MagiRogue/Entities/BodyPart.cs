using MagiRogue.Data.Enumerators;
using MagiRogue.Data.Serialization;
using MagiRogue.Utils;
using Newtonsoft.Json;
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
    public abstract class BodyPart
    {
        private double bodyPartHp;

        public string Id { get; set; }

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

        public double MaxBodyPartHp { get; set; }

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

        public int Volume { get; set; }

        /// <summary>
        /// The relative size of the being body part.
        /// So that a giant doens't have the arm the size of a normal human
        /// </summary>
        public int RelativeVolume { get; set; }

        /// <summary>
        /// Marks if the BP is right, left, or center, this is the property.
        /// </summary>

        public BodyPartOrientation Orientation { get; set; }

        public string BodyPartName { get; set; }

        [JsonIgnore]
        public MaterialTemplate BodyPartMaterial { get; set; }

        public string MaterialId { get; set; }

        /// <summary>
        /// Determines how effective the races regen is...
        /// Values are between 0 and 1, where 0 is no regen for the limb and 1 is full regen.
        /// </summary>
        public double RateOfHeal { get; set; }
        public bool CanHeal { get; set; }
        public BodyPartFunction LimbFunction { get; set; }
        public List<Wound> Wounds { get; set; }

        public BodyPart()
        {
            Wounds = new();
        }

        public void ApplyHeal(double rateOfHeal, bool regenLostLimb = false)
        {
            foreach (Wound wound in Wounds)
            {
                if (!regenLostLimb && (wound.Severity is not InjurySeverity.Missing || wound.Severity is not InjurySeverity.Pulped))
                {
                    wound.Recovery = MathMagi.Round(rateOfHeal + wound.Recovery);
                    if (wound.Recovery >= MathMagi.ReturnPositive(wound.HpLost))
                    {
                        wound.Recovered = true;
                    }
                    if (wound.Recovery > 0)
                    {
                        double newHp = MathMagi.Round(BodyPartHp + rateOfHeal);
                        BodyPartHp = newHp;
                    }
                }
            }
            Wounds.RemoveAll(a => a.Recovered);
        }

        private string DebuggerDisplay
        {
            get
            {
                return string.Format($"{nameof(BodyPart)} : {BodyPartName}");
            }
        }

        public virtual void CalculateWound(Wound wound)
        {
            BodyPartHp -= wound.HpLost;
            Wounds.Add(wound);
        }
    }
}