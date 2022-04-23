using MagiRogue.Entities;
using MagiRogue.GameSys.Tiles;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.GameSys.Magic
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
        public MagicSchool RitualSchool { get; set; }

        public Rituals(int baseDifficulty,
            List<ISpellEffect> ritualEffect,
            string ritualName,
            string ritualDescription,
            int mpNodeCost,
            string ritualId,
            MagicSchool ritualSchool)
        {
            BaseDifficulty = baseDifficulty;
            RitualEffect = ritualEffect;
            RitualName = ritualName;
            RitualDescription = ritualDescription;
            MpNodeCost = mpNodeCost;
            RitualId = ritualId;
            RitualSchool = ritualSchool;
        }

        public void PerformRitual(NodeTile node, Actor caster, Point target)
        {
            if (node.MpPoints >= MpNodeCost)
            {
                node.DestroyTile(new TileFloor(node.Position, "stone"));
                foreach (var effect in RitualEffect)
                {
                    effect.ApplyEffect(target, caster, new SpellBase(RitualId, RitualName,
                        RitualSchool, 0, BaseDifficulty, MpNodeCost));
                }

                GameLoop.UIManager.MessageLog.Add($"{caster.Name} performed the {RitualName}!");
                return;
            }

            GameLoop.UIManager.MessageLog.Add($"{caster.Name} didn't have enough mana in the node for the ritual!");
        }

        public override string ToString()
        {
            return $"{RitualName} :\n\n{RitualDescription}";
        }
    }
}