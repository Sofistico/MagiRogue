using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Priority_Queue;

namespace MagiRogue.System
{
    // A Roguelike turn system
    // http://blog.deadreckoned.com/post/91616626322/havenguard-turn-system
    // Requires a fast max-heap Priority Queue (I like this one: http://blog.mischel.com/2007/06/22/priority-queue-in-c/)
    // Also from https://gist.github.com/stevewoolcock/1ae490569ca5a143060c
    public interface ITimeEntity
    {
        /// <summary>
        /// The delegate to execute when the object's turn is executed.
        /// </summary>
        public Action TurnHandler { get; set; }

        // <summary>
        /// The entity's speed rating.
        /// </summary>
        public int Speed { get; set; }

        /// <summary>
        /// The entity's current energy rating.
        /// </summary>
        public int Energy { get; set; }
    }

    public class TimeSystem
    {
        /// <summary>
        /// The base speed unit.
        /// </summary>
        private const int baseSpeed = 10;

        /// <summary>
        /// The amount of energy required to execute an object's turn.
        /// </summary>
        public const int TurnEnergy = 100;

        private List<ITimeEntity> entities = new List<ITimeEntity>();
        private HashSet<ITimeEntity> register = new HashSet<ITimeEntity>();
        private SimplePriorityQueue<ITimeEntity, int> turnQueue = new SimplePriorityQueue<ITimeEntity, int>();

        /// <summary>
        /// Adds an entity to the system.
        /// </summary>
        /// <param name="entity">The TurnEntity to add</param>
        public void Register(ITimeEntity entity)
        {
            if (!register.Contains(entity))
            {
                entities.Add(entity);
                register.Add(entity);
            }
        }

        /// <summary>
        /// Removes an entity from the system.
        /// </summary>
        /// <param name="entity">The TurnEntity to remove</param>
        public void Deregister(ITimeEntity entity)
        {
            if (register.Remove(entity))
            {
                entities.Remove(entity);
            }
        }

        /// <summary>
        /// Applies energy recovery and executes turn handles for entities who's energy has reached the turn energy rating.
        /// </summary>
        public void NextTurn()
        {
            if (entities.Count == 0)
                return;

            // Reset the turn queue
            turnQueue.Clear();

            // Continually apply energy recovery to all entities in the system, until
            // the queue has at least one entity to execute
            while (turnQueue.Count == 0)
            {
                for (int i = 0; i < entities.Count; ++i)
                {
                    ITimeEntity entity = entities[i];
                    entity.Energy += CalculateEnergyRecovery(entity.Speed);

                    // Once the entity has enough energy, it can be added to the turn queue
                    // The queue will prioritize entities by their energy ratings, so the entities
                    // with the most amount of energy will execute their turn first
                    if (entity.Energy >= TurnEnergy)
                    {
                        turnQueue.Enqueue(entity, entity.Energy);
                    }
                }
            }

            // Dequeue and execute the handler for each entities in the turn queue until empty
            // Note that the handler is only executed if the entity is still registered in the system, since
            // an object may be deregistered by the time its turn handler is executed
            while (turnQueue.Count > 0)
            {
                ITimeEntity entity = turnQueue.Dequeue();
                if (entity.TurnHandler != null && register.Contains(entity))
                    entity.TurnHandler();
            }
        }

        /// <summary>
        /// Calculates the energy recovery rate for a given speed.
        /// </summary>
        /// <param name="speed">The speed</param>
        /// <returns>The energy recovery amount for the specified speed.</returns>
        private int CalculateEnergyRecovery(int speed)
        {
            // TODO: Implement the actual recovry table lookup
            return baseSpeed + speed;
        }
    }
}