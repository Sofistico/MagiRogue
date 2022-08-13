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
        public int Endurance { get; set; }
        public int Strength { get; set; }

        public Body()
        {
            Equipment = new Dictionary<Limb, Item>();
            Anatomy = new();
        }

        public void ApplyStaminaRegen(double staminaRegen)
        {
            if (Stamina < MaxStamina)
            {
                double newSta = staminaRegen + Stamina;
                Stamina = MathMagi.Round(newSta);
            }
        }

        public Item GetArmorOnLimbIfAny(Limb limb)
        {
            return Equipment[limb].EquipType is not Data.Enumerators.EquipType.None ? Equipment[limb] : null;
        }

        public void InitialStamina()
        {
            MaxStamina = Endurance * 1000;
            Stamina = MaxStamina;
        }
    }
}