using System;

namespace MagiRogue.Utils
{
    public class RandomNames
    {
        public static string SettlementNameGen()
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
            Name += consonants[GameLoop.GlobalRand.NextInt(consonants.Length + 1)].ToUpper();
            Name += vowels[GameLoop.GlobalRand.NextInt(vowels.Length + 1)];
            int b = 2; //b tells how many times a new letter has been added. It's 2 right now because the first two letters are already in the name.
            while (b < len)
            {
                Name += consonants[GameLoop.GlobalRand.NextInt(consonants.Length + 1)];
                b++;
                Name += vowels[GameLoop.GlobalRand.NextInt(vowels.Length + 1)];
                b++;
            }

            return Name;
        }

        internal static string RandomNames()
        {
            throw new NotImplementedException();
        }
    }
}