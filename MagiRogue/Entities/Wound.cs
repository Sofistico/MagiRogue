using MagiRogue.Data.Enumerators;
using System;
using System.Collections.Generic;
using System.Drawing;

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
        public List<Tissue> Tissues { get; set; }

        public InjurySeverity Severity { get; set; }
        public bool Infected { get; set; }
        public bool Treated { get; set; }
        public DamageTypes DamageSource { get; set; }
        public double Recovery { get; set; }
        public bool Recovered { get; set; }

        /// <summary>
        /// Contact area of the wound. Is initially the lesser of the weapon or body
        /// part contact areas.It grows with cumulative hits.Body parts and non-weapon
        /// items have contact = (size / 10) ^ (2 / 3).
        /// </summary>
        public int ContactArea { get; set; }

        /// <summary>
        /// This is strain. For skin/muscle/fat it is usually around 50000, and for
        /// bone 100-113. This number heals over time towards 0. A wound that only
        /// has strain is called "dented".
        /// </summary>
        public int Strain { get; set; }

        /// <summary>
        /// This 0-100 percentage is related to cumulative damage. In cases where
        /// multiple axe hacks are necessary for severing a limb, it must reach 100
        /// before severing occurs.In cases where a weapon can't completely penetrate
        /// a tissue, it is related to the weapon's penetration number. This percentage
        /// heals towards 0 over time.
        /// </summary>
        public int CurrentPenetrationPercentage { get; set; }
        public int MaxPenetrationPercentage { get; set; }

        public Wound()
        {
        }

        public Wound(int volumyInjury, DamageTypes damageSource, List<Tissue> tissues)
        {
            VolumeInjury = volumyInjury;
            DamageSource = damageSource;
            Tissues = tissues;
        }

        public Wound(double bleeding, int volumyInjury, InjurySeverity severity, List<Tissue> tissues)
        {
            Bleeding = bleeding;
            VolumeInjury = volumyInjury;
            Severity = severity;
            Tissues = tissues;
        }
    }
}