using MagiRogue.Data.Enumerators;
using MagiRogue.Data.Serialization;
using MagiRogue.GameSys.Physics;
using MagiRogue.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MagiRogue.Entities
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public abstract class BodyPart
    {
        public string Id { get; set; }

        public double BodyPartWeight
            => (double)MathMagi.ReturnPositive(MathMagi.GetWeightWithDensity(BodyPartMaterial.Density, Volume));

        /// <summary>
        /// The size of the bodypart in cm³
        /// </summary>

        public int Volume { get; set; }

        /// <summary>
        /// The relative size of the being body part.
        /// So that a giant doens't have the arm the size of a normal human
        /// </summary>
        public double RelativeVolume { get; set; }

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

        public bool CanHeal { get; set; } = true;
        public BodyPartFunction BodyPartFunction { get; set; }

        public bool Working { get; set; } = true;

        // TODO: FOR THE FUTURE!
        public List<Tissue> Tissues { get; set; }

        public List<Wound> Wounds { get; set; } = new();

        public bool NeedsHeal => Wounds.Count > 0;

        [JsonConstructor()]
        protected BodyPart(string materialId)
        {
            MaterialId = materialId;
            Tissues = new();
            BodyPartMaterial = PhysicsManager.SetMaterial(materialId);
        }

        public void ApplyHeal(double rateOfHeal, bool regenLostLimb = false)
        {
            //foreach (Wound wound in Wounds)
            //{
            //    // if the wound is festering and the injury is that bad, then no need to check if it will heal
            //    // cuz it will not!
            //    if (!regenLostLimb && !wound.Treated
            //        && (wound.Severity is not InjurySeverity.Bruise
            //        || wound.Severity is not InjurySeverity.Minor))
            //    {
            //        continue;
            //    }

            //    if (regenLostLimb
            //        || (wound.Severity is not InjurySeverity.Missing
            //        && wound.Severity is not InjurySeverity.Pulped))
            //    {
            //        wound.Recovery = MathMagi.Round(rateOfHeal + wound.Recovery);
            //        if (wound.Recovery >= MathMagi.ReturnPositive(wound.VolumeInjury))
            //        {
            //            wound.Recovered = true;
            //        }
            //        if (wound.Recovery > 0)
            //        {
            //            double newHp = MathMagi.Round(BodyPartHp + rateOfHeal);
            //            BodyPartHp = newHp;
            //        }
            //    }
            //}
            //if (Wounds.Count > 0)
            //    Wounds.RemoveAll(a => a.Recovered);
            //if (NeedsHeal)
            //    BodyPartHp += rateOfHeal;
        }

        private string DebuggerDisplay
        {
            get
            {
                return string.Format($"{nameof(BodyPart)} : {BodyPartName}");
            }
        }

        public virtual void AddWound(Wound wound)
        {
            // will be calculated by how big the wound was made into the specified tissue
            // the wound size is calculated in CombatUtils.cs
            Wounds.Add(wound);
        }

        public abstract BodyPart Copy();
    }
}