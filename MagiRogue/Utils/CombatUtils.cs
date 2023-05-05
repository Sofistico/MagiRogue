using MagiRogue.Data.Enumerators;
using MagiRogue.Data.Serialization;
using MagiRogue.Entities;
using MagiRogue.GameSys.Magic;
using System.Collections.Generic;
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

        public static void DealDamage(double attackMomentum,
            MagiEntity entity,
            DamageTypes dmgType,
            MaterialTemplate? attackMaterial = null,
            Attack? attack = null,
            BodyPart? limbAttacking = null,
            BodyPart? limbAttacked = null,
            MagiEntity? attacker = null,
            Item? weapon = null)
        {
            if (entity is Actor actor)
            {
                var tissuesPenetrated = CalculateNumTissueLayersPenetrated(attackMomentum,
                    limbAttacked.Tissues,
                    actor.Body.GetArmorOnLimbIfAny(limbAttacked),
                    attackMaterial,
                    attack,
                    attacker.Volume,
                    weapon);

                // any remaining dmg goes to create a wound
                if (tissuesPenetrated.Count > 0)
                {
                    // need to redo this to take into account the new tissue based wound!
                    Wound woundTaken = new Wound(dmgType,
                        tissuesPenetrated);

                    actor.GetAnatomy().Injury(woundTaken, limbAttacked, actor);

                    if (actor.CheckIfDed())
                    {
                        Commands.ActionManager.ResolveDeath(actor);
                    }

                    StringBuilder woundString = new($"The {entity.Name} received ");
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
                    GameLoop.AddMessageLog($"The attack glances {entity.Name}!");
                    return;
                }
            }

            if (entity is Item item)
            {
                item.Condition -= (int)attackMomentum;
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
        public static (bool, BodyPart?, BodyPart, DamageTypes, Item?, MaterialTemplate?) ResolveHit(
            Actor attacker,
            Actor defender,
            StringBuilder attackMessage,
            Attack attack,
            bool firstPerson = false,
            Limb? limbAttacked = null)
        {
            Item wieldedItem = attacker.WieldedItem();
            BodyPart bpAttacking = attacker.GetAttackingLimb(attack);

            // Create a string that expresses the attacker and defender's names as well as the attack verb
            string person = firstPerson ? "You" : attacker.Name;
            string verb = firstPerson ? attack.AttackVerb[0] : attack.AttackVerb[1];
            string with = attack?.AttacksUsesLimbName == true ? firstPerson
                    ? $" with your {bpAttacking.BodyPartName}"
                    : $" with {attacker.GetAnatomy().Pronoum()} {bpAttacking.BodyPartName}"
                : "";

            attackMessage.AppendFormat("{0} {1} the {2}{3}", person, verb, defender.Name, with);
            var materialUsed = wieldedItem is null ? bpAttacking.BodyPartMaterial : wieldedItem.Material;
            // TODO: Granularize this more!
            if (attacker.GetRelevantAttackAbility(wieldedItem) + Mrn.Exploding2D6Dice >
                defender.GetDefenseAbility()
                + Mrn.Exploding2D6Dice)
            {
                limbAttacked ??= defender.GetAnatomy().GetRandomLimb();
                return (true, limbAttacked, bpAttacking, attack.DamageTypes, wieldedItem, materialUsed);
            }
            else
            {
                return (false, null, bpAttacking, attack.DamageTypes, wieldedItem, materialUsed);
            }
        }

        /// <summary>
        /// Calculates the outcome of a defender's attempt
        /// at blocking incoming hits.
        /// Modifies a StringBuilder messages that will be displayed
        /// in the MessageLog, expressing the number of hits blocked.
        /// </summary>
        /// <param name="defender"></param>
        /// <returns></returns>
        public static double ResolveDefenseAndGetAttackMomentum(Actor attacker,
            Actor defender,
            bool hit,
            BodyPart limbAttacking,
            Attack attack,
            Item? wieldedItem = null)
        {
            double totalDamage = 0;

            if (hit)
            {
                double attackMomentum;
                if (wieldedItem is null)
                    attackMomentum = GetAttackMomentum(attacker, limbAttacking, attack);
                else
                    attackMomentum = GetAttackMomentumWithItem(attacker, wieldedItem, attack);

                // some moar randomness!
                attackMomentum += Mrn.Exploding2D6Dice;

                var damageWithoutPenetration = attackMomentum - (Mrn.Exploding2D6Dice) + (defender.Body.Endurance * 0.5);
                var penetrationDamage = damageWithoutPenetration * attack.PenetrationPercentage;
                var finalDamage = damageWithoutPenetration + penetrationDamage;
                finalDamage = MathMagi.Round(finalDamage);

                if (defender.SituationalFlags.Contains(ActorSituationalFlags.Prone))
                    finalDamage *= 2;

                totalDamage += finalDamage;
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
                * attacker.GetRelevantAttackAbilityMultiplier(attack.AttackAbility))
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
        public static double GetAttackMomentum(Actor attacker, BodyPart limbAttacking, Attack attack)
        {
            return MathMagi.Round(
                ((attacker.GetStrenght() + Mrn.Exploding2D6Dice)
                * (attacker.GetRelevantAbilityMultiplier(attack.AttackAbility) + 1)
                * attacker.GetAttackVelocity(attack))
                + (1 + (attacker.Volume / (limbAttacking.BodyPartMaterial.Density * limbAttacking.Volume))));
        }

        /// <summary>
        /// Calculates the damage a defender takes after a successful hit
        /// and subtracts it from its Health
        /// Then displays the outcome in the MessageLog.
        /// </summary>
        /// <param name="defender"></param>
        /// <param name="momentum"></param>
        public static void ResolveDamage(Actor defender,
            Actor attacker,
            double momentum,
            DamageTypes dmgType,
            BodyPart limbAttacked,
            BodyPart limbAttacking,
            MaterialTemplate attackMaterial,
            Attack attack,
            Item? weapon = null)
        {
            if (momentum > 0)
            {
                DealDamage(momentum, defender, dmgType, attackMaterial, attack, limbAttacking, limbAttacked, attacker, weapon);
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

        private static List<Tissue> CalculateNumTissueLayersPenetrated(double attackMomentum,
            List<Tissue> tissues,
            Item targetArmor,
            MaterialTemplate attackMaterial,
            Attack attack,
            int attackVolume,
            Item? weapon = null)
        {
            // TODO: One day take a reaaaaaaalllllllly long look at this method!
            var list = new List<Tissue>();
            double remainingEnergy = attackMomentum;

            double armorEffectiveness = CalculateArmorEffectiveness(targetArmor,
                attackVolume,
                attackMaterial,
                remainingEnergy,
                attack,
                weapon);

            remainingEnergy -= armorEffectiveness;
            for (int i = 0; i < tissues.Count; i++)
            {
                if (remainingEnergy <= 0)
                    return list;

                Tissue tissue = tissues[i];

                // Calculate the amount of energy required to penetrate the tissue
                double energyToPenetrate = CalculateEnergyCostToPenetrateMaterial(tissue.Material,
                    tissue.Volume,
                    attackMaterial,
                    attack,
                    attackMomentum,
                    attackVolume,
                    0,
                    weapon is null ? 0 : weapon.QualityMultiplier());

                if (remainingEnergy > energyToPenetrate)
                {
                    remainingEnergy -= energyToPenetrate;
                    list.Add(tissue);
                }
                else
                {
                    // The attack has lost all of its energy and stopped in this tissue layer, doing blunt damage
                    break;
                }
            }

            // The projectile has penetrated all tissue layers
            return list;
        }

        private static double CalculateArmorEffectiveness(Item armor,
            int attackSize,
            MaterialTemplate attackMaterial,
            double attackMomentum,
            Attack attack,
            Item? weapon)
        {
            double momentumAfterArmor = attackMomentum;
            switch (armor.ArmorType)
            {
                case ArmorType.Leather:
                case ArmorType.Chain:
                case ArmorType.Plate:
                    momentumAfterArmor = CalculateEnergyCostToPenetrateMaterial(armor.Material,
                        armor.Volume,
                        attackMaterial,
                        attack,
                        attackMomentum,
                        attackSize,
                        armor.QualityMultiplier(),
                        weapon is not null ? weapon.QualityMultiplier() : 0);

                    break;

                default:
                    break;
            }

            return momentumAfterArmor;
        }

        private static double CalculateEnergyCostToPenetrateMaterial(MaterialTemplate defenseMaterial,
            double defenseMaterialVolume,
            MaterialTemplate attackMaterial,
            Attack attack,
            double originalMomentum,
            int attackVolume,
            double armorQualityMultiplier = 0,
            double weaponQualityModifier = 0)
        {
            return attack.DamageTypes switch
            {
                DamageTypes.Blunt => CalculateBluntDefenseCost(defenseMaterial,
                                        attackMaterial,
                                        originalMomentum,
                                        attack.ContactArea,
                                        attackVolume,
                                        armorQualityMultiplier,
                                        defenseMaterialVolume,
                                        weaponQualityModifier),
                DamageTypes.Pierce or DamageTypes.Sharp => CalculateEdgedDefenseCost(defenseMaterial,
                                        attackMaterial,
                                        defenseMaterialVolume,
                                        attack.ContactArea,
                                        originalMomentum,
                                        armorQualityMultiplier,
                                        weaponQualityModifier,
                                        attackVolume),
                _ => defenseMaterial.Hardness * 0.9,
            };
        }

        // Edged defense calculations
        public static double CalculateEdgedDefenseCost(MaterialTemplate defenseMaterial,
            MaterialTemplate attackMaterial,
            double layerVolume,
            int attackContactArea,
            double originalMomentum,
            double armorQualityModifier,
            double weaponQualityModifier,
            double attackVolume)
        {
            double shearFRatio = attackMaterial.ShearFracture / defenseMaterial.ShearFracture;
            double shearYRatio = attackMaterial.ShearYield / defenseMaterial.ShearYield;
            var momentumReq = (shearYRatio + ((attackContactArea + 1) * shearFRatio))
                * (10 + (2 * armorQualityModifier)) / (attackMaterial.MaxEdge * weaponQualityModifier);
            if (originalMomentum >= momentumReq)
            {
                double momentumCost = 0;

                // Check if weapon can dent the layer
                if (attackMaterial.ShearYield > defenseMaterial.ShearYield)
                {
                    momentumCost += 0.1 * layerVolume;
                }

                // Check if weapon can cut the layer
                if (attackMaterial.ShearFracture > defenseMaterial.ShearFracture)
                {
                    momentumCost += 0.1 * layerVolume;
                }

                return (originalMomentum - momentumCost) * 0.95;
            }
            else
            {
                // damage is converted to blunt!
                return CalculateBluntDefenseCost(defenseMaterial,
                    attackMaterial,
                    originalMomentum,
                    attackContactArea,
                    attackVolume,
                    armorQualityModifier,
                    layerVolume,
                    weaponQualityModifier);
            }
        }

        // Blunt defense calculations
        public static double CalculateBluntDefenseCost(MaterialTemplate defenseMaterial,
            MaterialTemplate attackMaterial,
            double originalMomentum,
            int attackContactArea,
            double attackSize,
            double armorQualityMultiplier,
            double layerVolume,
            double weaponQualityModifier)
        {
            var bluntDeflection = 2 * attackSize * defenseMaterial.ImpactYield
                < attackContactArea * defenseMaterial.Density;
            if (bluntDeflection && defenseMaterial.StrainsAtYield < 50)
            {
                return 0;
            }
            else
            {
                var minimumMomentum = ((2 * defenseMaterial.ImpactFracture) - defenseMaterial.ImpactYield)
                    * (2 + (0.4 * armorQualityMultiplier)) * (attackContactArea * (weaponQualityModifier + 1));
                if (originalMomentum >= minimumMomentum)
                {
                    double momentumCost = 0;
                    // Check if weapon can dent the layer
                    if (attackMaterial.ShearYield + originalMomentum < defenseMaterial.ShearFracture)
                    {
                        momentumCost += 0.1 * layerVolume;
                    }

                    // Check if layer can fracture from impact
                    if (defenseMaterial.ImpactYield < originalMomentum
                        && defenseMaterial.ImpactFracture > originalMomentum)
                    {
                        momentumCost += 0.2 * layerVolume;
                    }

                    // Add momentum cost for complete fracture
                    if (defenseMaterial.ImpactFracture < originalMomentum)
                    {
                        momentumCost += layerVolume;
                    }
                    return (originalMomentum - momentumCost) * 0.95;
                }
            }

            return originalMomentum * 0.01;
        }
    }
}
