using MagiRogue.Data.Enumerators;
using System.Collections.Generic;

namespace MagiRogue.GameSys.Civ
{
    public class Noble
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string MaleName { get; set; }
        public string FemaleName { get; set; }
        public string Spouse { get; set; }
        public string SpouseMaleName { get; set; }
        public string SpouseFemaleName { get; set; }
        public int MaxAmmount { get; set; }

        /// <summary>
        /// Goes from 1 to 3, 1 is just the settlement level,
        /// 2 is for interacting beetween settlement and 3 is beetween civs
        /// </summary>
        public int ImportanceLevel { get; set; }
        public int Precedence { get; set; }
        public string[] RequiredForPos { get; set; }
        public List<Responsability> Responsabilities { get; set; }

        public Noble()
        {
        }
    }
}