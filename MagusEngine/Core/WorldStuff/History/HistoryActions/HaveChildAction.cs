using MagiRogue.Data.Enumerators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagusEngine.Core.WorldStuff.History.HistoryActions
{
    internal sealed class HaveChildAction : IHistoryAct
    {
        public bool? Act(HistoricalFigure figure)
        {
            HistoricalFigure spouse = GetSpouse(figure);
            if (spouse is null)
                return false;
            if (CompatibleGenderForBabies(spouse, figure) && (!figure.Pregnant || !spouse.Pregnant))
            {
                figure.MakeBabyWith(spouse);
                return true;
            }
            return false;
        }

        private static HistoricalFigure GetSpouse(HistoricalFigure hf)
        {
            return hf.GetRelatedHfSpouseId();
        }

        private static bool CompatibleGenderForBabies(HistoricalFigure spouse, HistoricalFigure figure)
        {
            var allowedGenders = new Sex[]
            {
                Sex.Male,
                Sex.Female
            };
            if (allowedGenders.Contains(spouse.HFGender)
                && allowedGenders.Contains(figure.HFGender))
            {
                return spouse.HFGender != figure.HFGender;
            }
            return false;
        }
    }
}
