using System;
using System.ServiceProcess;
using System.Text;
using log4net.Config;

namespace ConnectsterService
{
    /// <summary>
    /// Console testing program for Connectster Service
    /// </summary>
    static class Program
    {
        static void Main()
        {
            //Initialize Logging 
            XmlConfigurator.Configure();
    
            //start service
            var servicesToRun = new ServiceBase[] 
                                              { 
                                                  new Service() 
                                              };
            ServiceBase.Run(servicesToRun);
        }
    }
}
