using MagiRogue.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.Entities
{
    /// <summary>
    /// Stats of the body... how strong, your equip, and etc...
    /// </summary>
    public class Body
    {
        private double stamina;

        /// <summary>
        /// The anatomy of the actor
        /// </summary>
        public Anatomy Anatomy { get; set; }

        /// <summary>
        /// The equipment that the actor is curently using
        /// </summary>
        public Dictionary<Limb, Item> Equipment { get; set; }
        public int ViewRadius { get; set; }
        public bool IsDed { get; set; }

        public double Stamina
        {
            get => stamina < MaxStamina ? stamina : MaxStamina;

            set
            {
                if (value < MaxStamina)
                    stamina = value;
                else
                {
                    stamina = MaxStamina;
                }
            }
        }
        public double MaxStamina { get; set; }
        public double StaminaRegen { get; set; }
        public double GeneralSpeed { get; set; }
        public int Toughness { get; set; }
        public int Strength { get; set; }

        public Body()
        {
            Anatomy = new();
        }

        public Body(Actor actor)
        {
            Equipment = new Dictionary<Limb, Item>();
            Anatomy = new(actor);
        }

        public void ApplyStaminaRegen(double staminaRegen)
        {
            if (Stamina < MaxStamina)
            {
                double newSta = staminaRegen + Stamina;
                Stamina = MathMagi.Round(newSta);
            }
        }
    }
}