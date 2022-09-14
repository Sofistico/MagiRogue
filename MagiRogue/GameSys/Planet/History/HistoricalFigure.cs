using MagiRogue.Data.Enumerators;
using MagiRogue.Utils;
using MagiRogue.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using MagiRogue.Data;
using MagiRogue.GameSys.Civ;
using System.Text;

namespace MagiRogue.GameSys.Planet.History
{
    /// <summary>
    /// A class that models a historical figure of the world, if it's still alive, there should be
    /// an associetaded actor to represent it.
    /// </summary>
    public sealed class HistoricalFigure
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
        public List<int> RelatedSites { get; set; } = new();
        public List<string> RelatedHFs { get; set; } = new();
        public Mind Mind { get; set; } = new();
        public Soul Soul { get; set; }
        public List<SpecialFlag> SpecialFlags { get; set; } = new();
        public List<Noble> NobleTitles { get; set; } = new();
        public int CurrentAge { get; set; }

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
                if (e is AbilityName.None)
                    continue;
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
            Ability bestSkill = new Ability(AbilityName.None, 0);
            if (skills.Count > 0)
            {
                bestSkill = skills.MaxBy(i => i.Score);
                skills.Remove(bestSkill);
            }

            Ability secondBestSkill;
            if (skills.Count > 0)
            {
                secondBestSkill = skills.MaxBy(i => i.Score);
                skills.Remove(secondBestSkill);
            }
            else
                secondBestSkill = new Ability(AbilityName.None, 0);

            if (!DetermineProfessionFromBestSkills(bestSkill, secondBestSkill))
            {
                // if no profession was found, then the figure isn't great with any profession sadly!
                // it's tough being a legenday chemist in the middle ages....
                Mind.Profession = DataManager.QueryProfessionInData("vagabond");
            }
        }

        private bool DetermineProfessionFromBestSkills(Ability bestAbility, Ability secondBest)
        {
            var professions = DataManager.ListOfProfessions
                .Where(i => i.Ability.First() == bestAbility.ReturnAbilityEnumFromString()).ToList();

            if (professions.Count > 0)
            {
                foreach (var prof in professions)
                {
                    if (prof.Ability.Length > 0)
                    {
                        if (prof.Ability.Contains(secondBest.ReturnAbilityEnumFromString()))
                        {
                            Mind.Profession = prof;

                            return true;
                        }
                    }
                }
                // if you didn't find anything, then anything is worth!
                Mind.Profession = professions.GetRandomItemFromList();
                return true;
            }
            return false;
        }

        public void AddLegend(Legend legend)
        {
            Legends.Add(legend);
        }

        public void AddLegend(string legend, int yearWhen)
        {
            Legend newLegend = new Legend(legend, yearWhen);
            Legends.Add(newLegend);
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

        public void AddNewNoblePos(Noble noble, int year, Civilization civ)
        {
            if (NobleTitles.Contains(noble))
                return;
            if (!RelatedCivs.Contains(civ.Id))
                RelatedCivs.Add(civ.Id);

            NobleTitles.Add(noble);
            StringBuilder initialLegend = new StringBuilder($"In the year {year} ");
            initialLegend.Append($"{Name} ascended to the post of {noble.Name} ");
            initialLegend.Append($"with {CurrentAge} years as a member of {civ.Name}");
            AddLegend(initialLegend.ToString(), year);
        }

        public void HistoryAct(int year, Tiles.WorldTile[,] tiles, List<Civilization> civs)
        {
            HistoryAction historyAction = new HistoryAction(this,
                year,
                civs,
                tiles);
            historyAction.Act();
            Legends.Add(historyAction.Legend);
        }

        public Personality GetPersonality()
            => Mind.Personality;
    }
}