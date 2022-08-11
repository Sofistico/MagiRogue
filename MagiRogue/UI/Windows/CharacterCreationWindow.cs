using MagiRogue.Data;
using MagiRogue.Entities;
using MagiRogue.Entities.StarterScenarios;
using MagiRogue.UI.Controls;
using MagiRogue.Utils;
using SadConsole;
using SadConsole.Instructions;
using SadConsole.UI.Controls;
using SadRogue.Primitives;
using System;
using Console = SadConsole.Console;

namespace MagiRogue.UI.Windows
{
    public class CharacterCreationWindow : MagiBaseWindow
    {
        private Player player;
        private const string helpText = "\n\rHere we begin the process of creating your player, there is much to be" +
            " done before you can create, you must first decide the 3 most important attributes that determine" +
            " what your character can and can't do. \n\nThese are the Body Stat(Used to determine various effects that" +
            " your body will have, like total hp, how likely you are to dogde or how likely you are to " +
            "recover from diseases). \n\nThe Mind Stat(Your general skill set, how likely you are to learn something new" +
            " how long you take to research new spells or train a new ability from zero as well as the cost)." +
            "\n\nThe Soul Stat(Your mp stat, how high that is your innate resistance before trainging as well a major" +
            " effect across the board in the spell casting portion). \n\nYou will have 120 points to distribute as you see" +
            " fit for your base stats.";
        private TextBox charName;
        private Scenario chosenScenario;

        public CharacterCreationWindow(int width, int height) : base(width, height, "Character Creation")
        {
            SetUpButtons(width);
        }

        private void SetUpButtons(int width)
        {
            string begin = "Begin!";
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
                player = CreatePlayer();
                if (player is not null)
                    GameLoop.UIManager.StartGame(player);
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
                var typingInstruction = new DrawString(ColoredString.Parser.Parse(helpText))
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

            charName = new(25)
            {
                Position = new Point(Width / 2 - 9, Height / 2 + 5)
            };
            charName.IsVisible = true;
            Controls.Add(charName);
            SetupSelectionButtons(beginGame,
                helpButton);

            //// Body Stat
            //AddToDictionary(SetPlusAndMinusButtons(Width / 2 + 10, Height / 2 - 10, Stats.Body));
            //AddToDictionary(SetPlusAndMinusButtons(Width / 2 + 10, Height / 2 - 9, Stats.Mind));
            //AddToDictionary(SetPlusAndMinusButtons(Width / 2 + 10, Height / 2 - 8, Stats.Soul));
            //AddToDictionary(SetPlusAndMinusButtons(Width / 2 + 10, Height / 2 - 7, Stats.Str));
            //AddToDictionary(SetPlusAndMinusButtons(Width / 2 + 10, Height / 2 - 6, Stats.Pre));
            //AddToDictionary(SetPlusAndMinusButtons(Width / 2 + 15, Height / 2 - 1, Stats.ShapSkill));
            //AddToDictionary(SetPlusAndMinusButtons(Width / 2 + 15, Height / 2, Stats.Attack));
            //AddToDictionary(SetPlusAndMinusButtons(Width / 2 + 15, Height / 2 + 1, Stats.Defense));
        }

        private Player CreatePlayer()
        {
            if (charName.Text != "")
                player = new Player(charName.Text, Color.White, Color.Black, Point.None);
            else
            {
                /*#if DEBUG
                                player = Player.TestPlayer();
                                return player;
                #endif*/

                PopWindow error = new PopWindow("Error");
                error.Surface.Clear();
                error.Surface.Print(1, 1, "You need to insert a name!");
                error.Show(true);
                return null;
            }
            //int stamina = GoRogue.Random.GlobalRandom.DefaultRNG.NextInt(5, bodyStat + 10) + 3;
            //int mana = GoRogue.Random.GlobalRandom.DefaultRNG.NextInt(8, soulStat + 10);
            double speed = MathMagi.Round(GoRogue.Random.GlobalRandom
                .DefaultRNG.NextDouble(0.9, 1.1));

            //player.Magic.ShapingSkill = shapSkill;
            // The first spell any mage learns is magic missile, it's the first proper combat spell that doens't require hours of practice
            // to work,
            // but if the player doens't have enough shaping skills for the spell, the player would play as an failed mage.
            player.Magic.KnowSpells.Add(DataManager.QuerySpellInData("magic_missile"));
            player.Volume = GoRogue.Random.GlobalRandom.DefaultRNG.NextInt(160, 200);
            player.Weight = GoRogue.Random.GlobalRandom.DefaultRNG.NextInt(50, 90);

            return player;
        }

        public override string ToString() => "Character Creation Screen";
    }
}