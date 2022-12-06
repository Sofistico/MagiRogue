using MagiRogue.Data;
using MagiRogue.Data.Enumerators;
using MagiRogue.GameSys.Civ;
using MagiRogue.GameSys.Planet.TechRes;
using MagiRogue.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.GameSys.Planet.History.HistoryActions
{
    internal sealed class GenericDeityStuff : IHistoryAct
    {
        private HistoricalFigure figure;

        public bool? Act(HistoricalFigure figure)
        {
            this.figure = figure;
            return MythStuff(figure);
        }

        private bool? MythStuff(HistoricalFigure figure)
        {
            // fuck deities!
            if (figure.SpecialFlags.Contains(Data.Enumerators.SpecialFlag.DeityDescended))
            {
                if (figure.CheckForInsurrection())
                {
                    PerformDivineInsurrection();
                    return true;
                }
                if (CheckForDeityGiftGiving())
                {
                    // gift something!
                    DeityGivesGiftToCiv();
                    return true;
                }
                if (figure.CheckForAgressiveInfluence())
                {
                    DeityChangesCivTendency(CivilizationTendency.Aggresive);
                    return true;
                }
                if (figure.CheckForStudiousInfluence())
                {
                    DeityChangesCivTendency(CivilizationTendency.Studious);
                    return true;
                }
            }
            return false;
        }

        private void DeityChangesCivTendency(CivilizationTendency tendency)
        {
            Civilization civ = figure.GetRelatedCivFromFigure(RelationType.PatronDeity, Find.Civs);
            if (civ is null || civ.Tendency == tendency)
                return;
            civ.Tendency = tendency;

            figure.AddLegend($"The {figure.Name} changed {figure.PronoumPossesive()} followers to {figure.PronoumPossesive()} own agressive tendencies!", Find.Year);
        }

        private void DeityGivesGiftToCiv()
        {
            if (figure.CheckForAnyStudious())
            {
                // deities ignore pre-requisites!
                // TODO: See the impact
                Research gift = DataManager.ListOfResearches.Where(i => i.ValidDeityGift)
                    .ToArray().GetRandomItemFromList();

                Civilization civ = figure.GetRelatedCivFromFigure(RelationType.PatronDeity, Find.Civs);
                Site site = civ.Sites.GetRandomItemFromList();
                site.AddDiscovery(Discovery.ReturnDiscoveryFromResearch(gift, figure, site));
            }
            if (figure.CheckForGreed())
            {
                Civilization civ = figure.GetRelatedCivFromFigure(RelationType.PatronDeity, Find.Civs);
                int weatlh = GameLoop.GlobalRand.NextInt(100, 500);
                civ.Wealth += weatlh;
                figure.AddLegend($"The {figure.Name} gifted {figure.PronoumPossesive()} followers generated wealth in the value of {weatlh}",
                    Find.Year);
            }
        }

        private bool CheckForDeityGiftGiving()
        {
            return figure.GetPersonality().Sacrifice >= 25
                && figure.RelatedCivs.Any(i => i.Relation is RelationType.PatronDeity);
        }

        private void PerformDivineInsurrection()
        {
            Civilization civ = Find.Civs.GetRandomItemFromList();
            (Noble ruler, HistoricalFigure noKingAnymore) = civ.GetRulerNoblePosition();
            if (noKingAnymore is not null)
            {
                figure.AddRelatedHf(noKingAnymore.Id, HfRelationType.Enemy);

                civ.RemoveNoble(ruler, noKingAnymore, Find.Year,
                    $"because the {figure.MythWho} {figure.Name} assumed control of the civilization!");
            }
            civ.AppointNewNoble(ruler, figure, Find.Year,
                "because it took it's position as the divine ruler of the civilization!");
            figure.AddNewRelationToCiv(civ.Id, RelationType.PatronDeity);
        }
    }
}
