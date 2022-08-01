using System;
using System.Diagnostics;
using System.ServiceProcess;

namespace WCFHost
{
    public class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            /*
            WCFHost.Start();
            Console.WriteLine("Hosting Services!...");
            Console.ReadKey();
            */

            try
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                    new WCFHost()
                };
                ServiceBase.Run(ServicesToRun);
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("Application", ex.ToString(), EventLogEntryType.Error);
            }
            
            
        }
    }
}
