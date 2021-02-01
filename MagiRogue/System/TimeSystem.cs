using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MagiRogue.Entities;
using Priority_Queue;

namespace MagiRogue.System
{
    public class TimeSystem : ITimeSystem
    {
        public event EventHandler TurnPassed;

        /// <summary>
        /// Every 100 energy represents 1 second
        /// </summary>
        public const int EnergyToTurn = 100;

        public int Turns { get; set; } = 0;

        // Add a priority queue to represent the queue that an actor will act, or a linked dictionary, or whatever
        private SimplePriorityQueue<Entity, int> turnQueue = new SimplePriorityQueue<Entity, int>();

        public void RegisterEntity(Entity entity)
        {
            if (!turnQueue.Contains(entity))
            {
                turnQueue.EnqueueWithoutDuplicates(entity, entity.EnergyGain);
            }
        }

        public void DeRegisterEntity(Entity entity)
        {
            if (turnQueue.Contains(entity))
            {
                turnQueue.Remove(entity);
            }
        }

        public void ProgressTime()
        {
            if (turnQueue.Count == 0)
                return;
        }
    }
}