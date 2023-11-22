using Diviner.Controls;
using MagusEngine.Core.Magic;
using SadConsole;
using SadConsole.Input;

namespace Diviner.Windows
{
    /// <summary>
    /// The window of the spell selector.
    /// </summary>
    public class SpellSelectWindow : PopWindow
    {
        private readonly MagiButton _castButton;
        private readonly Dictionary<char, SpellBase> _hotKeys;

        private SpellBase _selectedSpell;
        private Action<SpellBase> _onCast;
        private double _currentMana;

        public SpellSelectWindow(double currentMana) : base("Select your spell")
        {
            _hotKeys = new Dictionary<char, SpellBase>();
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
                _onCast?.Invoke(_selectedSpell!);
                Hide();
            };
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

            _selectedSpell = null!;

            _castButton.IsEnabled = false;

            RefreshControls(listSpells);

            base.Show(true);
        }

        private List<MagiButton> BuildSpellsControls(List<SpellBase> listSpells)
        {
            _hotKeys.Clear();

            int yCount = 1;

            var spellOrdered = listSpells
                .OrderBy(s => s.SpellName)
                .ToArray();
            var list = new List<MagiButton>(spellOrdered.Length);
            for (int i = 0; i < spellOrdered.Length; i++)
            {
                var hotkeyLetter = (char)(96 + yCount);
                var s = spellOrdered[i];
                _hotKeys.Add(hotkeyLetter, s);
                var spellButton = new MagiButton(ButtonWidth - 2)
                {
                    Text = $"{hotkeyLetter}. {s.SpellName}",
                    Position = new Point(1, yCount++),
                    IsEnabled = _currentMana >= s.ManaCost,
                    Action = () => OnSpellSelected(s),
                };
                spellButton.Click += (_, __) =>
                {
                    _onCast(s);
                    Hide();
                };
                spellButton.Focused += (_, __) => spellButton.Action.Invoke();
                list.Add(spellButton);
            }
            for (int i = 1; i < list.Count; i++)
            {
                list[i - 1].NextSelection = list[i];
                list[i].PreviousSelection = list[i - 1];
            }

            return list;
        }

        public void OnSpellSelected(SpellBase spell)
        {
            _selectedSpell = spell;
            _descriptionArea.Clear();
            _descriptionArea.Cursor.Position = new Point(0, 1);
            _descriptionArea.Cursor.Print(_selectedSpell.ToString());
            _descriptionArea.Cursor.Position = new Point(0, 5);
            if (_selectedSpell.Description is not null)
                _descriptionArea.Cursor.Print(_selectedSpell.Description);
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
