using MagusEngine.Exceptions;
using SadConsole;
using SadConsole.Components;
using SadConsole.Input;
using Console = SadConsole.Console;

namespace MagusEngine.Components.EntityComponents
{
    public class MoveViewPortHandler : KeyboardConsoleComponent
    {
        public override void OnAdded(IScreenObject host)
        {
            if (host is not Console)
            {
                throw new GenericException($"{nameof(MoveViewPortHandler)} can only be used on {nameof(Console)}");
            }
        }

        public override void ProcessKeyboard(IScreenObject host, Keyboard keyboard, out bool handled)
        {
            // Upcast this because we know we're only using it with a Console type.
            Console console = (Console)host;

            if (keyboard.IsKeyDown(Keys.Left))
            {
                console.ViewPosition = console.ViewPosition.Translate((-1, 0));
            }

            if (keyboard.IsKeyDown(Keys.Right))
            {
                console.ViewPosition = console.ViewPosition.Translate((1, 0));
            }

            if (keyboard.IsKeyDown(Keys.Up))
            {
                console.ViewPosition = console.ViewPosition.Translate((0, -1));
            }

            if (keyboard.IsKeyDown(Keys.Down))
            {
                console.ViewPosition = console.ViewPosition.Translate((0, +1));
            }

            handled = true;
        }
    }
}
