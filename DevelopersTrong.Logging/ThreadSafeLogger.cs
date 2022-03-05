/*
 * http://www.developersalley.com/blog/post/2012/02/09/How-To-Write-To-Seperate-Log-Files-During-Multi-Threaded-Processing-Using-Log4Net-PropertyFilters.aspx
 */
using System;
using System.Collections.Generic;
using System.Text;
using log4net;
using log4net.Core;
using log4net.Config;
using log4net.Layout;
using log4net.Repository;
using log4net.Appender;
using log4net.Repository.Hierarchy;

namespace DevelopersTrong.Logging
{
    public class ThreadSafeLogger
    {
        private const string _LOGNAME = "LogName";
        static readonly object _locker = new object();
        private ILog _logger;

        /// <summary>
        /// Allow a log file name to be passed in
        /// </summary>
        /// <param name="className"></param>
        /// <param name="LogFileName"></param>
        public ThreadSafeLogger(string ClassName, string LogPath)
        {
            if (!String.IsNullOrEmpty(LogPath))
            {
                //clean up path
                if (!LogPath.EndsWith("\\"))
                    LogPath += "\\";
            }
            else
                LogPath = "";

            //make the nice log path
            LogPath = String.Format("{0}{1}.log", LogPath, ClassName);

            lock (_locker)
            {
                log4net.ThreadContext.Properties[_LOGNAME] = ClassName;

                _logger = LogManager.GetLogger("Root");
                if (!_logger.Logger.Repository.Configured)
                    XmlConfigurator.Configure();

                //add the appender and set the filter now...
                AddAppender(ClassName, CreateFileAppender(ClassName, LogPath));
            }
        }

        #region LOG4NET UTILITIES
        /// <summary>
        /// Add an appender
        /// </summary>
        /// <param name="LoggerName"></param>
        /// <param name="Appender"></param>
        private void AddAppender(string LoggerName, log4net.Appender.IAppender Appender)
        {
            log4net.ILog log = log4net.LogManager.GetLogger(LoggerName);
            log4net.Repository.Hierarchy.Logger l = (log4net.Repository.Hierarchy.Logger)log.Logger;
            l.AddAppender(Appender);
        }

        /// <summary>
        /// Find a named appender already attached to a logger
        /// </summary>
        /// <param name="appenderName"></param>
        /// <returns></returns>
        public log4net.Appender.IAppender FindAppender(string AppenderName)
        {
            foreach (log4net.Appender.IAppender appender in log4net.LogManager.GetRepository().GetAppenders())
            {
                if (appender.Name == AppenderName)
                    return appender;
            }
            return null;
        }

        /// <summary>
        /// Create an appender
        /// </summary>
        /// <param name="name"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public log4net.Appender.IAppender CreateFileAppender(string ClassName, string FileName)
        {
            log4net.Appender.RollingFileAppender appender = (log4net.Appender.RollingFileAppender)FindAppender(ClassName);

            if (appender != null)
                return appender;

            appender = new log4net.Appender.RollingFileAppender();
            appender.Name = ClassName;
            appender.File = FileName;
            appender.AppendToFile = true;
            appender.MaximumFileSize = "10MB";
            appender.MaxSizeRollBackups = 5;
            appender.RollingStyle = RollingFileAppender.RollingMode.Size;
            appender.StaticLogFileName = true;

            //// add the filter for the log source
            log4net.Filter.PropertyFilter filter = new log4net.Filter.PropertyFilter();

            filter.Key = _LOGNAME;
            filter.StringToMatch = ClassName;
            filter.AcceptOnMatch = true;

            //add deny all filter
            log4net.Filter.DenyAllFilter filterDeny = new log4net.Filter.DenyAllFilter();

            appender.AddFilter(filter);
            appender.AddFilter(filterDeny);

            log4net.Layout.PatternLayout layout = new log4net.Layout.PatternLayout();
            layout.ConversionPattern = "%d [%t] %-5p %c - %m%n";
            layout.ActivateOptions();

            appender.Layout = layout;
            appender.ActivateOptions();
            log4net.Config.BasicConfigurator.Configure(appender);
            return appender;
        }
        #endregion

        #region INFO LOGGING

        /// <summary>
        /// Log a message to a particular log name
        /// </summary>
        /// <param name="messageFormat"></param>
        /// <param name="classname"></param>
        public void LogInfo(string MessageFormat, string ClassName)
        {
            lock (_locker)
            {
                log4net.ThreadContext.Properties[_LOGNAME] = ClassName;

                if (_logger.IsInfoEnabled)
                    _logger.Info(MessageFormat);
            }
        }
        #endregion

        #region DEBUG LOGGING
        /// <summary>
        /// Log debug message to a file
        /// </summary>
        /// <param name="messageFormat"></param>
        /// <param name="classname"></param>
        public void LogDebug(string MessageFormat, string ClassName)
        {
            lock (_locker)
            {
                log4net.ThreadContext.Properties[_LOGNAME] = ClassName;

                if (_logger.IsDebugEnabled)
                    _logger.Debug(MessageFormat);
            }
        }
        #endregion

        #region ERROR LOGGING
        /// <summary>
        /// Logs an error message to a specific log
        /// </summary>
        /// <param name="messageFormat"></param>
        /// <param name="classname"></param>
        public void LogError(string MessageFormat, string ClassName)
        {
            lock (_locker)
            {
                log4net.ThreadContext.Properties[_LOGNAME] = ClassName;

                if (_logger.IsErrorEnabled)
                    _logger.Error(MessageFormat);
            }
        }
        #endregion
    }
}
