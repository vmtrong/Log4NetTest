using log4net;
using Log4NetSimple.Logging;
using System;
using System.Reflection;
using System.Threading;

namespace Log4NetSimple
{
    class Program
    {
        private static ILog _log; 
        static void Main(string[] args)
        {
            //LogConfigWriteFileConfig.Setup();
            LogConfigNoFileConfig.Setup();

            _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);



            if (_log.IsDebugEnabled) // có thể bỏ đk này vì Root.Level = Level.All
            {
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
            }
        }
        //Console.ReadLine();
    }

}
