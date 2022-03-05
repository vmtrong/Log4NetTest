using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using log4net;

namespace Common_Logging_Adapter
{
    class Program
    {
        static void Main(string[] args)
        {
            LogConfiguration.SetupLog4net();

            var logger = LogManager.GetLogger(typeof(Program));

            logger.Info("Wellcome to logging configuration 1.");
            logger.Debug("Wellcome to logging configuration 2.");
            logger.Warn("Wellcome to logging configuration 3.");
            logger.Fatal("Wellcome to logging configuration 4.");
            logger.Error("Wellcome to logging configuration 5.");


            logger= LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            logger.Error("Wellcome to logging configuration 6.");

            Console.ReadKey();
        }
    }
}
