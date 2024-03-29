﻿using Arquimedes.Enumerators;
using MagusEngine.Core.Entities.Base;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagusEngine.Core.Entities
{
    public sealed class Wound
    {
        public double Bleeding { get; set; }

        /// <summary>
        /// The total area of the wound in cm3 Is initially the lesser of the weapon or body part
        /// contact areas.It grows with cumulative hits. Body parts and non-weapon items have
        /// contact = (size / 10) ^ (2 / 3).
        /// </summary>
        public double VolumeInjury => Parts.Sum(i => i.VolumeFraction);

        /// <summary>
        /// This is strain. For skin/muscle/fat it is usually around 50, and for bone 10-11.3. This
        /// number heals over time towards 0. A wound that only has strain is called "dented".
        /// </summary>
        public double Strain => Parts.Sum(i => i.Strain);

        /// <summary>
        /// In what tissues the wound occured?
        /// </summary>
        public List<PartWound> Parts { get; set; } = new();

        public InjurySeverity Severity { get; set; }
        public bool Infected { get; set; }
        public bool Treated { get; set; }
        public DamageType InitialDamageSource { get; set; }
        public double Recovery { get; set; }
        public bool Recovered { get; set; }

        ///// <summary>
        ///// This 0-100 percentage is related to cumulative damage. In cases where
        ///// multiple axe hacks are necessary for severing a limb, it must reach 100
        ///// before severing occurs.In cases where a weapon can't completely penetrate
        ///// a tissue, it is related to the weapon's penetration number. This percentage
        ///// heals towards 0 over time.
        ///// </summary>
        //public int CurrentPenetrationPercentage { get; set; }
        //public int MaxPenetrationPercentage { get; set; }

        public Wound(DamageType damageSource, List<PartWound> parts)
        {
            InitialDamageSource = damageSource;
            Parts = parts;
        }

        public Wound(DamageType damageSource, List<Tissue> tissues)
        {
            InitialDamageSource = damageSource;
            foreach (var tissue in tissues)
            {
                Parts.Add(new PartWound(tissue.Volume, tissue.Material.ImpactStrainsAtYield ?? 0, tissue, damageSource));
            }
        }

        public string ReturnWoundStatus()
        {
            // TODO: Finish this
            var bobBuilder = new StringBuilder();
            foreach (var part in Parts)
            {
                bobBuilder.Append(part);
            }
            return bobBuilder.ToString();
        }

        public double GetBaseBleedingRate()
        {
            double result = 0;
            for (int i = 0; i < Parts.Count; i++)
            {
                result += Parts[i].Tissue.BleedingRate;
            }
            return result;
        }
    }

    public sealed class PartWound
    {
        /// <summary>
        /// The total area of the wound in cm3 Is initially the lesser of the weapon or body part
        /// contact areas. It grows with cumulative hits. Body parts and non-weapon items have
        /// contact = (size / 10) ^ (2 / 3).
        /// </summary>
        public double VolumeFraction { get; set; }

        // determines how dented the material is, slowly heals to 0
        public double Strain { get; set; } = 0;

        public DamageType PartDamage { get; set; }
        public Tissue Tissue { get; set; }

        public double? TotalVolume => Tissue?.Volume;
        public bool WholeTissue => VolumeFraction >= TotalVolume;
        public bool IsOnlyDented => VolumeFraction <= 0 && Strain > 0;

        public PartWound(double volume, double strain, Tissue tissue, DamageType damageType)
        {
            VolumeFraction = volume;
            Strain = strain;
            Tissue = tissue;
            PartDamage = damageType;
        }
    }
}
