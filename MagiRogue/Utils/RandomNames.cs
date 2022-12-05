using MagiRogue.Utils.Extensions;
using System;
using System.Text;

namespace MagiRogue.Utils
{
    public class RandomNames
    {
        public static string SiteNameGen()
        {
            string[] townNames = new string[] {"Blencathra",
                                    "Bellmoral",
                                    "Todmorden",
                                    "Larkinge",
                                    "Garmsby",
                                    "Three Streams",
                                    "Lullin",
                                    "Darkwell",
                                    "Portsmouth",
                                    "Ilfracombe",
                                    "Clacton",
                                    "Threlkeld",
                                    "Quan Ma",
                                    "Culcheth",
                                    "Aramore",
                                    "Fanfoss",
                                    "Damerel",
                                    "Perthlochry",
                                    "Fernsworth",
                                    "Calcherth",
                                    "Emall",
                                    "Marnmouth",
                                    "Taewe",
                                    "Mossley",
                                    "Whaelrdrake",
                                    "Ashbourne",
                                    "Pendle",
                                    "Alnwick",
                                    "Farncombe",
                                    "Porthaethwy",
                                    "Glanyrafon",
                                    "Peterbrugh",
                                    "Colchester",
                                    "Kara's Vale",
                                    "Aberuthven",
                                    "Alnwick",
                                    "Hythe",
                                    "Malrton",
                                    "Wombourne",
                                    "Silverkeep",
                                    "Moonbright",
                                    "Drumchapel",
                                    "Auchenshuggle",
                                    "Fallholt",
                                    "Aeberuthey",
                                    "Sutton",
                                    "Garen's Well",
                                    "Middlesborough",
                                    "Wolfpine",
                                    "Holbeck",
                                    "Cesterfield",
                                    "Caelfall",
                                    "Larnwick",
                                    "Haerndean",
                                    "Blencathra",
                                    "Broken Shield",
                                    "Arkmunster",
                                    "Macclesfield",
                                    "Damerel",
                                    "Stathmore" };

            return townNames[GameLoop.GlobalRand.NextInt(townNames.Length)];
        }

        public static string GiberishName(int len)
        {
            string[] consonants = { "b", "c", "d", "f", "g", "h", "j", "k", "l", "m", "l", "n", "p", "q", "r", "s", "sh", "zh", "t", "v", "w", "x" };
            string[] vowels = { "a", "e", "i", "o", "u", "ae", "y" };
            string Name = "";
            Name += consonants[GameLoop.GlobalRand.NextInt(consonants.Length)].ToUpper();
            Name += vowels[GameLoop.GlobalRand.NextInt(vowels.Length)];
            int b = 2; //b tells how many times a new letter has been added. It's 2 right now because the first two letters are already in the name.
            while (b < len)
            {
                Name += consonants[GameLoop.GlobalRand.NextInt(consonants.Length)];
                b++;
                Name += vowels[GameLoop.GlobalRand.NextInt(vowels.Length)];
                b++;
            }

            return Name;
        }

        public static string GiberishFullName(int firstNameLen, int lastNameLen)
        {
            StringBuilder str = new(GiberishName(firstNameLen));
            str.Append(' ');
            str.Append(GiberishName(lastNameLen));
            return str.ToString();
        }

        public static string RandomNamesFromLanguage(Data.Serialization.Language language)
        {
            var words = language.ReturnWords();
            var firstName = words.GetRandomItemFromList();
            var lastName = words.GetRandomItemFromList();

            var str = new StringBuilder();
            str.Append(firstName.TranslatedWord.FirstLetterUpper());
            str.Append(' ');
            str.Append(lastName.TranslatedWord.FirstLetterUpper());

            return str.ToString();
        }

        public static string RandomNamesFromLanguage(string languageId)
        {
            return RandomNamesFromLanguage(Data.DataManager.QueryLanguageInData(languageId));
        }

        public static string RandomNamesFromRandomLanguage()
            => RandomNamesFromLanguage(Data.DataManager.ListOfLanguages.GetRandomItemFromList());
    }
}