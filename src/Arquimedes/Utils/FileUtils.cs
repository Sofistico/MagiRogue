namespace Arquimedes.Utils
{
    public static class FileUtils
    {
        private static readonly string _appDomain = AppDomain.CurrentDomain.BaseDirectory;

        public static string[] GetFiles(string wildCard)
        {
            string originalWildCard = wildCard;

            string pattern = Path.GetFileName(originalWildCard);
            string realDir = originalWildCard[..^pattern.Length];

            // Get absolutepath
            string absPath = Path.GetFullPath(Path.Combine(_appDomain, realDir));

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
    }
}
