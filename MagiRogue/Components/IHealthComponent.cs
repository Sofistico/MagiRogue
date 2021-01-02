using GoRogue.GameFramework.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.Components
{
    public interface IHealthComponent : IGameObjectComponent
    {
        event EventHandler<float> HealthChanged;

        float CurrentHealth { get; }
        float MaxHealth { get; }
        bool Dead { get; }
        float BaseHpRegen { get; }

        void DealDamage(float damage, string message);

        void ApplyHealing(float healing);

        void ApplyBaseHpRegen();
    }
}