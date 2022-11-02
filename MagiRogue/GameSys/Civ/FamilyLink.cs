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
            if (!Nodes.Any(i => i.OtherFigure.Id == otherFigure.Id))
                Nodes.Add(new FamilyNode(type, figure, otherFigure));
        }

        public FamilyNode[] GetOtherFamilyNodesByRelations(HfRelationType toFind)
        {
            return Nodes.Where(i => i.Relation == toFind).ToArray();
        }

        public bool GetIfExistsAnyRelationOfType(HfRelationType toFind)
        {
            return Nodes.Any(i => i.Relation == toFind);
        }

        public void SetMotherChildFatherRelation(HistoricalFigure hfMother,
            HistoricalFigure hfChild,
            int year)
        {
            var hfFather = GetOtherFamilyNodesByRelations(HfRelationType.Married)[0].OtherFigure;

            SetChildMotherFather(hfMother, hfChild, hfFather);

            // gods why
            StringBuilder bb = new StringBuilder($"In the year {year}");
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

        public HistoricalFigure? GetSpouseIfAny()
        {
            return Nodes.FirstOrDefault(i => i.Relation is HfRelationType.Married)?.OtherFigure;
        }
    }

    public class FamilyNode : BasicTreeNode<FamilyNode>
    {
        public HfRelationType Relation { get; set; }
        public HistoricalFigure OtherFigure { get; set; }
        public HistoricalFigure Figure { get; set; }

        public FamilyNode(HfRelationType relation, HistoricalFigure figure, HistoricalFigure otherFigure)
        {
            Relation = relation;
            Figure = figure;
            OtherFigure = otherFigure;
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