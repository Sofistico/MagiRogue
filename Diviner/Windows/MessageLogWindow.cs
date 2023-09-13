using GoRogue.Messaging;
using MagusEngine.Bus.UiBus;
using SadConsole;
using SadRogue.Primitives;
using Console = SadConsole.Console;

namespace Diviner.Windows
{
    //A scrollable window that displays messages
    //using a FIFO (first-in-first-out) Queue data structure
    public class MessageLogWindow : MagiBaseWindow,
        ISubscriber<AddMessageLog>,
        ISubscriber<HideMessageLogMessage>
    {
        //max number of lines to store in message log
        private const int maxLines = 100;

        // a Queue works using a FIFO structure, where the first line added
        // is the first line removed when we exceed the max number of lines
        private readonly Queue<string> lines;

        // the messageConsole displays the active messages
        private readonly Console messageConsole;

        //scrollbar for message console
        private readonly SadConsole.UI.Controls.ScrollBar messageScrollBar;

        //Track the current position of the scrollbar
        private int scrollBarCurrentPosition;

        // account for the thickness of the window border to prevent UI element spillover
        private const int windowBorderThickness = 2;

        public bool MessageSent { get; set; }

        // Create a new window with the title centered
        // the window is draggable by default
        public MessageLogWindow(int width, int height, string title) : base(width, height, title)
        {
            lines = new Queue<string>();

            // add the message console, reposition, enable the viewport, and add it to the window
            messageConsole = new SadConsole.Console(width - windowBorderThickness, maxLines)
            {
                Position = new Point(1, 1)
            };
            messageConsole.Surface.View = new Rectangle(0, 0, width - 1, height - windowBorderThickness);
            messageConsole.Surface.DefaultBackground = Color.Black;

            // create a scrollbar and attach it to an event handler, then add it to the Window
            messageScrollBar = new SadConsole.UI.Controls.ScrollBar(Orientation.Vertical, height - windowBorderThickness)
            {
                Position = new Point(messageConsole.Width + 1, messageConsole.Position.X),
                IsEnabled = false
            };
            FocusedMode = FocusBehavior.None;
            messageScrollBar.ValueChanged += MessageScrollBarValueChanged;
            Controls.Add(messageScrollBar);

            // enable mouse input
            //UseMouse = true;

            // Add the child consoles to the window
            Children.Add(messageConsole);
        }

        public void PrintMessage(string message, bool newLine = true)
        {
            lines.Enqueue(message);

            // when exceeding the max number of lines remove the oldest one
            if (lines.Count > maxLines)
            {
                lines.Dequeue();
            }

            // This here says that there will be a new line, so i don't need position the cursor:
            if (newLine)
                messageConsole.Cursor.Print(message).NewLine();
            else
                messageConsole.Cursor.Print(message);
            // unashamed code stealing to make the same message collapsible
            /*if (newMessage == _lastMessage)
            {
                this.Print(lastMessage.Length + offset, _yPos, $"x{++_messageCounter}");
            }
            else
            {
                this.Print(++_yPos, newMessage);
                _lastMessage = newMessage;
                _messageCounter = 1;
            }*/

            Show();
            MessageSent = true;
        }

        // Controls the position of the messagelog viewport
        // based on the scrollbar position using an event handler
        private void MessageScrollBarValueChanged(object? sender, EventArgs e)
        {
            messageConsole.Surface.View = new Rectangle(0,
                messageScrollBar.Value
                + windowBorderThickness,
                messageConsole.Width,
                messageConsole.Surface.View.Height);
        }

        public override void Update(TimeSpan time)
        {
            base.Update(time);
            //var focus = Game.Instance.FocusedScreenObjects;

            // Ensure that the scrollbar tracks the current position of the messageConsole.
            if (messageConsole.Surface.TimesShiftedUp != 0 ||
                messageConsole.Cursor.Position.Y >= messageConsole.Surface.View.Height + scrollBarCurrentPosition)
            {
                //enable the scrollbar once the messagelog has filled up with enough text to warrant scrolling
                messageScrollBar.IsEnabled = true;

                // Make sure we've never scrolled the entire size of the buffer
                if (scrollBarCurrentPosition < messageConsole.Height - messageConsole.Surface.View.Height)
                {
                    // Record how much we've scrolled to enable how far back the bar can see
                    scrollBarCurrentPosition += messageConsole.Surface.TimesShiftedUp != 0
                        ? messageConsole.Surface.TimesShiftedUp : 1;
                }

                // Determines the scrollbar's max vertical position
                // Thanks @Kaev for simplifying this math!
                messageScrollBar.Maximum = scrollBarCurrentPosition - windowBorderThickness;

                // This will follow the cursor since we move the render area in the event.
                messageScrollBar.Value = scrollBarCurrentPosition;

                // Reset the shift amount.
                messageConsole.Surface.TimesShiftedUp = 0;
            }
        }

        public void HideIfNoMessageThisTurn()
        {
            if (MessageSent)
                return;
            Hide();
        }

        public void Handle(AddMessageLog message)
        {
            if (message.PlayerCanSee)
                PrintMessage(message.Message);
        }

        public void Handle(HideMessageLogMessage message)
        {
            HideIfNoMessageThisTurn();
        }
    }
}