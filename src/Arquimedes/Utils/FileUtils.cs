using System.Collections.Concurrent;
using Arquimedes.Interfaces;

namespace Arquimedes.Utils
{
    public static class FileUtils
    {
        private static readonly string _appDomain = AppDomain.CurrentDomain.BaseDirectory;

        public static string[] GetFiles(string wildCard)
        {
            try
            {
                string originalWildCard = wildCard;

                string pattern = Path.GetFileName(originalWildCard);
                string realDir = originalWildCard[..^pattern.Length];

                // Get absolutepath
                string absPath = Path.GetFullPath(Path.Combine(_appDomain, realDir));

                return Directory.GetFiles(absPath, pattern, SearchOption.AllDirectories);
            }
            catch (Exception)
            {
                return [];
            }
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

            // List<List<T>> listTList = [];
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
                        // Locator.GetService<MagiLog>().Log($"Failed to load file {file}: {ex.Message}");
                        System.Console.WriteLine($"Something went wrong {ex}");
                        return;
                    }
                    // listTList.Add(tList);
                });
            }
            catch (System.Exception ex)
            {
                // Locator.GetService<MagiLog>().Log(ex);
                System.Console.WriteLine($"Something went wrong {ex}");
                throw;
            }
            return [.. result];
        }
    }
}
