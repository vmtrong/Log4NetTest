using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Log4NetMultiLogger
{
    class Program
    {
        static void Main(string[] args)
        {
            var arg = "myArg";
            var loggerName = "MyLogger";
            var log = LogMaster.GetLogger(arg, loggerName);

            log.Info("Hi");
        }
    }
}
