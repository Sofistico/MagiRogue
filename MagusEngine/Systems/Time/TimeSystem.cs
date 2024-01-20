using MagusEngine.Serialization;
using Priority_Queue;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MagusEngine.Systems.Time
{
    /// <summary>
    /// Time system using nodes and a priority queue
    /// </summary>
    [JsonConverter(typeof(TimeJsonConverter))]
    public class TimeSystem : ITimeSystem
    {
        // Time system inspired by https://www.gridsagegames.com/blog/2019/04/turn-time-systems/ and
        // https://github.com/AnotherEpigone/moving-castles/tree/master/MovingCastles/GameSystems/Time
        // Add a priority queue to represent the queue that an actor will act, or a linked
        // dictionary, or whatever
        private readonly SimplePriorityQueue<ITimeNode, long> turnQueue = new();

        public event EventHandler<TimeDefSpan>? TurnPassed;

        public TimeDefSpan TimePassed { get; }
        public int Turns => (int)TimePassed.Seconds;
        public long Tick => TimePassed.Ticks;

        public TimeSystem(long initialTick = 0)
        {
            TimePassed = new TimeDefSpan(initialTick);
            turnQueue = new SimplePriorityQueue<ITimeNode, long>();
        }

        public TimeSystem(int initialYear)
        {
            TimePassed = new TimeDefSpan(initialYear);
            turnQueue = new SimplePriorityQueue<ITimeNode, long>();
        }

        public void RegisterEntity(ITimeNode node)
        {
            if (!turnQueue.Contains(node))
            {
                turnQueue.Enqueue(node, node.Tick);
            }
        }

        public void DeRegisterEntity(ITimeNode node)
        {
            if (turnQueue.Contains(node))
            {
                turnQueue.Remove(node);
            }
        }

        public ITimeNode? NextNode()
        {
            if (turnQueue.Count == 0)
                return null;

            var node = turnQueue.Dequeue();
            TimePassed.SetTick(node.Tick);

            TurnPassed?.Invoke(this, TimePassed);

            return node;
        }

        public IEnumerable<ITimeNode> Nodes => turnQueue;

        public long GetTimePassed(long time) => TimePassed.Ticks + time;

        public int GetNHoursFromTurn(int hours)
        {
            return Turns * TimeDefSpan.SecondsPerHour * hours;
        }
    }
}
