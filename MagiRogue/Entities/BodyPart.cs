using MagiRogue.Data.Enumerators;
using MagiRogue.Data.Serialization;
using MagiRogue.Utils;
using Newtonsoft.Json;
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
            => (double)MathMagi.ReturnPositive(MathMagi.GetWeightWithDensity(GetBodyPartDensity(), Volume));

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

        public BodyPartFunction BodyPartFunction { get; set; }

        public bool Working { get; set; } = true;

        public List<Tissue> Tissues { get; set; }

        public List<Wound> Wounds { get; set; } = new();

        public bool NeedsHeal => Wounds.Count > 0;

        public List<BodyPart> Insides { get; set; }

        public string? Category { get; set; }

        [JsonConstructor()]
        protected BodyPart()
        {
            Tissues = new();
            Insides = new();
        }

        public void ApplyHeal(double raceHealingRate, bool regenLostLimb = false)
        {
            foreach (Wound wound in Wounds)
            {
                // if the wound is festering and the injury is that bad, then no need to check if it will heal
                // cuz it will not!
                if (!wound.Treated
                    && !regenLostLimb
                    && (wound.Severity is not InjurySeverity.Bruise
                    || wound.Severity is not InjurySeverity.Minor))
                {
                    continue;
                }

                if (regenLostLimb
                    || (wound.Severity is not InjurySeverity.Missing
                    && wound.Severity is not InjurySeverity.Pulped))
                {
                    wound.Recovery = MathMagi.Round(raceHealingRate + wound.Recovery);
                    wound.Parts.ForEach(i =>
                    {
                        raceHealingRate *= i.Tissue.HealingRate;
                        i.Strain -= raceHealingRate;
                        i.VolumeFraction -= raceHealingRate;
                        if (i.VolumeFraction < 0)
                            i.VolumeFraction = 0;
                    });
                    if (wound.Recovery >= MathMagi.ReturnPositive(wound.VolumeInjury))
                    {
                        wound.Recovered = true;
                        wound.Parts.Clear();
                    }
                }
            }
            if (Wounds.Count > 0)
            {
                Wounds.RemoveAll(a => a.Recovered);
            }
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
            // the wound size is calculated in CombatUtils.cs
            Wounds.Add(wound);
        }

        public abstract BodyPart Copy();

        private double GetBodyPartDensity()
        {
            return Tissues.ConvertAll(i => i.Material).Sum(i => i.Density);
        }

        public MaterialTemplate GetStructuralMaterial()
        {
            return Tissues.Find(i => i.Flags.Contains(TissueFlag.Structural)).Material;
        }
    }
}