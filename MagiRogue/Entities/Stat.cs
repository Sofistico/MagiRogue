using MagiRogue.Utils;
using System;
using System.Runtime.Serialization;

namespace MagiRogue.Entities
{
    public class Stat
    {
        #region Stats

        #region StatsFields

        private float health; // Will remove in favor of a limb like system
        private float maxHealth; // Will remove in favor of a limb like system
        private float _baseHpRegen;
        private float _baseManaRegen; // Must take into account the ambient mana
                                      // from the enviroment and inside the body of the caster
        private float personalMana; // Will be measured in magnitude, in how many magic missile you can cast
        private float _maxPersonalMana;
        private float originalMana;
        private int ambientMana; // This will be here to model a new MOL like magic system
        private int baseStrength;
        private int baseAttack;
        private int protection;
        private int defenseChance;
        private int bodyStat = 1; // the minimum value is 1, the maximum is 20
        private int mindStat = 1; // the minimum value is 1, the maximum is 20
        private int soulStat = 1; // the minimum value is 1, the maximum is 20

        #endregion StatsFields

        #region StatsProperties

        [DataMember]
        public float Health
        {
            get
            {
                return health;
            }

            set
            {
                health = value;
            }
        }

        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        public float MaxHealth
        {
            get
            {
                if (maxHealth < 0)
                {
                    maxHealth = 0;
                    return maxHealth;
                }

                if (maxHealth == 0 && health != 0)
                {
                    maxHealth = health;
                    return maxHealth;
                }

                return maxHealth;
            }

            set
            {
                if (value < 0)
                {
                    maxHealth = 0;
                    return;
                }

                if (maxHealth < health)
                {
                    maxHealth = health;
                    return;
                }
                maxHealth = value;
            }
        }

        /// <summary>
        /// Should be between 0.01 to 2, it's applied before any possible multipliers
        /// </summary>
        [DataMember]
        public float BaseHpRegen
        {
            get { return _baseHpRegen; }
            set { _baseHpRegen = value; }
        }

        /// <summary>
        /// Should be between 0.01 to 2, it's applied before any possible multipliers
        /// </summary>
        [DataMember]
        public float BaseManaRegen
        {
            get { return _baseManaRegen; }
            set { _baseManaRegen = value; }
        }

        [DataMember]
        public float PersonalMana
        {
            get
            {
                if (personalMana < 0)
                {
                    personalMana = 0;
                }

                return personalMana;
            }

            set
            {
                if (value < 0)
                {
                    personalMana = 0;
                    return;
                }

                if (value >= MaxPersonalMana && MaxPersonalMana != 0)
                {
                    personalMana = MaxPersonalMana;
                    return;
                }

                personalMana = value;
            }
        }

        public float MaxPersonalMana
        {
            get
            {
                if (_maxPersonalMana < 0)
                {
                    _maxPersonalMana = 0;
                    return _maxPersonalMana;
                }

                if (_maxPersonalMana == 0 && personalMana != 0)
                {
                    _maxPersonalMana = personalMana;
                    return _maxPersonalMana;
                }

                return _maxPersonalMana;
            }

            set
            {
                if (value < 0)
                {
                    _maxPersonalMana = 0;
                    return;
                }

                if (_maxPersonalMana < personalMana)
                {
                    _maxPersonalMana = personalMana;
                    return;
                }
                _maxPersonalMana = value;
            }
        }

        /// <summary>
        /// attack strength
        /// </summary>
        [DataMember]
        public int Strength
        {
            get { return baseStrength; }

            set
            {
                baseStrength = bodyStat + value;
            }
        }

        /// <summary>
        /// How likely an entity is to hit something.
        /// </summary>
        [DataMember]
        public int BaseAttack
        {
            get
            {
                if (baseAttack == 0)
                    return 1;
                return baseAttack;
            }

            set
            {
                baseAttack = (int)(BodyStat + value + (0.5 * MindStat));
            }
        }

        /// <summary>
        /// defensive strength, how much damage is lessened when you are hit
        /// </summary>
        [DataMember]
        public int Protection
        {
            get { return protection; }
            set { protection = value + (BodyStat / 2); }
        }

        /// <summary>
        /// chance of successfully blocking a hit, how likely you are either to dodge or block damage
        /// </summary>
        [DataMember]
        public int Defense
        {
            get { return defenseChance; }
            set { defenseChance = value + bodyStat + (MindStat / 2); }
        }

        /// <summary>
        /// The body stat of the actor
        /// </summary>
        [DataMember]
        public int BodyStat
        {
            get { return bodyStat; }
            set { bodyStat = value; }
        }

        /// <summary>
        /// The mind stat of the actor
        /// </summary>
        [DataMember]
        public int MindStat
        {
            get { return mindStat; }
            set { mindStat = value; }
        }

        /// <summary>
        /// The soul stat of the actor
        /// </summary>
        [DataMember]
        public int SoulStat
        {
            get { return soulStat; }
            set { soulStat = value; }
        }

        /// <summary>
        /// Formula is (soul*2) + mind + body, this is raw mana made outside the body, it difers from just mana because it's
        /// produced by the world, it's hard to use and gives a boost to mana regen, also if you get more than you can
        /// handle you become crazy.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        public int AmbientMana
        {
            get { return ambientMana; }

            set
            {
                if (value <= bodyStat + mindStat + (soulStat * 2))
                    ambientMana = value;
                else
                    ambientMana = bodyStat + mindStat + (soulStat * 2);
            }
        }

        /// <summary>
        /// The view radius of the actor, for seeing things
        /// </summary>
        [DataMember]
        public int ViewRadius { get; set; } = 5;

        /// <summary>
        /// The speed of the actor, how fast it does the things, goes from 0.5 to 2.0
        /// </summary>
        [DataMember]
        public float Speed { get; set; } = 1;

        /// <summary>
        /// How likely you are to hit something, it's a bonus to a dice roll of a d20 against defense, if zero
        /// means you don't have any bonus
        /// </summary>
        public int Precision { get; set; } = 10;

        /// <summary>
        /// How many attacks per turn a unit have.
        /// </summary>
        public int AttackSpeed { get; set; } = 1;

        #endregion StatsProperties

        #endregion Stats

        #region Constructor

        public Stat()
        {
        }

        #endregion Constructor

        #region Methods

        public void SetAttributes(
           int viewRadius,
           float health,
           float baseHpRegen,
           int bodyStat,
           int mindStat,
           int soulStat,
           int baseAttack,
           int attackChance,
           int protection,
           int defenseChance,
           float speed,
           float _baseManaRegen,
           int personalMana)
        {
            this.ViewRadius = viewRadius;
            this.Health = health;
            this.BaseHpRegen = baseHpRegen;
            this.BodyStat = bodyStat;
            this.MindStat = mindStat;
            this.SoulStat = soulStat;
            this.Strength = baseAttack;
            this.BaseAttack = attackChance;
            this.Protection = protection;
            this.Defense = defenseChance;
            this.Speed = speed;
            this.BaseManaRegen = _baseManaRegen;
            this.PersonalMana = personalMana;
        }

        public void ApplyHpRegen()
        {
            if (Health < MaxHealth)
            {
                float newHp = (BaseHpRegen + Health);
                Health = MathMagi.Round(newHp);
            }
        }

        public void ApplyManaRegen()
        {
            if (PersonalMana < MaxPersonalMana)
            {
                float newMana = (BaseManaRegen + PersonalMana);
                PersonalMana = MathMagi.Round(newMana);
            }
        }

        public void ApplyAllRegen()
        {
            ApplyHpRegen();
            ApplyManaRegen();
        }

        #endregion Methods
    }
}