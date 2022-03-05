/*
 * https://phamtuantech.com/cau-hinh-log4net-va-logging-adapter-trong-code-c/
 */


using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;

namespace Common_Logging_Adapter
{
    internal static class LogConfiguration
    {
        internal static void SetupLog4net()
        {
            var hierarchy = (Hierarchy)LogManager.GetRepository();
            hierarchy.Root.AddAppender(GetConsoleAppender());
            hierarchy.Root.AddAppender(GetRollingAppender());
            hierarchy.Root.Level = Level.All;
            hierarchy.Configured = true;

            //var logger = LogManager.GetLogger(typeof(LogConfiguration));
            //logger.Info($"Init Log4net successfully.");

            var logger = LogManager.GetLogger("");
            logger.Info("\n\n==================== App START ====================");
            

            SetupCommonLogging();

            
        }
        internal static void SetupCommonLogging()
        {
            //var commonLoggingConfig = new Common.Logging.Configuration.NameValueCollection();
            //commonLoggingConfig["configType"] = "EXTERNAL";

            //var adapter = new Common.Logging.Log4Net.Log4NetLoggerFactoryAdapter(commonLoggingConfig);
            //Common.Logging.LogManager.Adapter = adapter;

            //var logger = Common.Logging.LogManager.GetLogger(typeof(LogConfiguration));
            //logger.Info($"[SetupLog4net] Init common.logging successfully.");
        }

        private static ConsoleAppender GetConsoleAppender()
        {
            var patternLayout = new PatternLayout();
            //patternLayout.ConversionPattern = "%date [%thread] %-5level %logger [%property{NDC}] - %message%newline";
            patternLayout.ConversionPattern = "[%2thread] %-5level %-40.60logger{3} - %message%newline";
            patternLayout.ActivateOptions();

            var consoler = new ConsoleAppender();
            consoler.Layout = patternLayout;
            consoler.Threshold = Level.Info;
            return consoler;
        }

        private static RollingFileAppender GetRollingAppender()
        {
            var patternLayout = new PatternLayout();
            //patternLayout.ConversionPattern = "%d [%2t] %-5p %-40.60c{3} - %m%n";
            patternLayout.ConversionPattern = "[%d] [%2t] [%-5p] [%-40.60c{3}] - %m%n";
            //patternLayout.ConversionPattern = "%date [%thread] %-5level %logger [%property{NDC}] - %message%newline";
            //patternLayout.ConversionPattern = "[%date{yyyy-MM-dd HH:mm:ss.fff}] [%t] [%-5level] [%-40.60logger{3}] - %message%newline";
            patternLayout.ActivateOptions();

            var roller = new RollingFileAppender();
            roller.AppendToFile = true;
            roller.File = @"Logs\Log4net.log";
            roller.Layout = patternLayout;
            roller.MaxSizeRollBackups = 5;
            roller.MaximumFileSize = "2MB";
            roller.RollingStyle = RollingFileAppender.RollingMode.Size;
            roller.StaticLogFileName = true;
            roller.ActivateOptions();
            return roller;
        }
    }
}
