using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using MagusEngine.Bus.UiBus;
using MagusEngine.Utils.Extensions;

namespace MagusEngine.Services
{
    public enum LogLevel
    {
        Debug,
        Trace,
        Information,
        Warning,
        Error,
        Critical,
    }

    public static class MagiLog
    {
        private static void Log(List<string> messages, string fileName, LogLevel logLevel = LogLevel.Information)
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
                    str.AppendLine();
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
                if (logLevel == LogLevel.Error)
                {
                    Locator
                        .GetService<MessageBusService>()
                        .SendMessage(new AddMessageLog("Logged an error in the logs file!"));
                    System.Console.WriteLine(str.ToString());
                }
#endif
            });
        }

        public static void Log(string error, string fileName = "log", LogLevel logLevel = LogLevel.Information)
        {
            Log([error], fileName, logLevel);
        }

        public static void Log(Exception ex, string message = "", string fileName = "log")
        {
            var messageFormated = message.IsNullOrEmpty() ? "" : $"\nMessage: {message}";
            Log([$"An exception has occurred.{messageFormated}\nEX: {ex}"], fileName, LogLevel.Error);
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
