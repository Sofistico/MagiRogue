using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace MagiRogue.Data
{
    public class SaveAndLoad
    {
        private const string _folderName = "Saves";
        private string dir = AppDomain.CurrentDomain.BaseDirectory;

        public SaveAndLoad()
        {
            // empty
        }

        public void CreateNewSaveFolder()
        {
            if (!Directory.Exists(dir + $@"\{_folderName}"))
            {
                Directory.CreateDirectory(_folderName);
            }
        }

        public void SaveGameToFolder(GameState gameState, string saveName)
        {
            string path = dir + $@"\{_folderName}\{saveName}";

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
    }
}