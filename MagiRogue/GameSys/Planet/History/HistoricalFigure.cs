using MagiRogue.Data.Enumerators;
using MagiRogue.Utils;
using MagiRogue.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using MagiRogue.Data;

namespace MagiRogue.GameSys.Planet.History
{
    /// <summary>
    /// A class that models a historical figure of the world, if it's still alive, there should be
    /// an associetaded actor to represent it.
    /// </summary>
    public class HistoricalFigure
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Sex HFGender { get; set; }
        public string Race { get; set; }
        public List<Legend> Legends { get; set; } = new();
        public MythWho? MythWho { get; set; }
        public int YearBorn { get; set; }
        public int? YearDeath { get; set; }
        public bool IsAlive { get; set; }

        //public Entity? AssocietedEntity { get; set; }
        public List<int> RelatedCivs { get; set; } = new();
        public List<int> RelatedSettlements { get; set; } = new();
        public List<string> RelatedHFs { get; set; } = new();
        public Mind Mind { get; set; } = new();
        public Soul Soul { get; set; }
        public List<SpecialFlag> SpecialFlags { get; set; } = new();

        public HistoricalFigure(string name,
            Sex hFGender,
            List<Legend> legends,
            int yearBorn,
            int? yearDeath,
            bool isAlive,
            string desc,
            string raceId)
        {
            Name = name;
            HFGender = hFGender;
            Legends = legends;
            YearBorn = yearBorn;
            YearDeath = yearDeath;
            IsAlive = isAlive;
            Description = desc;
            Race = raceId;
        }

        public HistoricalFigure(string name, string description, Sex hFGender, string race, bool isAlive)
        {
            Name = name;
            Description = description;
            HFGender = hFGender;
            Race = race;
            IsAlive = isAlive;
        }

        public HistoricalFigure(string name, string description)
        {
            Name = name;
            Description = description;
        }

        public void GenerateRandomPersonality()
        {
            Personality personality = Mind.Personality;
            // reflection because i'm lazy
            var properties = typeof(Personality).GetProperties();
            foreach (var property in properties)
            {
                property.SetValue(personality, GameLoop.GlobalRand.NextInt(-50, 50 + 1));
            }
        }

        public void GenerateRandomSkills()
        {
            var enums = Enum.GetValues(typeof(AbilityName));

            foreach (AbilityName e in enums)
            {
                bool hasSkill = Mrn.OneIn(10);
                if (hasSkill)
                {
                    // if the die explode, it's a genius!
                    int abilityScore = Mrn.Exploding2D6Dice;
                    Mind.AddAbilityToDictionary(new Ability(e, abilityScore));
                }
            }
        }

        public void DefineProfession()
        {
            List<Ability> skills = new List<Ability>(Mind.Abilities.Values.ToList());
            Ability bestSkill = skills.MaxBy(i => i.Score);
            skills.Remove(bestSkill);
            Ability secondBestSkill = skills.MaxBy(i => i.Score);
            skills.Remove(secondBestSkill);

            var professions = DataManager.ListOfProfessions
                .Where(i => i.Ability.First() == bestSkill.ReturnAbilityEnumFromString()).ToList();

            if (professions.Count > 0)
            {
                foreach (var prof in professions)
                {
                    if (prof.Ability.Length > 0)
                    {
                        if (prof.Ability.Contains(secondBestSkill.ReturnAbilityEnumFromString()))
                        {
                            Mind.Profession = prof;

                            return;
                        }
                    }
                }
                // if you didn't find anything, then anything is worth!
                Mind.Profession = professions.GetRandomItemFromList();
            }
            else
            {
                Mind.Profession = DataManager.QueryProfessionInData("vagabond");
            }
        }

        public void AddLegend(Legend legend)
        {
            Legends.Add(legend);
        }

        public string Pronoum()
        {
            if (HFGender == Sex.Male)
                return "him";
            if (HFGender == Sex.Female)
                return "her";
            return "it";
        }

        public string PronoumPossesive()
        {
            if (HFGender == Sex.Male)
                return "his";
            if (HFGender == Sex.Female)
                return "hers";
            return "it's";
        }

        public Myth MythAct(int id)
        {
            // make the figure do some act as a myth
            MythWhat[] whats = Enum.GetValues<MythWhat>();
            MythWhat what = whats.GetRandomItemFromList();
            MythAction[] actions = Enum.GetValues<MythAction>();
            MythAction action = actions.GetRandomItemFromList();

            Myth myth = new(id)
            {
                MythWho = MythWho ?? Data.Enumerators.MythWho.None,
                MythWhat = what,
                MythAction = action,
                Name = Name,
            };

            return myth;
        }
    }
}