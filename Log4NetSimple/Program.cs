using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Repository.Hierarchy;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

namespace Log4NetSimple
{
    class Program
    {
        private static ILog _log;
        static void Main(string[] args)
        {
            LogConfiguration.Setup();

            _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

            // Set an end time
            var endTime = DateTime.Now.AddSeconds(30);

            long logEntryNo = 0;
            while (DateTime.Now < endTime)
            {
                // Write a log and delay, so we don't spam our log
                _log.Debug("My message " + logEntryNo);
                logEntryNo++;
                Thread.Sleep(500);
            }

            Console.ReadLine();
        }
    }
}
