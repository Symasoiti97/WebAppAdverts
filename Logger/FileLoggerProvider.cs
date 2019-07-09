using Microsoft.Extensions.Logging;

namespace Logger
{
    public class FileLoggerProvider : ILoggerProvider
    {
        private string _path;

        public FileLoggerProvider(string path)
        {
            _path = path;
        }

        public ILogger CreateLogger(string categoryName = "FileLogger")
        {
            return new FileLogger(_path);
        }

        public void Dispose()
        {
        }
    }
}
