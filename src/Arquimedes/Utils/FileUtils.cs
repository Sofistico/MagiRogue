using System.Collections.Concurrent;
using Arquimedes.Interfaces;

namespace Arquimedes.Utils
{
    public static class FileUtils
    {
        private static readonly string _appDomain = AppDomain.CurrentDomain.BaseDirectory;

        public static string[] GetFiles(string wildCard)
        {
            // Normalize the wildcard path to use correct separators
            string originalWildCard = wildCard;

            string pattern = Path.GetFileName(originalWildCard);
            string realDir = originalWildCard[..^pattern.Length];

            // Get absolutepath
            string absPath = Path.GetFullPath(Path.Combine(_appDomain, realDir)).Replace('\\', Path.DirectorySeparatorChar);

            return Directory.GetFiles(absPath, pattern, SearchOption.AllDirectories);
        }

        public static string? GetAllTextFromFile(FileInfo file)
        {
            try
            {
                using var fileStream = file.Open(FileMode.Open, FileAccess.Read, FileShare.Read);
                using var streamReader = new StreamReader(fileStream);
                return streamReader.ReadToEnd();
            }
            catch (IOException)
            {
                throw;
            }
        }

        public static Dictionary<string, T> GetSourceTreeDict<T>(string wildCard) where T : IJsonKey
        {
            return GetSourceTreeList<T>(wildCard).ToDictionary(static val => val.Id);
        }

        public static List<T> GetSourceTreeList<T>(string wildCard)
        {
            string[] files = FileUtils.GetFiles(wildCard);

            ConcurrentBag<T> result = [];

            try
            {
                Parallel.ForEach(files, file =>
                        {
                        try
                        {
                        foreach (T? item in JsonUtils.JsonDeseralize<List<T>>(file)!)
                        {
                        result.Add(item);
                        }

                        }
                        catch (System.Exception ex)
                        {
                        System.Console.WriteLine($"Something went wrong {ex}");
                        return;
                        }
                        });
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine($"Something went wrong {ex}");
                throw;
            }
            return [.. result];
        }
    }
}
