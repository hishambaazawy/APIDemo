using System.Text;

namespace APIDemo.Services
{
    public class LogsService
    {
        private readonly IWebHostEnvironment _env;
        private readonly string FullPath = "";
        public LogsService(IWebHostEnvironment env)
        {
            _env = env;
            FullPath = env.WebRootPath + @"\Logs";
            try
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(FullPath);
                if (directoryInfo.Exists == false) directoryInfo.Create();
            }
            catch { }
        }
        public string GetFullMessage(Exception exception)
        {
            var stringBuilder = new StringBuilder();
            while (exception != null)
            {
                stringBuilder.AppendLine(exception.Message);
                stringBuilder.AppendLine(exception.StackTrace);
                exception = exception.InnerException;
            }
            return stringBuilder.ToString();
        }
        public async void TraceError(Exception exception)
        {
            try
            {
                List<string> bs = new List<string>();
                bs.Add($"------------------------------------{DateTime.Now.ToString("yyyy-MM-dd H:mm:ss")}---------------------------------------");
                bs.Add(GetFullMessage(exception));
                bs.Add("------------------------------------------------------------------------------------");
                File.AppendAllLines(FullPath + @"\Error.txt", bs);
            }
            catch { }
        }
        public async void Trace(string exception)
        {
            try
            {
                List<string> bs = new List<string>();
                bs.Add($"------------------------------------{DateTime.Now.ToString("yyyy-MM-dd H:mm:ss")}---------------------------------------");
                bs.Add(exception);
                bs.Add("------------------------------------------------------------------------------------");
                File.AppendAllLines(FullPath + @"\Trace.txt", bs);
            }
            catch { }

        }
    }
}
