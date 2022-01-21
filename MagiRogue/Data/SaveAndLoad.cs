using SadConsole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using MagiRogue.Data.Serialization;

namespace MagiRogue.Data
{
    public class SaveAndLoad
    {
        private const string _folderName = "Saves";
        private readonly string dir = AppDomain.CurrentDomain.BaseDirectory;
        private string saveDir;
        private string savePathName;

        public SaveAndLoad()
        {
            CreateNewSaveFolder();
            Serializer.Settings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented,
            };
        }

        private void CreateNewSaveFolder()
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
            saveDir = path;
            savePathName = saveName;

            SaveMapToSaveFolder(gameState.Universe.WorldMap.AssocietatedMap, "WorldMap");
            for (int i = 0; i < gameState.Universe.AllChunks.Length; i++)
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
            }
        }

        public void SaveMapToSaveFolder(MapTemplate map, string fileName)
        {
            Serializer.Save(map, $@"Saves\TestFile\{fileName}", true);
            //Serializer.Save(new SadConsole.Console(10, 10), @"Saves\TestConsole", true);
        }

        public void SaveMapToSaveFolder(MapTemplate map)
        {
            Serializer.Save(map, $"{map.MapId}", true);
        }
    }
}