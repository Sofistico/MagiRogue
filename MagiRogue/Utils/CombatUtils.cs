using MagiRogue.Data.Enumerators;
using MagiRogue.Entities;
using MagiRogue.GameSys.Magic;
using System.Text;

namespace MagiRogue.Utils
{
    public static class CombatUtils
    {
        public static DamageTypes SetFlag(DamageTypes a, DamageTypes b)
        {
            return a | b;
        }

        public static DamageTypes UnsetFlag(DamageTypes a, DamageTypes b)
        {
            return a & (~b);
        }

        // Works with "None" as well
        public static bool HasFlag(DamageTypes a, DamageTypes b)
        {
            return (a & b) == b;
        }

        public static DamageTypes ToogleFlag(DamageTypes a, DamageTypes b)
        {
            return a ^ b;
        }

        public static void DealDamage(double dmg, MagiEntity entity, DamageTypes dmgType,
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
                // decide to take only half of the damage,
                // since the stamina right now protects everything!
                double defenseFromStamina = MathMagi.Round(actor.Body.Stamina * actor.GetAnatomy().FitLevel * 0.5);
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

                    //GameLoop.AddMessageLog($"   The {entity.Name} took {dmg} {dmgType} total damage in the {limbAttacking.BodyPartName}!");
                    StringBuilder woundString = new($"   The {entity.Name} received ");
                    switch (woundTaken.Severity)
                    {
                        case InjurySeverity.Inhibited:
                            woundString.Append("an ");
                            break;

                        default:
                            woundString.Append("a ");
                            break;
                    }
                    woundString.Append(woundTaken.Severity).Append(" wound in the ").Append(limbAttacking.BodyPartName);

                    GameLoop.AddMessageLog(woundString.ToString());
                    return;
                }
                else
                {
                    GameLoop.AddMessageLog($"   The attack glances {entity.Name}!");
                    return;
                }
            }

