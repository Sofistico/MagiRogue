using MagiRogue.Data.Enumerators;

namespace MagiRogue.Entities
{
    public sealed class Wound
    {
        public double Bleeding { get; set; }

        /// <summary>
        /// The total area of the wound in cm3
        /// </summary>
        public int VolumeInjury { get; set; }

        // to be decided if is a wound object per tissue
        // or one wound object for each injury
        /// <summary>
        /// In what tissue the wound occured?
        /// </summary>
        public Tissue Tissue { get; set; }

        public InjurySeverity Severity { get; set; }
        public bool Infected { get; set; }
        public bool Treated { get; set; }
        public DamageTypes DamageSource { get; set; }
        public double Recovery { get; set; }
        public bool Recovered { get; set; }

        public Wound()
        {
        }

        public Wound(int volumyInjury, DamageTypes damageSource)
        {
            VolumeInjury = volumyInjury;
            DamageSource = damageSource;
        }

        public Wound(double bleeding, int volumyInjury, InjurySeverity severity)
        {
            Bleeding = bleeding;
            VolumeInjury = volumyInjury;
            Severity = severity;
        }
    }
}