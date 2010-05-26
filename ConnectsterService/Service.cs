using System.ServiceProcess;
using System.Threading;
using Connectster.Server;

namespace ConnectsterService
{
    partial class Service : ServiceBase
    {
        private Thread _thread;

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
            const int threeMinutes = 1000*60*60*3;
            Thread.Sleep(threeMinutes);
            _thread.Abort();
        }
    }
}
