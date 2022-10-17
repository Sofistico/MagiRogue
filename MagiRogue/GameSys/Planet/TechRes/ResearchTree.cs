using MagiRogue.Data.Enumerators;
using MagiRogue.Entities;
using MagiRogue.GameSys.Civ;
using MagiRogue.GameSys.Planet.History;
using MagiRogue.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MagiRogue.GameSys.Planet.TechRes
{
    public class ResearchTree
    {
        public List<ResearchTreeNode> Nodes { get; set; }
        public ResearchTreeNode CurrentResearchFocus { get; set; }

        public ResearchTree()
        {
            Nodes = new();
        }

        public void ConfigureNodes()
        {
            foreach (ResearchTreeNode node in Nodes)
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

        public void GetNodeForResearch(HistoricalFigure figure)
        {
            if (Nodes.Any(i => !i.Finished))
            {
                ResearchTreeNode[] nodes = GetUnfinishedNodes();
                // priotize by roots first
                if (nodes.Any(i => i.IsRoot))
                {
                    int maxTries = 30;
                    int tries = 0;
                    while (tries <= maxTries)
                    {
                        var getRoot = nodes.GetRandomItemFromList();
                        bool hasAnyAbility = CheckIfHasAbilityToResearchNode(figure, getRoot);
                        if (hasAnyAbility)
                        {
                            CurrentResearchFocus = getRoot;
                            return;
                        }
                        tries++;
                        if (figure.TrainingFocus is not AbilityName.None)
                            return;
                    }
                }
                // if all roots are researched, then go select any that has all parents researched!
                if (nodes.Any(i => i.Parents.All(i => i.Finished)))
                {
                    ResearchTreeNode[] nodesWithParentsDone =
                        nodes.Where(i => i.Parents.All(i => i.Finished)).ToArray();
                    List<ResearchTreeNode> nodesWithEnoughAbilitiesForRes
                        = new List<ResearchTreeNode>();
                    for (int i = 0; i < nodesWithParentsDone.Length; i++)
                    {
                        ResearchTreeNode possibleNode = nodesWithParentsDone[i];
                        bool hasAbility = CheckIfHasAbilityToResearchNode(figure, possibleNode);
                        if (hasAbility)
                            nodesWithEnoughAbilitiesForRes.Add(possibleNode);
                    }
                    if (nodesWithEnoughAbilitiesForRes.Count > 0)
                        CurrentResearchFocus = nodesWithEnoughAbilitiesForRes.GetRandomItemFromList();
                }
                return;
            }
        }

        private static bool CheckIfHasAbilityToResearchNode(HistoricalFigure figure, ResearchTreeNode node)
        {
            var abilities = node.GetRequisiteAbilitiesForResearch(figure);
            return abilities is not null && abilities.Count > 0;
        }

        private ResearchTreeNode[] GetUnfinishedNodes()
        {
            ResearchTreeNode[] nodes = Nodes.Where(i => !i.Finished).ToArray();
            return nodes;
        }

        private void DefineNodeRelation(ResearchTreeNode childNode, string str)
        {
            var parentNode = Nodes.FirstOrDefault(i => i.Research.Id.Equals(str));
            if (parentNode is not null)
            {
                childNode.Parents.Add(parentNode);
                parentNode.Children.Add(childNode);
            }
        }
    }

    public class ResearchTreeNode
    {
        public Research Research { get; set; }
        public List<ResearchTreeNode> Children { get; set; }
        public List<ResearchTreeNode> Parents { get; set; }
        public int RequiredRP { get; set; } // RP = Research Points
        public int CurrentRP { get; set; }
        public bool Finished { get => CurrentRP >= RequiredRP; }
        public bool IsRoot { get => Parents.Count <= 0; }

        public ResearchTreeNode(Research research, int currentRpIfAny = 0)
        {
            Research = research;
            CurrentRP = currentRpIfAny;
            RequiredRP = Research.Difficulty * (Mrn.Exploding2D6Dice / 2);
            Parents = new();
            Children = new();
        }

        public void ForceFinish()
        {
            CurrentRP = RequiredRP;
        }

        public List<AbilityName> GetRequisiteAbilitiesForResearch()
        {
            var getResAbilities = Research.AbilityRequired;
            List<AbilityName> abilitiesNeeded = new();
            int count = getResAbilities.Count;
            // special handling for unusual cases for
            // AnyCraft, AnyResearch, AnyMagic, AnyCombat and AnyJob
            for (int i = 0; i < count; i++)
            {
                string abilityString = getResAbilities[i];
                switch (abilityString)
                {
                    case "AnyCraft":
                        abilitiesNeeded.AddRange(new AbilityName[]
                        {
                            AbilityName.Mason,
                            AbilityName.WoodCraft,
                            AbilityName.Forge,
                            AbilityName.Smelt,
                            AbilityName.Weaver,
                            AbilityName.Alchemy,
                            AbilityName.Enchantment
                        });
                        break;

                    case "AnyResearch":
                        abilitiesNeeded.AddRange(new AbilityName[]
                        {
                            AbilityName.MagicTheory,
                            AbilityName.MagicLore,
                            AbilityName.Research,
                            AbilityName.Mathematics,
                            AbilityName.Astronomer,
                            AbilityName.Chemist,
                            AbilityName.Physics,
                            AbilityName.Enginner,
                        });
                        break;

                    case "AnyMagic":
                        abilitiesNeeded.AddRange(new AbilityName[]
                        {
                            AbilityName.MagicTheory,
                            AbilityName.MagicLore
                        });
                        break;

                    case "AnyCombat":
                        abilitiesNeeded.AddRange(new AbilityName[]
                        {
                            AbilityName.Unarmed,
                            AbilityName.Misc,
                            AbilityName.Sword,
                            AbilityName.Staff,
                            AbilityName.Hammer,
                            AbilityName.Spear,
                            AbilityName.Axe,
                        });
                        break;

                    case "AnyJob":
                        abilitiesNeeded.AddRange(new AbilityName[]
                        {
                            AbilityName.Farm,
                            AbilityName.Medicine,
                            AbilityName.Surgeon,
                            AbilityName.Miner,
                            AbilityName.Brewer,
                            AbilityName.Cook,
                        });
                        break;

                    default:
                        abilitiesNeeded.Add(Enum.Parse<AbilityName>(abilityString));
                        break;
                }
            }

            return abilitiesNeeded;
        }

        public List<AbilityName> GetRequisiteAbilitiesForResearch(HistoricalFigure figure)
        {
            if (figure.TrainingFocus is AbilityName.None)
                return null;
            var abilities = GetRequisiteAbilitiesForResearch();
            List<AbilityName> abilitiesIntersection = figure.Mind.ReturnIntersectionAbilities(abilities);
            if (abilitiesIntersection.Count <= 0)
            {
                figure.SetCurrentAbilityTrainingFocus(abilities.GetRandomItemFromList());
            }

            return abilitiesIntersection;
        }
    }
}