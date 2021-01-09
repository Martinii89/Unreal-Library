using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace UELib.Logging
{
    public interface ILogger
    {
        void WriteLine(string message);
    }

    public class ConsoleLogger : ILogger
    {
        public void WriteLine(string message)
        {
            Console.WriteLine(message);
        }
    }

    public class FileLogger : ILogger
    {
        private readonly string logFile;

        public FileLogger(string fileName = "log", bool overwriteFile = true)
        {
            logFile = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\\{fileName}.txt";
            if (overwriteFile)
            {
                DeleteOldLogfile();
            }
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
            using (var writer = new StreamWriter(logFile, append: true))
            {
                var now = DateTime.Now.ToLongTimeString();
                writer.WriteLine($"{now}: {message}");
            }
        }
    }

    public static class Log
    {

        private static ILogger logger;
        public static bool IsDebugEnabled { get; set; }
        public static bool IsInfoEnabled { get; set; }
        public static bool IsWarnEnabled { get; set; }
        public static bool IsErrorEnabled { get; set; }
        public static bool IsFatalEnabled { get; set; }
        public static int DeserializationErrors = 0;

        public static void SetLogger(ILogger logger)
        {
            Log.logger = logger;
        }

        public static void Debug(string message) { if (Log.IsDebugEnabled) { logger.WriteLine(message); } }
        public static void Info(string message)  { if (Log.IsInfoEnabled)  { logger.WriteLine(message); } }
        public static void Warn(string message)  { if (Log.IsWarnEnabled)  { logger.WriteLine(message); } }
        public static void Error(string message) { if (Log.IsErrorEnabled) { logger.WriteLine(message); } }
        public static void Fatal(string message) { if (Log.IsFatalEnabled) { logger.WriteLine(message); } }
        public static void DeserializationError(string message)
        {
            Error(message);
            Log.DeserializationErrors++;
        }

        static Log()
        {
            IsDebugEnabled = false;
            IsInfoEnabled = true;
            IsWarnEnabled = false;
            IsErrorEnabled = false;
            IsFatalEnabled = true;
            //logger = new ConsoleLogger(); //Default to console logging
            logger = new FileLogger("Eliot.UELib"); //Default to console logging
        }
    }
}
