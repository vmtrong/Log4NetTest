using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Repository.Hierarchy;
using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Web;

namespace Log4NetTest
{
    public static class Logger
    {
        static Logger()
        {
            string configPath = string.Empty;
            if (HttpContext.Current != null)
            {
                configPath = HttpContext.Current.Server.MapPath("~");
            }
            else
            {
                configPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            }
            //configPath += "\\log4net_Config.xml";
            configPath += "\\log4net.config";

            Logger.CreateLo4NetFile(configPath);
            XmlConfigurator.Configure(new FileInfo(configPath));

            // Change the file location
            string logPath = Path.GetTempPath() + @"\Log4NetTest\";
            Hierarchy h = (Hierarchy)LogManager.GetRepository();
            foreach (IAppender a in h.Root.Appenders)
            {
                if (a is RollingFileAppender)
                {
                    RollingFileAppender fa = (RollingFileAppender)a;
                    var fileOnConfig = fa.File;
                    //reset fileOnConfig
                    fa.File = logPath;
                    fa.ActivateOptions();
                }
                if (a is FileAppender)
                {
                    FileAppender fa = (FileAppender)a;
                    var fileOnConfig = fa.File;
                    //reset fileOnConfig
                    fa.File = logPath;
                    fa.ActivateOptions();
                }
            }
        }

