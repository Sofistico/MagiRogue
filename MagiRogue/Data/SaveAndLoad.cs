using SadConsole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using MagiRogue.Data.Serialization;
using MagiRogue.System;
using MagiRogue.Utils;

namespace MagiRogue.Data
{
    public class SaveAndLoad
    {
        private const string _folderName = "Saves";
        private static readonly string dir = AppDomain.CurrentDomain.BaseDirectory;
        private string saveDir;
        private string savePathName;
        private JsonSerializerSettings settings;

        public SaveAndLoad()
        {
            CreateNewSaveFolder();
            settings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                TypeNameHandling = TypeNameHandling.None,
            };
            Serializer.Settings = settings;
        }

        private static void CreateNewSaveFolder()
        {
            if (!Directory.Exists(dir + $@"\{_folderName}"))
            {
                Directory.CreateDirectory(_folderName);
            }
        }

        public void SaveGameToFolder(Universe gameState, string saveName)
        {
            savePathName = saveName;
            string path = dir + $@"{_folderName}\{saveName}";

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            saveDir = path;

            // TODO: see if there is a need for this code vs just loading it in one big file!
            //SaveMapToSaveFolder(gameState.Universe.WorldMap.AssocietatedMap, "WorldMap");
            /*for (int i = 0; i < gameState.Universe.AllChunks.Length; i++)
            {
                var chunk = gameState.Universe.AllChunks[i];
                if (chunk is null)
                    continue;
                for (int z = 0; z < chunk.LocalMaps.Length; z++)
                {
                    var map = chunk.LocalMaps[z];
                    if (map is not null)
                    {
                        SaveMapToSaveFolder(map);
                    }
                }
            }*/
            //Write(gameState);
            try
            {
                Serializer.Save(gameState, @$"{_folderName}\{saveName}\GameState", true);
            }
            catch (Exception)
            {
                Directory.Delete(Path.Combine(dir, _folderName, saveName), true);
                throw;
            }

            //var map = LoadGameState("GameState");
        }

        public void SaveJsonToSaveFolder(string json)
        {
            Serializer.Save(json, $@"{saveDir}\Test", true);
        }

        public static Universe LoadGame(string saveName)
        {
            string path = $@"Saves\{saveName}\GameState";
            return Serializer.Load<Universe>(path, true);
        }

        public static bool CheckIfStringIsValidSave(string saveName)
        {
            if (!string.IsNullOrEmpty(saveName))
            {
                string[] files = SaveUtils.GetSaveNameArray(SaveUtils.ReturnAllSaveFiles());

                return files.Any(s => saveName.Equals(s));
            }
            else
                return false;
        }
    }
}