﻿using SadConsole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using MagiRogue.Data.Serialization;
using MagiRogue.System;

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
                Formatting = Formatting.Indented,
            };
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
            Serializer.Save(gameState, @$"{_folderName}\{saveName}\GameState", true);

            //var map = LoadGameState("GameState");
        }

        public static bool CheckIfThereIsSaveFile()
        {
            string[] files = Directory.GetDirectories(Path.Combine(dir, _folderName));

            return files.Length > 0;
        }

        public void SaveMapToSaveFolder(MapTemplate map, string fileName)
        {
            Serializer.Save(map, $@"{saveDir}\{fileName}", true);
        }

        public void SaveMapToSaveFolder(string map, string fileName)
        {
            Serializer.Save(map, $@"{saveDir}\{fileName}", true);
        }

        public void SaveMapToSaveFolder(MapTemplate map)
        {
            Serializer.Save(map, $@"{saveDir}\{map.MapId}", true);
        }

        public void SaveJsonToSaveFolder(string json)
        {
            Serializer.Save(json, $@"{saveDir}\Test", true);
        }

        public MapTemplate LoadMapFile(string fileName)
        {
            string path = $@"Saves\{savePathName}\{fileName}";
            return Serializer.Load<Map>(path, true);
        }

        public Universe LoadGameState(string fileName)
        {
            string path = $@"Saves\{savePathName}\{fileName}";
            return Serializer.Load<Universe>(path, true);
        }
    }
}