using MagiRogue.Data.Enumerators;
using MagiRogue.GameSys.Planet.History;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MagiRogue.GameSys.Civ
{
    public class FamilyLink
    {
        public List<FamilyNode> Family { get; set; }

        public FamilyLink()
        {
            Family = new();
        }

        public FamilyLink(List<FamilyNode> family)
        {
            Family = family;
        }

        public void AddToFamilyLink(HistoricalFigure figure, HfRelationType type, HistoricalFigure otherFigure)
            => Family.Add(new FamilyNode(type, figure, otherFigure.Id));

        public FamilyNode[] GetOtherFamilyNodesByRelations(HistoricalFigure whoToSearchIn, HfRelationType toFind)
        {
            return Family.Where(i => whoToSearchIn.Id == i.OtherFigureId
                && i.Relation == toFind).ToArray();
        }

        public bool GetIfExistsAnyRelationOfType(HistoricalFigure figure, HfRelationType toFind)
        {
            return Family.Any(i => i.Figure.Id == figure.Id && i.Relation == toFind);
        }

        public void SetMotherChildFatherRelation(HistoricalFigure hfMother, HistoricalFigure hfChild)
        {
            AddToFamilyLink(hfMother, HfRelationType.OwnChild, hfChild);
            AddToFamilyLink(hfChild, HfRelationType.Mother, hfMother);
            // find the fahter
            var hfFather = GetOtherFamilyNodesByRelations(hfMother, HfRelationType.Married);
            AddToFamilyLink(hfFather[0].Figure, HfRelationType.OwnChild, hfChild);
        }

        public void SetMarriedRelation(HistoricalFigure hfOne, HistoricalFigure hfTwo)
        {
            AddToFamilyLink(hfOne, HfRelationType.Married, hfTwo);
            AddToFamilyLink(hfTwo, HfRelationType.Married, hfOne);
        }
    }

    public class FamilyNode
    {
        public HfRelationType Relation { get; set; }
        public int OtherFigureId { get; set; }
        public HistoricalFigure Figure { get; set; }

        public FamilyNode(HfRelationType relation, HistoricalFigure figure, int otherFigureId)
        {
            Relation = relation;
            Figure = figure;
            OtherFigureId = otherFigureId;
        }
    }
}