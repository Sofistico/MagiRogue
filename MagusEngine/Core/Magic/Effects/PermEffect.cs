using MagiRogue.Data.Enumerators;
using MagiRogue.Entities;
using Newtonsoft.Json;

namespace MagusEngine.Core.Magic.Effects
{
    public class PermEffect : IPermEffect
    {
        // Will be here for remembering, do not know how it will proced
        // 18/12/2021 - I don't really remember why i made it
        // private const int _totalTime = Time.TimeHelper.Year;

        public int NodeCost { get; set; }
        public EffectType EffectType { get; set; }
        public ISpellEffect Enchantment { get; set; }
        public Actor Caster { get; set; }
        public string EnchantName { get; set; }
        public string EnchantDesc { get; set; }

        [JsonConstructor]
        public PermEffect(Actor caster, ISpellEffect enchantment, int nodeCost, string enchantName,
            string enchantDesc)
        {
            Caster = caster;
            Enchantment = enchantment;
            NodeCost = nodeCost;
            EnchantName = enchantName;
            EnchantDesc = enchantDesc;
        }

        public void Enchant(int nodesSacrificed)
        {
            if (nodesSacrificed >= NodeCost)
            {
                Enchantment.ApplyEffect(Caster.Position, Caster, new SpellBase());
            }
            else
                GameLoop.UIManager.MessageLog
                    .PrintMessage($"{Caster.Name} does not have enough nodes for the enchantment");
        }
    }
}