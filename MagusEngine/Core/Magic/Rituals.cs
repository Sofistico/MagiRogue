using MagiRogue.Data.Enumerators;
using MagiRogue.Entities;
using MagiRogue.GameSys.Tiles;
using System.Collections.Generic;

namespace MagusEngine.Core.Magic
{
    public class Rituals
    {
        /// <summary>
        /// Also means the spell level, goes from 1 to 10.
        /// </summary>
        public int BaseDifficulty { get; private set; }
        public List<ISpellEffect> RitualEffect { get; set; }
        public string RitualName { get; set; }
        public string RitualDescription { get; set; }
        public int MpNodeCost { get; set; }
        public string RitualId { get; set; }
        public ArtMagic RitualSchool { get; set; }
        public int DurationOfRitual { get; set; }
        public int RitualRange { get; set; }

        public Rituals(int baseDifficulty,
            List<ISpellEffect> ritualEffect,
            string ritualName,
            string ritualDescription,
            int mpNodeCost,
            string ritualId,
            ArtMagic ritualSchool,
            int durationRitual,
            int ritualRange)
        {
            BaseDifficulty = baseDifficulty;
            RitualEffect = ritualEffect;
            RitualName = ritualName;
            RitualDescription = ritualDescription;
            MpNodeCost = mpNodeCost;
            RitualId = ritualId;
            RitualSchool = ritualSchool;
            DurationOfRitual = durationRitual;
            RitualRange = ritualRange;
        }

        public void PerformRitual(NodeTile node, Actor caster, Point target)
        {
            if (node.MpPoints >= MpNodeCost)
            {
                node.DestroyTile(new TileFloor(node.Position, "stone"));
                foreach (var effect in RitualEffect)
                {
                    effect.ApplyEffect(target, caster, new SpellBase(RitualId, RitualName,
                        RitualSchool, RitualRange, BaseDifficulty, MpNodeCost));
                }

                GameLoop.AddMessageLog($"{caster.Name} performed the {RitualName}!");
                return;
            }

            GameLoop.AddMessageLog($"{caster.Name} didn't have enough mana in the node for the ritual!");
        }

        public override string ToString()
        {
            return $"{RitualName} :\n\n{RitualDescription}";
        }
    }
}