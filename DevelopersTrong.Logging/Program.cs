using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DevelopersTrong.Logging
{
    class Program
    {
         static ThreadSafeLogger _Logger = new ThreadSafeLogger("Program", "C:\\logs");

        static void Main(string[] args)
        {
            _Logger.LogInfo("Hello", "Program");
            Console.ReadKey();
        }
    }
}
