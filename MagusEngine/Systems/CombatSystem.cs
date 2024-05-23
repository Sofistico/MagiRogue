using Arquimedes.Enumerators;
using MagusEngine.Actions;
using MagusEngine.Bus.MapBus;
using MagusEngine.Bus.UiBus;
using MagusEngine.Core;
using MagusEngine.Core.Entities;
using MagusEngine.Core.Entities.Base;
using MagusEngine.Core.Magic;
using MagusEngine.ECS.Components.MagiObjComponents;
using MagusEngine.ECS.Components.TilesComponents;
using MagusEngine.Services;
using MagusEngine.Systems.Physics;
using MagusEngine.Systems.Time.Nodes;
using MagusEngine.Utils;
using MagusEngine.Utils.Extensions;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagusEngine.Systems
{
    public static class CombatSystem
    {
        #region Damage

        public static void DealDamage(double attackMomentum,
            MagiEntity entity,
            DamageType dmgType,
            Material? attackMaterial = null,
            Attack? attack = null,
            BodyPart? limbAttacked = null,
            Item? weapon = null,
            BodyPart? limbAttacking = null,
            Spell? spellUsed = null)
        {
            if (entity is Actor actor)
            {
                double attackVolume = CalculateAttackVolume(spellUsed, weapon, limbAttacking);

                // calculate how many part wounds the actor will receive!
                var woundParts = CalculatePartWoundsReceived(attackMomentum,
                    limbAttacked!,
                    actor.Body.GetArmorOnLimbIfAny(limbAttacked!),
                    attackMaterial!,
                    attack!,
                    attackVolume,
                    weapon,
                    actor.Body.Anatomy.GetAllWounds());

                if (woundParts.Count > 0)
                {
                    ApplyWoundsToActor(actor, dmgType, woundParts, limbAttacked);

                    if (actor.CheckIfDed())
                        ActionManager.ResolveDeath(actor);

                    SendWoundMessages(woundParts);
                    return;
                }
                else
                {
                    // no wound was received! either the armor or the tissue absorbed all damage.
                    NotifyAttackGlance(entity);
                    return;
                }
            }

            if (entity is Item item)
            {
                item.Condition -= (int)attackMomentum;
            }
        }

        private static double CalculateAttackVolume(Spell? spellUsed, Item? weapon, BodyPart? limbAttacking)
        {
            if (spellUsed != null)
                return spellUsed.Effects.Sum(i => i.Volume);
            else if (weapon != null)
                return weapon.Volume;
            else if (limbAttacking != null)
                return limbAttacking.Volume;
            else
                return 1;
        }

        private static void ApplyWoundsToActor(Actor actor, DamageType dmgType, List<PartWound> woundParts, BodyPart? limbAttacked)
        {
            Wound woundTaken = new(dmgType, woundParts);
            actor.ActorAnatomy.Injury(woundTaken, limbAttacked!, actor);
        }

        private static void SendWoundMessages(List<PartWound> woundParts)
        {
            StringBuilder woundString = new(" ");
            foreach (var partWound in woundParts)
            {
                woundString.AppendLine(DetermineDamageMessage(partWound.PartDamage, partWound));
            }
            Locator.GetService<MessageBusService>().SendMessage<AddMessageLog>(new(woundString.ToString()));
        }

        private static void NotifyAttackGlance(MagiEntity entity)
        {
            Locator.GetService<MessageBusService>()
                .SendMessage<AddMessageLog>(new($"The attack glances {entity.Name}!"));
        }

        /// <summary>
        /// Calculates the damage a defender takes after a successful hit and subtracts it from its
        /// Health Then displays the outcome in the MessageLog.
        /// </summary>
        /// <param name="defender"></param>
        /// <param name="momentum"></param>
        public static void ResolveDamage(MagiEntity defender,
            double momentum,
            DamageType dmgType,
            BodyPart limbAttacked,
            Material attackMaterial,
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
                    .SendMessage<AddMessageLog>(new($"{defender.Name} received no damage!", true));
            }
        }

        private static List<PartWound> CalculatePartWoundsReceived(double attackMomentum,
            BodyPart partInjured,
            Item targetArmor,
            Material attackMaterial,
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

                if (!tissues.TryDequeue(out Tissue? tissue))
                {
                    if (partInjured.Insides.Count == 0)
                        break; // even if there is energy, there is no more tissue to penetrate... might do some stuff ltr
                    var rngInside = partInjured.Insides.GetRandomItemFromList();
                    if (rngInside is null)
                        continue;
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
                PartWound partWound = new(woundVolume, strain, tissue, attack.DamageType!);
                if (remainingEnergy >= energyToPenetrate)
                {
                    remainingEnergy -= energyToPenetrate;
                    list.Add(partWound);
                }
                else
                {
                    // The attack has lost all of its energy and stopped in this tissue layer, doing
                    // blunt damage
                    partWound.PartDamage.Type = DamageTypes.Blunt;
                    list.Add(partWound);
                    break;
                }
            }

            // The projectile has penetrated all tissue layers
            return list;
        }

        private static string DetermineDamageMessage(DamageType partDamage, PartWound wound)
        {
            StringBuilder baseMessage = new();
            // i liked this, a lot.
            if (partDamage.Type is DamageTypes.Blunt)
            {
                if (wound.Tissue.Material.ImpactStrainsAtYield <= 24.999
                    && wound.Strain >= wound.Tissue.Material.ImpactStrainsAtYield)
                {
                    baseMessage.Append(partDamage.SeverityDmgString[2]);
                }
                else if (wound.Tissue.Material.ImpactStrainsAtYield <= 49.999
                    && wound.Strain >= wound.Tissue.Material.ImpactStrainsAtYield)
                {
                    baseMessage.Append(partDamage.SeverityDmgString[1]);
                }
                else
                {
                    baseMessage.Append(partDamage.SeverityDmgString[0]);
                }
            }
            else if (partDamage.Type is DamageTypes.Sharp || partDamage.Type is DamageTypes.Pierce)
            {
                if (!wound.WholeTissue)
                    baseMessage.Append(partDamage.SeverityDmgString[0]);
                else
                    baseMessage.Append(partDamage.SeverityDmgString[1]);
            }
            else
            {
                var percent = MathMagi.GetPercentageBasedOnMax(wound.VolumeFraction, wound?.TotalVolume ?? 0);
                var fractionNecessaryToChange = 100 / partDamage.SeverityDmgString.Length;
                int index = (int)(percent / fractionNecessaryToChange);
                if (index != 0 && index <= partDamage.SeverityDmgString.Length)
                    baseMessage.Append(partDamage.SeverityDmgString[index]);
            }

            baseMessage.Append(" the ").Append(wound?.Tissue.Name);

            return baseMessage.ToString();
        }

        public static void ApplyHealing(int dmg, Actor stats, bool healsStamina = false)
        {
            if (healsStamina)
            {
                // Recovr stamina first
                stats.Body.Stamina += dmg;

                if (stats.Body.Stamina >= stats.Body.MaxStamina)
                {
                    stats.Body.Stamina = stats.Body.MaxStamina;

                    Locator.GetService<MessageBusService>().SendMessage<AddMessageLog>(new("You feel your tiredeness fade away"));
                }
            }

            // then here heal the limbs
            // TODO: Add the function to do it
            Locator.GetService<MessageBusService>().SendMessage<AddMessageLog>(new($"You healed for {dmg} damage"));
        }

        #endregion Damage

        #region SpellDmg

        private static void ResolveResist(MagiEntity poorGuy,
            Actor caster,
            Spell spellCasted,
            ISpellEffect effect,
            Attack attack)
        {
            int luck = Mrn.Exploding2D6Dice;
            if (effect.IsResistable || MagicManager.PenetrateResistance(spellCasted, caster, poorGuy, luck))
                DealDamage(effect.BaseDamage, poorGuy, effect.GetDamageType()!, attack: attack, spellUsed: spellCasted);
            else
                Locator.GetService<MessageBusService>().SendMessage<AddMessageLog>(new($"{poorGuy.Name} resisted the effects of {spellCasted.Name}"));
        }

        public static void ResolveSpellHit(MagiEntity poorGuy, Actor caster, Spell spellCasted, ISpellEffect effect, Attack attack)
        {
            var hit = ResolveSpellHit(poorGuy, caster, spellCasted, effect);
            if (hit)
            {
                ResolveResist(poorGuy, caster, spellCasted, effect, attack);
            }
        }

        public static bool ResolveSpellHit(MagiEntity poorGuy, Actor caster, Spell spellCasted, ISpellEffect effect)
        {
            if (!effect.CanMiss)
            {
                return true;
            }
            else
            {
                int diceRoll = (int)Math.Round((Mrn.Exploding2D6Dice + caster.GetPrecision()) * spellCasted.Proficiency);
                // the actor + exploding dice is the dice that the target will throw for either
                // defense or blocking the projectile
                // TODO: When shield is done, needs to add the shield or any protection against the spell
                if (poorGuy is Actor actor && diceRoll >= actor.GetDefenseAbility() + Mrn.Exploding2D6Dice)
                {
                    return true;
                }
                else
                {
                    Locator.GetService<MessageBusService>().SendMessage<AddMessageLog>(new($"{caster.Name} missed {poorGuy.Name}!"));
                    return false;
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
        public static (bool, BodyPart?, BodyPart, DamageType?, Item?, Material) ResolveHit(Actor attacker,
            Actor defender,
            StringBuilder attackMessage,
            Attack attack,
            bool firstPerson = false,
            Limb? limbAttacked = null)
        {
            Item? wieldedItem = attacker.WieldedItem();
            BodyPart bpAttacking = attacker.GetAttackingLimb(attack) ?? throw new ArgumentNullException(nameof(attack));

            // Create a string that expresses the attacker and defender's names as well as the
            // attack verb
            string? person = firstPerson ? "You" : attacker.Name;
            string verb = firstPerson ? attack.AttackVerb[0] : attack.AttackVerb[1];
            string with = attack?.AttacksUsesLimbName == true ? firstPerson
                    ? $" with your {bpAttacking.BodyPartName}"
                    : $" with {attacker.ActorAnatomy.Pronoum()} {bpAttacking.BodyPartName}"
                : "";

            attackMessage.AppendFormat("{0} {1} the {2}{3}", person, verb, defender.Name, with);
            var materialUsed = wieldedItem is null ? bpAttacking.Tissues.Find(i => i.Flags.Contains(TissueFlag.Structural)
                || i.Flags.Contains(TissueFlag.Muscular))?.Material
                : wieldedItem?.Material;
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
                limbAttacked ??= defender.ActorAnatomy.GetRandomLimb();
                return (true, limbAttacked, bpAttacking, attack!.DamageType, wieldedItem, materialUsed);
            }
            else
            {
                return (false, null, bpAttacking, attack!.DamageType, wieldedItem, materialUsed);
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

        #region Projectile

        public static void ShootProjectile(double force,
            Point origin,
            Item projectile,
            Direction direction,
            int angle,
            MagiEntity shooter,
            Point? target = null)
        {
            if (projectile is null || shooter?.CurrentMagiMap is null)
                return;
            // interesting but wrong!
            var initialVelocity = PhysicsSystem.CalculateInitialVelocityFromMassAndForce(projectile.Mass, force);
            var map = shooter.CurrentMagiMap;
            if (initialVelocity == 0)
                ActionManager.DropItem(projectile, origin, map, shooter);

            var range = PhysicsSystem.CalculateProjectileRange(initialVelocity, angle, PhysicsConstants.PlanetGravity);
            var time = Convert.ToInt64(PhysicsSystem.CalculateProjectileTime(initialVelocity, angle, PhysicsConstants.PlanetGravity));
            Point pointToGo = target ?? direction.GetPointToGoFromOrigin(origin, (int)range);

            if (map.CheckForIndexOutOfBounds(pointToGo))
                pointToGo = map.NormalizePointInsideMap(pointToGo);

            var projectileComp = new ProjectileComp(time,
                origin,
                pointToGo,
                direction,
                true,
                null,
                force);
            projectile.AddComponent(projectileComp, ProjectileComp.Tag);
            projectileComp.UpdatePath(shooter.CurrentMagiMap);
            Locator.GetService<MessageBusService>().SendMessage<AddEntitiyCurrentMap>(new(projectile));
            Locator.GetService<MessageBusService>().SendMessage<AddTurnNode>(new(new ComponentTimeNode(time, projectile.ID, projectileComp.Travel)));
        }

        public static void HitProjectile(MagiEntity? projectile, Point lastPoint, DamageType dmg, Material material, double force, bool ignoresObstacles)
        {
            // get the parent.Position properyy and check if there is anything in pos
            // if there is, get the first item in the list
            // if it is an actor, resolve the hit
            // if it is an item, resolve the hit
            // if it is an wall, resolve if will get stuck or not
            if (projectile is null || projectile.CurrentMagiMap is null)
                return;
            var point = projectile.Position;
            var map = projectile.CurrentMagiMap;
            var entities = map.GetEntitiesAt<MagiEntity>(point, map.LayerMasker.Mask((int)MapLayer.ACTORS, (int)MapLayer.ITEMS, (int)MapLayer.FURNITURE));
            if (entities.Any())
            {
                var entity = entities.GetRandomItemFromList();
                if (entity == null)
                {
                    Locator.GetService<MagiLog>().Log($"An error occured, Entity is null! - Point: {point}, Map: {map.MapId}");
                    return;
                }
                var projectileAttack = Attack.ConstructGenericAttack(projectile.Name, ["hit", "hits"], dmg.Id, true);
                DealDamage(force, entity, dmg, material, projectileAttack);
            }
            else
            {
                string? message = null;
                // probably a wall or somestuff like that!
                if (!map.IsTileWalkable(point, ignoresObstacles))
                {
                    var tile = map.GetTileAt(point);
                    message = $"The {projectile.Name} hits the {tile.Name}!";
                    projectile.Position = lastPoint;
                    //var projectileMaterial = projectile.GetMaterial();
                    // check to see if the wall or the projectile will get hurt by the impact
                    //TODO: See if it's in pascals or mega pascals
                    if (tile.Material.ImpactStrainsAtYield >= projectile.GetMaterial().ImpactFractureMpa)
                    {
                        var item = (Item)projectile;
                        item.Condition -= (int)Math.Sqrt((double)(item.Material.ImpactFractureMpa - tile.Material.ImpactStrainsAtYield));
                    }
                    else
                    {
                        var damage = (int)Math.Sqrt((double)(tile.Material.ImpactStrainsAtYield - projectile.GetMaterial().ImpactFractureMpa));
                        tile.AddComponent(new DamagedTileComponent(damage), DamagedTileComponent.Tag);
                    }
                }
                else
                {
                    projectile.Position = point;
                    // the projectile hit a wall!
                }
                if (!message.IsNullOrEmpty())
                    Locator.GetService<MessageBusService>().SendMessage<AddMessageLog>(new(message));
            }
        }

        #endregion Projectile

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
            StringBuilder deathMessage = new();
            deathMessage.AppendFormat("{0} died", defender.Name);
            // dump the dead actor's inventory (if any) at the map position where it died
            if (defender.Inventory.Count > 0)
            {
                foreach (Item item in defender.Inventory)
                {
                    // move the Item to the place where the actor died
                    item.Position = defender.Position;

                    // Now let the MultiSpatialMap know that the Item is visible
                    //Find.CurrentMap.AddMagiEntity(item);
                    Locator.GetService<MessageBusService>().SendMessage<AddEntitiyCurrentMap>(new(item));
                }

                // Clear the actor's inventory. Not strictly necessary, but makes for good coding habits!
                defender.Inventory.Clear();
            }

            // actor goes bye-bye
            Locator.GetService<MessageBusService>().SendMessage<RemoveEntitiyCurrentMap>(new(defender));

            if (defender is Player)
            {
                Locator.GetService<MessageBusService>().SendMessage<AddMessageLog>(new($" {defender.Name} was killed."));
            }

            // Now show the deathMessage in the messagelog
            Locator.GetService<MessageBusService>().SendMessage<AddMessageLog>(new(deathMessage.ToString()));
        }

        #endregion Death Resolve

        #region Physics

        private static double CalculateArmorEffectiveness(Item armor,
            double attackSize,
            Material attackMaterial,
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
                        momentumAfterArmor = CalculateEnergyCostToPenetrateMaterial(armor.Material!,
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
                        momentumAfterArmor = CalculateEnergyCostToPenetrateMaterial(armor.Material!,
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

        private static double CalculateEnergyCostToPenetrateMaterial(Material defenseMaterial,
            //double defenseMaterialVolume,
            Material attackMaterial,
            Attack attack,
            double originalMomentum,
            double attackVolume,
            double armorQualityMultiplier = 0,
            double weaponQualityModifier = 0)
        {
            return attack.DamageType?.Type switch
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
        public static double CalculateEdgedDefenseCost(Material defenseMaterial,
            Material attackMaterial,
            //double layerVolume,
            int attackContactArea,
            double originalMomentum,
            double armorQualityModifier,
            double weaponQualityModifier,
            double attackVolume)
        {
            double shearFRatio = (attackMaterial.ShearFracture ?? 1 / defenseMaterial.ShearFracture ?? 1);
            double shearYRatio = (attackMaterial.ShearYield ?? 1 / defenseMaterial.ShearYield ?? 1);
            var momentumReq = (shearYRatio + ((attackContactArea + 1) * shearFRatio)) * (10 + (2 * armorQualityModifier))
                / (attackMaterial.MaxEdge * (weaponQualityModifier + 1));
            if (originalMomentum >= momentumReq)
            {
                return (double)((double)originalMomentum * (defenseMaterial.ShearStrainAtYield ?? 1 / 50000));
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
        public static double CalculateBluntDefenseCost(Material defenseMaterial,
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
                var minimumMomentum = ((2 * defenseMaterial.ImpactFracture) - defenseMaterial.ImpactYield)
                    * (2 + (0.4 * armorQualityMultiplier)) * (attackContactArea * (weaponQualityModifier + 1));
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
        public static double GetAttackMomentumWithItem(Actor attacker, Item? wieldedItem, Attack attack)
        {
            return (MathMagi.Round(
                ((attacker.GetStrenght() + wieldedItem?.BaseDmg ?? 0 + Mrn.Exploding2D6Dice)
                * attacker.GetRelevantAttackAbilityMultiplier(attack.AttackAbility))
                + (10 + (2 * wieldedItem?.QualityMultiplier() ?? 1))) * attacker.GetAttackVelocity(attack))
                + (1 + (attacker.Volume / ((wieldedItem?.Material?.DensityKgM3 ?? 1) * wieldedItem?.Volume ?? 1)));
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
                ((attacker.GetStrenght() + Mrn.Exploding2D6Dice)
                * (attacker.GetRelevantAbilityMultiplier(attack.AttackAbility) + 1)
                * attacker.GetAttackVelocity(attack))
                + (1 + (attacker.Volume / ((limbAttacking.GetStructuralMaterial()?.DensityKgM3 ?? 1) * limbAttacking.Volume))));
        }

        #endregion Physics
    }
}
