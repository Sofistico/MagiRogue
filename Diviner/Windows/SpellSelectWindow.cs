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
        private SpellBase _selectedSpell;
        private Action<SpellBase> _onCast;
        private double _currentMana;

        public SpellSelectWindow(double currentMana) : base("Select your spell")
        {
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
                if (_hotKeys.TryGetValue(key.Character, out var objSpell) && objSpell is SpellBase spell && _currentMana >= spell.MagicCost)
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

            SetupSelectionButtons(BuildHotKeysButtons(listSpells, OnSpellSelected));
        }
    }
}
