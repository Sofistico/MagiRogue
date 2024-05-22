using Diviner.Controls;
using MagusEngine.Actions;
using MagusEngine.Core.Entities;
using MagusEngine.Systems;
using MagusEngine.Utils.Extensions;
using SadConsole.Input;

namespace Diviner.Windows
{
    public class WaitWindow : PopWindow
    {
        private const string waitTitle = "How long you want to wait?";
        private const string wait = "{0} - Wait {1} {2}";

        public WaitWindow() : base(waitTitle)
        {
            int yCount = 1;
            var wait1Hour = new MagiButton(string.Format(wait, CharExtension.GetCharHotkey(yCount), 1, "hour"), () =>
                {
                    ActionManager.WaitForNTurns(Find.Universe.Time.GetNHoursFromTurn(1), (Actor)Find.CurrentMap?.ControlledEntitiy);
                    Hide();
                }, new SadRogue.Primitives.Point(ButtonWidth, yCount));
            var wait3Hour = new MagiButton(string.Format(wait, CharExtension.GetCharHotkey(yCount++), 3, "hours"), () =>
                {
                    ActionManager.WaitForNTurns(Find.Universe.Time.GetNHoursFromTurn(3),
                        (Actor)Find.CurrentMap?.ControlledEntitiy);
                    Hide();
                }, new SadRogue.Primitives.Point(ButtonWidth, yCount));
            var wait6Hour = new MagiButton(string.Format(wait, CharExtension.GetCharHotkey(yCount++), 6, "hours"), () =>
                {
                    ActionManager.WaitForNTurns(Find.Universe.Time.GetNHoursFromTurn(6),
                        (Actor)Find.CurrentMap?.ControlledEntitiy);
                    Hide();
                }, new SadRogue.Primitives.Point(ButtonWidth, yCount));
            _hotKeys.Add('a', (wait1Hour, true));
            _hotKeys.Add('b', (wait3Hour, true));
            _hotKeys.Add('c', (wait6Hour, true));
            SetupSelectionButtons(wait1Hour, wait3Hour, wait6Hour);
            Tag = Enums.WindowTag.Wait;
        }

        public override bool ProcessKeyboard(Keyboard info)
        {
            foreach (var key in info.KeysPressed)
            {
                if (_hotKeys.TryGetValue(key.Character, out var tuple))
                {
                    ((MagiButton)tuple.Item1).Action?.Invoke();

                    return true;
                }
            }
            return base.ProcessKeyboard(info);
        }
    }
}
