using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.Data.Serialization
{
    public class Language
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public List<List<string>> Words { get; set; }

        public Language()
        {
        }

        public List<Word> ReturnWords()
        {
            List<Word> words = new List<Word>();
            foreach (List<string> item in Words)
            {
                words.Add(new Word(item));
            }

            return words;
        }
    }

    public class Word
    {
        public string TranslatedWord { get; set; }
        public List<string> RealWord { get; set; } = new();

        public Word()
        {
        }

        public Word(List<string> words)
        {
            TranslatedWord = words[0];
            for (int i = 1; i < words.Count; i++)
            {
                RealWord.Add(words[i]);
            }
        }
    }
}