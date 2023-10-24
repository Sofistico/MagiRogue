using Arquimedes.Enumerators;
using Diviner.Controls;
using MagusEngine;
using MagusEngine.Bus;
using MagusEngine.Bus.UiBus;
using MagusEngine.Core.Entities;
using MagusEngine.Core.Entities.Base;
using MagusEngine.Core.Entities.StarterScenarios;
using MagusEngine.Factory;
using MagusEngine.Services;
using MagusEngine.Systems;
using SadConsole;
using SadConsole.Instructions;
using SadConsole.UI.Controls;
using SadRogue.Primitives;
using Console = SadConsole.Console;

namespace Diviner.Windows
{
    public class CharacterCreationWindow : MagiBaseWindow
    {
        private Player player;
        private const string helpText = " WARNING! NO LONGER RELEVANT! SUBJECT TO CHANGE\n\rHere we begin the process of creating your player, there is much to be" +
            " done before you can create, you must first decide the 3 most important attributes that determine" +
            " what your character can and can't do. \n\nThese are the Body Stat(Used to determine various effects that" +
            " your body will have, like total hp, how likely you are to dogde or how likely you are to " +
            "recover from diseases). \n\nThe Mind Stat(Your general skill set, how likely you are to learn something new" +
            " how long you take to research new spells or train a new ability from zero as well as the cost)." +
            "\n\nThe Soul Stat(Your mp stat, how high that is your innate resistance before trainging as well a major" +
            " effect across the board in the spell casting portion). \n\nYou will have 120 points to distribute as you see" +
            " fit for your base stats.";
        private TextBox charName;
        private ListBox scenarioChooser;
        private ListBox raceChooser;
        private List<RadioButton> sexChooser;

        // for later, see https://discord.com/channels/501465397518925843/501465397518925850/1010610089939435712
        private readonly List<Console> consolePatches;
        private List<RadioButton> raceRadio;
        private Console basicConsole;
        private Console raceConsole;
        private Console scenarioConsole;

        public CharacterCreationWindow(int width, int height) : base(width, height, "Character Creation")
        {
            SetUpButtons(width);
            consolePatches = new();
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
                    Locator.GetService<MessageBusService>().SendMessage<StartGameMessage>(new(player));
            };
            helpButton.Click += (_, __) =>
            {
                // We want this to print on a sub region of the main console, so we'll create a sub
                // view and use that
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
                    TotalTimeToPrint = TimeSpan.FromSeconds(3), // 0.5 seconds per line of text
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
                    pop.Surface.View = new Rectangle(0, scrollBar.Value + WindowBorderThickness,
                        pop.Width, pop.ViewHeight);
                };
                window.Children.Add(pop);
                window.Controls.Add(scrollBar);
                Children.Add(window);
                pop.SadComponents.Add(typingInstruction);
                window.Show(true);
            };
            Point namePoint = new Point((Width / 2) - 9, (Height / 2) + 5);
            charName = new(25)
            {
                Position = namePoint,
                IsVisible = false
            };
            const string back = "Go Back";
            MagiButton goBack = new(back.Length + 2)
            {
                Text = back,
                Position = new Point(width / 2, 27)
            };
            goBack.Click += (_, __) =>
            {
                Hide();
                Locator.GetService<MessageBusService>().SendMessage<ShowMainMenuMessage>();
            };

            Controls.Add(charName);
            SetupSelectionButtons(beginGame,
                helpButton,
                goBack);
            SetupCharCreationButtons();
        }

        private void SetupCharCreationButtons()
        {
            Point scenarioPoint = new SadRogue.Primitives.Point(Width / 4 - 15, Height / 2 - 10);
            scenarioChooser = new ListBox(30, 10)
            {
                Position = scenarioPoint,
                DrawBorder = true,
            };
            foreach (var item in DataManager.ListOfScenarios)
            {
                scenarioChooser.Items.Add(item);
            }

            Controls.Add(scenarioChooser);
            const string scenarioText = "Select your starting scenario:";
            PrintUpFromPosition(scenarioPoint, scenarioText);

            sexChooser = new List<RadioButton>();
            Point starterPosSexRadio = new SadRogue.Primitives.Point(Width / 2 - 10, Height / 2 - 10);
            foreach (var item in Enum.GetValues(typeof(Sex)))
            {
                string text = item.ToString();
                RadioButton radio = new RadioButton(text.Length + 4, 1)
                {
                    Position = starterPosSexRadio,
                    Text = text,
                    IsVisible = false
                };
                radio.Click += Radio_Click;
                sexChooser.Add(radio);
            }

            for (int i = 1; i < sexChooser.Count; i++)
            {
                var current = sexChooser[i];
                var previous = sexChooser[i - 1];
                Controls.Add(previous);
                current.PlaceRelativeTo(previous, Direction.Types.Right);
            }

            Point racePoint = new SadRogue.Primitives.Point(Width / 2 + 20, Height / 2 - 10);
            raceChooser = new ListBox(20, 10)
            {
                Position = racePoint,
                DrawBorder = true,
                IsVisible = false
            };

            Controls.Add(raceChooser);

            scenarioChooser.SelectedItemChanged += (_, __) =>
            {
                PrintUpFromPosition(starterPosSexRadio, "Choose your sex:");
                foreach (var item in sexChooser)
                {
                    item.IsVisible = true;
                }
            };
        }

        private void Radio_Click(object? sender, EventArgs e)
        {
            PrintUpFromPosition(raceChooser.Position, "Now, choose from the available races:");
            raceChooser.IsVisible = true;
            raceChooser.Items.Clear();
            Scenario scenario = (Scenario)scenarioChooser.SelectedItem;
            foreach (string race in scenario.RacesAllowed)
            {
                raceChooser.Items.Add(DataManager.QueryRaceInData(race));
            }

            raceChooser.SelectedItemChanged += (_, __) =>
            {
                charName.IsVisible = true;
                PrintUpFromPosition(charName.Position, "Finally, choose your Name:");
            };
        }

        private Player CreatePlayer()
        {
            if (string.IsNullOrEmpty(charName.Text))
            {
                /*#if DEBUG
                                player = Player.TestPlayer();
                                return player;
                #endif*/

                ShowError("You need to insert a name!");
                return null;
            }
            if (!sexChooser.Exists(i => i.IsSelected))
            {
                ShowError("You need to select a sex!");
                return null;
            }
            if (scenarioChooser.SelectedItem is null)
            {
                ShowError("You need to select a scenario!");
                return null;
            }
            if (raceChooser.SelectedItem is null)
            {
                ShowError("You need to select a race!");
                return null;
            }

            var scenario = (Scenario)scenarioChooser.SelectedItem;
            var race = (Race)raceChooser.SelectedItem;
            var sex = sexChooser.Find(i => i.IsSelected);
            player = EntityFactory.PlayerCreatorFromZero(Point.None,
                race.Id,
                charName.Text,
                Enum.Parse<Sex>(sex.Text),
                scenario.Id);

            return player;
        }

        private static void ShowError(string textError)
        {
            PopWindow error = new PopWindow("Error");
            error.Surface.Clear();
            error.Surface.Print(1, 1, textError);
            error.Show(true);
        }

        public override string ToString() => "Character Creation Screen";
    }
}