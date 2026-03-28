using Arquimedes.Enumerators;
using Arquimedes.Settings;
using MagusEngine;
using SadConsole.Input;

namespace Diviner.Extensions
{
    public static class KeyboardExtensions
    {
        private static readonly Dictionary<string, Keys> _map = new(StringComparer.OrdinalIgnoreCase)
        {
            ["up_arrow"] = Keys.Up,
            ["down_arrow"] = Keys.Down,
            ["left_arrow"] = Keys.Left,
            ["right_arrow"] = Keys.Right,
            ["up"] = Keys.Up,
            ["down"] = Keys.Down,
            ["left"] = Keys.Left,
            ["right"] = Keys.Right,

            ["enter"] = Keys.Enter,
            ["return"] = Keys.Enter,
            ["esc"] = Keys.Escape,
            ["escape"] = Keys.Escape,
            ["space"] = Keys.Space,
            ["spacebar"] = Keys.Space,
            ["tab"] = Keys.Tab,
            ["backspace"] = Keys.Back,
            ["capslock"] = Keys.CapsLock,
            ["pause"] = Keys.Pause,
            ["shift"] = Keys.LeftShift,
            ["left_shift"] = Keys.LeftShift,
            ["right_shift"] = Keys.RightShift,
            ["ctrl"] = Keys.LeftControl,
            ["control"] = Keys.LeftControl,
            ["left_ctrl"] = Keys.LeftControl,
            ["right_ctrl"] = Keys.RightControl,
            ["alt"] = Keys.LeftAlt,
            ["left_alt"] = Keys.LeftAlt,
            ["right_alt"] = Keys.RightAlt,

            ["0"] = Keys.D0,
            ["1"] = Keys.D1,
            ["2"] = Keys.D2,
            ["3"] = Keys.D3,
            ["4"] = Keys.D4,
            ["5"] = Keys.D5,
            ["6"] = Keys.D6,
            ["7"] = Keys.D7,
            ["8"] = Keys.D8,
            ["9"] = Keys.D9,

            ["a"] = Keys.A,
            ["b"] = Keys.B,
            ["c"] = Keys.C,
            ["d"] = Keys.D,
            ["e"] = Keys.E,
            ["f"] = Keys.F,
            ["g"] = Keys.G,
            ["h"] = Keys.H,
            ["i"] = Keys.I,
            ["j"] = Keys.J,
            ["k"] = Keys.K,
            ["l"] = Keys.L,
            ["m"] = Keys.M,
            ["n"] = Keys.N,
            ["o"] = Keys.O,
            ["p"] = Keys.P,
            ["q"] = Keys.Q,
            ["r"] = Keys.R,
            ["s"] = Keys.S,
            ["t"] = Keys.T,
            ["u"] = Keys.U,
            ["v"] = Keys.V,
            ["w"] = Keys.W,
            ["x"] = Keys.X,
            ["y"] = Keys.Y,
            ["z"] = Keys.Z,

            ["numpad_0"] = Keys.NumPad0,
            ["numpad_1"] = Keys.NumPad1,
            ["numpad_2"] = Keys.NumPad2,
            ["numpad_3"] = Keys.NumPad3,
            ["numpad_4"] = Keys.NumPad4,
            ["numpad_5"] = Keys.NumPad5,
            ["numpad_6"] = Keys.NumPad6,
            ["numpad_7"] = Keys.NumPad7,
            ["numpad_8"] = Keys.NumPad8,
            ["numpad_9"] = Keys.NumPad9,
            ["numpad_add"] = Keys.Add,
            ["numpad_subtract"] = Keys.Subtract,
            ["numpad_multiply"] = Keys.Multiply,
            ["numpad_divide"] = Keys.Divide,
            ["numpad_decimal"] = Keys.Decimal,

            ["f1"] = Keys.F1,
            ["f2"] = Keys.F2,
            ["f3"] = Keys.F3,
            ["f4"] = Keys.F4,
            ["f5"] = Keys.F5,
            ["f6"] = Keys.F6,
            ["f7"] = Keys.F7,
            ["f8"] = Keys.F8,
            ["f9"] = Keys.F9,
            ["f10"] = Keys.F10,
            ["f11"] = Keys.F11,
            ["f12"] = Keys.F12,

            ["pageup"] = Keys.PageUp,
            ["pagedown"] = Keys.PageDown,
            ["home"] = Keys.Home,
            ["end"] = Keys.End,
            ["insert"] = Keys.Insert,
            ["delete"] = Keys.Delete,

            [";"] = Keys.OemSemicolon,
            ["+"] = Keys.OemPlus,
            [","] = Keys.OemComma,
            ["-"] = Keys.OemMinus,
            ["."] = Keys.OemPeriod,
            ["/"] = Keys.OemQuestion,
            ["`"] = Keys.OemTilde,
            ["["] = Keys.OemOpenBrackets,
            ["\\"] = Keys.OemPipe,
            ["]"] = Keys.OemCloseBrackets,
            ["'"] = Keys.OemQuotes,
            ["backslash"] = Keys.OemBackslash,

            ["dot"] = Keys.OemPeriod,
            ["comma"] = Keys.OemComma,
            ["minus"] = Keys.OemMinus,
            ["plus"] = Keys.OemPlus,
            ["period"] = Keys.OemPeriod,
            ["question"] = Keys.OemQuestion,
            ["tilde"] = Keys.OemTilde,
            ["bracket_left"] = Keys.OemOpenBrackets,
            ["bracket_right"] = Keys.OemCloseBrackets,
            ["quote"] = Keys.OemQuotes,
            ["backslash_alt"] = Keys.OemBackslash
        };

        public static Keys GetKeyFromString(string str)
        {
            if (!_map.TryGetValue(str, out var key))
                return Keys.None;
            return key;
        }

        public static KeymapAction? GetActionFromKey(this Keyboard info)
        {
            var inputs = Locator.GetService<Dictionary<InputKey, InputSetting>>();

            foreach (var key in info.KeysPressed)
            {
                var modifiers = info.KeysDown.Where(i =>
                    i.Key == Keys.LeftShift ||
                    i.Key == Keys.RightShift ||
                    i.Key == Keys.LeftControl ||
                    i.Key == Keys.RightControl ||
                    i.Key == Keys.LeftAlt ||
                    i.Key == Keys.RightAlt
                ).Select(i =>
                {
                    var key = i.Key.ToString();
                    if (key.Contains("shift", StringComparison.OrdinalIgnoreCase))
                        return "Shift";
                    if (key.Contains("control", StringComparison.OrdinalIgnoreCase))
                        return "Control";
                    return "Alt";
                }).ToArray();

                if (!inputs.TryGetValue(new(key.Key, modifiers), out var input))
                    break;
                return input.Action;
            }
            return null;
        }
    }
}
