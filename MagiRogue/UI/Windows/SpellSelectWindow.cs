﻿using SadConsole;
using System;
using Console = SadConsole.Console;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MagiRogue.System;
using MagiRogue.System.Magic;
using MagiRogue.Entities;
using MagiRogue.UI.Controls;
using SadRogue.Primitives;
using SadConsole.Input;

namespace MagiRogue.UI.Windows
{
    public class SpellSelectWindow : PopWindow
    {
        //private const int SpellButtonWidth = 40;

        private List<SpellBase> _spellList;
        private MagiButton _castButton;
        private readonly Console _descriptionArea;
        private readonly Dictionary<char, SpellBase> _hotKeys;

        private SpellBase _selectedSpell;
        private Action<SpellBase> _onCast;
        private double _currentMana;

        public SpellSelectWindow(List<SpellBase> spellKnow, double currentMana) : base("Select your spell")
        {
            _hotKeys = new Dictionary<char, SpellBase>();
            _spellList = spellKnow;
            _currentMana = currentMana;

            const string castText = "Cast";
            int castButtonWidth = castText.Length + 2;

            _castButton = new MagiButton(castButtonWidth)
            {
                Text = castText,
                Position = new Point(Width - castButtonWidth - 1, Height - 2)
            };
            _castButton.Click += (_, __) =>
            {
                _onCast?.Invoke(_selectedSpell);
                Hide();
            };

            _descriptionArea = new Console(Width - ButtonWidth - 3, Height - 4)
            {
                Position = new Point(ButtonWidth + 2, 1)
            };

            Children.Add(_descriptionArea);
        }

        public override bool ProcessKeyboard(Keyboard info)
        {
            foreach (var key in info.KeysPressed)
            {
                if (_hotKeys.TryGetValue(key.Character, out var spell) && _currentMana >= spell.ManaCost)
                {
                    _onCast(spell);
                    Hide();
                    return true;
                }
            }
            return base.ProcessKeyboard(info);
        }

        public void Show(List<SpellBase> listSpells, Action<SpellBase> onCast, double currentMana)
        {
            _currentMana = currentMana;
            _onCast = onCast;

            _selectedSpell = null;
            _descriptionArea.Clear();

            _castButton.IsEnabled = false;

            RefreshControls(listSpells);

            base.Show(true);
        }

        private Dictionary<MagiButton, Action> BuildSpellsControls(List<SpellBase> listSpells)
        {
            _hotKeys.Clear();

            int yCount = 1;

            var controlDictionary = listSpells
                .OrderBy(s => s.SpellName)
                .ToDictionary
                (s =>
                    {
                        var hotkeyLetter = (char)(96 + yCount);
                        _hotKeys.Add(hotkeyLetter, s);

                        var spellButton = new MagiButton(ButtonWidth - 2)
                        {
                            Text = $"{hotkeyLetter}. {s.SpellName}",
                            Position = new Point(1, yCount++),
                            IsEnabled = _currentMana >= s.ManaCost
                        };
                        spellButton.Click += (_, __) =>
                        {
                            _onCast(s);
                            Hide();
                        };

                        return spellButton;
                    },
                    s => (Action)(() => OnSpellSelected(s)));

            var buttons = controlDictionary.Keys.ToArray();
            for (int i = 1; i < buttons.Length; i++)
            {
                buttons[i - 1].NextSelection = buttons[i];
                buttons[i].PreviousSelection = buttons[i - 1];
            }

            return controlDictionary;
        }

        public void OnSpellSelected(SpellBase spell)
        {
            _selectedSpell = spell;
            _descriptionArea.Clear();
            _descriptionArea.Cursor.Position = new Point(0, 1);
            _descriptionArea.Cursor.Print(_selectedSpell.ToString());
            _castButton.IsEnabled = true;
        }

        private void RefreshControls(List<SpellBase> listSpells)
        {
            Controls.Clear();

            Controls.Add(_castButton);

            SetupSelectionButtons(BuildSpellsControls(listSpells));
        }
    }
}