using Arquimedes.Enumerators;
using GoRogue.GameFramework;
using MagusEngine.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MagusEngine.ECS.Components.ActorComponents
{
    public class Need
    {
        public string Name { get; set; }
        public bool Vital { get; set; }

        // Should be roughly, less means that it has a higher priority 10 = should be at least once
        // per week or stuff like that. 9 = at the very least 3 times per week. 8 = every day at
        // least once. 7 to less, progessive less time till the next in special, 0 means special
        // circustances, like a fight or demon pacts or whatever, things that need imediate action
        // rather than a constant need. means that creatures with the fight need set to 0 will only
        // fight in response to fighting
        public double Priority { get; set; } // what is the priority of the need? how many times must it be fulfilled?
        public double PerceivedPriority => PercentFulfilled.HasValue ? Math.Pow(1 - PercentFulfilled.Value, Priority + 1) : (int)Priority;
        public ActionsEnum ActionToFulfillNeed { get; set; }
        public string PersonalityTrait { get; set; }
        public string HintFulfill { get; set; }

        public int TurnCounter { get; set; }

        public int? MaxTurnCounter { get => Priority == 0 ? null : (int)(Priority * 100000); }

        /// <summary>
        /// Percent of how much it's fulfilled
        /// </summary>
        public double? PercentFulfilled { get => MaxTurnCounter.HasValue ? MathMagi.GetInversePercentageBasedOnMax(TurnCounter, MaxTurnCounter.Value) : null; }

        public IGameObject? Objective { get; set; }

        public Need(string name,
            bool vital,
            double priority,
            ActionsEnum actionsToFulfillNeed,
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
            new Need("Eat", true, 1, ActionsEnum.Eat, "SelfControl", "food" ),
            new Need("Drink", true, 0.75, ActionsEnum.Drink, "Temperance", "drink" ),
            new Need("Sleep", true, 2, ActionsEnum.Sleep, "Lazyness", "rest" ),
            new Need("Restlessness", false, 0.00007, ActionsEnum.Wander, "Patience", "wander"),
            //new Need("Fight", false, 0, Actions.Fight, "Peace", "battle" ),
        };

        public bool TickNeed()
        {
            return TurnCounter++ >= MaxTurnCounter;
        }

        public override string ToString()
        {
            return $"Need: {Name} Vital: {Vital} Priority: {Priority} Action: {ActionToFulfillNeed} Persona: {PersonalityTrait} Hint: {HintFulfill} Percent fulfill: {PercentFulfilled}";
        }

        public void Fulfill()
        {
            TurnCounter = 0;
        }

        public void Fulfill(int fulfillPower)
        {
            TurnCounter -= fulfillPower;
        }
    }

    public class NeedCollection : ICollection<Need>
    {
        private readonly List<Need> needs;
        private readonly Queue<Need> prioQueue;

        public int Count => needs.Count;

        public bool IsReadOnly => ((ICollection<Need>)needs).IsReadOnly;

        public Need this[int index]
        {
            get => needs[index];
            set => needs[index] = value;
        }

        public NeedCollection(List<Need> needs)
        {
            this.needs = needs;
            prioQueue = new();
        }

        public static NeedCollection WithCommonNeeds() => new(Need.CommonNeeds());

        public void Add(Need item)
        {
            needs.Add(item);
        }

        public void AddPriority(Need item)
        {
            prioQueue.Enqueue(item);
        }

        public bool GetPriority(out Need? item)
        {
            if (prioQueue.TryDequeue(out item))
                return true;
            var list = needs.FindAll(i => i.PercentFulfilled <= 25);
            item = list.MaxBy(i => i.PerceivedPriority);
            return item is not null;
        }

        public void Clear()
        {
            needs.Clear();
        }

        public bool Contains(Need item)
        {
            return needs.Contains(item);
        }

        public void CopyTo(Need[] array, int arrayIndex)
        {
            needs.CopyTo(array, arrayIndex);
        }

        public bool Remove(Need item)
        {
            return needs.Remove(item);
        }

        public IEnumerator<Need> GetEnumerator()
        {
            return ((IEnumerable<Need>)needs).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)needs).GetEnumerator();
        }
    }

    public class NeedFulfill
    {
        public double FulfillPercent { get; set; }
        public ActionsEnum AssocietedAction { get; set; }

        public NeedFulfill(double fulfill, ActionsEnum actions)
        {
            FulfillPercent = fulfill;
            AssocietedAction = actions;
        }
    }
}
