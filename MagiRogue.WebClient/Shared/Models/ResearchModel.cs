namespace MagiRogue.WebClient.Shared.Models
{
    public class ResearchModel
    {
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
        public string AbilityRequired { get; set; } = null!;
        public string TechResearched { get; set; }

        //// Which research id this is an upgrade from!
        //// example: wound leech draining -> wound cleaning
        //public string UpgradeFrom { get; set; }
        public string RequiredRes { get; set; } = null!;

        // you can only research one or the other!
        public string ExclusiveOr { get; set; } = null!;
    }
}
