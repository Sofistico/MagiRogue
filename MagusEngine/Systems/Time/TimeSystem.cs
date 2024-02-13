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
        public event EventHandler<TimeDefSpan>? TurnPassed;

        public TimeDefSpan TimePassed { get; }
        public int Turns => (int)TimePassed.Seconds;
        public long Tick => TimePassed.Ticks;
        public SimplePriorityQueue<ITimeNode, long> Nodes { get; } = new();

        public TimeSystem(long initialTick = 0)
        {
            TimePassed = new TimeDefSpan(initialTick);
            Nodes = new SimplePriorityQueue<ITimeNode, long>();
        }

        public TimeSystem(int initialYear)
        {
            TimePassed = new TimeDefSpan(initialYear);
            Nodes = new SimplePriorityQueue<ITimeNode, long>();
        }

        public void RegisterEntity(ITimeNode node)
        {
            if (!Nodes.Contains(node))
            {
                Nodes.Enqueue(node, node.Tick);
            }
        }

        public void DeRegisterEntity(ITimeNode node)
        {
            if (Nodes.Contains(node))
            {
                Nodes.Remove(node);
            }
        }

        public ITimeNode? NextNode()
        {
            if (Nodes.Count == 0)
                return null;

            var node = Nodes.Dequeue();
            TimePassed.SetTick(node.Tick);

            TurnPassed?.Invoke(this, TimePassed);

            return node;
        }

        public long GetTimePassed(long time) => TimePassed.Ticks + time;

        public int GetNHoursFromTurn(int hours)
        {
            return Turns * TimeDefSpan.SecondsPerHour * hours;
        }
    }
}
