#define DEBUG
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace WSAdminPaqWrapper
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            /*    System.Diagnostics.EventLog eventLogService = new System.Diagnostics.EventLog();

                if (!System.Diagnostics.EventLog.SourceExists("WSAdminPaqWrapperService"))
                {
                    System.Diagnostics.EventLog.CreateEventSource("WSAdminPaqWrapperService", "WSAdminPaqWrapperLog");
                }

                eventLogService.Source = "WSAdminPaqWrapperService";
                eventLogService.Log = "WSAdminPaqWrapperLog";

                eventLogService.WriteEntry("DEBUG ETL Process Execution BEGIN.");

                CommonAdminPaq.AdminPaqLib apl = new CommonAdminPaq.AdminPaqLib();
                apl.SetDllFolder();

                Process.Main.Execute(eventLogService);
                eventLogService.WriteEntry("DEBUG ETL Process Execution END.");*/

            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
			    { 
				    new WSAdminPaqWrapper() 
			    };
            ServiceBase.Run(ServicesToRun);

        }
    }
}
