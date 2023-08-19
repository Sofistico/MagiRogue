using System.Text;

namespace MagusEngine.Services
{
    public class MagiLog
    {
        public void Log(List<string> errors)
        {
            // so that it doesn't block the main thread!
            Task.Run(() =>
            {
                if (errors.Count == 0)
                    return;
                var path = new StringBuilder(AppDomain.CurrentDomain.BaseDirectory).Append(@"\log.txt").ToString();
                StringBuilder str = new StringBuilder($"{DateTime.Now:dd/MM/yyyy} ");
                foreach (var item in errors)
                {
                    str.AppendLine(item);
                    str.AppendLine();
                }
                if (!File.Exists(path))
                {
                    File.Create(path).Close();
                }

                File.AppendAllText(path, str.ToString());
#if DEBUG
                //AddMessageLog("Logged an error in the logs file!");
#endif
            });
        }

        public void Log(string error)
        {
            Log(new List<string>() { error });
        }
    }
}
