using MagiRogue.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.System.Magic
{
    public class Rituals
    {
        public int BaseDifficulty { get; private set; }
        public List<ISpellEffect> RitualEffect { get; set; }
        public string RitualName { get; set; }
        public string RitualDescription { get; set; }

        public Rituals(int baseDifficulty,
            List<ISpellEffect> ritualEffect,
            string ritualName,
            string ritualDescription)
        {
            BaseDifficulty = baseDifficulty;
            RitualEffect = ritualEffect;
            RitualName = ritualName;
            RitualDescription = ritualDescription;
        }

        public void PerformRitual()
        {
        }

        public override string ToString()
        {
            return $"{RitualName} :\n\n{RitualDescription}";
        }
    }
}