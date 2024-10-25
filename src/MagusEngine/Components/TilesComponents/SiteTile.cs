using MagusEngine.Core.Civ;

namespace MagusEngine.Components.TilesComponents
{
    public class SiteTile
    {
        public Site? SiteInfluence { get; set; }

        public SiteTile()
        {
        }

        public SiteTile(Site? siteInfluence)
        {
            SiteInfluence = siteInfluence;
        }
    }
}
