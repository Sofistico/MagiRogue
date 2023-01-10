using MagiRogue.Data.Serialization;
using Priority_Queue;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MagiRogue.GameSys.Time
{
    /// <summary>
    /// Time system using nodes and a priority queue
    /// </summary>
    [JsonConverter(typeof(TimeJsonConverter))]
    public class TimeSystem : ITimeSystem
    {
        // Time system inspired by https://www.gridsagegames.com/blog/2019/04/turn-time-systems/ and https://github.com/AnotherEpigone/moving-castles/tree/master/MovingCastles/GameSystems/Time
        // Add a priority queue to represent the queue that an actor will act, or a linked dictionary, or whatever
        private readonly SimplePriorityQueue<ITimeNode, long> turnQueue = new SimplePriorityQueue<ITimeNode, long>();
        private readonly TimeDefSpan timeSpan;

        public event EventHandler<TimeDefSpan> TurnPassed;

        [JsonIgnore]
        public TimeDefSpan TimePassed => new TimeDefSpan(timeSpan.Ticks);

        [JsonIgnore]
        public int Turns => (int)timeSpan.Seconds;

        public TimeSystem(long initialTick = 0)
        {
            timeSpan = new TimeDefSpan(initialTick);
            turnQueue = new SimplePriorityQueue<ITimeNode, long>();
        }

        public TimeSystem(int initialYear)
        {
            timeSpan = new TimeDefSpan(initialYear);
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

        public ITimeNode NextNode()
        {
            if (turnQueue.Count == 0)
                return null;

            var node = turnQueue.Dequeue();
            timeSpan.SetTick(node.Tick);

            TurnPassed?.Invoke(this, TimePassed);

            return node;
        }

        public IEnumerable<ITimeNode> Nodes => turnQueue;

        public long GetTimePassed(long time) => TimePassed.Ticks + time;
    }
}