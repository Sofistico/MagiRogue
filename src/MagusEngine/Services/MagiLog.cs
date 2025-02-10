using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using MagusEngine.Bus.UiBus;

namespace MagusEngine.Services
{
    public enum LogLevel
    {
        Info,
        Warn,
        Error,
    }

    public class MagiLog
    {
        public void Log(List<string> messages, string fileName, LogLevel logLevel = LogLevel.Info)
        {
            // so that it doesn't block the main thread!
            Task.Run(() =>
            {
                if (messages.Count == 0)
                    return;
                var path = new StringBuilder(AppDomain.CurrentDomain.BaseDirectory)
                    .Append($@"\{fileName}.txt")
                    .ToString();
                StringBuilder str = new($"{DateTime.Now:dd/MM/yyyy HH:mm:ss} ");
                foreach (var item in messages)
                {
                    str.Append(logLevel.ToString() + ": " + item);
                }
                if (!File.Exists(path))
                {
                    File.Create(path).Close();
                }
                while (IsFileLocked(new FileInfo(path)))
                {
                }
                File.AppendAllText(path, str.ToString());
#if DEBUG
                Locator
                    .GetService<MessageBusService>()
                    .SendMessage(new AddMessageLog("Logged an error in the logs file!"));
                System.Console.WriteLine(str.ToString());
#endif
            });
        }

        public void Log(string error, string fileName = "log", LogLevel logLevel = LogLevel.Info)
        {
            Log([error], fileName, logLevel);
        }

        public void Log(Exception ex, string fileName = "log")
        {
            Log([$"An exception has occurred.\nEX: {ex}"], fileName, LogLevel.Error);
        }

        private static bool IsFileLocked(FileInfo file)
        {
            try
            {
                using FileStream stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);
                stream.Close();
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }

            //file is not locked
            return false;
        }
    }
}
