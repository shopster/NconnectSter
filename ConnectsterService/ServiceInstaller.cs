using System;using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;


namespace ConnectsterService
{
    [RunInstaller(true)]
    public partial class ServiceInstaller : Installer
    {
        public ServiceInstaller()
        {
            InitializeComponent();

            var processInstaller = new ServiceProcessInstaller();
            var serviceInstaller = new System.ServiceProcess.ServiceInstaller();

            processInstaller.Account = ServiceAccount.LocalSystem;
            serviceInstaller.StartType = ServiceStartMode.Automatic;
            serviceInstaller.ServiceName = "Connectster Server";
            Installers.Add(serviceInstaller);
            Installers.Add(processInstaller);
        }
    }
}
