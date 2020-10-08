using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.Entities.Materials
{
    public class MaterialJsonClass
    {
        public class Flesh
        {
            public bool Flamability { get; set; }
            public int Hardness { get; set; }
            public int MP_Infusion_Limit { get; set; }
            public bool CanRegen { get; set; }
            public double Density { get; set; }
            public int Melting_Point { get; set; }
        }

        public class Stone
        {
            public bool Flamability { get; set; }
            public int Hardness { get; set; }
            public int MP_Infusion_Limit { get; set; }
            public bool CanRegen { get; set; }
            public double Density { get; set; }
            public int Melting_Point { get; set; }
        }

        public class Iron
        {
            public bool Flamability { get; set; }
            public int Hardness { get; set; }
            public int MP_Infusion_Limit { get; set; }
            public bool CanRegen { get; set; }
            public double Density { get; set; }
            public int Melting_Point { get; set; }
            public int Boiling_Point { get; set; }
        }

        public class Wood
        {
            public bool Flamability { get; set; }
            public int Hardness { get; set; }
            public int MP_Infusion_Limit { get; set; }
            public bool CanRegen { get; set; }
            public double Density { get; set; }
        }

        public class Water
        {
            public bool Flamability { get; set; }
            public int Hardness { get; set; }
            public int MP_Infusion_Limit { get; set; }
            public bool CanRegen { get; set; }
            public double Density { get; set; }
            public int Melting_Point { get; set; }
            public int Boiling_Point { get; set; }
        }

        public class Materials
        {
            public Flesh Flesh { get; set; }
            public Stone Stone { get; set; }
            public Iron Iron { get; set; }
            public Wood Wood { get; set; }
            public Water Water { get; set; }
        }
    }
}