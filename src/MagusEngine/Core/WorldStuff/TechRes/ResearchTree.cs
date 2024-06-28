using Arquimedes.Enumerators;
using MagusEngine.Core.WorldStuff.History;
using MagusEngine.Utils;
using MagusEngine.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MagusEngine.Core.WorldStuff.TechRes
{
    public class ResearchTree : BasicTreeStructure<ResearchTreeNode>
    {
        //public List<ResearchTreeNode> Nodes { get; set; }
        public ResearchTreeNode? CurrentResearchFocus { get; set; }

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
                if (nodes.Any(i => i?.IsRoot == true))
                {
                    const int maxTries = 30;
                    int tries = 0;
                    while (tries <= maxTries)
                    {
                        var getRoot = nodes.Where(i => i?.IsRoot == true).ToList().GetRandomItemFromList();
                        bool hasAnyAbility = CheckIfHasAbilityToResearchNode(figure, getRoot);
                        if (hasAnyAbility)
                        {
                            CurrentResearchFocus = getRoot;
                            return;
                        }
                        tries++;
                        if (figure.TrainingFocus is not AbilityCategory.None)
                            return;
                    }
                }
                // if all roots are researched, then go select any that has all parents researched!
                if (nodes.Any(i => i.Parents.All(i => i.Finished)))
                {
                    ResearchTreeNode[] nodesWithParentsDone =
                        nodes.Where(i => i.Parents.All(i => i.Finished)).ToArray();
                    List<ResearchTreeNode> nodesWithEnoughAbilitiesForRes = new();
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
            }
        }

        private static bool CheckIfHasAbilityToResearchNode(HistoricalFigure figure, ResearchTreeNode node)
        {
            var abilities = node.GetRequisiteAbilitiesForResearch(figure);
            return abilities?.Count > 0;
        }

        private ResearchTreeNode[] GetUnfinishedNodes()
        {
            return Nodes.Where(i => !i.Finished).ToArray();
        }

        private void DefineNodeRelation(ResearchTreeNode childNode, string str)
        {
            var parentNode = Nodes.Find(i => i.Research.Id.Equals(str));
            if (parentNode is not null)
            {
                childNode?.Parents?.Add(parentNode);
                parentNode?.Children?.Add(childNode!);
            }
        }
    }

    public class ResearchTreeNode : BasicTreeNode<ResearchTreeNode>
    {
        public Research Research { get; set; }

        //public List<ResearchTreeNode> Children { get; set; }
        //public List<ResearchTreeNode> Parents { get; set; }
        public int RequiredRP { get; set; } // RP = Research Points
        public int CurrentRP { get; set; }
        public bool Finished { get => CurrentRP >= RequiredRP; }
        //public bool IsRoot { get => Parents.Count <= 0; }

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

        public List<AbilityCategory> GetRequisiteAbilitiesForResearch()
        {
            var getResAbilities = Research.AbilityRequired;
            List<AbilityCategory> abilitiesNeeded = new();
            int count = getResAbilities.Count;
            // special handling for unusual cases for AnyCraft, AnyResearch, AnyMagic, AnyCombat and AnyJob
            for (int i = 0; i < count; i++)
            {
                string abilityString = getResAbilities[i];
                switch (abilityString)
                {
                    case "AnyCraft":
                        abilitiesNeeded.AddRange(new AbilityCategory[]
                        {
                            AbilityCategory.Mason,
                            AbilityCategory.WoodCraft,
                            AbilityCategory.Forge,
                            AbilityCategory.Smelt,
                            AbilityCategory.Weaver,
                            AbilityCategory.Alchemy,
                            AbilityCategory.Enchantment,
                            AbilityCategory.GlassMaking
                        });
                        break;

                    case "AnyResearch":
                        abilitiesNeeded.AddRange(new AbilityCategory[]
                        {
                            AbilityCategory.MagicTheory,
                            AbilityCategory.MagicLore,
                            AbilityCategory.Research,
                            AbilityCategory.Mathematics,
                            AbilityCategory.Astronomer,
                            AbilityCategory.Chemist,
                            AbilityCategory.Physics,
                            AbilityCategory.Enginner,
                        });
                        break;

                    case "AnyMagic":
                        abilitiesNeeded.AddRange(new AbilityCategory[]
                        {
                            AbilityCategory.MagicTheory,
                            AbilityCategory.MagicLore
                        });
                        break;

                    case "AnyCombat":
                        abilitiesNeeded.AddRange(new AbilityCategory[]
                        {
                            AbilityCategory.Unarmed,
                            AbilityCategory.Misc,
                            AbilityCategory.Sword,
                            AbilityCategory.Staff,
                            AbilityCategory.Hammer,
                            AbilityCategory.Spear,
                            AbilityCategory.Axe,
                        });
                        break;

                    case "AnyJob":
                        abilitiesNeeded.AddRange(new AbilityCategory[]
                        {
                            AbilityCategory.Farm,
                            AbilityCategory.Medicine,
                            AbilityCategory.Surgeon,
                            AbilityCategory.Miner,
                            AbilityCategory.Brewer,
                            AbilityCategory.Cook,
                        });
                        break;

                    default:
                        abilitiesNeeded.Add(Enum.Parse<AbilityCategory>(abilityString));
                        break;
                }
            }

            return abilitiesNeeded;
        }

        public List<AbilityCategory>? GetRequisiteAbilitiesForResearch(HistoricalFigure figure)
        {
            if (figure.TrainingFocus is AbilityCategory.None)
                return null;
            var abilities = GetRequisiteAbilitiesForResearch();
            List<AbilityCategory> abilitiesIntersection = figure.Mind.ReturnIntersectionAbilities(abilities);
            if (abilitiesIntersection.Count == 0)
            {
                figure.SetCurrentAbilityTrainingFocus(abilities.GetRandomItemFromList());
            }

            return abilitiesIntersection;
        }
    }
}