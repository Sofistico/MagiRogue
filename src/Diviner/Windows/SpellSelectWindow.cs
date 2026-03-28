using Diviner.Controls;
using MagusEngine.Core.Magic;
using MagusEngine.Systems;
using MagusEngine.Utils.Extensions;
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
        private Spell _selectedSpell;
        private Action<Spell> _onCast;
        private double _currentMana;

        public SpellSelectWindow(double currentMana) : base("Select your spell")
        {
            _currentMana = currentMana;

            const string castText = "Cast";

            _castButton = new MagiButton(castText, Width, Height)
            {
                Text = castText,
            };
            _castButton.Click += (_, __) =>
            {
                CastSpell(_selectedSpell!);
            };
        }

        public override bool ProcessKeyboard(Keyboard info)
        {
            foreach (var key in info.KeysPressed)
            {
                if (key.Key == Keys.Enter && _selectedSpell is not null)
                {
                    CastSpell(_selectedSpell);
                    return true;
                }
                if (_hotKeys.TryGetValue(key.Character, out var tuple) && tuple.Item2)
                {
                    CastSpell((Spell)tuple.Item1);

                    return true;
                }
            }
            return base.ProcessKeyboard(info);
        }

        private void CastSpell(Spell spell)
        {
            _onCast(spell);
            Hide();
        }

        public void Show(List<Spell> listSpells, Action<Spell> onCast, double currentMana)
        {
            _currentMana = currentMana;
            _onCast = onCast;

            _selectedSpell = null!;

            _castButton.IsEnabled = false;

            RefreshControls(listSpells);

            base.Show(true);
        }

        public void OnSpellSelected(Spell spell)
        {
            _selectedSpell = spell;
            _descriptionArea.Clear();
            _descriptionArea.Cursor.Position = new Point(0, 1);
            _descriptionArea.Cursor.Print(_selectedSpell.ToString());
            _descriptionArea.Cursor.Position = new Point(0, 5);
            if (!_selectedSpell.Description.IsNullOrEmpty())
                _descriptionArea.Cursor.Print(_selectedSpell.Description!);
            _castButton.IsEnabled = true;
        }

        private void RefreshControls(List<Spell> listSpells)
        {
            Controls.Clear();

            Controls.Add(_castButton);

            SetupSelectionButtons(BuildHotKeysButtons(listSpells, OnSpellSelected, new Func<Spell, bool>(s => s.CanCast(Find.Universe.Player, false))));
        }
    }
}
