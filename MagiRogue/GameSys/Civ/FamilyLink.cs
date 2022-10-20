using MagiRogue.Data.Enumerators;
using MagiRogue.GameSys.Planet.History;
using MagiRogue.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagiRogue.GameSys.Civ
{
    public class FamilyLink : BasicTreeStructure<FamilyNode>
    {
        public FamilyLink()
        {
            Nodes = new();
        }

        public FamilyLink(List<FamilyNode> family)
        {
            Nodes = family;
        }

        public void AddToFamilyLink(HistoricalFigure figure,
            HfRelationType type,
            HistoricalFigure otherFigure)
        {
            if (!Nodes.Any(i => i.OtherFigureId == otherFigure.Id))
                Nodes.Add(new FamilyNode(type, figure, otherFigure.Id));
        }

        public FamilyNode[] GetOtherFamilyNodesByRelations(HistoricalFigure whoToSearchIn, HfRelationType toFind)
        {
            return Nodes.Where(i => whoToSearchIn.Id == i.OtherFigureId
                && i.Relation == toFind).ToArray();
        }

        public bool GetIfExistsAnyRelationOfType(HistoricalFigure figure, HfRelationType toFind)
        {
            return Nodes.Any(i => i.Figure.Id == figure.Id
            && i.Relation == toFind
            && i.Figure.IsAlive);
        }

        public void SetMotherChildFatherRelation(HistoricalFigure hfMother,
            HistoricalFigure hfChild,
            int year)
        {
            HistoricalFigure hfFather =
                GetOtherFamilyNodesByRelations(hfMother, HfRelationType.Married)[0].Figure;

            SetChildMotherFather(hfMother, hfChild, hfFather);

            // gods why
            StringBuilder bb = new StringBuilder();
            string father = $"{bb} the {hfFather.Name} had a child with {hfMother.Name}, the child was named {hfChild.Name}";
            string mother = $"{bb} the  {hfMother.Name} conceived a child with {hfFather.Name}, the child was named {hfChild.Name}";
            string child = $"{bb} was born child of {hfMother.Name} and {hfFather.Name}";

            hfChild.AddLegend(child, year);
            hfFather.AddLegend(father, year);
            hfMother.AddLegend(mother, year);
        }

        private void SetChildMotherFather(HistoricalFigure hfMother,
            HistoricalFigure hfChild,
            HistoricalFigure hfFather)
        {
            AddToFamilyLink(hfMother, HfRelationType.OwnChild, hfChild);
            AddToFamilyLink(hfChild, HfRelationType.Mother, hfMother);
            // find the fahter
            AddToFamilyLink(hfFather, HfRelationType.OwnChild, hfChild);
            AddToFamilyLink(hfChild, HfRelationType.Father, hfFather);
        }

        public void SetMarriedRelation(HistoricalFigure hfOne, HistoricalFigure hfTwo)
        {
            this.AddToFamilyLink(hfOne, HfRelationType.Married, hfTwo);
        }

        public int? GetSpouseIfAny()
        {
            return Nodes.FirstOrDefault(i => i.Relation is HfRelationType.Married)?.Figure.Id;
        }
    }

    public class FamilyNode : BasicTreeNode<FamilyNode>
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

        public bool IsCloseFamily()
        {
            if (Relation is HfRelationType.Married)
                return true;
            if (Relation is HfRelationType.OwnChild && Figure.IsAdult())
                return true;
            return false;
        }
    }
}