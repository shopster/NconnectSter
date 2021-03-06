﻿using System.ServiceProcess;
using System.Threading;
using Connectster.Server;

namespace ConnectsterService
{
    partial class Service : ServiceBase
    {
        private readonly Thread _thread;

        private readonly ConnectsterServer _server;
        public Service()
        {
            InitializeComponent();
            _server = new ConnectsterServer();
            var job = new ThreadStart(_server.Start);
            _thread = new Thread(job);
        }

        protected override void OnStart(string[] args)
        {
            _thread.Start();
        }

        protected override void OnStop()
        {
            _server.Stop();
            const int tenSeconds = 1000*10;
            Thread.Sleep(tenSeconds);
            _thread.Abort();
        }
    }
}
