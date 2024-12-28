using Arquimedes.DataStructures;
using Arquimedes.Enumerators;
using MagusEngine.Core.WorldStuff.History;
using MagusEngine.Services;
using MagusEngine.Systems;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagusEngine.Core.Civ
{
    public class FamilyLink : BasicTreeStructure<FamilyNode>
    {
        public FamilyLink()
        {
            Nodes = [];
        }

        public FamilyLink(List<FamilyNode> family)
        {
            Nodes = family;
        }

        public void AddToFamilyLink(HistoricalFigure? figure,
            HfRelationType type,
            HistoricalFigure? otherFigure)
        {
            if (otherFigure is null)
                return;
            if (!Nodes!.Any(i => i.OtherFigureId == otherFigure.Id))
                Nodes?.Add(new FamilyNode(type, figure!, otherFigure));
        }

        public FamilyNode[]? GetOtherFamilyNodesByRelations(HfRelationType toFind)
        {
            return Nodes?.FindAll(i => i.Relation == toFind).ToArray();
        }

        public bool? GetIfExistsAnyRelationOfType(HfRelationType toFind)
        {
            return Nodes?.Any(i => i.Relation == toFind);
        }

        public void SetMotherChildFatherRelation(HistoricalFigure hfMother,
            HistoricalFigure hfChild,
            int year)
        {
            try
            {
                var hfFather = GetOtherFamilyNodesByRelations(HfRelationType.Married)?[0]?.OtherFigure;

                SetChildMotherFather(hfMother, hfChild, hfFather);

                // gods why
                StringBuilder bb = new();
                if (hfFather is not null)
                {
                    string father = $"{bb} the {hfFather.Name} had a child with {hfMother.Name}, the child was named {hfChild.Name}";
                    hfFather.AddLegend(father, year);
                    if (hfFather.SpecialFlags.Contains(SpecialFlag.Myth))
                    {
                        hfChild.SpecialFlags.Add(SpecialFlag.Supernatural);
                        hfChild.SpecialFlags.Add(SpecialFlag.DemiMyth);
                    }
                }
                string mother;
                string child;
                if (hfFather is null)
                {
                    mother = $"{bb} the {hfMother.Name} conceived a child with an unknow father, the child was named {hfChild.Name}";
                    child = $"{bb} was born child of {hfMother.Name} and an unknow father";
                }
                else
                {
                    mother = $"{bb} the {hfMother.Name} conceived a child with {hfFather.Name}, the child was named {hfChild.Name}";
                    child = $"{bb} was born child of {hfMother.Name} and {hfFather.Name}";
                }

                hfChild.AddLegend(child, year);
                hfMother.AddLegend(mother, year);
            }
            catch (Exception)
            {
                Locator.GetService<MagiLog>().Log($"Something went wrong in the creation of children! Mother: {hfMother.Id} Child: {hfChild.Id}");
            }
        }

        private void SetChildMotherFather(HistoricalFigure hfMother,
            HistoricalFigure hfChild,
            HistoricalFigure? hfFather)
        {
            AddToFamilyLink(hfMother, HfRelationType.OwnChild, hfChild);
            AddToFamilyLink(hfChild, HfRelationType.Mother, hfMother);
            // find the fahter
            if (hfFather is null)
            {
                AddToFamilyLink(hfFather, HfRelationType.OwnChild, hfChild);
                AddToFamilyLink(hfChild, HfRelationType.Father, hfFather!);
            }
        }

        public void SetMarriedRelation(HistoricalFigure hfOne, HistoricalFigure hfTwo)
        {
            AddToFamilyLink(hfOne, HfRelationType.Married, hfTwo);
        }

        public HistoricalFigure? GetSpouseIfAny()
        {
            return Nodes?.Find(static i => i.Relation is HfRelationType.Married)?.OtherFigure;
        }
    }

    public class FamilyNode : BasicTreeNode<FamilyNode>
    {
        public HfRelationType Relation { get; set; }

        [JsonIgnore]
        public HistoricalFigure OtherFigure => Find.Figures!.Find(i => i.Id == OtherFigureId)!;

        [JsonIgnore]
        public HistoricalFigure Figure => Find.Figures!.Find(i => i.Id == FigureId)!;
        public int OtherFigureId { get; set; }
        public int FigureId { get; set; }

        public FamilyNode()
        {
        }

        public FamilyNode(HfRelationType relation, int figure, int otherFigure)
        {
            Relation = relation;
            FigureId = figure;
            OtherFigureId = otherFigure;
        }

        public FamilyNode(HfRelationType relation, HistoricalFigure figure, HistoricalFigure otherFigure)
        {
            Relation = relation;
            FigureId = figure.Id;
            if (otherFigure is not null)
                OtherFigureId = otherFigure.Id;
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
