using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.Utils
{
    internal class FileUtils
    {
        private static readonly string _appDomain = AppDomain.CurrentDomain.BaseDirectory;

        public static string[] GetFiles(string wildCard)
        {
            string originalWildCard = wildCard;

            string pattern = Path.GetFileName(originalWildCard);
            string realDir = originalWildCard[..^pattern.Length];

            // Get absolutepath
            string absPath = Path.GetFullPath(Path.Combine(_appDomain, realDir));

            string[] files = Directory.GetFiles(absPath, pattern, SearchOption.TopDirectoryOnly);
            return files;
        }
    }
}