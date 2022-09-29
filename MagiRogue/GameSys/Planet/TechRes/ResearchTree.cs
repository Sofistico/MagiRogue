using MagiRogue.GameSys.Civ;
using MagiRogue.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MagiRogue.GameSys.Planet.TechRes
{
    public class ResearchTree
    {
        public List<ReserachTreeNode> Nodes { get; set; }
        public ReserachTreeNode CurrentResearchFocus { get; set; }

        public void ConfigureNodes()
        {
            foreach (ReserachTreeNode node in Nodes)
            {
                Research res = node.Research;
                if (res.RequiredRes.Count > 0)
                {
                    foreach (var str in res.RequiredRes)
                    {
                        DefineNodeRelation(node, str);
                    }
                }
            }
        }

        private void DefineNodeRelation(ReserachTreeNode childNode, string str)
        {
            var parentNode = Nodes.FirstOrDefault(i => i.Research.Id.Equals(str));
            if (parentNode is not null)
            {
                childNode.Parents.Add(parentNode);
                parentNode.Children.Add(childNode);
            }
        }
    }

    public class ReserachTreeNode
    {
        public Research Research { get; set; }
        public List<ReserachTreeNode> Children { get; set; }
        public List<ReserachTreeNode> Parents { get; set; }
        public int RequiredRP { get; set; } // RP = Research Points
        public int CurrentRP { get; set; }
        public bool Finished { get => CurrentRP >= RequiredRP; }

        public ReserachTreeNode(Research research, int currentRpIfAny = 0)
        {
            Research = research;
            CurrentRP = currentRpIfAny;
            RequiredRP = Research.Difficulty * (Mrn.Exploding2D6Dice / 2);
            Parents = new();
            Children = new();
        }
    }
}