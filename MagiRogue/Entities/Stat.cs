using System;

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
        private float personalMana; // Wil be measured in magnitude, in how many magic missile you can cast
        private int ambientMana; // This will be here to model a new MOL like magic system
        private int attack;
        private int attackChance;
        private int defense;
        private int defenseChance;
        private int bodyStat = 1; // the minimum value is 1, the maximum is 20
        private int mindStat = 1; // the minimum value is 1, the maximum is 20
        private int soulStat = 1; // the minimum value is 1, the maximum is 20
        private int godPower;
        private bool godly;

        #endregion StatsFields

        #region StatsProperties

        /// <summary>
        /// current health
        /// </summary>
        public float Health
        {
            get { return health; }
            set { health = value; }
        }

        /// <summary>
        /// maximum health
        /// </summary>
        public float MaxHealth
        {
            get { return maxHealth; }
            set { maxHealth = value; }
        }

        public float BaseHpRegen
        {
            get { return _baseHpRegen; }
            set { _baseHpRegen = value; }
        }

        public float BaseManaRegen
        {
            get { return _baseManaRegen; }
            set { _baseManaRegen = value; }
        }

        /// <summary>
        /// the limit is soul + mind + body, a more potent form than natural mana
        /// \nFor every 100 ml/kg of blood, creates 0,1 of blood mana
        /// </summary>
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
                personalMana = value;
            }
        }

        /// <summary>
        /// attack strength
        /// </summary>
        public int Attack
        {
            get { return attack; }
            set
            {
                attack = bodyStat + value;
            }
        }

        /// <summary>
        /// percent chance of successful hit
        /// </summary>
        public int AttackChance
        {
            get { return attackChance; }
            set
            {
                attackChance = BodyStat + value;
            }
        }

        /// <summary>
        /// defensive strength
        /// </summary>
        public int Defense
        {
            get { return defense; }
            set { defense = value; }
        }

        /// <summary>
        /// percent chance of successfully blocking a hit
        /// </summary>
        public int DefenseChance
        {
            get { return defenseChance; }
            set { defenseChance = value; }
        }

        /// <summary>
        /// The body stat of the actor
        /// </summary>
        public int BodyStat
        {
            get { return bodyStat; }
            set { bodyStat = value; }
        }

        /// <summary>
        /// The mind stat of the actor
        /// </summary>
        public int MindStat
        {
            get { return mindStat; }
            set { mindStat = value; }
        }

        /// <summary>
        /// The soul stat of the actor
        /// </summary>
        public int SoulStat
        {
            get { return soulStat; }
            set { soulStat = value; }
        }

        /// <summary>
        /// The god stat of the actor, checks if the actor is a god as well
        /// </summary>
        public int GodPower
        {
            get { return godPower; }
            set
            {
                if (godly)
                {
                    godPower = value;
                }
                else
                {
                    godPower = 0;
                }
            }
        }

        // To do magic this value must be true, because magic and being a god are the same thing.
        public bool Godly
        {
            get { return godly; }
            set { godly = value; }
        }

        /// <summary>
        /// Formula is (soul*2) + mind + body, this is raw mana made outside the body, it difers from just mana because it's
        /// produced by the world, it's hard to use and gives a boost to mana regen, also if you get more than you can
        /// handle you become crazy.
        /// </summary>
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
        public int ViewRadius { get; set; }

        public float Speed { get; set; }

        #endregion StatsProperties

        #endregion Stats

        #region Constructor

        public Stat()
        {
        }

        public Stat(float health, float maxHealth, float baseHpRegen, float bloodyMana, int naturalMana, int attack, int attackChance, int defense, int defenseChance, int bodyStat, int mindStat, int soulStat, int godPower, bool godly = false)
        {
            this.health = health;
            this.maxHealth = maxHealth;
            _baseHpRegen = baseHpRegen;
            this.personalMana = bloodyMana;
            this.ambientMana = naturalMana;
            this.attack = attack;
            this.attackChance = attackChance;
            this.defense = defense;
            this.defenseChance = defenseChance;
            this.bodyStat = bodyStat;
            this.mindStat = mindStat;
            this.soulStat = soulStat;
            this.godPower = godPower;
            this.godly = godly;
        }

        #endregion Constructor

        #region Methods

        public void SetAttributes(
            int viewRadius,
            float health,
            float maxHealth,
            float baseHpRegen,
            int bodyStat,
            int mindStat,
            int soulStat,
            int attack,
            int attackChance,
            int defense,
            int defenseChance,
            int godPower,
            float speed,
            bool godly = false
            )
        {
            this.ViewRadius = viewRadius;
            this.Health = health;
            this.MaxHealth = maxHealth;
            this.BaseHpRegen = baseHpRegen;
            this.BodyStat = bodyStat;
            this.MindStat = mindStat;
            this.SoulStat = soulStat;
            this.Attack = attack;
            this.AttackChance = attackChance;
            this.Defense = defense;
            this.DefenseChance = defenseChance;
            this.Godly = godly;
            if (godly)
                this.GodPower = godPower;
            this.Speed = speed;
        }

        public void SetAttributes(
           Actor actor,
           string name,
           int viewRadius,
           float health,
           float maxHealth,
           float baseHpRegen,
           int bodyStat,
           int mindStat,
           int soulStat,
           int attack,
           int attackChance,
           int defense,
           int defenseChance,
           int godPower,
           float speed,
           bool godly = false
           )
        {
            actor.Name = name;
            this.ViewRadius = viewRadius;
            this.Health = health;
            this.MaxHealth = maxHealth;
            this.BaseHpRegen = baseHpRegen;
            this.BodyStat = bodyStat;
            this.MindStat = mindStat;
            this.SoulStat = soulStat;
            this.Attack = attack;
            this.AttackChance = attackChance;
            this.Defense = defense;
            this.DefenseChance = defenseChance;
            this.Godly = godly;
            if (godly)
                this.GodPower = godPower;
            this.Speed = speed;
        }

        public void ApplyHpRegen()
        {
            if (Health < MaxHealth)
            {
                float newHp = (BaseHpRegen + Health);
                Health = (float)Math.Round(newHp, 1);
            }
        }

        #endregion Methods
    }
}