using MagiRogue.Commands;
using MagiRogue.Entities;
using MagiRogue.UI.Controls;
using MagiRogue.Utils.Extensions;
using SadConsole.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.UI.Windows
{
    public class WaitWindow : PopWindow
    {
        private const string waitTitle = "How long you want to wait?";
        private const string wait = "{0} - Wait {1} {2}";
        private readonly Dictionary<char, MagiButton> hotKeys = new();

        public WaitWindow() : base(waitTitle)
        {
            int yCount = 1;
            var wait1Hour = new MagiButton(string.Format(wait, CharExtension.GetCharHotkey(yCount), 1, "hour"),
                () =>
                {
                    ActionManager.WaitForNTurns(GameLoop.GetNHoursFromTurn(1),
                        (Actor)GameLoop.GetCurrentMap()?.ControlledEntitiy);
                    Hide();
                }, new SadRogue.Primitives.Point(ButtonWidth, yCount));
            hotKeys.Add('a', wait1Hour);
            AddToDictionary(wait1Hour);
            Tag = Enums.WindowTag.Wait;
        }

        public override bool ProcessKeyboard(Keyboard info)
        {
            foreach (var key in info.KeysPressed)
            {
                if (hotKeys.TryGetValue(key.Character, out var button))
                {
                    button.Action?.Invoke();
                    //Hide();

                    return true;
                }
            }
            return base.ProcessKeyboard(info);
        }
    }
}
