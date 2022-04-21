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
using System.Runtime.Serialization;

namespace MagiRogue.Data
{
    /// <summary>
    /// This class takes care of the saving and loading of save files
    /// </summary>
    public class SaveAndLoad
    {
        private const string _folderName = "Saves";
        private const int chunkPartition = 500;
        private static readonly string dir = AppDomain.CurrentDomain.BaseDirectory;

        [DataMember]
        public string SaveDir { get; set; }

        [DataMember]
        public string SavePathName { get; set; }
        private JsonSerializerSettings settings;

        [JsonConstructor]
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

            //SaveChuncksToFile(gameState.AllChunks);
            if (gameState.CurrentChunk is not null)
                SaveChunkInPos(gameState.CurrentChunk, gameState.CurrentChunk.ToIndex(
                   gameState.GetWorldMap().Width));
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
            if (SavePathName is not null && SavePathName.Equals(saveName)) return;
            SavePathName = saveName;
            string path = dir + $@"{_folderName}\{saveName}";

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            SaveDir = path;
        }

        /// <summary>
        /// Saves an array of chunks to a file, throws an exception
        /// </summary>
        /// <param name="chunks"></param>
        /// <exception cref="Exception"></exception>
        private void SaveChuncksToFile(RegionChunk[] chunks, int indexToStart = 0)
        {
            try
            {
                List<RegionChunk> newChunks = new List<RegionChunk>();
                if (chunks is null) return;
                int countArray = chunks.Length;
                for (int i = 0; i < countArray; i++)
                {
                    if (chunks[i] is not null)
                        newChunks.Add(chunks[i]);
                }
                int countList = newChunks.Count;

                for (int i = 0; i < countList; i++)
                {
                    if (i % chunkPartition == 0 || (i != 0 || i == chunks.Length))
                    {
                        if (indexToStart == 0)
                            Serializer.Save(newChunks, @$"{_folderName}\{SavePathName}\Chunks_{i / chunkPartition}.mr", true);
                        else
                            Serializer.Save(newChunks, @$"{_folderName}\{SavePathName}\Chunks_{indexToStart / chunkPartition}.mr", true);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to save chunk to a file! \nMessage: " + ex.Message);
            }
        }

        /// <summary>
        /// Test method, in the future will be deleted
        /// </summary>
        /// <param name="json"></param>
        public void SaveJsonToSaveFolder(string json)
        {
            Serializer.Save(json, $@"{SaveDir}\Test", true);
        }

        /// <summary>
        /// Loads the game provided by the save name
        /// </summary>
        /// <param name="saveName"></param>
        /// <returns></returns>
        public static Universe LoadGame(string saveName)
        {
            try
            {
                string path = $@"{_folderName}\{saveName}\GameState.mr";
                Universe uni = Serializer.Load<Universe>(path, true);
                return uni;
            }
            catch (Exception ex)
            {
                throw new Exception("The game failed to load! here is the error message: " + ex.Message);
            }
        }

        /// <summary>
        /// Load all chunks in the file, should be between <see cref="chunkPartition"/> chunks per file
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
        /// Load all chunks directly from a file.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private static RegionChunk[] LoadChunks(string file)
        {
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
            if (string.IsNullOrWhiteSpace(file))
            {
                SaveChunckToFile(chunk, index);
            }
            else
            {
                List<RegionChunk> chunks = LoadChunks(SavePathName,
                    chunk.ToIndex(GameLoop.Universe.WorldMap.AssocietatedMap.Width)).ToList();
                chunks.RemoveAll(c => c.ChunckPos() == chunk.ChunckPos());
                chunks.Add(chunk);

                SaveChuncksToFile(chunks.ToArray(), index);
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
            string path = $@"{SaveDir}\Chunks_{index / chunkPartition}.mr";
            string pattern = Path.GetFileName(path);
            string realDir = path[..^pattern.Length];
            string fullPath = Path.Combine(dir, realDir);
            string file =
                Directory.GetFiles(fullPath, pattern, SearchOption.TopDirectoryOnly).FirstOrDefault();
            return file;
        }

        private string GetChunckRelativeFile(int index)
        {
            string path = $@"{_folderName}\{SavePathName}\Chunks_{index / chunkPartition}.mr";
            return path;
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
            string file = $@"Saves\{saveName}\Chunks_{currentChunkIndex / chunkPartition}.mr";
            /*string pattern = Path.GetFileName(path);
            string realDir = path[..^pattern.Length];
            string fullPath = Path.Combine(dir, realDir);
            string file =
                Directory.GetFiles(fullPath, pattern, SearchOption.TopDirectoryOnly).FirstOrDefault();*/
            return file;
        }

        /// <summary>
        /// Saves a single chunck to a file
        /// </summary>
        /// <param name="chunk"></param>
        private void SaveChunckToFile(RegionChunk chunk, int index)
        {
            SaveChuncksToFile(new RegionChunk[] { chunk }, index);
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

        public RegionChunk GetChunkAtIndex(int index, int mapWidth)
        {
            string file = GetChunckRelativeFile(index);

            var regions = LoadChunks(file);

            return regions.FirstOrDefault(i => i.ToIndex(mapWidth) == index);
        }
    }
}