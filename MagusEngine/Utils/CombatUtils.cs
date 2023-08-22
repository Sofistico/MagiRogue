using Arquimedes.Enumerators;
using MagusEngine.Bus.MapBus;
using MagusEngine.Bus.UiBus;
using MagusEngine.Commands;
using MagusEngine.Core.Entities;
using MagusEngine.Core.Entities.Base;
using MagusEngine.Core.Magic;
using MagusEngine.Serialization;
using MagusEngine.Services;
using MagusEngine.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace MagusEngine.Utils
{
    public static class CombatUtils
    {
        #region Flags

        public static DamageTypes SetFlag(DamageTypes a, DamageTypes b)
        {
            return a | b;
        }

        public static DamageTypes UnsetFlag(DamageTypes a, DamageTypes b)
        {
            return a & ~b;
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

        #endregion Flags

        #region Damage

        public static void DealDamage(double attackMomentum,
            MagiEntity entity,
            DamageTypes dmgType,
            MaterialTemplate? attackMaterial = null,
            Attack? attack = null,
            BodyPart? limbAttacked = null,
            Item? weapon = null,
            BodyPart? limbAttacking = null)
        {
            if (entity is Actor actor)
            {
                double attackVolume = weapon is null ? limbAttacking.Volume : weapon.Volume;
                // calculate how many part wounds the actor will receive!
                var woundParts = CalculatePartWoundsReceived(attackMomentum,
                    limbAttacked,
                    actor.Body.GetArmorOnLimbIfAny(limbAttacked),
                    attackMaterial,
                    attack,
                    attackVolume,
                    weapon,
                    actor.Body.Anatomy.GetAllWounds());

                if (woundParts.Count > 0)
                {
                    // need to redo this to take into account the new tissue based wound!
                    Wound woundTaken = new Wound(dmgType,
                        woundParts);

                    actor.GetAnatomy().Injury(woundTaken, limbAttacked, actor);

                    if (actor.CheckIfDed())
                    {
                        ActionManager.ResolveDeath(actor);
                    }
                    StringBuilder woundString = new(" ");

                    for (int i = 0; i < woundParts.Count; i++)
                    {
                        var partWound = woundParts[i];
                        woundString.AppendLine(DetermineDamageMessage(partWound.PartDamage, partWound));
                    }

                    Locator.GetService<MessageBusService>()
                        .SendMessage<MessageSent>(new(woundString.ToString()));
                    return;
                }
                else
                {
                    // no wound was received! either the armor or the tissue absorbed all damage.
                    Locator.GetService<MessageBusService>()
                        .SendMessage<MessageSent>(new($"The attack glances {entity.Name}!"));
                    return;
                }
            }

            if (entity is Item item)
            {
                item.Condition -= (int)attackMomentum;
            }
        }

        /// <summary>
        /// Calculates the damage a defender takes after a successful hit and subtracts it from its
        /// Health Then displays the outcome in the MessageLog.
        /// </summary>
        /// <param name="defender"></param>
        /// <param name="momentum"></param>
        public static void ResolveDamage(Actor defender,
            double momentum,
            DamageTypes dmgType,
            BodyPart limbAttacked,
            MaterialTemplate attackMaterial,
            Attack attack,
            Item? weapon = null,
            BodyPart? limbAttacking = null)
        {
            if (momentum > 0)
            {
                DealDamage(momentum, defender, dmgType, attackMaterial, attack, limbAttacked, weapon, limbAttacking);
            }
            else
            {
                Locator.GetService<MessageBusService>()
                    .SendMessage<MessageSent>(new($"{defender.Name} received no damage!", true));
            }
        }

        private static List<PartWound> CalculatePartWoundsReceived(double attackMomentum,
            BodyPart partInjured,
            Item targetArmor,
            MaterialTemplate attackMaterial,
            Attack attack,
            double attackVolume,
            Item? weapon = null,
            List<Wound>? preExistingWounds = null)
        {
            // TODO: One day take a reaaaaaaalllllllly long look at this method!
            var list = new List<PartWound>();
            Queue<Tissue> tissues = new(partInjured.Tissues);
            double remainingEnergy = attackMomentum;
            if (targetArmor is not null)
            {
                double armorEffectiveness = CalculateArmorEffectiveness(targetArmor,
                    attackVolume,
                    attackMaterial,
                    remainingEnergy,
                    attack,
                    weapon);
                remainingEnergy -= armorEffectiveness;
            }
#if DEBUG
            int loopAmount = 0;
#endif
            while (true) // be realllllllllly careful about this loop...
            {
#if DEBUG
                if (++loopAmount == 1000)
                    Locator.GetService<MagiLog>().Log("Something went really wrong...");
#endif
                if (remainingEnergy <= 0)
                    return list;

                if (!tissues.TryDequeue(out Tissue tissue))
                {
                    if (partInjured.Insides.Count == 0)
                        break; // even if there is energy, there is no more tissue to penetrate... might do some stuff ltr
                    var rngInside = partInjured.Insides.GetRandomItemFromList();
                    foreach (var tis in rngInside.Tissues)
                    {
                        tissues.Enqueue(tis);
                    }
                    continue;
                }

                // Calculate the amount of energy required to penetrate the tissue
                double energyToPenetrate = CalculateEnergyCostToPenetrateMaterial(tissue.Material,
                    //tissue.Volume,
                    attackMaterial,
                    attack,
                    attackMomentum,
                    attackVolume,
                    0,
                    weapon is null ? 0 : weapon.QualityMultiplier());

                var attackTotalContactArea = attackVolume * (double)((double)attack.ContactArea / 100);

                // let's see if it will just be better to use the tissue.volume!
                var tissueContactArea = Math.Pow(tissue.Volume, 2 / 3);
                double woundVolume = attackTotalContactArea <= tissueContactArea
                    ? (double)(tissue.Volume * (double)attackTotalContactArea / (double)tissueContactArea)
                    : tissue.Volume;

                double strain = attackTotalContactArea / attackMomentum;
                PartWound partWound = new PartWound(woundVolume, strain, tissue, attack.DamageTypes);
                if (remainingEnergy >= energyToPenetrate)
                {
                    remainingEnergy -= energyToPenetrate;
                    list.Add(partWound);
                }
                else
                {
                    // The attack has lost all of its energy and stopped in this tissue layer, doing
                    // blunt damage
                    partWound.PartDamage = DamageTypes.Blunt;
                    list.Add(partWound);
                    break;
                }
            }

            // The projectile has penetrated all tissue layers
            return list;
        }

        private static string DetermineDamageMessage(DamageTypes partDamage, PartWound wound)
        {
            StringBuilder baseMessage = new();
            if (partDamage is DamageTypes.Blunt)
            {
                if (wound.Tissue.Material.ImpactStrainsAtYield <= 24.999
                    && wound.Strain >= wound.Tissue.Material.ImpactStrainsAtYield)
                {
                    baseMessage.Append("fracturating");
                }
                else if (wound.Tissue.Material.ImpactStrainsAtYield <= 49.999
                    && wound.Strain >= wound.Tissue.Material.ImpactStrainsAtYield)
                {
                    baseMessage.Append("torn");
                }
                else
                {
                    baseMessage.Append("bruising");
                }
            }
            else if (partDamage is DamageTypes.Sharp || partDamage is DamageTypes.Pierce)
            {
                baseMessage.Append("tearing");
                if (wound.WholeTissue)
                    baseMessage.Append(" apart");
            }

            baseMessage.Append(" the ").Append(wound.Tissue.Name);

            return baseMessage.ToString();
        }

        public static void ApplyHealing(int dmg, Actor stats, DamageTypes healingType, bool healsStamina = false)
        {
            if (healsStamina)
            {
                // Recovr stamina first
                stats.Body.Stamina += dmg;

                if (stats.Body.Stamina >= stats.Body.MaxStamina)
                {
                    stats.Body.Stamina = stats.Body.MaxStamina;

                    Locator.GetService<MessageBusService>().SendMessage<MessageSent>(new("You feel your inner fire full"));
                }
            }

            // then here heal the limbs
            // TODO: Add the function to do it

            StringBuilder bobTheBuilder = new StringBuilder($"You healed for {dmg} damage");
            switch (healingType)
            {
                case DamageTypes.None:
                    Locator.GetService<MessageBusService>().SendMessage<MessageSent>(new(bobTheBuilder.Append(", feeling your bones and skin growing over your wounds!").ToString()));
                    break;

                case DamageTypes.Force:
                    Locator.GetService<MessageBusService>().SendMessage<MessageSent>(new(bobTheBuilder.Append(", filling your movements with a spring!").ToString()));
                    break;

                case DamageTypes.Fire:
                    Locator.GetService<MessageBusService>().SendMessage<MessageSent>(new(bobTheBuilder.Append(", firing your will!").ToString()));
                    break;

                case DamageTypes.Cold:

                    Locator.GetService<MessageBusService>().SendMessage<MessageSent>(new(bobTheBuilder.Append(", leaving you lethargic.").ToString()));
                    break;

                case DamageTypes.Poison:
                    Locator.GetService<MessageBusService>().SendMessage<MessageSent>(new(bobTheBuilder.Append(", ouch it hurt!").ToString()));
                    break;

                case DamageTypes.Acid:
                    stats.Body.Stamina -= dmg;
                    Locator.GetService<MessageBusService>().SendMessage<MessageSent>(new(bobTheBuilder.Append(", dealing equal damage to yourself, shouldn't have done that.").ToString()));
                    break;

                case DamageTypes.Shock:
                    Locator.GetService<MessageBusService>().SendMessage<MessageSent>(new(bobTheBuilder.Append(", felling yourself speeding up!").ToString()));
                    break;

                case DamageTypes.Soul:
                    Locator.GetService<MessageBusService>().SendMessage<MessageSent>(new(bobTheBuilder.Append(", feeling your soul at rest.").ToString()));
                    break;

                case DamageTypes.Mind:
                    Locator.GetService<MessageBusService>().SendMessage<MessageSent>(new(bobTheBuilder.Append(", feeling your mind at ease.").ToString()));
                    break;

                default:
                    break;
            }
        }

        #endregion Damage

        #region SpellDmg

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
                Locator.GetService<MessageBusService>().SendMessage<MessageSent>(new($"{poorGuy.Name} resisted the effects of {spellCasted.SpellName}"));
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
                // the actor + exploding dice is the dice that the target will throw for either
                // defense or blocking the projectile
                // TODO: When shield is done, needs to add the shield or any protection against the spell
                if (poorGuy is Actor actor && diceRoll >= actor.GetDefenseAbility() + Mrn.Exploding2D6Dice)
                {
                    ResolveResist(poorGuy, caster, spellCasted, effect);
                }
                else
                {
                    Locator.GetService<MessageBusService>().SendMessage<MessageSent>(new($"{caster.Name} missed {poorGuy.Name}!"));
                }
            }
        }

        #endregion SpellDmg

        #region Melee hit

        /// <summary>
        /// Calculates the outcome of an attacker's attempt at scoring a hit on a defender, using
        /// the attacker's AttackChance and a random d100 roll as the basis. Modifies a
        /// StringBuilder message that will be displayed in the MessageLog
        /// </summary>
        /// <param name="attacker"></param>
        /// <param name="defender"></param>
        /// <param name="attackMessage"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">throws if the attack is null</exception>
        public static (bool, BodyPart?, BodyPart, DamageTypes, Item?, MaterialTemplate?) ResolveHit(
            Actor attacker,
            Actor defender,
            StringBuilder attackMessage,
            Attack attack,
            bool firstPerson = false,
            Limb? limbAttacked = null)
        {
            Item wieldedItem = attacker.WieldedItem();
            BodyPart bpAttacking = attacker.GetAttackingLimb(attack) ?? throw new ArgumentNullException(nameof(attack));

            // Create a string that expresses the attacker and defender's names as well as the
            // attack verb
            string person = firstPerson ? "You" : attacker.Name;
            string verb = firstPerson ? attack.AttackVerb[0] : attack.AttackVerb[1];
            string with = attack?.AttacksUsesLimbName == true ? firstPerson
                    ? $" with your {bpAttacking.BodyPartName}"
                    : $" with {attacker.GetAnatomy().Pronoum()} {bpAttacking.BodyPartName}"
                : "";

            attackMessage.AppendFormat("{0} {1} the {2}{3}", person, verb, defender.Name, with);
            var materialUsed = wieldedItem is null
                ? bpAttacking.Tissues.Find(i => i.Flags.Contains(TissueFlag.Structural)
                    || i.Flags.Contains(TissueFlag.Muscular))?.Material
                : wieldedItem.Material;
            if (materialUsed is null)
            {
                Locator.GetService<MagiLog>().Log("Material was null!");
                materialUsed = bpAttacking.Tissues[0].Material;
            }
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
        /// Calculates the outcome of a defender's attempt at blocking incoming hits. Modifies a
        /// StringBuilder messages that will be displayed in the MessageLog, expressing the number
        /// of hits blocked.
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

                var damageWithoutPenetration = attackMomentum - Mrn.Exploding2D6Dice + (defender.Body.Endurance * 0.5);
                var penetrationDamage = damageWithoutPenetration * attack.PenetrationPercentage;
                var finalDamage = damageWithoutPenetration + penetrationDamage;
                finalDamage = MathMagi.Round(finalDamage);

                if (defender.SituationalFlags.Contains(ActorSituationalFlags.Prone))
                    finalDamage *= 2;

                totalDamage += finalDamage;
            }

            return totalDamage;
        }

        #endregion Melee hit

        #region Death Resolve

        /// <summary>
        /// Removes an Actor that has died and displays a message showing the actor that has died,
        /// and the loot they dropped
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
            // dump the dead actor's inventory (if any) at the map position where it died
            if (defender.Inventory.Count > 0)
            {
                foreach (Item item in defender.Inventory)
                {
                    // move the Item to the place where the actor died
                    item.Position = defender.Position;

                    // Now let the MultiSpatialMap know that the Item is visible
                    //GameLoop.GetCurrentMap().AddMagiEntity(item);
                    Locator.GetService<MessageBusService>().SendMessage<AddEntitiyCurrentMap>(new(item));
                }

                // Clear the actor's inventory. Not strictly necessary, but makes for good coding habits!
                defender.Inventory.Clear();
            }

            // actor goes bye-bye
            Locator.GetService<MessageBusService>().SendMessage<RemoveEntitiyCurrentMap>(new(defender));

            if (defender is Player)
            {
                Locator.GetService<MessageBusService>().SendMessage<MessageSent>(new($" {defender.Name} was killed."));
            }

            // Now show the deathMessage in the messagelog
            Locator.GetService<MessageBusService>().SendMessage<MessageSent>(new(deathMessage.ToString()));
        }

        #endregion Death Resolve

        #region Physics

        private static double CalculateArmorEffectiveness(Item armor,
            double attackSize,
            MaterialTemplate attackMaterial,
            double attackMomentum,
            Attack attack,
            Item? weapon)
        {
            double momentumAfterArmor = attackMomentum;
            var percentileRoll = Mrn.Normal1D100Dice;
            if (percentileRoll >= armor.Coverage)
            {
                switch (armor.ArmorType)
                {
                    // chain converts the attack damage to blunt!
                    case ArmorType.Chain:
                        attackMaterial.ImpactStrainsAtYield = 50;
                        momentumAfterArmor = CalculateEnergyCostToPenetrateMaterial(armor.Material,
                            //armor.Volume,
                            attackMaterial,
                            attack,
                            attackMomentum,
                            attackSize,
                            armor.QualityMultiplier(),
                            weapon is not null ? weapon.QualityMultiplier() : 0);
                        break;

                    case ArmorType.Leather:
                    case ArmorType.Plate:
                        momentumAfterArmor = CalculateEnergyCostToPenetrateMaterial(armor.Material,
                            //armor.Volume,
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
            }

            return momentumAfterArmor;
        }

        private static double CalculateEnergyCostToPenetrateMaterial(MaterialTemplate defenseMaterial,
            //double defenseMaterialVolume,
            MaterialTemplate attackMaterial,
            Attack attack,
            double originalMomentum,
            double attackVolume,
            double armorQualityMultiplier = 0,
            double weaponQualityModifier = 0)
        {
            return attack.DamageTypes switch
            {
                DamageTypes.Blunt => CalculateBluntDefenseCost(defenseMaterial,
                                        //attackMaterial,
                                        originalMomentum,
                                        attack.ContactArea,
                                        attackVolume,
                                        armorQualityMultiplier,
                                        //defenseMaterialVolume,
                                        weaponQualityModifier),
                DamageTypes.Pierce or DamageTypes.Sharp => CalculateEdgedDefenseCost(defenseMaterial,
                                        attackMaterial,
                                        //defenseMaterialVolume,
                                        attack.ContactArea,
                                        originalMomentum,
                                        armorQualityMultiplier,
                                        weaponQualityModifier,
                                        attackVolume),
                _ => (defenseMaterial.Hardness ?? 1) * 0.9,
            };
        }

        // Edged defense calculations
        public static double CalculateEdgedDefenseCost(MaterialTemplate defenseMaterial,
            MaterialTemplate attackMaterial,
            //double layerVolume,
            int attackContactArea,
            double originalMomentum,
            double armorQualityModifier,
            double weaponQualityModifier,
            double attackVolume)
        {
            double shearFRatio = (double)((double)attackMaterial.ShearFracture / (double)defenseMaterial.ShearFracture);
            double shearYRatio = (double)((double)attackMaterial.ShearYield / (double)defenseMaterial.ShearYield);
            var momentumReq = (shearYRatio + (attackContactArea + 1) * shearFRatio)
                * (10 + 2 * armorQualityModifier) / (attackMaterial.MaxEdge * (weaponQualityModifier + 1));
            if (originalMomentum >= momentumReq)
            {
                return (double)((double)originalMomentum * (double)(defenseMaterial.ShearStrainAtYield / 50000));
            }
            else
            {
                // damage is converted to blunt!
                return CalculateBluntDefenseCost(defenseMaterial,
                    //attackMaterial,
                    originalMomentum,
                    attackContactArea,
                    attackVolume,
                    armorQualityModifier,
                    //layerVolume,
                    weaponQualityModifier);
            }
        }

        // Blunt defense calculations
        public static double CalculateBluntDefenseCost(MaterialTemplate defenseMaterial,
            //MaterialTemplate attackMaterial,
            double originalMomentum,
            int attackContactArea,
            double attackSize,
            double armorQualityMultiplier,
            //double layerVolume,
            double weaponQualityModifier)
        {
            var bluntDeflection = 2 * attackSize * defenseMaterial.ImpactYield
                < attackContactArea * defenseMaterial.DensityKgM3;
            if (bluntDeflection && defenseMaterial.ImpactStrainsAtYield < 50)
            {
                return 0;
            }
            else
            {
                var minimumMomentum = (2 * defenseMaterial.ImpactFracture - defenseMaterial.ImpactYield)
                    * (2 + 0.4 * armorQualityMultiplier) * (attackContactArea * (weaponQualityModifier + 1));
                if (originalMomentum >= minimumMomentum)
                {
                    return originalMomentum * ((defenseMaterial.ImpactStrainsAtYield ?? 1) / 50000);
                }
            }

            return originalMomentum * 0.9;
        }

        /// <summary>
        /// Gets the momentum of the attack, based partially on the Dwarf Fortress math on attack
        /// momentum calculation
        /// <para>
        /// <seealso href="https://dwarffortresswiki.org/index.php/DF2014:Weapon">Link to DF wiki article</seealso>
        /// </para>
        /// </summary>
        /// <param name="attacker"></param>
        /// <param name="wieldedItem"></param>
        public static double GetAttackMomentumWithItem(Actor attacker, Item wieldedItem, Attack attack)
        {
            return MathMagi.Round(
                (attacker.GetStrenght() + wieldedItem.BaseDmg + Mrn.Exploding2D6Dice)
                * attacker.GetRelevantAttackAbilityMultiplier(attack.AttackAbility)
                + (10 + 2 * wieldedItem.QualityMultiplier())) * attacker.GetAttackVelocity(attack)
                + (1 + attacker.Volume / ((wieldedItem.Material.DensityKgM3 ?? 1) * wieldedItem.Volume));
        }

        /// <summary>
        /// Gets the momentum of the attack, based partially on the Dwarf Fortress math on attack
        /// momentum calculation
        /// <para>
        /// <seealso href="https://dwarffortresswiki.org/index.php/DF2014:Weapon">Link to DF wiki article</seealso>
        /// </para>
        /// </summary>
        /// <param name="attacker"></param>
        public static double GetAttackMomentum(Actor attacker, BodyPart limbAttacking, Attack attack)
        {
            return MathMagi.Round(
                (attacker.GetStrenght() + Mrn.Exploding2D6Dice)
                * (attacker.GetRelevantAbilityMultiplier(attack.AttackAbility) + 1)
                * attacker.GetAttackVelocity(attack)
                + (1 + attacker.Volume / ((limbAttacking.GetStructuralMaterial()?.DensityKgM3 ?? 1) * limbAttacking.Volume)));
        }

        #endregion Physics
    }
}
