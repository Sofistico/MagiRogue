using System;
using System.IO;

namespace MagiRogue.Utils
{
    public class SaveUtils
    {
        private const string _folderName = "Saves";
        private static readonly string dir = AppDomain.CurrentDomain.BaseDirectory;

        /// <summary>
        /// Gets the save name from the path
        /// </summary>
        /// <param name="savePath"></param>
        /// <returns></returns>
        public static string GetSaveName(string savePath)
        {
            string[] arrayString = savePath.Split('\\');
            return arrayString[^1];
        }

        public static string[] GetSaveNameArray(string[] saves)
        {
            string[] array = new string[saves.Length];
            for (int i = 0; i < saves.Length; i++)
            {
                array[i] = GetSaveName(saves[i]);
            }
            return array;
        }

        public static bool CheckIfThereIsSaveFile()
        {
            string[] files;
            try
            {
                files = Directory.GetDirectories(Path.Combine(dir, _folderName));
            }
            catch (IOException)
            {
                return false;
            }

            return files.Length > 0;
        }

        public static string[] ReturnAllSaveFiles() => Directory.GetDirectories(Path.Combine(dir, _folderName));
    }
}