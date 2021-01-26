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

        public int EnergyPool { get; set; } = 0;
        public int Turns { get; set; } = 0;
        public int Tick { get; set; } = 0;

        // Add a priority queue to represent the queue that an actor will act, or a linked dictionary, or whatever

        public void ProcessTick()
        {
        }

        private void ProcessTurn()
        {
            Tick = 0;
            Turns++;
        }
    }
}