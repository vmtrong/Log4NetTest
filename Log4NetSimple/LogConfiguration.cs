using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Repository.Hierarchy;
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Log4NetSimple
{
    internal static class LogConfiguration
    {
        static string _logPath = Path.GetTempPath() + @"\Log4NetSimple\";
        private static ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        internal static void Setup()
        {
            //https://tuanphamdg.wordpress.com/2015/01/01/log4net-trong-c-va-tam-quan-trong-cua-viec-tao-log-nhat-ky-lam-viec/
            XmlConfigurator.Configure(new FileInfo("log4net.config"));

            // Change the file location
            // Solution 1
            var appender = (LogManager.GetRepository() as Hierarchy).Root.Appenders
                .OfType<FileAppender>()
                .First();
            if (appender != null)
            {
                appender.File = _logPath;
                appender.ActivateOptions();
            }

            // Solution 2
            /*
            Hierarchy h = (Hierarchy)LogManager.GetRepository();
            foreach (IAppender a in h.Root.Appenders)
            {
                if (a is RollingFileAppender)
                {
                    RollingFileAppender fa = (RollingFileAppender)a;
                    var fileOnConfig = fa.File;
                    //reset fileOnConfig
                    fa.File = _logPath;
                    fa.ActivateOptions();
                    break;
                }
            }*/

            _log.Info("\n\n==================== App START ====================");

            CleanUp();
        }
        internal static void CleanUp()
        {
            var maxAgeRollBackups = Convert.ToInt32(ConfigurationManager.AppSettings["MaxAgeRollBackups"].ToString());

            _log.InfoFormat("Deleting old log files (>{0} days)...", maxAgeRollBackups);

            // Delete log files older than n days
            if (Directory.Exists(_logPath))
            {
                // Solution 1
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

                // Solution 2
                /*
                var now = DateTime.Now;
                var max = new TimeSpan(maxAgeRollBackups, 0, 0, 0); // maxAgeRollBackups days
                foreach (var file in from file in Directory.GetFiles(_logPath)
                                     let modTime = File.GetLastWriteTime(file)
                                     let age = now.Subtract(modTime)
                                     where age > max
                                     select file)
                {
                    try
                    {
                        File.Delete(file);
                        _log.Info("Deleted old log file: " + file);
                    }
                    catch (IOException ex)
                    {
                        _log.Warn("Failed to delete log file: " + file + "(" + ex.Message + ")");
                    }
                }*/
            }
            _log.Info("Old log files deleted!");
        }
    }
}
