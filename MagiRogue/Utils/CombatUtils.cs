using MagiRogue.Entities;
using System;
using System.Text;

namespace MagiRogue.Utils
{
    public static class CombatUtils
    {
        public static DamageType SetFlag(DamageType a, DamageType b)
        {
            return a | b;
        }

        public static DamageType UnsetFlag(DamageType a, DamageType b)
        {
            return a & (~b);
        }

        // Works with "None" as well
        public static bool HasFlag(DamageType a, DamageType b)
        {
            return (a & b) == b;
        }

        public static DamageType ToogleFlag(DamageType a, DamageType b)
        {
            return a ^ b;
        }

        public static void DealDamage(int dmg, Entity entity, DamageType dmgType)
        {
            if (entity is Actor actor)
            {
                if (!actor.CanBeAttacked)
                {
                    GameLoop.UIManager.MessageLog.Add("Can't hit your target");
                    return;
                }

                actor.Stats.Health -= dmg;

                if (actor.Stats.Health < 0)
                {
                    Commands.CommandManager.ResolveDeath(actor);
                }
            }

            if (entity is Item item)
            {
                item.Condition -= dmg;
            }

            GameLoop.UIManager.MessageLog.Add($"The {entity.Name} took {dmg} {dmgType} damage!");
        }

        public static void ApplyHealing(int dmg, Stat stats, DamageType healingType)
        {
            stats.Health += dmg;

            if (stats.Health >= stats.MaxHealth)
            {
                stats.Health = stats.MaxHealth;

                GameLoop.UIManager.MessageLog.Add("You don't need healing");
                return;
            }

            StringBuilder bobTheBuilder = new StringBuilder($"You healed for {dmg} damage");
            switch (healingType)
            {
                case DamageType.None:
                    GameLoop.UIManager.MessageLog.Add(bobTheBuilder
                        .Append(", feeling your bones and flesh growing over your wounds!").ToString());
                    break;

                case DamageType.Force:
                    GameLoop.UIManager.MessageLog.Add(bobTheBuilder
                        .Append(", filling your movements with a spring!").ToString());
                    break;

                case DamageType.Fire:
                    GameLoop.UIManager.MessageLog.Add(bobTheBuilder
                        .Append(", firing your will!").ToString());
                    break;

                case DamageType.Cold:

                    GameLoop.UIManager.MessageLog.Add(bobTheBuilder
                        .Append(", leaving you lethargic.").ToString());
                    break;

                case DamageType.Poison:
                    GameLoop.UIManager.MessageLog.Add(bobTheBuilder
                        .Append(", ouch it hurt!").ToString());
                    break;

                case DamageType.Acid:
                    stats.Health -= dmg;
                    GameLoop.UIManager.MessageLog.Add(bobTheBuilder
                        .Append(", dealing equal damage to yourself, shouldn't have done that.").ToString());
                    break;

                case DamageType.Shock:
                    GameLoop.UIManager.MessageLog.Add(bobTheBuilder
                        .Append(", felling yourself speeding up!").ToString());
                    break;

                case DamageType.Soul:
                    GameLoop.UIManager.MessageLog.Add(bobTheBuilder
                        .Append(", feeling your soul at rest.").ToString());
                    break;

                case DamageType.Mind:
                    GameLoop.UIManager.MessageLog.Add(bobTheBuilder
                        .Append(", feeling your mind at ease.").ToString());
                    break;

                default:
                    break;
            }
        }
    }

    [Flags]
    public enum DamageType
    {
        None = 0,
        Blunt = 1,
        Sharp = 2,
        Point = 3,
        Force = 4,
        Fire = 5,
        Cold = 6,
        Poison = 7,
        Acid = 8,
        Shock = 9,
        Soul = 10,
        Mind = 11
    }
}