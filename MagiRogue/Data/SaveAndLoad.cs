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
    /// <summary>
    /// This class takes care of the saving and loading of save files
    /// </summary>
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

        /// <summary>
        /// Creates a new save game folder, used in the initialization of the method
        /// </summary>
        private static void CreateNewSaveFolder()
        {
            if (!Directory.Exists(dir + $@"\{_folderName}"))
            {
                Directory.CreateDirectory(_folderName);
            }
        }

        /// <summary>
        /// Saves the game to the save folder under the saveName provided by the parameter
        /// </summary>
        /// <param name="gameState"></param>
        /// <param name="saveName"></param>
        public void SaveGameToFolder(Universe gameState, string saveName)
        {
            CreateSaveNameFolder(saveName);

            SaveChuncksToFile(gameState.AllChunks);

            try
            {
                Serializer.Save(gameState, @$"{_folderName}\{saveName}\GameState.mr", true);
            }
            catch (Exception)
            {
                Directory.Delete(Path.Combine(dir, _folderName, saveName), true);
                throw;
            }
        }

        /// <summary>
        /// creates the save name folder
        /// </summary>
        /// <param name="saveName"></param>
        private void CreateSaveNameFolder(string saveName)
        {
            if (savePathName.Equals(saveName)) return;
            savePathName = saveName;
            string path = dir + $@"{_folderName}\{saveName}";

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            saveDir = path;
        }

        /// <summary>
        /// Saves an array of chunks to a file, throws an exception
        /// </summary>
        /// <param name="chunks"></param>
        /// <exception cref="Exception"></exception>
        private void SaveChuncksToFile(RegionChunk[] chunks)
        {
            try
            {
                List<RegionChunk> newChunks = new List<RegionChunk>();
                int count = chunks.Length;
                for (int i = 0; i < count; i++)
                {
                    if (i % 1000 == 0 && (i != 0 || i == chunks.Length))
                    {
                        if (newChunks.Count != 0)
                        {
                            Serializer.Save(newChunks, @$"{_folderName}\{savePathName}\Chunks_{i / 1000}.mr", true);
                            newChunks.Clear();
                        }
                    }
                    else
                    {
                        if (chunks[i] is not null)
                            newChunks.Add(chunks[i]);
                    }
                }
            }
            catch (Exception)
            {
                throw new Exception("Failed to save chunk to a file!");
            }
        }

        /// <summary>
        /// Test method, in the future will be deleted
        /// </summary>
        /// <param name="json"></param>
        public void SaveJsonToSaveFolder(string json)
        {
            Serializer.Save(json, $@"{saveDir}\Test", true);
        }

        /// <summary>
        /// Loads the game provided by the save name
        /// </summary>
        /// <param name="saveName"></param>
        /// <returns></returns>
        public static Universe LoadGame(string saveName)
        {
            string path = $@"Saves\{saveName}\GameState.mr";
            Universe uni = Serializer.Load<Universe>(path, true);
            if (uni.CurrentChunk != null)
            {
                uni.AllChunks = LoadChunks(saveName,
                    uni.CurrentChunk.ToIndex(uni.WorldMap.AssocietatedMap.Width));
            }
            else
            {
                LoadChunks(saveName, Point.ToIndex(uni.Player.Position.X,
                    uni.Player.Position.Y,
                    uni.WorldMap.AssocietatedMap.Width));
            }
            return uni;
        }

        /// <summary>
        /// Load all chunks in the file, should be between 1000 chunks per file
        /// </summary>
        /// <param name="saveName"></param>
        /// <param name="currentChunkIndex"></param>
        /// <returns></returns>
        private static RegionChunk[] LoadChunks(string saveName, int currentChunkIndex)
        {
            // need to remake, so that it will only load the relevant chunk
            string file = GetChunckFileStatic(saveName, currentChunkIndex);
            return Serializer.Load<RegionChunk[]>(file, true);
        }

        /// <summary>
        /// Saves the chunk at a position
        /// </summary>
        /// <param name="chunk"></param>
        /// <param name="index"></param>
        public void SaveChunkInPos(RegionChunk chunk, int index)
        {
            string file = GetChunckFile(index);
            if (!string.IsNullOrWhiteSpace(file))
            {
                SaveChunckToFile(chunk);
            }
            else
            {
                List<RegionChunk> chunks = LoadChunks(savePathName,
                    chunk.ToIndex(GameLoop.Universe.WorldMap.AssocietatedMap.Width)).ToList();
                chunks.Add(chunk);
                SaveChuncksToFile(chunks.ToArray());
            }
        }

        /// <summary>
        /// gets the chunk file by it's index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private string GetChunckFile(int index)
        {
            // need to remake, so that it will only load the relevant chunk
            string path = $@"Saves\{saveDir}\Chunks_{index / 1000}.mr";
            string pattern = Path.GetFileName(path);
            string realDir = path[..^pattern.Length];
            string fullPath = Path.Combine(dir, realDir);
            string file =
                Directory.GetFiles(fullPath, pattern, SearchOption.TopDirectoryOnly).FirstOrDefault();
            return file;
        }

        /// <summary>
        /// same as <see cref="GetChunckFile"/>, but static
        /// </summary>
        /// <param name="saveName"></param>
        /// <param name="currentChunkIndex"></param>
        /// <returns></returns>
        private static string GetChunckFileStatic(string saveName, int currentChunkIndex)
        {
            // need to remake, so that it will only load the relevant chunk
            string path = $@"Saves\{saveName}\Chunks_{currentChunkIndex / 1000}.mr";
            string pattern = Path.GetFileName(path);
            string realDir = path[..^pattern.Length];
            string fullPath = Path.Combine(dir, realDir);
            string file =
                Directory.GetFiles(fullPath, pattern, SearchOption.TopDirectoryOnly).FirstOrDefault();
            return file;
        }

        /// <summary>
        /// Saves a single chunck to a file
        /// </summary>
        /// <param name="chunk"></param>
        private void SaveChunckToFile(RegionChunk chunk)
        {
            SaveChuncksToFile(new RegionChunk[] { chunk });
        }

        /// <summary>
        /// Checks if the string is a valid save to load
        /// </summary>
        /// <param name="saveName"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Deletes the save folder + everything inside of the folder
        /// </summary>
        /// <param name="saveName"></param>
        public static void DeleteSave(string saveName)
        {
            try
            {
                Directory.Delete(Path.Combine(dir, _folderName, saveName), true);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}