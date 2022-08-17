using MagiRogue.Data.Enumerators;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.Entities
{
    public class Wound
    {
        public double Bleeding { get; set; }
        public double HpLost { get; set; }
        public InjurySeverity Severity { get; set; }
        public bool Infected { get; set; }
        public bool Treated { get; set; }
        public DamageType DamageSource { get; set; }
        public double Recovery { get; set; }
        public bool Recovered { get; set; }

        public Wound()
        {
        }

        public Wound(double hpLost, DamageType damageSource)
        {
            HpLost = hpLost;
            DamageSource = damageSource;
        }

        public Wound(double bleeding, double hpLost, InjurySeverity severity)
        {
            Bleeding = bleeding;
            HpLost = hpLost;
            Severity = severity;
        }
    }
}