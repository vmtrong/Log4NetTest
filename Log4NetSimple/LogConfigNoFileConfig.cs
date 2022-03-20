using log4net;
using log4net.Repository.Hierarchy;
using log4net.Core;
using log4net.Appender;
using log4net.Layout;
using System.IO;
using System.Configuration;
using System.Globalization;
using System.Reflection;
using System;

namespace Log4NetSimple.Logging
{
    public class LogConfigNoFileConfig
    {
        private static string _logPath;
        private static ILog _log;
        static LogConfigNoFileConfig()
        {
            _logPath = Path.GetTempPath() + @"\Log4NetSimple\";
            _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        }
        public static void Setup()
        {
            Hierarchy hierarchy = (Hierarchy)LogManager.GetRepository();

            PatternLayout patternLayout = new PatternLayout();
            patternLayout.ConversionPattern = "%d [%t] %-5p %c - %m%n";
            patternLayout.ActivateOptions();

            // ConsoleAppender
            var consoler = new ConsoleAppender();
            consoler.Layout = patternLayout;
            //consoler.Threshold = Level.Info;
            hierarchy.Root.AddAppender(consoler);

            // RollingFileAppender
            var roller = new RollingFileAppender();
            roller.AppendToFile = true;
            roller.File = _logPath;
            roller.DatePattern = "'log'_yyyy-MM-dd'.log'";
            roller.Layout = patternLayout;
            roller.MaxSizeRollBackups = 5;
            roller.MaximumFileSize = "2MB";
            roller.RollingStyle = RollingFileAppender.RollingMode.Composite;
            roller.StaticLogFileName = false;
            roller.PreserveLogFileNameExtension = true;
            
            var filter = new log4net.Filter.LevelRangeFilter();
            filter.LevelMin = GetLogLevel("MinLevel");
            filter.LevelMax = GetLogLevel("MaxLevel");
            roller.AddFilter(filter);
            
            roller.ActivateOptions();
            hierarchy.Root.AddAppender(roller);

            // MemoryAppender
            /*
            var memory = new MemoryAppender();
            memory.ActivateOptions();
            hierarchy.Root.AddAppender(memory);
            */
            hierarchy.Root.Level = Level.All;
            hierarchy.Configured = true;

            _log.Info("\n\n==================== App START ====================");

            CleanUp();
        }
        internal static void CleanUp()
        {
            var maxAgeRollBackups = GetMaxAgeRollBackups();

            _log.InfoFormat("Deleting old log files (>{0} days)...", maxAgeRollBackups);

            // Delete log files older than n days
            if (Directory.Exists(_logPath))
            {
                foreach (string file in Directory.GetFiles(Path.GetDirectoryName(_logPath), "*.log.*"))
                {
                    if (System.IO.File.GetLastWriteTime(file) < DateTime.Today.AddDays(-1 * maxAgeRollBackups))
                        try
                        {
                            File.Delete(file);
                            _log.Info("Deleted old log file: " + file);
                        }
                        catch (IOException ex)
                        {
                            _log.Warn("Failed to delete log file: " + file + "(" + ex.Message + ")");
                        }
                }

            }
            _log.Info("Old log files deleted!");
        }
        static Level GetLogLevel(string key)
        {
            Level retVal = Level.All;
            var level = ConfigurationManager.AppSettings[key];
            if (level != null)
            {
                switch (level.ToString(CultureInfo.CurrentCulture).ToUpper())
                {
                    case "DEBUG":
                        retVal = Level.Debug;
                        break;
                    case "INFO":
                        retVal = Level.Info;
                        break;
                    case "WARN":
                        retVal = Level.Warn;
                        break;
                    case "ERROR":
                        retVal = Level.Error;
                        break;
                    case "FATAL":
                        retVal = Level.Fatal;
                        break;
                    case "OFF":
                        retVal = Level.Off;
                        break;
                    default:
                        retVal = Level.All;
                        break;
                }
            }
            return retVal;
        }
        static int GetMaxAgeRollBackups()
        {
            int retVal = 30; // default 30 dáys
            var max = ConfigurationManager.AppSettings["MaxAgeRollBackups"];
            if (max != null)
            {
                int.TryParse(max, out retVal);
            }
            return retVal;
        }
    }
}
