using MagiRogue.GameSys.Civ;
using MagiRogue.Utils;
using System.Collections.Generic;

namespace MagiRogue.GameSys.Planet.TechRes
{
    public class ResearchTree
    {
        public List<ReserachTreeNode> Nodes { get; set; }
        public ReserachTreeNode CurrentResearchFocus { get; set; }
    }

    public class ReserachTreeNode
    {
        public Research Research { get; set; }
        public List<ReserachTreeNode> Children { get; set; }
        public int RequiredRP { get; set; } // RP = Research Points
        public int CurrentRP { get; set; }
        public bool Finished { get => CurrentRP >= RequiredRP; }

        public ReserachTreeNode(Discovery discovery, int currentRpIfAny = 0)
        {
            CurrentRP = currentRpIfAny;
            RequiredRP = discovery.WhatWasResearched.Difficulty * (Mrn.Exploding2D6Dice / 2);
        }
    }
}