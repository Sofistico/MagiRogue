using MagiRogue.Data.Enumerators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.DataCreatorWpf
{
    public class ResearchViewModel : ObservableObject, IPageView
    {
        public string PageName { get => "Research"; }

        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public int Difficulty { get; set; }
        public bool IsMagical { get; set; }

        // is more a flavor or a pure intermediary step into research
        public bool IntermediareRes { get; set; }

        // can be found exploring the world?
        public bool ValidFindInExploration { get; set; }

        // can a deity just flat out give it out to it's followers?
        public bool ValidDeityGift { get; set; }

        // values: Any from AbilityName.cs and any from AnyCraft, AnyResearch, AnyMagic, AnyCombat and AnyJob
        public List<string> AbilityRequired { get; set; } = null!;
        public List<Tech> TechResearched { get; set; } = null!;

        //// Which research id this is an upgrade from!
        //// example: wound leech draining -> wound cleaning
        //public string UpgradeFrom { get; set; }
        public List<string> RequiredRes { get; set; } = null!;

        // you can only research one or the other!
        public List<string> ExclusiveOr { get; set; } = null!;
    }
}
