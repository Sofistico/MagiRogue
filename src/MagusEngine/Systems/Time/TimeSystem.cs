using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using GoRogue.Messaging;
using MagusEngine.Bus.MapBus;
using MagusEngine.Core.Entities.Base;
using MagusEngine.Serialization;
using MagusEngine.Systems.Time.Nodes;
using Priority_Queue;

namespace MagusEngine.Systems.Time
{
    /// <summary>
    /// Time system using nodes and a priority queue
    /// </summary>
    [JsonConverter(typeof(TimeJsonConverter))]
    public class TimeSystem : ITimeSystem, ISubscriber<AddTurnNode>
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

        public void RegisterNode(ITimeNode node)
        {
            if (!Nodes.Contains(node))
            {
                Nodes.Enqueue(node, node.Tick);
            }
        }

        public void RemoveNode(ITimeNode node)
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

        public void Handle(AddTurnNode message)
        {
            switch (message.Node)
            {
                case EntityTimeNode:
                    AddEntityToTime(message.Node.Id, message.Node.Tick);
                    break;

                case TickActionNode component:
                    AddComponentToTime(component, component.Tick);
                    break;
            }
        }

        private void AddComponentToTime(TickActionNode componentTurn, long ticks)
        {
            RegisterNode(new TickActionNode(GetTimePassed(ticks), componentTurn.Id, componentTurn.Action));
        }

        private void AddEntityToTime(uint entityId, long time = 0)
        {
            // register to next turn
            if (!Nodes.Any(i => i.Id.Equals(entityId)))
                RegisterNode(new EntityTimeNode(entityId, GetTimePassed(time)));
        }
    }
}