            if (entity is Item item)
            {
                item.Condition -= (int)dmg;
            }
        }

        public static void ApplyHealing(int dmg, Actor stats, DamageTypes healingType)
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
                case DamageTypes.None:
                    GameLoop.AddMessageLog(bobTheBuilder
                        .Append(", feeling your bones and skin growing over your wounds!").ToString());
                    break;

                case DamageTypes.Force:
                    GameLoop.AddMessageLog(bobTheBuilder
                        .Append(", filling your movements with a spring!").ToString());
                    break;

                case DamageTypes.Fire:
                    GameLoop.AddMessageLog(bobTheBuilder
                        .Append(", firing your will!").ToString());
                    break;

                case DamageTypes.Cold:

                    GameLoop.AddMessageLog(bobTheBuilder
                        .Append(", leaving you lethargic.").ToString());
                    break;

                case DamageTypes.Poison:
                    GameLoop.AddMessageLog(bobTheBuilder
                        .Append(", ouch it hurt!").ToString());
                    break;

                case DamageTypes.Acid:
                    stats.Body.Stamina -= dmg;
                    GameLoop.AddMessageLog(bobTheBuilder
                        .Append(", dealing equal damage to yourself, shouldn't have done that.").ToString());
                    break;

                case DamageTypes.Shock:
                    GameLoop.AddMessageLog(bobTheBuilder
                        .Append(", felling yourself speeding up!").ToString());
                    break;

                case DamageTypes.Soul:
                    GameLoop.AddMessageLog(bobTheBuilder
                        .Append(", feeling your soul at rest.").ToString());
                    break;

                case DamageTypes.Mind:
                    GameLoop.AddMessageLog(bobTheBuilder
                        .Append(", feeling your mind at ease.").ToString());
                    break;

                default:
                    break;
            }
        }

        private static void ResolveResist(MagiEntity poorGuy,
            Actor caster,
            SpellBase spellCasted,
            ISpellEffect effect)
        {
            int luck = Mrn.Exploding2D6Dice;
            if (MagicManager.PenetrateResistance(spellCasted, caster, poorGuy, luck))
            {
                DealDamage(effect.BaseDamage, poorGuy, effect.SpellDamageType);
            }
            else
            {
                GameLoop.AddMessageLog($"{poorGuy.Name} resisted the effects of {spellCasted.SpellName}");
            }
        }

        public static void ResolveSpellHit(MagiEntity poorGuy, Actor caster, SpellBase spellCasted,
            ISpellEffect effect)
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
        public static (bool, Limb?, Limb, DamageTypes, Item?) ResolveHit(
            Actor attacker,
            Actor defender,
            StringBuilder attackMessage,
            Attack attack,
            bool firstPerson = false,
            Limb? limbAttacked = null)
        {
            Item wieldedItem = attacker.WieldedItem();
            Limb limbAttacking = attacker.GetAttackingLimb(attack);

            // Create a string that expresses the attacker and defender's names as well as the attack verb
            if (!firstPerson)
            {
                if (attack.AttacksUsesLimbName == true)
                {
                    attackMessage.AppendFormat("{0} {1}", attacker.Name, attack.AttackVerb[1]);
                }
                else
                {
                    attackMessage.AppendFormat("{0} {1} {2}",
                        attacker.Name,
                        attacker.GetAnatomy().Pronoum(),
                        attack.AttackVerb[1]);
                }
            }
            else
            {
                if (attack.AttacksUsesLimbName == true)
                    attackMessage.AppendFormat("You {0} with your {1}", attack.AttackVerb[0], limbAttacking.BodyPartName);
                else
                    attackMessage.AppendFormat("You {0}", attack.AttackVerb[0]);
            }
            attackMessage.AppendFormat(" the {0}", defender.Name);

            if (attacker.GetRelevantAttackAbility(wieldedItem) + Mrn.Exploding2D6Dice
                > defender.GetDefenseAbility() + Mrn.Exploding2D6Dice)
            {
                var attackType = attacker.GetDamageType();
                limbAttacked ??= defender.GetAnatomy().GetRandomLimb();
                return (true, limbAttacked, limbAttacking, attackType, wieldedItem);
            }
            else
            {
                return (false, null, limbAttacking, attacker.GetDamageType(), wieldedItem);
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
        public static double ResolveDefense(Actor attacker,
            Actor defender,
            bool hit,
            //StringBuilder attackMessage,
            //StringBuilder defenseMessage,
            Limb limbToHit,
            DamageTypes damageType,
            Limb limbAttacking,
            Attack attack,
            Item? wieldedItem = null)
        {
            double totalDamage = 0;

            if (hit)
            {
                // Create a string that displays the defender's name and outcomes

                double loopDamage;
                // TODO: adds a way to get the attack of the weapon or fist or something else
                if (wieldedItem is null)
                    loopDamage = GetAttackMomentum(attacker, limbAttacking, attack);
                else
                    loopDamage = GetAttackMomentumWithItem(attacker, wieldedItem, attack);

                // some moar randomness!
                loopDamage += Mrn.Exploding2D6Dice;

                double protection = defender.GetProtection(limbToHit);

                switch (damageType)
                {
                    case DamageTypes.Blunt:
                        protection *= 0.8;
                        break;

                    case DamageTypes.Point:
                        protection *= 0.5;
                        break;

                    case DamageTypes.Soul:
                    case DamageTypes.Mind:
                        protection = 0;
                        break;
                }
                var damage =
                    loopDamage
                    - ((protection + Mrn.Exploding2D6Dice) * attack.PenetrationPercentage)
                    + (defender.Body.Endurance * 0.5);
                loopDamage = MathMagi.Round(damage);

                totalDamage += loopDamage;
            }

            return totalDamage;
        }

        /// <summary>
        /// Gets the momentum of the attack, based partially on the Dwarf Fortress math on attack momentum calculation
        /// <para><seealso href="https://dwarffortresswiki.org/index.php/DF2014:Weapon">Link to DF wiki article</seealso></para>
        /// </summary>
        /// <param name="attacker"></param>
        /// <param name="wieldedItem"></param>
        /// <returns></returns>
        public static double GetAttackMomentumWithItem(Actor attacker, Item wieldedItem, Attack attack)
        {
            return (MathMagi.Round(
                ((attacker.GetStrenght() + wieldedItem.BaseDmg + Mrn.Exploding2D6Dice)
                * attacker.GetRelevantAttackAbilityMultiplier(wieldedItem.WeaponType))
                + (10 + (2 * wieldedItem.QualityMultiplier()))) * attacker.GetAttackVelocity(attack))
                + (1 + (attacker.Volume / (wieldedItem.Material.Density * wieldedItem.Volume)));
        }

        /// <summary>
        /// Gets the momentum of the attack, based partially on the Dwarf Fortress math on attack momentum calculation
        /// <para><seealso href="https://dwarffortresswiki.org/index.php/DF2014:Weapon">Link to DF wiki article</seealso></para>
        /// </summary>
        /// <param name="attacker"></param>
        /// <param name="wieldedItem"></param>
        /// <returns></returns>
        public static double GetAttackMomentum(Actor attacker, Limb limbAttacking, Attack attack)
        {
            return MathMagi.Round(
                ((attacker.GetStrenght() + Mrn.Exploding2D6Dice)
                * (attacker.GetRelevantAbilityMultiplier(AbilityName.Unarmed) + 1)
                * attacker.GetAttackVelocity(attack))
                + (1 + (attacker.Volume / (limbAttacking.BodyPartMaterial.Density * limbAttacking.Volume))));
        }

        /// <summary>
        /// Calculates the damage a defender takes after a successful hit
        /// and subtracts it from its Health
        /// Then displays the outcome in the MessageLog.
        /// </summary>
        /// <param name="defender"></param>
        /// <param name="damage"></param>
        public static void ResolveDamage(Actor defender,
            double damage,
            DamageTypes dmgType,
            Limb limbAttacked,
            Limb limbAttacking)
        {
            if (damage > 0)
            {
                DealDamage(damage, defender, dmgType, limbAttacking, limbAttacked);
            }
            else
            {
                GameLoop.AddMessageLog($"{defender.Name} received no damage!");
            }
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
                    GameLoop.GetCurrentMap().AddMagiEntity(item);
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
