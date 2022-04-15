using System;
using System.IO;
using UELib.Logging;
using System.Reflection;

namespace AssetExtraction
{
    public class MyFileLogger : ILogger, IDisposable
    {
        private readonly string logFile;
        readonly StreamWriter writer;

        public MyFileLogger(string fileName = "log", bool overwriteFile = true)
        {
            logFile = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\\{fileName}.txt";
            if (overwriteFile)
            {
                DeleteOldLogfile();
            }
            writer = new StreamWriter(logFile, append: true);
            writer.AutoFlush = true;
        }

        private void DeleteOldLogfile()
        {
            if (File.Exists(logFile))
            {
                File.Delete(logFile);
            }
        }


        public void WriteLine(string message)
        {
            writer.WriteLine(message);
        }

        public void Dispose()
        {
            ((IDisposable)writer).Dispose();
        }
    }
}