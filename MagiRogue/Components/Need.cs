using GoRogue.Components.ParentAware;
using MagiRogue.Data.Enumerators;
using MagiRogue.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.Components
{
    public struct Need
    {
        public string Name { get; set; }
        public bool Vital { get; set; }

        // Should be roughly
        // 10 = pretty much everyday to maintain 3~4 times per day.
        // 9 = at the very least 3 times per day.
        // 8 = every day at least once.
        // 7 to less, need to do only some times
        // in special, 0 means special circustances, like a fight or demon pacts or whatever, things that need imediate action rather than a constant need.
        // means that creatures with the fight need set to 0 will only fight in response to fighting
        public int Priority { get; set; } // what is the priority of the need? how many times must it be fulfilled?
        public Actions ActionToFulfillNeed { get; set; }
        public string PersonalityTrait { get; set; }
        public string HintFulfill { get; set; }

        public Need(string name,
            bool vital,
            int priority,
            Actions actionsToFulfillNeed,
            string personalityTrait,
            string hintFulfill)
        {
            Name = name;
            Vital = vital;
            Priority = priority;
            ActionToFulfillNeed = actionsToFulfillNeed;
            PersonalityTrait = personalityTrait;
            HintFulfill = hintFulfill;
        }

        public static List<Need> CommonNeeds() => new()
        {
            new Need("Eat", true, 9, Actions.Eat, "SelfControl", "food" ),
            new Need("Drink", true, 10, Actions.Drink, "Temperance", "drink" ),
            new Need("Sleep", true, 8, Actions.Sleep, "Lazyness", "rest" ),
            new Need("Fight", false, 0, Actions.Fight, "Peace", "battle" ),
        };

        public override string ToString()
        {
            return $"Need: {Name} Vital: {Vital} Priority: {Priority} Action: {ActionToFulfillNeed} Persona: {PersonalityTrait} Hint: {HintFulfill}";
        }
    }

    public class NeedCollection : IEnumerable
    {
        public List<Need> Needs { get; }

        public NeedCollection(List<Need> needs)
        {
            Needs = needs;
        }

        public static NeedCollection WithCommonNeeds() => new NeedCollection(Need.CommonNeeds());

        public IEnumerator GetEnumerator()
        {
            foreach (var item in Needs)
            {
                yield return item;
            }
        }
    }
}
