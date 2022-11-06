using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.Entities
{
    /// <summary>
    /// Defines the personality of the mind... Has values ranging from -50 to 50, being 0 the middle
    /// Based on this parameters, controls the response of an inteligent entity, animals and non-inteligent
    /// have other responses and thus no personality...
    /// </summary>
    public class Personality
    {
        private Dictionary<string, int> cached { get; set; }

        public int Anger { get; set; }
        public int Law { get; set; }
        public int Loyalty { get; set; }
        public int Familiy { get; set; }
        public int Friendship { get; set; }
        public int Power { get; set; }
        public int Cunning { get; set; }
        public int Tradition { get; set; }
        public int Independence { get; set; }
        public int SelfControl { get; set; }
        public int Harmony { get; set; }
        public int HardWork { get; set; }
        public int Craftmanship { get; set; }
        public int MartialProwess { get; set; }
        public int Sacrifice { get; set; }
        public int Perseverance { get; set; }
        public int Nature { get; set; }
        public int Peace { get; set; }
        public int Knowledge { get; set; }
        public int Commerce { get; set; }
        public int Romance { get; set; }
        public int Leisure { get; set; }
        public int Lazyness { get; set; }
        public int Greed { get; set; }

        // just some empty constructor, cuz i love them!
        public Personality()
        {
        }

        // ctor from hell
        public Personality(int anger, int law, int loyalty, int familiy, int friendship, int power, int cunning,
            int tradition, int independence, int selfControl, int harmony, int hardWork, int sacrifice,
            int perseverance, int nature, int peace, int knowledge, int craftmanship, int martialProwess, int commerce, int rommance, int leisure, int lazyness)
        {
            Anger = anger;
            Law = law;
            Loyalty = loyalty;
            Familiy = familiy;
            Friendship = friendship;
            Power = power;
            Cunning = cunning;
            Tradition = tradition;
            Independence = independence;
            SelfControl = selfControl;
            Harmony = harmony;
            HardWork = hardWork;
            Sacrifice = sacrifice;
            Perseverance = perseverance;
            Nature = nature;
            Peace = peace;
            Knowledge = knowledge;
            Craftmanship = craftmanship;
            MartialProwess = martialProwess;
            Commerce = commerce;
            Romance = rommance;
            Leisure = leisure;
            Lazyness = lazyness;
        }

        public Dictionary<string, int> ReturnAsDictionary(bool cleanCache = false)
        {
            if (cleanCache || cached is null)
                cached = GetType().GetProperties().ToDictionary(p => p.Name, p => (int)p.GetValue(this));
            return cached;
        }
    }
}