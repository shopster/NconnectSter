using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Connectster.Server;
using log4net.Config;

namespace ConnectsterService
{
    static class TestConsoleProgram
    {
        static void Main()
        {
            //Initialize Logging 
            XmlConfigurator.Configure();

            //Start server
            var server = new ConnectsterServer();
            server.Start();
        }
    }
}
