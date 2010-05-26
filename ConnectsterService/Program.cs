using System;
using System.ServiceProcess;
using System.Text;

namespace ConnectsterService
{
    /// <summary>
    /// Console testing program for Connectster Service
    /// </summary>
    static class Program
    {
        static void Main()
        {
            Console.WriteLine("Starting connectster server..");
            var servicesToRun = new ServiceBase[] 
                                              { 
                                                  new Service() 
                                              };
            ServiceBase.Run(servicesToRun);
        }
    }
}
