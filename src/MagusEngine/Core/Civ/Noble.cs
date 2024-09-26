using Arquimedes.Enumerators;
using System.Collections.Generic;

namespace MagusEngine.Core.Civ
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
        /// Goes from 1 to 3, 1 is just the Site level, 2 is for interacting beetween Site and 3 is
        /// beetween civs
        /// </summary>
        public int ImportanceLevel { get; set; }
        public int Precedence { get; set; }
        public string[] RequiredForPos { get; set; }
        public List<Responsability> Responsabilities { get; set; }
        public NobleSuccession Succession { get; set; }

        public Noble()
        {
        }
    }
}