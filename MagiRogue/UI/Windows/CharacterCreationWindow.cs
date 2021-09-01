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
        private const int startPoints = 60;
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

            SetupSelectionButtons(beginGame,
                helpButton);

            // Body Stat
            AddToDictionary(SetPlusAndMinusButtons(Width / 2 + 10, Height / 2 - 10, StatEnum.Body));
            AddToDictionary(SetPlusAndMinusButtons(Width / 2 + 10, Height / 2 - 9, StatEnum.Mind));
            AddToDictionary(SetPlusAndMinusButtons(Width / 2 + 10, Height / 2 - 8, StatEnum.Soul));
            AddToDictionary(SetPlusAndMinusButtons(Width / 2 + 10, Height / 2 - 7, StatEnum.Str));
            AddToDictionary(SetPlusAndMinusButtons(Width / 2 + 10, Height / 2 - 6, StatEnum.Pre));
            AddToDictionary(SetPlusAndMinusButtons(Width / 2 + 15, Height / 2 - 3, StatEnum.ShapSkill));
            AddToDictionary(SetPlusAndMinusButtons(Width / 2 + 15, Height / 2 - 2, StatEnum.Attack));
            AddToDictionary(SetPlusAndMinusButtons(Width / 2 + 15, Height / 2 - 1, StatEnum.Defense));
        }

        private MagiButton[] SetPlusAndMinusButtons(int x, int y, StatEnum statEnum)
        {
            MagiButton plus = new("+".Length + 2)
            {
                Text = "+",
                Position = new Point(x, y)
            };
            plus.Click += (_, __) =>
            {
                CalculatePoints(statEnum);
            };
            MagiButton minus = new("-".Length + 2)
            {
                Text = "-",
                Position = new Point(x + 4, y)
            };
            minus.Click += (_, __) =>
            {
                SubtractPoints(statEnum);
            };

            return new MagiButton[] { plus, minus };
        }

        private void SubtractPoints(StatEnum statEnum)
        {
            if (totalSpent < startPoints && totalSpent > 0)
            {
                switch (statEnum)
                {
                    case StatEnum.Body:
                        if (bodyStat > 0 && RemoveTotalSpent(bodyStat))
                            bodyStat--;
                        break;

                    case StatEnum.Mind:
                        if (mindStat > 0 && RemoveTotalSpent(mindStat))
                            mindStat--;
                        break;

                    case StatEnum.Soul:
                        if (soulStat > 0 && RemoveTotalSpent(soulStat))
                            soulStat--;
                        break;

                    case StatEnum.Str:
                        if (str > 0 && RemoveTotalSpent(str))
                            str--;
                        break;

                    case StatEnum.Pre:
                        if (precision > 0 && RemoveTotalSpent(precision))
                            precision--;
                        break;

                    case StatEnum.ShapSkill:
                        if (shapSkill > 0 && RemoveTotalSpent(shapSkill))
                            shapSkill--;
                        break;

                    case StatEnum.Attack:
                        if (attackSkill > 0 && RemoveTotalSpent(attackSkill))
                            attackSkill--;
                        break;

                    case StatEnum.Defense:
                        if (defenseSkill > 0 && RemoveTotalSpent(defenseSkill))
                            defenseSkill--;
                        break;

                    default:
                        break;
                }
            }
        }

        private void CalculatePoints(StatEnum statEnum)
        {
            if (totalSpent <= startPoints)
            {
                switch (statEnum)
                {
                    case StatEnum.Body:
                        if (bodyStat < 5)
                        {
                            if (CalculateTotalSpent(bodyStat))
                                bodyStat++;
                        }
                        break;

                    case StatEnum.Mind:
                        if (mindStat < 5)
                        {
                            if (CalculateTotalSpent(mindStat))
                                mindStat++;
                        }
                        break;

                    case StatEnum.Soul:
                        if (soulStat < 5)
                        {
                            if (CalculateTotalSpent(soulStat))
                                soulStat++;
                        }
                        break;

                    case StatEnum.Str:
                        if (str < 12)
                        {
                            if (CalculateTotalSpent(str))
                                str++;
                        }
                        break;

                    case StatEnum.Pre:
                        if (precision < 12)
                        {
                            if (CalculateTotalSpent(precision))
                                precision++;
                        }
                        break;

                    case StatEnum.ShapSkill:
                        if (shapSkill < 12)
                        {
                            if (CalculateTotalSpent(shapSkill))
                                shapSkill++;
                        }
                        break;

                    case StatEnum.Attack:
                        if (attackSkill < 12)
                        {
                            if (CalculateTotalSpent(attackSkill))
                                attackSkill++;
                        }
                        break;

                    case StatEnum.Defense:
                        if (defenseSkill < 12)
                        {
                            if (CalculateTotalSpent(defenseSkill))
                                defenseSkill++;
                        }
                        break;

                    default:
                        break;
                }
            }
        }

        private bool CalculateTotalSpent(int stat)
        {
            if (totalSpent < startPoints)
            {
                totalSpent += stat + 1 * 2;
                if (totalSpent > startPoints)
                {
                    totalSpent -= stat + 1 * 2;
                    return false;
                }
                return true;
            }
            return false;
        }

        private bool RemoveTotalSpent(int stat)
        {
            if (totalSpent > 0)
            {
                // This -1 is to represent that since the stat will go down, it needs to account here as well.
                totalSpent -= stat + 1 * 2 - 1;
                return true;
            }
            return false;
        }

        private enum StatEnum
        {
            Body,
            Mind,
            Soul,
            Str,
            Pre,
            ShapSkill,
            Attack,
            Defense
        }

        public override void Update(TimeSpan time)
        {
            if (!GameLoop.UIManager.MainMenu.GameStarted)
            {
                Surface.Print(Width / 2 - 9, Height / 2 - 12, $"Points: {totalSpent} / {startPoints}");
                Surface.Print(Width / 2 - 9, Height / 2 - 11, "Stats: each is 1 + n * 2, n being the current number of points");
                Surface.Print(Width / 2 - 9, Height / 2 - 10, $"Body Stat : {bodyStat} / 5");
                Surface.Print(Width / 2 - 9, Height / 2 - 9, $"Mind Stat : {mindStat} / 5");
                Surface.Print(Width / 2 - 9, Height / 2 - 8, $"Soul Stat : {soulStat} / 5");
                Surface.Print(Width / 2 - 9, Height / 2 - 7, $"Strength: {str} / 12");
                Surface.Print(Width / 2 - 9, Height / 2 - 6, $"Precision: {precision} / 12");
                Surface.Print(Width / 2 - 9, Height / 2 - 4, "Skills: each is 1 + n * 2");
                Surface.Print(Width / 2 - 9, Height / 2 - 3, $"Shaping Skills : {shapSkill} / 12");
                Surface.Print(Width / 2 - 9, Height / 2 - 2, $"Attack Skill: {attackSkill} / 12");
                Surface.Print(Width / 2 - 9, Height / 2 - 1, $"Defense Skill: {defenseSkill} / 12");
            }
            base.Update(time);
        }

        public override string ToString() => "Character Creation Screen";
    }
}