using MagiRogue.Data.Enumerators;
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
            get => stamina;

            set
            {
                if (value < MaxStamina)
                    stamina = value;
                else
                {
                    value = MaxStamina;
                    stamina = value;
                }
            }
        }
        public double MaxStamina { get; set; }
        public double StaminaRegen { get; set; } = 10;
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
            Equipment.TryGetValue(limb.Id, out Item item);
            if (item is null)
                return null;
            return item.EquipType is not EquipType.None ? item : null;
        }

        public void InitialStamina()
        {
            MaxStamina = Endurance * 100;
            Stamina = MaxStamina;
        }
    }
}