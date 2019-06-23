using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

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
