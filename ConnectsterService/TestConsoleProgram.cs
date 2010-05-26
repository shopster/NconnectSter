using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Connectster.Server;

namespace ConnectsterService
{
    static class TestConsoleProgram
    {
        static void Main()
        {
            var server = new ConnectsterServer();
            server.Start();
        }
    }
}
