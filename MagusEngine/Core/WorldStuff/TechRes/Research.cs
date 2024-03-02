using Arquimedes.Enumerators;
using MagusEngine.Core.Entities.Base;
using System.Collections.Generic;

namespace MagusEngine.Core.WorldStuff.TechRes
{
    public class Research : IJsonKey
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Difficulty { get; set; }
        public bool IsMagical { get; set; }

        // is more a flavor or a pure intermediary step into research
        public bool IntermediareRes { get; set; }

        // can be found exploring the world?
        public bool ValidFindInExploration { get; set; }

        // can a deity just flat out give it out to it's followers?
        public bool ValidDeityGift { get; set; }

        // values: Any from AbilityName.cs and any from AnyCraft, AnyResearch, AnyMagic, AnyCombat
        //         and AnyJob
        public List<string> AbilityRequired { get; set; }
        public List<Tech> TechResearched { get; set; }

        //// Which research id this is an upgrade from!
        //// example: wound leech draining -> wound cleaning
        //public string UpgradeFrom { get; set; }
        public List<string> RequiredRes { get; set; }

        // you can only research one or the other!
        public List<string> ExclusiveOr { get; set; }

        public Research()
        {
            TechResearched = new List<Tech>();
            RequiredRes = new List<string>();
            AbilityRequired = new List<string>();
            ExclusiveOr = new List<string>();
        }
    }
}