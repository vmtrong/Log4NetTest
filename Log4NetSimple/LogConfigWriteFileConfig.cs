using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Repository.Hierarchy;
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;

namespace Log4NetSimple
{
    internal static class LogConfigWriteFileConfig
    {
        static string _logPath = Path.GetTempPath() + @"\Log4NetSimple\";
        private static ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        internal static void Setup()
        {
            ////https://tuanphamdg.wordpress.com/2015/01/01/log4net-trong-c-va-tam-quan-trong-cua-viec-tao-log-nhat-ky-lam-viec/
            //XmlConfigurator.Configure(new FileInfo("log4net.config"));


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

            CreateLog4NetFile(configPath);
            XmlConfigurator.ConfigureAndWatch(new FileInfo(configPath));

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
                    //break;
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

        private static void CreateLog4NetFile(string FilePath)
        {
            try
            {
                if (!File.Exists(FilePath))
                {
                    using (TextWriter tw = File.CreateText(FilePath))
                    {
                        StringBuilder text = new StringBuilder();
                        //text.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
                        text.AppendLine("<!-- This section contains the log4net configuration settings -->");
                        text.AppendLine("<log4net>");
                        text.AppendLine("	<root>");
                        text.AppendLine("		<!-- ALL/DEBUG/INFO/WARN/ERROR/FATAL/OFF -->");
                        text.AppendLine("		<!--<level value=\"ALL\" />-->");
                        text.AppendLine("		<level value=\"DEBUG\" />");
                        text.AppendLine("		<appender-ref ref=\"console\" />");
                        text.AppendLine("		<appender-ref ref=\"file\" />");
                        text.AppendLine("		<!-- <appender-ref ref=\"FileAppender\" /> -->");
                        text.AppendLine("	</root>");
                        text.AppendLine();
                        text.AppendLine("	<!--ConsoleAppender: log ra màng hình, chỉ hiển thị khi ứng dụng có của sổ Console-->");
                        text.AppendLine("	<appender name=\"console\" type=\"log4net.Appender.ConsoleAppender\">");
                        text.AppendLine("		<layout type=\"log4net.Layout.PatternLayout\">");
                        text.AppendLine("			<conversionPattern value=\"%d [%t] %-5p %c - %m%n\" />");
                        text.AppendLine("		</layout>");
                        text.AppendLine("	</appender>");
                        text.AppendLine();
                        text.AppendLine("	<!--RollingFileAppender: Tạo ra tập tin log, khi tập tin này quá giới hạn thì nó sẽ bị đổi tên và tạo tập tin mới để log, vậy chúng ta sẽ có nhiều tập tin log.-->");
                        text.AppendLine("	<appender name=\"file\" type=\"log4net.Appender.RollingFileAppender\">");
                        text.AppendLine("		<!--<file value=\"C:\\Users\\${USERNAME}\\AppData\\Local\\Temp\\Log4NetSimple\" />-->");
                        text.AppendLine("		<!--Sử dung ${USERNAME} sẽ không đảm bảo nếu máy sử dụng tài khoản AD, sẽ ko tồn tại được dẫn này -> Sẽ đổi đường dẫn này trong code Program.cs-->");
                        text.AppendLine("		<file value=\"Log\\\" />");
                        text.AppendLine("		<appendToFile value=\"true\" /> <!--Cho phép ghi thêm dòng vào tệp-->");
                        text.AppendLine("		<datePattern value=\"'log'_yyyy-MM-dd'.log'\" />");
                        text.AppendLine("		<rollingStyle value=\"Composite\" /> <!--Đặt thành hỗn hợp để cho phép cuộn theo ngày và kích thước.-->");
                        text.AppendLine("		<maxSizeRollBackups value=\"10\" />	<!--Số lượng tệp sao lưu tối đa cần giữ.-->");
                        text.AppendLine("		<maximumFileSize value=\"1MB\" /> <!--Cuộn qua tệp nếu đạt đến giới hạn kích thước này. Một số nguyên bắt đầu từ 1 được thêm vào giữa phần mở rộng ngày và tệp, ví dụ: log_2022-03-19.1.log-->");
                        text.AppendLine("		<staticLogFileName value=\"false\" /> <!--Cho phép đổi tên-->");
                        text.AppendLine("		<preserveLogFileNameExtension value=\"true\" /> <!--Cho phép tên file đứng trước đuôi file-->");
                        text.AppendLine("		<encoding value=\"utf-8\" />");
                        text.AppendLine("		<layout type=\"log4net.Layout.PatternLayout\">");
                        text.AppendLine("			<conversionPattern value=\"%d [%t] %-5p %c - %m%n\"/> <!-- Change %c -> %-40.60c{3} to rate 3 tab-->");
                        text.AppendLine("		</layout>");
                        text.AppendLine("	</appender>");
                        text.AppendLine();
                        text.AppendLine("	<!--FileAppender: tạo ra duy nhất một file log-->");
                        text.AppendLine("	<appender name=\"FileAppender\" type=\"log4net.Appender.FileAppender\">");
                        text.AppendLine("		<file value=\"mylogfile.log\" />");
                        text.AppendLine("		<appendToFile value=\"true\" />");
                        text.AppendLine("		<lockingModel type=\"log4net.Appender.FileAppender+MinimalLock\" /> <!--Khóa và mở khóa tệp nhật ký dùng chung, để cho phép ứng dụng khác có thể ghi cùng vào file log này-->");
                        text.AppendLine("		<layout type=\"log4net.Layout.PatternLayout\">");
                        text.AppendLine("			<conversionPattern value=\"%date [%thread] %level %logger - %message%newline\" />");
                        text.AppendLine("		</layout>");
                        text.AppendLine("		<filter type=\"log4net.Filter.LevelRangeFilter\">");
                        text.AppendLine("			<levelMin value=\"DEBUG\" />");
                        text.AppendLine("			<levelMax value=\"FATAL\" />");
                        text.AppendLine("		</filter>");
                        text.AppendLine("	</appender>");
                        text.AppendLine("</log4net>");

                        tw.WriteLine(text);
                        tw.Close();
                    }
                }
            }
            catch
            {
            }
        }
    }
}
