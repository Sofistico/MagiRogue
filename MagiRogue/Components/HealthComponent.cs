using GoRogue.GameFramework;
using GoRogue.GameFramework.Components;
using MagiRogue.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.Components
{
    public class HealthComponent : IHealthComponent, IGameObjectComponent
    {
        private float currentHealth;

        public float CurrentHealth
        {
            get { return currentHealth; }
            private set
            {
                if (value == currentHealth)
                {
                    return;
                }

                float prevHealth = currentHealth;
                currentHealth = value;
                HealthChanged?.Invoke(this, prevHealth);
            }
        }

        public event EventHandler<float> HealthChanged;

        public float MaxHealth { get; }

        public bool Dead => currentHealth == 0;

        public float BaseHpRegen { get; }

        public IGameObject Parent { get; set; }

        public HealthComponent(Actor actor)
        {
            CurrentHealth = actor.Health;
            MaxHealth = actor.MaxHealth;
            BaseHpRegen = actor.BaseHpRegen;
        }

        public HealthComponent(float health, float maxHealth, float baseHpRegen)
        {
            currentHealth = health;
            MaxHealth = maxHealth;
            BaseHpRegen = baseHpRegen;
        }

        public void ApplyBaseHpRegen()
        {
            ApplyHealing(BaseHpRegen);
        }

        public void ApplyHealing(float healing)
        {
            CurrentHealth = Math.Min(MaxHealth, CurrentHealth + healing);
        }

        public void DealDamage(float damage, string message)
        {
        }
    }
}