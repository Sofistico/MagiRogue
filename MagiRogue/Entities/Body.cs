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
    public class Body : IStat
    {
        [JsonProperty(nameof(Stamina))]
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
                {
                    stamina = value;
                }
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
        public int Toughness { get => Stats["Toughness"]; set => Stats["Toughness"] = value; }
        public int Endurance { get => Stats["Endurance"]; set => Stats["Endurance"] = value; }
        public int Strength { get => Stats["Strength"]; set => Stats["Strength"] = value; }
        public List<string> MaterialsId { get; set; }
        public Dictionary<string, int> Stats { get; set; }

        public List<Scar> Scars { get; set; }

        public Body()
        {
            Equipment = new Dictionary<string, Item>();
            MaterialsId = new();
            Anatomy = new();
            Stats = new Dictionary<string, int>() { { "Strength", 0 }, { "Toughness", 0 }, { "Endurance", 0 } };
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

        public int GetCurrentAge()
        {
            return Anatomy.CurrentAge;
        }
    }
}