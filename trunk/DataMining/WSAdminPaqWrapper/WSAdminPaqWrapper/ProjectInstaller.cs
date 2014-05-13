using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Diagnostics;
using System.ServiceProcess;


namespace WSAdminPaqWrapper
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();

            // Remove the default Event Log Installer
            EventLogInstaller DefaultInstaller = null;
            
            foreach (Installer installer in apwServiceInstaller.Installers)
            {
                if (installer is EventLogInstaller)
                {
                    DefaultInstaller = (EventLogInstaller)installer;
                    break;
                }
            }
            if (DefaultInstaller != null)
            {
                apwServiceInstaller.Installers.Remove(DefaultInstaller);
            }
        }

        private void apwServiceProcessInstaller_BeforeInstall(object sender, InstallEventArgs e)
        {
            List<ServiceController> services = ServiceController.GetServices().ToList();

            foreach (ServiceController s in services)
            {
                if (s.ServiceName == this.apwServiceInstaller.ServiceName)
                {
                    ServiceInstaller ServiceInstallerObj = new ServiceInstaller();
                    ServiceInstallerObj.Context = new System.Configuration.Install.InstallContext();
                    ServiceInstallerObj.Context = Context;
                    ServiceInstallerObj.ServiceName = "WSAdminPaqWrapper";
                    ServiceInstallerObj.Uninstall(null);

                    ServiceInstallerObj.StartType = ServiceStartMode.Automatic;
                    ServiceInstallerObj.DisplayName = "WSAdminPaqWrapper";
                    ServiceInstallerObj.Description = "AdminPaq wrapper for postgres database";
                }
            }
        }
    }
}
