using MagiRogue.Data.Serialization;
using MagiRogue.Utils;
using Newtonsoft.Json;
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
        [JsonProperty("Stamina")]
        private double stamina;

        /// <summary>
        /// The anatomy of the actor
        /// </summary>
        public Anatomy Anatomy { get; set; }

        /// <summary>
        /// The equipment that the actor is curently using
        /// </summary>
        public Dictionary<string, Item> Equipment { get; set; }
        public int ViewRadius { get; set; }
        public bool IsDed { get; set; }

        [JsonIgnore]
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
        public double StaminaRegen { get; set; } = 100;
        public double GeneralSpeed { get; set; }
        public int Toughness { get; set; }
        public int Endurance { get; set; }
        public int Strength { get; set; }
        public List<string> MaterialsId { get; set; }

        public Body()
        {
            Equipment = new Dictionary<string, Item>();
            MaterialsId = new();
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
            return Equipment[limb.Id].EquipType is not Data.Enumerators.EquipType.None ? Equipment[limb.Id] : null;
        }

        public void InitialStamina()
        {
            MaxStamina = Endurance * 1000;
            Stamina = MaxStamina;
        }
    }
}