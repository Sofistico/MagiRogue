using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.Entities
{
    public class Body
    {
        /// <summary>
        /// The anatomy of the actor
        /// </summary>
        public Anatomy Anatomy { get; set; }
        public int StrengthScore { get; internal set; }

        /// <summary>
        /// The equipment that the actor is curently using
        /// </summary>
        public Dictionary<Limb, Item> Equipment { get; set; }
        public int ViewRadius { get; internal set; }

        public Body()
        {
            Anatomy = new();
        }

        public Body(Actor actor)
        {
            Equipment = new Dictionary<Limb, Item>();
            Anatomy = new(actor);
        }
    }
}