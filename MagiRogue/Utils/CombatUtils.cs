using MagiRogue.Data.Enumerators;
using MagiRogue.Entities;
using MagiRogue.GameSys.Magic;
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

                actor.GetAnatomy().Health -= dmg;

                if (actor.CheckIfDed())
                {
                    Commands.CommandManager.ResolveDeath(actor);
                }
            }

            if (entity is Item item)
            {
                item.Condition -= dmg;
            }

            GameLoop.UIManager.MessageLog.Add($"The {entity.Name} took {dmg} {dmgType} total damage!");
        }

        public static void ApplyHealing(int dmg, Actor stats, DamageType healingType)
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

        private static void ResolveResist(Entity poorGuy, Actor caster, SpellBase spellCasted, ISpellEffect effect)
        {
            int luck = Mrn.Exploding2D6Dice;
            if (MagicManager.PenetrateResistance(spellCasted, caster, poorGuy, luck))
            {
                DealDamage(effect.BaseDamage, poorGuy, effect.SpellDamageType);
            }
            else
                GameLoop.UIManager.MessageLog.Add($"{poorGuy.Name} resisted the effects of {spellCasted.SpellName}");
        }

        public static void ResolveSpellHit(Entity poorGuy, Actor caster, SpellBase spellCasted, IDamageSpellEffect effect)
        {
            if (!effect.CanMiss)
            {
                ResolveResist(poorGuy, caster, spellCasted, effect);
            }
            else
            {
                int diceRoll = Mrn.Exploding2D6Dice + caster.GetPrecisionAbility();
                // the actor + exploding dice is the dice that the target will throw for either defense or blocking the projectile
                // TODO: When shield is done, needs to add the shield or any protection against the spell
                if (poorGuy is Actor actor && diceRoll >= actor.GetDefenseAbility() + Mrn.Exploding2D6Dice)
                {
                    ResolveResist(poorGuy, caster, spellCasted, effect);
                }
                else
                {
                    GameLoop.UIManager.MessageLog.Add($"{caster.Name} missed {poorGuy.Name}!");
                }
            }
        }
    }
}