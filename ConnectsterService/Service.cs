using System.ServiceProcess;
using Connectster.Server;

namespace ConnectsterService
{
    partial class Service : ServiceBase
    {
        private readonly ConnectsterServer _server;
        public Service()
        {
            InitializeComponent();
            _server = new ConnectsterServer();
        }

        protected override void OnStart(string[] args)
        {
            _server.Start(); 
        }

        protected override void OnStop()
        {
            _server.Stop();
        }
    }
}
