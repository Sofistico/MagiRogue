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

            GameLoop.UIManager.MessageLog.Add($"The {entity.Name} took {dmg} {dmgType}");
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
                    GameLoop.UIManager.MessageLog.Add(bobTheBuilder.ToString());
                    break;

                case DamageType.Physical:
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

                case DamageType.Electricity:
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
        Physical = 1,
        Force = 2,
        Fire = 3,
        Cold = 4,
        Poison = 5,
        Acid = 6,
        Electricity = 7,
        Soul = 8,
        Mind = 9
    }
}