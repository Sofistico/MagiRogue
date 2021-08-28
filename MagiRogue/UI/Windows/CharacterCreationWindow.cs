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

namespace MagiRogue.UI.Windows
{
    public class CharacterCreationWindow : MagiBaseWindow
    {
        private Player player;
        private const string beginText = "\n\rHere we begin the process of creating your player, there is much to be" +
            " done before you can create, you must first decide the 3 most important attributes that determine" +
            " what your character can and can't do. \n\nThese are the Body Stat(Used to determine various effects that" +
            " your body will have, like total hp, how likely you are to dogde or how likely you are to " +
            "recover from diseases). \n\nThe Mind Stat(Your general skill set, how likely you are to learn something new" +
            " how long you take to research new spells or train a new ability from zero as well as the cost)." +
            "\n\nThe Soul Stat(Your mp stat, how high that is your innate resistance before trainging as well a major" +
            " effect across the board in the spell casting portion). \n\nYou will have 120 points to distribute as you see" +
            " fit for your base stats." +
            "\n\n There is also some stats more related to other things and skills, these will be explained next page.";

        public CharacterCreationWindow(int width, int height) : base(width, height, "Character Creation")
        {
            Surface.Clear();
            Cursor.Position = new Point(1, 1);
            Cursor.UseLinuxLineEndings = true;
            var typingInstruction = new DrawString(ColoredString.Parse(beginText))
            {
                TotalTimeToPrint = 8 // 0.5 seconds per line of text
            };

            Cursor.IsEnabled = false;
            Cursor.IsVisible = true;
            typingInstruction.Cursor = Cursor;

            SadComponents.Add(typingInstruction);

            // Cursor.Print(string.Join("\n", beginText));

            string begin = "Next Page";
            MagiButton beginGame = new MagiButton(begin.Length + 2)
            {
                Position = new Point(width / 2, 25),
                Text = begin
            };
            beginGame.Click += (_, __) => { GameLoop.UIManager.StartGame(player); };

            // We want this to print on a sub region of the main console, so we'll create a sub view and use that

            Controls.Add(beginGame);
        }

        public override string ToString() => "Character Creation Screen";
    }
}