        private static void CreateLo4NetFile(string FilePath)
        {
            try
            {
                if (!File.Exists(FilePath))
                {
                    using (TextWriter tw = File.CreateText(FilePath))
                    {
                        StringBuilder Log4NetContext = new StringBuilder();
                        //Log4NetContext.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
                        Log4NetContext.AppendLine("<!-- This section contains the log4net configuration settings -->");
                        Log4NetContext.AppendLine("<log4net>");
                        Log4NetContext.AppendLine("   <appender name=\"Console\" type=\"log4net.Appender.ConsoleAppender\">");
                        Log4NetContext.AppendLine("       <layout type=\"log4net.Layout.PatternLayout\">");
                        Log4NetContext.AppendLine("           <!-- Pattern to output the caller's file name and line number -->");
                        Log4NetContext.AppendLine("           <conversionPattern value=\"%d [%t] %-5p %c - %m%n\" />");
                        Log4NetContext.AppendLine("       </layout>");
                        Log4NetContext.AppendLine("   </appender>");
                        Log4NetContext.AppendLine();
                        Log4NetContext.AppendLine("   <appender name=\"ErrorLog\" type=\"log4net.Appender.RollingFileAppender\">");
                        Log4NetContext.AppendLine("       <file value=\"C:\\Users\\${USERNAME}\\AppData\\Local\\Temp\\SVPNManager\\\" />");
                        Log4NetContext.AppendLine("       <lockingModel type=\"log4net.Appender.FileAppender + MinimalLock\" />");// Chỉ 1 file
                        Log4NetContext.AppendLine("       <datePattern value=\"yyyy-MM-dd_'ErrorLog.log'\" />");
                        Log4NetContext.AppendLine("       <appendToFile value=\"true\" />");
                        Log4NetContext.AppendLine("       <rollingStyle value=\"Size\" />");
                        Log4NetContext.AppendLine("       <maxSizeRollBackups value=\"5\" />");
                        Log4NetContext.AppendLine("       <maximumFileSize value=\"10MB\" />");
                        Log4NetContext.AppendLine("       <staticLogFileName value=\"true\" />");
                        Log4NetContext.AppendLine("       <encoding value=\"utf-8\" />");
                        Log4NetContext.AppendLine("       <layout type=\"log4net.Layout.PatternLayout\">");
                        Log4NetContext.AppendLine("           <conversionPattern value=\"%d [%t] %-5p %c - %m%n\" />");
                        Log4NetContext.AppendLine("       </layout>");
                        Log4NetContext.AppendLine("       <filter type=\"log4net.Filter.LevelRangeFilter\">");
                        Log4NetContext.AppendLine("			  <param name=\"LevelMin\" value=\"error\"/>");
                        Log4NetContext.AppendLine("			  <param name=\"LevelMax\" value=\"error\"/>");
                        Log4NetContext.AppendLine("			  <param name=\"AcceptOnMatch\" value=\"true\"/>");
                        Log4NetContext.AppendLine("		  </filter>");
                        Log4NetContext.AppendLine("   </appender>");
                        Log4NetContext.AppendLine();
                        Log4NetContext.AppendLine("   <appender name=\"InfoLog\" type=\"log4net.Appender.RollingFileAppender\">");
                        Log4NetContext.AppendLine("       <file value=\"C:\\Users\\${USERNAME}\\AppData\\Local\\Temp\\SVPNManager\\\" />");
                        Log4NetContext.AppendLine("       <datePattern value=\"yyyy-MM-dd_'InfoLog.log'\" />");
                        Log4NetContext.AppendLine("       <staticLogFileName value=\"false\" />");
                        Log4NetContext.AppendLine("       <appendToFile value=\"true\" />");
                        Log4NetContext.AppendLine("       <rollingStyle value=\"Composite\" />");
                        Log4NetContext.AppendLine("       <maxSizeRollBackups value=\"10\" />");
                        Log4NetContext.AppendLine("       <maximumFileSize value=\"1MB\" />");
                        Log4NetContext.AppendLine("       <encoding value=\"utf-8\" />");
                        Log4NetContext.AppendLine("       <lockingModel type=\"log4net.Appender.FileAppender+MinimalLock\" />");
                        Log4NetContext.AppendLine("       <layout type=\"log4net.Layout.PatternLayout\">");
                        Log4NetContext.AppendLine("           <conversionPattern value=\"%d [%t] %-5p %c - %m%n\" />");
                        Log4NetContext.AppendLine("       </layout>");
                        Log4NetContext.AppendLine("       <filter type=\"log4net.Filter.LevelRangeFilter\">");
                        Log4NetContext.AppendLine("			  <param name=\"LevelMin\" value=\"info\"/>");
                        Log4NetContext.AppendLine("			  <param name=\"LevelMax\" value=\"info\"/>");
                        Log4NetContext.AppendLine("			  <param name=\"AcceptOnMatch\" value=\"true\"/>");
                        Log4NetContext.AppendLine("		  </filter>");
                        Log4NetContext.AppendLine("   </appender>");
                        Log4NetContext.AppendLine();
                        Log4NetContext.AppendLine("   <appender name=\"WarnLog\" type=\"log4net.Appender.RollingFileAppender\">");
                        Log4NetContext.AppendLine("       <file value=\"C:\\Users\\${USERNAME}\\AppData\\Local\\Temp\\SVPNManager\\\" />");
                        Log4NetContext.AppendLine("       <datePattern value=\"yyyy-MM-dd_'WarnLog.log'\" />");
                        Log4NetContext.AppendLine("       <staticLogFileName value=\"false\" />");
                        Log4NetContext.AppendLine("       <appendToFile value=\"true\" />");
                        Log4NetContext.AppendLine("       <rollingStyle value=\"Composite\" />");
                        Log4NetContext.AppendLine("       <maxSizeRollBackups value=\"10\" />");
                        Log4NetContext.AppendLine("       <maximumFileSize value=\"1MB\" />");
                        Log4NetContext.AppendLine("       <encoding value=\"utf-8\" />");
                        Log4NetContext.AppendLine("       <lockingModel type=\"log4net.Appender.FileAppender+MinimalLock\" />");
                        Log4NetContext.AppendLine("       <layout type=\"log4net.Layout.PatternLayout\">");
                        Log4NetContext.AppendLine("           <conversionPattern value=\"%d [%t] %-5p %c - %m%n\" />");
                        Log4NetContext.AppendLine("       </layout>");
                        Log4NetContext.AppendLine("       <filter type=\"log4net.Filter.LevelRangeFilter\">");
                        Log4NetContext.AppendLine("			  <param name=\"LevelMin\" value=\"warn\"/>");
                        Log4NetContext.AppendLine("			  <param name=\"LevelMax\" value=\"warn\"/>");
                        Log4NetContext.AppendLine("			  <param name=\"AcceptOnMatch\" value=\"true\"/>");
                        Log4NetContext.AppendLine("		  </filter>");
                        Log4NetContext.AppendLine("   </appender>");
                        Log4NetContext.AppendLine();
                        Log4NetContext.AppendLine("   <appender name=\"DebugLog\" type=\"log4net.Appender.RollingFileAppender\">");
                        Log4NetContext.AppendLine("       <file value=\"C:\\Users\\${USERNAME}\\AppData\\Local\\Temp\\SVPNManager\\\" />");
                        Log4NetContext.AppendLine("       <datePattern value=\"yyyy-MM-dd_'DebugLog.log'\" />");
                        Log4NetContext.AppendLine("       <staticLogFileName value=\"false\" />");
                        Log4NetContext.AppendLine("       <appendToFile value=\"true\" />");
                        Log4NetContext.AppendLine("       <rollingStyle value=\"Composite\" />");
                        Log4NetContext.AppendLine("       <maxSizeRollBackups value=\"10\" />");
                        Log4NetContext.AppendLine("       <maximumFileSize value=\"5MB\" />");
                        Log4NetContext.AppendLine("       <encoding value=\"utf-8\" />");
                        Log4NetContext.AppendLine("       <lockingModel type=\"log4net.Appender.FileAppender+MinimalLock\" />");
                        Log4NetContext.AppendLine("       <layout type=\"log4net.Layout.PatternLayout\">");
                        Log4NetContext.AppendLine("           <conversionPattern value=\"%d [%t] %-5p %c - %m%n\" />");
                        Log4NetContext.AppendLine("       </layout>");
                        Log4NetContext.AppendLine("       <filter type=\"log4net.Filter.LevelRangeFilter\">");
                        Log4NetContext.AppendLine("			  <param name=\"LevelMin\" value=\"debug\"/>");
                        Log4NetContext.AppendLine("			  <param name=\"LevelMax\" value=\"debug\"/>");
                        Log4NetContext.AppendLine("			  <param name=\"AcceptOnMatch\" value=\"true\"/>");
                        Log4NetContext.AppendLine("		  </filter>");
                        Log4NetContext.AppendLine("   </appender>");
                        Log4NetContext.AppendLine();
                        Log4NetContext.AppendLine("   <appender name=\"FatalLog\" type=\"log4net.Appender.RollingFileAppender\">");
                        Log4NetContext.AppendLine("       <file value=\"C:\\Users\\${USERNAME}\\AppData\\Local\\Temp\\SVPNManager\\\" />");
                        Log4NetContext.AppendLine("       <datePattern value=\"yyyy-MM-dd_'FatalLog.log'\" />");
                        Log4NetContext.AppendLine("       <staticLogFileName value=\"false\" />");
                        Log4NetContext.AppendLine("       <appendToFile value=\"true\" />");
                        Log4NetContext.AppendLine("       <rollingStyle value=\"Composite\" />");
                        Log4NetContext.AppendLine("       <maxSizeRollBackups value=\"10\" />");
                        Log4NetContext.AppendLine("       <maximumFileSize value=\"1MB\" />");
                        Log4NetContext.AppendLine("       <encoding value=\"utf-8\" />");
                        Log4NetContext.AppendLine("       <lockingModel type=\"log4net.Appender.FileAppender+MinimalLock\" />");
                        Log4NetContext.AppendLine("       <layout type=\"log4net.Layout.PatternLayout\">");
                        Log4NetContext.AppendLine("           <conversionPattern value=\"%d [%t] %-5p %c - %m%n\" />");
                        Log4NetContext.AppendLine("       </layout>");
                        Log4NetContext.AppendLine("       <filter type=\"log4net.Filter.LevelRangeFilter\">");
                        Log4NetContext.AppendLine("			  <param name=\"LevelMin\" value=\"fatal\"/>");
                        Log4NetContext.AppendLine("			  <param name=\"LevelMax\" value=\"fatal\"/>");
                        Log4NetContext.AppendLine("			  <param name=\"AcceptOnMatch\" value=\"true\"/>");
                        Log4NetContext.AppendLine("		  </filter>");
                        Log4NetContext.AppendLine("   </appender>");
                        Log4NetContext.AppendLine();
                        Log4NetContext.AppendLine("   <root>");
                        Log4NetContext.AppendLine("       <!-- All, Debug, Info, Warning, Error, Fatal, Console, OFF -->");
                        Log4NetContext.AppendLine("       <level value=\"ALL\" />");
                        Log4NetContext.AppendLine("       <!--<appender-ref ref=\"DebugLog\" />-->");
                        Log4NetContext.AppendLine("       <appender-ref ref=\"InfoLog\" />");
                        Log4NetContext.AppendLine("       <!--<appender-ref ref=\"WarnLog\" />-->");
                        Log4NetContext.AppendLine("       <appender-ref ref=\"ErrorLog\" />");
                        Log4NetContext.AppendLine("       <!--<appender-ref ref=\"FatalLog\" />-->");
                        Log4NetContext.AppendLine("       <!--<appender-ref ref=\"Console\" />-->");
                        Log4NetContext.AppendLine("   </root>");
                        Log4NetContext.AppendLine("</log4net>");

                        tw.WriteLine(Log4NetContext);
                        tw.Close();
                    }
                }
            }
            catch
            {
            }
        }

        public static void Debug(ref ILog iLog, object message)
        {
            if (iLog.IsDebugEnabled)
                iLog.Debug(message);
        }
        public static void Debug(ref ILog iLog, object message, Exception exception)
        {
            iLog.Debug(message, exception);
        }

        public static void Info(ref ILog iLog, object Message)
        {
            iLog.Info(Message);
        }

        public static void Warning(ref ILog iLog, object Message)
        {
            iLog.Warn(Message);
        }

        public static void Error(ref ILog iLog, object Message)
        {
            iLog.Error(Message);
        }

        public static void Fatal(ref ILog iLog, object Message)
        {
            iLog.Fatal(Message);
        }


    }
}
