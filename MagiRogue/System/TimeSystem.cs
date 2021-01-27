using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Priority_Queue;

namespace MagiRogue.System
{
    public class TimeSystem : ITime
    {
        /// <summary>
        /// Every 100 ticks represents 1 second
        /// </summary>
        public const int TickToTurn = 100;

        public int EnergyPoolActor { get; set; } = 0;
        public int Turns { get; set; } = 0;
        public int Tick { get; set; } = 0;

        // Add a priority queue to represent the queue that an actor will act, or a linked dictionary, or whatever
        private SimplePriorityQueue<Entities.Entity, int> turnQueue = new SimplePriorityQueue<Entities.Entity, int>();

        public void ProcessTick()
        {
            if (Tick.Equals(TickToTurn))
            {
                ProcessTurn();
            }
            else
            {
            }
        }

        private void ProcessTurn()
        {
            Tick = 0;
            Turns++;
        }
    }
}