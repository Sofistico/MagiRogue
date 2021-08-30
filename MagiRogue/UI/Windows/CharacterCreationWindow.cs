using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MagiRogue.Entities;
using MagiRogue.Data;
using SadConsole;
using SadConsole.Components;
using SadConsole.Input;
using MagiRogue.UI.Controls;
using SadRogue.Primitives;
using SadConsole.Instructions;
using SadConsole.UI.Controls;
using Console = SadConsole.Console;

namespace MagiRogue.UI.Windows
{
    public class CharacterCreationWindow : MagiBaseWindow
    {
        private Player player;
        private const int startPoints = 120;
        private int totalSpent = 0;
        private int bodyStat;
        private int mindStat;
        private int soulStat;
        private int shapSkill;
        private int attackSkill;
        private int defenseSkill;
        private int str;
        private int precision;
        private const string helpText = "\n\rHere we begin the process of creating your player, there is much to be" +
            " done before you can create, you must first decide the 3 most important attributes that determine" +
            " what your character can and can't do. \n\nThese are the Body Stat(Used to determine various effects that" +
            " your body will have, like total hp, how likely you are to dogde or how likely you are to " +
            "recover from diseases). \n\nThe Mind Stat(Your general skill set, how likely you are to learn something new" +
            " how long you take to research new spells or train a new ability from zero as well as the cost)." +
            "\n\nThe Soul Stat(Your mp stat, how high that is your innate resistance before trainging as well a major" +
            " effect across the board in the spell casting portion). \n\nYou will have 120 points to distribute as you see" +
            " fit for your base stats.";

        public CharacterCreationWindow(int width, int height) : base(width, height, "Character Creation")
        {
            SetUpButtons(width, height);
        }

        private void SetUpButtons(int width, int height)
        {
            string begin = "Next Page";
            MagiButton beginGame = new(begin.Length + 2)
            {
                Position = new Point(width / 2, 25),
                Text = begin
            };
            string help = "Help";
            MagiButton helpButton = new(help.Length + 2)
            {
                Text = help,
                Position = new Point(width / 2, 26)
            };
            beginGame.Click += (_, __) =>
            {
                GameLoop.UIManager.StartGame(new Player(Color.White, Color.Black, Point.None));
            };
            helpButton.Click += (_, __) =>
            {
                // We want this to print on a sub region of the main console, so we'll create a sub view and use that
                PopWindow window = new("Help Screen");
                Console pop = new(window.Width - 3, window.Height - WindowBorderThickness,
                    window.Width, Height);
                pop.Position = new Point(1, 1);
                pop.Cursor.Position = new Point(1, 1);
                pop.Cursor.IsEnabled = false;
                pop.Cursor.IsVisible = false;
                pop.Cursor.UseLinuxLineEndings = true;
                var typingInstruction = new DrawString(ColoredString.Parse(helpText))
                {
                    TotalTimeToPrint = 5, // 0.5 seconds per line of text
                    RemoveOnFinished = true
                };

                pop.Cursor.IsEnabled = false;
                pop.Cursor.IsVisible = true;
                typingInstruction.Cursor = pop.Cursor;
                ScrollBar scrollBar = new ScrollBar(Orientation.Vertical, window.Height - WindowBorderThickness)
                {
                    Position = new Point(window.Width - WindowBorderThickness, pop.Position.X)
                };
                // It's bugged, need to fix
                scrollBar.ValueChanged += (_, __) =>
                {
                    pop.View = new Rectangle(0, scrollBar.Value + WindowBorderThickness,
                        pop.Width, pop.View.Height);
                };
                window.Children.Add(pop);
                window.Controls.Add(scrollBar);
                Children.Add(window);
                pop.SadComponents.Add(typingInstruction);
                window.Show(true);
            };

            MagiButton plusButton = new MagiButton("+".Length + 2)
            {
                Text = "+",
                Position = new Point(Width / 2 + 10, Height / 2 - 10)
            };
            plusButton.Click += (_, __) =>
            {
                // Fuck me
            };
            MagiButton minusButton = new MagiButton("-".Length + 2)
            {
                Text = "-",
                Position = new Point(Width / 2 + 14, Height / 2 - 10)
            };

            SetupSelectionButtons(beginGame, helpButton, plusButton, minusButton);
        }

        private void PlayerCreate()
        {
        }

        public override void Update(TimeSpan time)
        {
            if (!GameLoop.UIManager.MainMenu.GameStarted)
            {
                Surface.Print(Width / 2 - 9, Height / 2 - 11, "Stats:");
                Surface.Print(Width / 2 - 9, Height / 2 - 10, $"Body Stat : {bodyStat} / 20");
                Surface.Print(Width / 2 - 9, Height / 2 - 9, $"Mind Stat : {mindStat} / 20");
                Surface.Print(Width / 2 - 9, Height / 2 - 8, $"Soul Stat : {soulStat} / 20");
                Surface.Print(Width / 2 - 9, Height / 2 - 7, $"Strength: {str}");
                Surface.Print(Width / 2 - 9, Height / 2 - 6, $"Precision: {precision}");
                Surface.Print(Width / 2 - 9, Height / 2 - 4, "Skills:");
                Surface.Print(Width / 2 - 9, Height / 2 - 3, $"Shaping Skills : {shapSkill}");
                Surface.Print(Width / 2 - 9, Height / 2 - 2, $"Attack Skill: {attackSkill}");
                Surface.Print(Width / 2 - 9, Height / 2 - 1, $"Defense Skill: {defenseSkill}");
            }
            base.Update(time);
        }

        public override string ToString() => "Character Creation Screen";
    }
}