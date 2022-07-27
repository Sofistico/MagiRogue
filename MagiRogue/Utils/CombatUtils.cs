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
                    GameLoop.AddMessageLog("Can't hit your target");
                    return;
                }

                actor.Body.Stamina -= dmg;

                if (actor.CheckIfDed())
                {
                    Commands.CommandManager.ResolveDeath(actor);
                }
            }

            if (entity is Item item)
            {
                item.Condition -= dmg;
            }

            GameLoop.AddMessageLog($"The {entity.Name} took {dmg} {dmgType} total damage!");
        }

        public static void ApplyHealing(int dmg, Actor stats, DamageType healingType)
        {
            // Recovr stamina first
            stats.Body.Stamina += dmg;

            if (stats.Body.Stamina >= stats.Body.MaxStamina)
            {
                stats.Body.Stamina = stats.Body.MaxStamina;

                GameLoop.AddMessageLog("You feel your inner fire full");
            }

            // then here heal the limbs
            // TODO: Add the function to do it

            StringBuilder bobTheBuilder = new StringBuilder($"You healed for {dmg} damage");
            switch (healingType)
            {
                case DamageType.None:
                    GameLoop.AddMessageLog(bobTheBuilder
                        .Append(", feeling your bones and flesh growing over your wounds!").ToString());
                    break;

                case DamageType.Force:
                    GameLoop.AddMessageLog(bobTheBuilder
                        .Append(", filling your movements with a spring!").ToString());
                    break;

                case DamageType.Fire:
                    GameLoop.AddMessageLog(bobTheBuilder
                        .Append(", firing your will!").ToString());
                    break;

                case DamageType.Cold:

                    GameLoop.AddMessageLog(bobTheBuilder
                        .Append(", leaving you lethargic.").ToString());
                    break;

                case DamageType.Poison:
                    GameLoop.AddMessageLog(bobTheBuilder
                        .Append(", ouch it hurt!").ToString());
                    break;

                case DamageType.Acid:
                    stats.Body.Stamina -= dmg;
                    GameLoop.AddMessageLog(bobTheBuilder
                        .Append(", dealing equal damage to yourself, shouldn't have done that.").ToString());
                    break;

                case DamageType.Shock:
                    GameLoop.AddMessageLog(bobTheBuilder
                        .Append(", felling yourself speeding up!").ToString());
                    break;

                case DamageType.Soul:
                    GameLoop.AddMessageLog(bobTheBuilder
                        .Append(", feeling your soul at rest.").ToString());
                    break;

                case DamageType.Mind:
                    GameLoop.AddMessageLog(bobTheBuilder
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
                GameLoop.AddMessageLog($"{poorGuy.Name} resisted the effects of {spellCasted.SpellName}");
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
                    GameLoop.AddMessageLog($"{caster.Name} missed {poorGuy.Name}!");
                }
            }
        }

        /// <summary>
        /// Calculates the outcome of an attacker's attempt
        /// at scoring a hit on a defender, using the attacker's
        /// AttackChance and a random d100 roll as the basis.
        /// Modifies a StringBuilder message that will be displayed
        /// in the MessageLog
        /// </summary>
        /// <param name="attacker"></param>
        /// <param name="defender"></param>
        /// <param name="attackMessage"></param>
        /// <returns></returns>
        public static int ResolveHit(Actor attacker, Actor defender, StringBuilder attackMessage)
        {
            // Create a string that expresses the attacker and defender's names
            int hits = 0;
            attackMessage.AppendFormat("{0} attacks {1}", attacker.Name, defender.Name);

            // The attacker's AttackSpeed value determines the number of attacks dice rolled
            // which will always be at the very least one attack per turn, so yeah need to look later into this
            for (int nmrAttack = 0; nmrAttack < attacker.GetNumberOfAttacks(); nmrAttack++)
            {
                //Resolve the dicing outcome and register a hit, governed by the
                //attacker's BaseAttack value.
                //TODO: Adds the fatigue atribute and any penalties.
                if (attacker.GetRelevantAbility() + Mrn.Exploding2D6Dice > defender.GetDefenseAbility() + Mrn.Exploding2D6Dice)
                    hits++;
            }

            return hits;
        }

        /// <summary>
        /// Calculates the outcome of a defender's attempt
        /// at blocking incoming hits.
        /// Modifies a StringBuilder messages that will be displayed
        /// in the MessageLog, expressing the number of hits blocked.
        /// </summary>
        /// <param name="defender"></param>
        /// <param name="hits"></param>
        /// <param name="attackMessage"></param>
        /// <param name="defenseMessage"></param>
        /// <returns></returns>
        public static int ResolveDefense(Actor attacker, Actor defender, int hits, StringBuilder attackMessage,
            StringBuilder defenseMessage)
        {
            int totalDamage = 0;

            if (hits > 0)
            {
                // Create a string that displays the defender's name and outcomes
                attackMessage.AppendFormat(" scoring {0} hits.", hits);

                for (int i = 0; i < hits; i++)
                {
                    int loopDamage;
                    Item wieldedItem = attacker.WieldedItem();
                    // TODO: adds a way to get the attack of the weapon or fist or something else
                    if (wieldedItem is null)
                        loopDamage = attacker.GetStrenght() + Mrn.Exploding2D6Dice;
                    else
                        loopDamage = attacker.GetStrenght() + wieldedItem.BaseDmg + Mrn.Exploding2D6Dice;

                    loopDamage -= defender.GetProtection() + Mrn.Exploding2D6Dice;

                    defenseMessage.AppendFormat("   Hit {0}: {1} was hit for {2} damage", hits, defender.Name, loopDamage);
                    totalDamage += loopDamage;
                }
            }
            else
                attackMessage.Append(" and misses completely!");

            return totalDamage;
        }

        /// <summary>
        /// Calculates the damage a defender takes after a successful hit
        /// and subtracts it from its Health
        /// Then displays the outcome in the MessageLog.
        /// </summary>
        /// <param name="defender"></param>
        /// <param name="damage"></param>
        public static void ResolveDamage(Actor defender, int damage, DamageType dmgType)
        {
            if (damage > 0)
            {
                DealDamage(damage, defender, dmgType);
            }
            else
                GameLoop.AddMessageLog($"{defender.Name} blocked all damage!");
        }

        /// <summary>
        /// Removes an Actor that has died
        /// and displays a message showing
        /// the actor that has died, and the loot they dropped
        /// </summary>
        /// <param name="defender"></param>
        public static void ResolveDeath(Actor defender)
        {
            // if the defender can't be killed, do nothing.
            if (!defender.CanBeKilled)
                return;

            // Set up a customized death message
            StringBuilder deathMessage = new StringBuilder();

            // dump the dead actor's inventory (if any)
            // at the map position where it died
            if (defender.Inventory.Count > 0)
            {
                deathMessage.AppendFormat("{0} died and dropped", defender.Name);

                foreach (Item item in defender.Inventory)
                {
                    // move the Item to the place where the actor died
                    item.Position = defender.Position;

                    // Now let the MultiSpatialMap know that the Item is visible
                    GameLoop.GetCurrentMap().Add(item);

                    // Append the item to the deathMessage
                    deathMessage.Append(", " + item.Name);
                }

                // Clear the actor's inventory. Not strictly
                // necessary, but makes for good coding habits!
                defender.Inventory.Clear();
            }
            else
            {
                // The monster carries no loot, so don't show any loot dropped
                deathMessage.Append('.');
            }

            // actor goes bye-bye
            GameLoop.GetCurrentMap().Remove(defender);

            if (defender is Player)
            {
                GameLoop.AddMessageLog($" {defender.Name} was killed.");
            }

            // Now show the deathMessage in the messagelog
            GameLoop.AddMessageLog(deathMessage.ToString());
        }
    }
}