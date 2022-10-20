using MagiRogue.Data.Enumerators;
using MagiRogue.Utils;
using MagiRogue.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using MagiRogue.Data;
using MagiRogue.GameSys.Civ;
using System.Text;
using MagiRogue.GameSys.Tiles;
using MagiRogue.GameSys.Planet.TechRes;
using MagiRogue.Data.Serialization;
using MagiRogue.GameSys.Magic;

namespace MagiRogue.GameSys.Planet.History
{
    /// <summary>
    /// A class that models a historical figure of the world, if it's still alive, there should be
    /// an associetaded actor to represent it.
    /// </summary>
    public sealed class HistoricalFigure
    {
        // years that the current long activity started
        private int seasonsOnActivity = 0;
        private int whatScoreToSettleForTrainingAbility;

        #region Props

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Sex HFGender { get; set; }
        public List<Legend> Legends { get; set; } = new();
        public MythWho MythWho { get; set; }
        public int YearBorn { get; set; }
        public int? YearDeath { get; set; }
        public bool IsAlive { get; set; }

        //public Entity? AssocietedEntity { get; set; }
        public List<CivRelation> RelatedCivs { get; set; } = new();
        public List<SiteRelation> RelatedSites { get; set; } = new();

        /// <summary>
        /// Should be used by any relation other than familial one, for family you should look for <see cref="FamilyLink"/>
        /// </summary>
        public List<HfRelation> RelatedHFs { get; set; } = new();
        public List<Discovery> DiscoveriesKnow { get; set; } = new();
        public Body Body { get; set; } = new();
        public Mind Mind { get; set; } = new();
        public Soul Soul { get; set; } = new();
        public List<SpecialFlag> SpecialFlags { get; set; } = new();
        public List<Noble> NobleTitles { get; set; } = new();
        public ResearchTree ResearchTree { get; set; }

        /// <summary>
        /// When someone is anxious, it's higher the chance that he will do something stupid!
        /// </summary>
        public bool AnxiousInRegardsToActivity { get; set; }

        /// <summary>
        /// Flag to check if the figure is doing some long term important stuff like research
        /// <br>The figure can only do one long term activity</br>
        /// </summary>
        public bool DoingLongTermActivity { get; set; }
        public AbilityName TrainingFocus { get; private set; }
        public bool Pregnant { get; private set; }

        /// <summary>
        /// Family links spans the same instance among many different entities
        /// </summary>
        public FamilyLink FamilyLink { get; set; } = new();

        public Point CurrentPos { get; private set; }

        #endregion Props

        #region Ctor

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
            Id = GameLoop.GetHfId();
            GetAnatomy().Race = raceId;
            GetAnatomy().Gender = hFGender;
        }

        public HistoricalFigure(string name, string description, Sex hFGender, string race, bool isAlive)
        {
            Name = name;
            Description = description;
            HFGender = hFGender;
            GetAnatomy().Race = race;
            GetAnatomy().Gender = hFGender;
            IsAlive = isAlive;
            Id = GameLoop.GetHfId();
        }

        public HistoricalFigure(string name, string description)
        {
            Name = name;
            Description = description;
            Id = GameLoop.GetHfId();
        }

        #endregion Ctor

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
            // TODO: Hfs will begins with X random amount of xp to spend, then they will use it rather than
            // having some random value!
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
            StringBuilder str = new StringBuilder($"In the year of {yearWhen}, ");
            str.Append(legend);
            Legend newLegend = new Legend(str.ToString(), yearWhen);
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

