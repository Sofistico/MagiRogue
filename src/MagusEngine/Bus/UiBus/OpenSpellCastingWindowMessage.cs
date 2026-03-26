using System;
using System.Collections.Generic;
using MagusEngine.Core.Magic;

namespace MagusEngine.Bus.UiBus
{
    public class OpenSpellCastingWindowMessage
    {
        public List<Spell> Spells { get; set; } = null!;
        public double CurrentMana { get; set; }
        public Action<Spell>? OnCast { get; set; }

        public OpenSpellCastingWindowMessage(List<Spell> spells, double currentMana, Action<Spell>? onCast)
        {
            Spells = spells;
            CurrentMana = currentMana;
            OnCast = onCast;
        }
    }
}
