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

        public static void DealDamage(double dmg, Entity entity, DamageType dmgType,
            Limb? limbAttacking = null, Limb? limbAttacked = null)
        {
            if (entity is Actor actor)
            {
                if (!actor.CanBeAttacked)
                {
                    GameLoop.AddMessageLog("Can't hit your target");
                    return;
                }

                // the stamina takes the hit first
                // decide to take only half of the damage, since the stamina right now protects everything!
                double defenseFromStamina = MathMagi.Round((actor.Body.Stamina * actor.GetAnatomy().FitLevel) * 0.5);
                dmg = MathMagi.Round(dmg - defenseFromStamina);
                //actor.Body.Stamina = actor.Body.Stamina < 0 ? 0 : actor.Body.Stamina;

                // any remaining dmg goes to create a wound
                if (dmg > 0)
                {
                    Wound woundTaken = new Wound(dmg, dmgType);

                    actor.GetAnatomy().Injury(woundTaken, limbAttacked, actor);

                    if (actor.CheckIfDed())
                    {
                        Commands.ActionManager.ResolveDeath(actor);
                    }

                    GameLoop.AddMessageLog($"   The {entity.Name} took {dmg} {dmgType} total damage in the {limbAttacking.BodyPartName}!");
                    StringBuilder woundString = new("   Receiving ");
                    switch (woundTaken.Severity)
                    {
                        case InjurySeverity.Inhibited:
                            woundString.Append($"an ");
                            break;

                        default:
                            woundString.Append($"a ");
                            break;
                    }
                    woundString.Append($"{woundTaken.Severity} wound!");

                    GameLoop.AddMessageLog(woundString.ToString());
                    return;
                }
                else
                {
                    GameLoop.AddMessageLog($"   The {entity.Name} took {defenseFromStamina} {dmgType} total damage to it's stamina!");
                    return;
                }
            }

            if (entity is Item item)
            {
                item.Condition -= (int)dmg;
            }
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
                        .Append(", feeling your bones and skin growing over your wounds!").ToString());
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
                int diceRoll = Mrn.Exploding2D6Dice + caster.GetPrecision();
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
        public static (bool, Limb?, Limb, DamageType) ResolveHit(Actor attacker,
            Actor defender, StringBuilder attackMessage)
        {
            // Create a string that expresses the attacker and defender's names
            attackMessage.AppendFormat("{0} attacks {1}", attacker.Name, defender.Name);
            Item wieldedItem = attacker.WieldedItem();
            Limb limbAttacking = attacker.GetAttackingLimb(wieldedItem);

            if (attacker.GetRelevantAttackAbility() + Mrn.Exploding2D6Dice
                > defender.GetDefenseAbility() + Mrn.Exploding2D6Dice)
            {
                var attackType = attacker.GetDamageType();
                Limb limbAttacked = defender.GetAnatomy().GetRandomLimb();
                return (true, limbAttacked, limbAttacking, attackType);
            }
            else
            {
                return (false, null, limbAttacking, attacker.GetDamageType());
            }
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
        public static double ResolveDefense(Actor attacker, Actor defender, bool hit, StringBuilder attackMessage,
            StringBuilder defenseMessage, Limb limbToHit, DamageType damageType, Limb limbAttacking)
        {
            double totalDamage = 0;

            if (hit)
            {
                // Create a string that displays the defender's name and outcomes

                double loopDamage;
                Item wieldedItem = attacker.WieldedItem();
                // TODO: adds a way to get the attack of the weapon or fist or something else
                if (wieldedItem is null)
                    loopDamage = GetAttackMomentum(attacker, limbAttacking);
                else
                    loopDamage = GetAttackMomentumWithItem(attacker, wieldedItem);
                // some moar randomness!
                loopDamage += Mrn.Exploding2D6Dice;

                double protection = defender.GetProtection(limbToHit);

                switch (damageType)
                {
                    case DamageType.Blunt:
                        protection *= 0.8;
                        break;

                    case DamageType.Point:
                        protection *= 0.5;
                        break;

                    case DamageType.Soul:
                        protection = 0;
                        break;

                    case DamageType.Mind:
                        protection = 0;
                        break;

                    default:
                        break;
                }
                var damage = loopDamage - (protection + Mrn.Exploding2D6Dice) + (defender.Body.Endurance * 0.5);
                loopDamage = MathMagi.Round(damage);

                //defenseMessage.AppendFormat("   {0} was hit for {1} damage", defender.Name, loopDamage);
                totalDamage += loopDamage;
            }
            //else
            //    attackMessage.Append(" and misses completely!");

            return totalDamage;
        }

        /// <summary>
        /// Gets the momentum of the attack, based partially on the Dwarf Fortress math on attack momentum calculation
        /// <para><seealso href="https://dwarffortresswiki.org/index.php/DF2014:Weapon">Link to DF wiki article</seealso></para>
        /// </summary>
        /// <param name="attacker"></param>
        /// <param name="wieldedItem"></param>
        /// <returns></returns>
        public static double GetAttackMomentumWithItem(Actor attacker, Item wieldedItem)
        {
            return MathMagi.Round(
                (attacker.GetStrenght() + wieldedItem.BaseDmg + Mrn.Exploding2D6Dice)
                * attacker.GetRelevantAttackAbilityMultiplier(wieldedItem.WeaponType)
                + (10 + 2 * wieldedItem.QualityMultiplier())) * attacker.GetAttackVelocity()
                + (1 + attacker.Volume / (wieldedItem.Material.Density * wieldedItem.Volume));
        }

        /// <summary>
        /// Gets the momentum of the attack, based partially on the Dwarf Fortress math on attack momentum calculation
        /// <para><seealso href="https://dwarffortresswiki.org/index.php/DF2014:Weapon">Link to DF wiki article</seealso></para>
        /// </summary>
        /// <param name="attacker"></param>
        /// <param name="wieldedItem"></param>
        /// <returns></returns>
        public static double GetAttackMomentum(Actor attacker, Limb limbAttacking)
        {
            return MathMagi.Round(
                (attacker.GetStrenght() + Mrn.Exploding2D6Dice)
                * (attacker.GetRelevantAbilityMultiplier(AbilityName.Unarmed) + 1)
                * attacker.GetAttackVelocity()
                + (1 + attacker.Volume / (limbAttacking.BodyPartMaterial.Density * limbAttacking.Volume)));
        }

        /// <summary>
        /// Calculates the damage a defender takes after a successful hit
        /// and subtracts it from its Health
        /// Then displays the outcome in the MessageLog.
        /// </summary>
        /// <param name="defender"></param>
        /// <param name="damage"></param>
        public static void ResolveDamage(Actor defender, double damage, DamageType dmgType,
            Limb limbAttacked, Limb limbAttacking)
        {
            if (damage > 0)
            {
                DealDamage(damage, defender, dmgType, limbAttacking, limbAttacked);
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
            deathMessage.AppendFormat("{0} died", defender.Name);
            // dump the dead actor's inventory (if any)
            // at the map position where it died
            if (defender.Inventory.Count > 0)
            {
                foreach (Item item in defender.Inventory)
                {
                    // move the Item to the place where the actor died
                    item.Position = defender.Position;

                    // Now let the MultiSpatialMap know that the Item is visible
                    GameLoop.GetCurrentMap().Add(item);
                }

                // Clear the actor's inventory. Not strictly
                // necessary, but makes for good coding habits!
                defender.Inventory.Clear();
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