        public Myth MythAct()
        {
            // make the figure do some act as a myth
            MythWhat[] whats = Enum.GetValues<MythWhat>();
            MythWhat what = whats.GetRandomItemFromList();
            MythAction[] actions = Enum.GetValues<MythAction>();
            MythAction action = actions.GetRandomItemFromList();

            Myth myth = new()
            {
                MythWho = this.MythWho,
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
            if (!RelatedCivs.Any(c => c.CivRelatedId == civ.Id))
            {
                if (civ.GetRulerNoblePosition().Item1.Equals(noble))
                    AddNewRelationToCiv(civ.Id, RelationType.Ruler);
                else
                    AddNewRelationToCiv(civ.Id, RelationType.Member);
            }

            NobleTitles.Add(noble);
            StringBuilder initialLegend = new StringBuilder();
            initialLegend.Append($"{Name} ascended to the post of {noble.Name} ");
            initialLegend.Append($"with {Body.Anatomy.CurrentAge} years as a member of {civ.Name}");
            AddLegend(initialLegend.ToString(), year);
        }

        public void AddNewRelationToCiv(int civId, RelationType relation)
        {
            CivRelation re = new CivRelation(Id, civId, relation);
            if (!RelatedCivs.Any(i => i.CivRelatedId == civId))
                RelatedCivs.Add(re);
            else
                RelatedCivs.FirstOrDefault(i => i.CivRelatedId == civId).Relation = relation;
        }

        public void HistoryAct(int year, WorldTile[,] tiles,
            List<Civilization> civs, List<HistoricalFigure> figures, List<Site> sites,
            AccumulatedHistory accumulatedHistory, List<Item> items)
        {
            // TODO: Calculate the impact of the instantiantion of this object in the future!
            HistoryAction historyAction = new HistoryAction(this,
                year,
                civs,
                tiles,
                figures,
                sites,
                accumulatedHistory,
                items);
            historyAction.Act();
        }

        public Personality GetPersonality()
            => Mind.Personality;

        // see if there is anyway to refactor this to be better!
        // maybe some good old OOP
        /// <summary>
        /// Adds a relation type to another historifacl figure!
        /// </summary>
        /// <param name="otherId"></param>
        /// <param name="relation"></param>
        /// <returns>true if already had previous relation, false if otherwise</returns>
        public bool AddRelatedHf(int otherId, HfRelationType relation)
        {
            if (RelatedHFs.Any(i => i.OtherHfId == otherId))
            {
                RelatedHFs.FirstOrDefault(i => i.OtherHfId == otherId).RelationType = relation;
                return true;
            }
            else
            {
                RelatedHFs.Add(new HfRelation(Id, otherId, relation));
                return false;
            }
        }

        public void AddRelatedSite(int otherId, SiteRelationTypes relation)
        {
            if (!RelatedSites.Any(i => i.OtherSiteId == otherId))
                RelatedSites.Add(new SiteRelation(Id, otherId, relation));
            else
            {
                var related = RelatedSites.FirstOrDefault(i => i.OtherSiteId == otherId);
                if (related.RelationType.HasFlag(relation))
                    return;
                related.RelationType &= relation;
            }
        }

        public SiteRelation FindSiteRelation(int siteId) =>
            RelatedSites.FirstOrDefault(i => i.OtherSiteId == siteId);

        public string GetRaceName()
        {
            Race race = Body.Anatomy.GetRace();
            return race.RaceName;
        }

        public string GetRaceNamePlural()
        {
            Race race = Body.Anatomy.GetRace();
            return race.NamePlural;
        }

        public Race GetRace()
        {
            Race race = Body.Anatomy.GetRace();
            return race;
        }

        internal void RemovePreviousSiteRelation(int id)
        {
            RelatedSites.RemoveAll(i => i.OtherSiteId == id);
        }

        public int? GetLivingSiteId()
        {
            return RelatedSites.FirstOrDefault(i => i.RelationType.HasFlag(SiteRelationTypes.LivesThere)).OtherSiteId;
        }

        public bool CheckForProlificStudious()
        {
            bool valuesInRange = GetPersonality().Knowledge >= 10
                && (GetPersonality().Perseverance >= 10
                || GetPersonality().HardWork >= 25);
            return valuesInRange;
        }

        public bool CheckForAgressiveInfluence()
        {
            return GetPersonality().Power >= 50
                && GetPersonality().Peace <= 0;
        }

        public Civilization GetRelatedCivFromFigure(RelationType relationType, List<Civilization> civs)
        {
            if (RelatedCivs.Count <= 0)
                return null;
            int civId = RelatedCivs.Find(i => i.Relation == relationType).CivRelatedId;
            Civilization civ = civs.Find(i => i.Id == civId);
            return civ;
        }

        public int GetMemberCivId()
            => RelatedCivs.FirstOrDefault(i => i.GetIfMember()).CivRelatedId;

        public bool CheckForInsurrection()
            => GetPersonality().Power >= 25;

        public bool DoResearch(double bonusResearchPower = 0)
        {
            if (ResearchTree.CurrentResearchFocus is null)
                return false;
            List<AbilityName> abilitiesIntersection = ResearchTree
                .CurrentResearchFocus
                .GetRequisiteAbilitiesForResearch(this);

            if (abilitiesIntersection is null || abilitiesIntersection.Count <= 0)
                return false;
            double totalRes = bonusResearchPower;
            foreach (AbilityName abiName in abilitiesIntersection)
            {
                // research should take years, but let's observe if will take too long as well...
                int abilityScore = Mind.GetAbility(abiName);
                totalRes += (abilityScore * Mrn.Exploding2D6Dice);
            }
            CheckForAnxious();

            ResearchTree.CurrentResearchFocus.CurrentRP += (int)MathMagi.Round(totalRes);
            seasonsOnActivity++;
            DoingLongTermActivity = true;

            return ResearchTree.CurrentResearchFocus.Finished;
        }

        public void ClearAnxiousness()
        {
            AnxiousInRegardsToActivity = false;
            seasonsOnActivity = 0;
        }

        public void CheckForAnxious()
        {
            if (seasonsOnActivity <= 4)
                return;

            if (GetPersonality().Leisure <= 0
            && GetPersonality().SelfControl <= 10
            && GetPersonality().Perseverance <= 0)
            {
                bool sucess = Mrn.OneIn(GetPersonality().Perseverance * -1);
                if (!sucess)
                {
                    AnxiousInRegardsToActivity = true;
                }
            }
        }

        public Discovery ReturnDiscoveryFromCurrentFocus(Site site)
        {
            return new Discovery(Id,
                $"{Name} finished research on {ResearchTree.CurrentResearchFocus.Research.Name}",
                $"{site.Name}",
                ResearchTree.CurrentResearchFocus.Research);
        }

        public void CleanupResearch(Site site, int year)
        {
            var disc = ReturnDiscoveryFromCurrentFocus(site);
            if (disc is null) // if for some reason dis is null
                return;
            AddLegend(disc.ReturnLegendFromDiscovery(year));
            site.AddDiscovery(disc);
            site.AddLegend($"the {Name} added a new discovery to the site {site.Name} knowlodge!", year);
            ResearchTree.CurrentResearchFocus = null;
            ClearAnxiousness();
            DoingLongTermActivity = false;
        }

        public void ForceCleanupResearch()
        {
            ResearchTree.CurrentResearchFocus = null;
            ClearAnxiousness();
            DoingLongTermActivity = false;
        }

        public bool CheckForAnyStudious()
        {
            return GetPersonality().Knowledge >= 0 && GetPersonality().HardWork >= 10;
        }

        internal bool CheckForAnger()
        {
            return GetPersonality().Anger >= 20 && GetPersonality().SelfControl <= 10;
        }

        public void TryAttackAndMurder(HistoricalFigure deadThing, int year, string specialReason = "")
        {
            var combatAbilityAttacker = Mind.CheckForCombatAbilities().GetRandomItemFromList();
            var defensiveAbilityAttacker = Mind.CheckForDefensiveAbilities().GetRandomItemFromList();
            var combatAbilityDefender = Mind.CheckForCombatAbilities().GetRandomItemFromList();
            var defensiveAbilyDefender = Mind.CheckForDefensiveAbilities().GetRandomItemFromList();

            StringBuilder killer = new StringBuilder($"In the year of {year} ");
            StringBuilder victim = new StringBuilder($"In the year of {year} ");

            // TODO: Generalize this in the future
            if (Mind.GetAbility(combatAbilityAttacker) >= deadThing.Mind.GetAbility(defensiveAbilyDefender))
            {
                deadThing.KillIt(year);
                killer.Append($"the {Name} killed {deadThing.Name}");
                killer.Append(specialReason);

                victim.Append($"the {deadThing.Name} was killed by {Name}");
                victim.Append(specialReason);

                AddLegend(new Legend(killer.ToString(), year));
                deadThing.AddLegend(new Legend(victim.ToString(), year));
                return;
            }
            if (Mind.GetAbility(combatAbilityDefender) <= deadThing.Mind.GetAbility(defensiveAbilityAttacker))
            {
                KillIt(year);
                killer.Append($"the {deadThing.Name} killed {Name}");
                killer.Append("in self defense");

                victim.Append($"the {Name} was killed by {deadThing.Name}");
                victim.Append("in self defense");
                AddLegend(new Legend(victim.ToString(), year));
                deadThing.AddLegend(new Legend(killer.ToString(), year));
                return;
            }

            killer.Append($"the {Name} tried to kill {deadThing.Name}");
            killer.Append(specialReason);
            killer.Append(" but failed!");

            victim.Append($"the {deadThing.Name} had a murder attempeted by {Name}");
            victim.Append(specialReason);
            killer.Append($" but {Pronoum()} failed!");

            AddLegend(new Legend(killer.ToString(), year));
            deadThing.AddLegend(new Legend(victim.ToString(), year));
        }

        public void KillIt(int year, string whyItDied = "")
        {
            YearDeath = year;
            IsAlive = false;
            if (!string.IsNullOrEmpty(whyItDied))
            {
                AddLegend(whyItDied, year);
            }
        }

        public void CleanupIfNotImportant(int year)
        {
            if (YearDeath - year >= 10)
            {
                ForceCleanupResearch();
                CleanRelations();
            }
        }

        private void CleanRelations()
        {
            RelatedCivs.Clear();
            RelatedHFs.Clear();
            RelatedSites.Clear();
        }

        public void SetupResearchTree(bool isMagical)
        {
            List<Research> researches;
            if (isMagical)
                researches = new List<Research>(DataManager.ListOfResearches);
            else
                researches = new List<Research>(DataManager.ListOfResearches.Where(i => !i.IsMagical));
            ResearchTree = new ResearchTree();
            foreach (Research item in researches)
            {
                ResearchTreeNode node = new ResearchTreeNode(item);
                ResearchTree.Nodes.Add(node);
                if (DiscoveriesKnow.Exists(i => i.WhatWasResearched.Id.Equals(item.Id)))
                    node.ForceFinish();
            }
            ResearchTree.ConfigureNodes();
        }

        public bool CheckForGreed()
        {
            return GetPersonality().Greed >= 10 && GetPersonality().SelfControl <= 10;
        }

        public bool CheckForStudiousInfluence()
        {
            return GetPersonality().Knowledge >= 10
                && GetPersonality().Perseverance > 0;
        }

        public void SetCurrentAbilityTrainingFocus(AbilityName abilityName)
        {
            TrainingFocus = abilityName;
        }

        public void TrainAbilityFocus()
        {
            if (TrainingFocus is not AbilityName.None)
            {
                whatScoreToSettleForTrainingAbility = Mrn.Normal2D6Dice;
                if (!Mind.Abilities.ContainsKey((int)TrainingFocus))
                {
                    Mind.Abilities.Add((int)TrainingFocus, new Ability(TrainingFocus, 0));
                }
                else
                {
                    Ability ability = Mind.ReturnAbilityFromName(TrainingFocus);
                    if (ability.ReturnAbilityEnumFromString() is not AbilityName.None)
                    {
                        ability.XpTotal +=
                            (Mind.Personality.HardWork + (Mrn.Exploding2D6Dice * Mind.Inteligence));
                    }
                    if (ability.Score >= whatScoreToSettleForTrainingAbility)
                        TrainingFocus = AbilityName.None;
                }
            }
            else
            {
                TrainingFocus = Enum.GetValues<AbilityName>().GetRandomItemFromList();
            }
        }

        public bool CheckForHardwork()
        {
            return GetPersonality().HardWork >= 10 || (GetPersonality().Perseverance >= 10);
        }

        public void SetStandardRaceFlags()
        {
            foreach (var flag in Body.Anatomy.GetRace()?.Flags)
            {
                AddNewFlag(flag);
            }
        }

        public void AddNewFlag(SpecialFlag flag)
        {
            if (SpecialFlags.Contains(flag))
                return;
            SpecialFlags.Add(flag);
        }

        internal bool CheckForRomantic()
        {
            return GetPersonality().Romance >= 0;
        }

        public bool CheckForFriendship()
        {
            return GetPersonality().Friendship >= 0;
        }

        public void Marry(HistoricalFigure randomPerson)
        {
            if (IsMarried())
                return;
            FamilyLink.SetMarriedRelation(this, randomPerson);
            randomPerson.FamilyLink.SetMarriedRelation(randomPerson, this);
        }

        public bool IsMarried()
        {
            return FamilyLink.GetIfExistsAnyRelationOfType(this, HfRelationType.Married);
        }

        public bool MakeFriend(HistoricalFigure randomPerson)
        {
            return AddRelatedHf(randomPerson.Id, HfRelationType.Friend);
        }

        public int? GetRelatedHfSpouseId()
        {
            return FamilyLink.GetSpouseIfAny();
        }

        public void MakeBabyWith(HistoricalFigure spouse)
        {
            int rngChance = Mrn.Normal1D100Dice;
            if (rngChance >= 90)
            {
                if (HFGender is Sex.Female)
                {
                    Pregnant = true;
                }
                else
                {
                    spouse.Pregnant = true;
                }
            }
        }

        public void ConceiveChild(Civilization civBorn, int yearBorn)
        {
            Pregnant = false;
            var hfChild = civBorn.CreateNewHfMemberFromBirth(yearBorn, FamilyLink);
            // mother child relation
            FamilyLink.SetMotherChildFatherRelation(this, hfChild, yearBorn);
        }

        public void ChangeLivingSite(int id)
        {
            if (RelatedSites.Any(i => i.RelationType is SiteRelationTypes.LivesThere))
            {
                var otherSite = RelatedSites.FirstOrDefault(i => i.RelationType is SiteRelationTypes.LivesThere);
                otherSite.OtherSiteId = id;
            }
            else
                AddRelatedSite(id, SiteRelationTypes.LivesThere);
        }

        public bool CheckForLoneniss()
        {
            return GetPersonality().Friendship <= -10
                || GetPersonality().Tradition <= 0
                || GetPersonality().Familiy <= -10;
        }

        public HistoricalFigure GetChildrenIfAny()
        {
            var family = FamilyLink.GetOtherFamilyNodesByRelations(this, HfRelationType.OwnChild);
            if (family.Length <= 0)
                return null;
            var child = family.GetRandomItemFromList();
            return child.Figure;
        }

        public bool HasPotentialToBeAWizard()
        {
            return Soul.MaxMana > GetRace().GetAverageRacialManaRange();
        }

        internal bool CheckIfIsChildOrBabyForRace()
        {
            var group = GetRace().GetAgeGroup(GetAnatomy().CurrentAge, GetAnatomy().Ages);
            if (group is AgeGroup.Baby || group is AgeGroup.Child)
                return true;
            return false;
        }

        public Anatomy GetAnatomy()
        {
            return Body.Anatomy;
        }

        public void SetBasicAnatomy()
        {
            SetStandardRaceFlags();
            GetAnatomy().BasicSetup();
        }

        public bool CheckForWanderlust()
        {
            return GetPersonality().Nature >= 15 || (GetPersonality().Independence >= 20
                && GetPersonality().Tradition <= 0);
        }

        public static void RemovePreviousCivRelationAndSetNew(CivRelation? prevRelation, RelationType newRelation)
        {
            if (prevRelation is not null)
                prevRelation.Relation = newRelation;
        }

        public void ChangeStayingSite(Point pos)
        {
            CurrentPos = pos;
        }

        public int? GetCurrentStayingSiteId(List<Site> sites)
        {
            return sites.FirstOrDefault(i => i.WorldPos == CurrentPos)?.Id;
        }

        public bool IsAdult()
        {
            return Body.GetCurrentAge() >= Body.Anatomy.GetRaceAdulthoodAge();
        }

        public void GenerateRandomStatsSpread()
        {
            SetupBodySoulAndMind();
        }

        private void SetupBodySoulAndMind()
        {
            Race race = GetRace();
            race.SetBodyPlan();
            SetBasicAnatomy();

            Body.Endurance = race.BaseEndurance;
            Body.Strength = race.BaseStrenght;
            Body.Toughness = race.BaseToughness;
            //Body.GeneralSpeed = race.GeneralSpeed;
            //Body.ViewRadius = race.RaceViewRadius;
            //Body.InitialStamina();

            Mind.Inteligence = race.BaseInt;
            Mind.Precision = race.BasePrecision;

            Soul.WillPower = race.BaseWillPower;
            Soul.BaseManaRegen = race.BaseManaRegen;
            Soul.InitialMana(Mind.Inteligence, race);

            //magic.InnateMagicResistance = race.BaseMagicResistance;
        }

        public void AddDiscovery(Discovery disc)
        {
            if (!DiscoveriesKnow.Any(i => i.WhatWasResearched.Id.Equals(disc.WhatWasResearched.Id)))
            {
                if (ResearchTree.CurrentResearchFocus.Research.Id.Equals(disc.WhatWasResearched.Id))
                {
                    ForceCleanupResearch(); // clean up reserach, since someone else discovery it before it!
                }
                DiscoveriesKnow.Add(disc);
            }
        }
    }
}