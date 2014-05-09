using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace WSAdminPaqWrapper
{
    public partial class WSAdminPaqWrapper : ServiceBase
    {
        public WSAdminPaqWrapper()
        {
            InitializeComponent();
            if (!System.Diagnostics.EventLog.SourceExists("WSAdminPaqWrapper"))
            {
                System.Diagnostics.EventLog.CreateEventSource("WSAdminPaqWrapper", "WSAdminPaqWrapperLog");
            }

            eventLogService.Source = "WSAdminPaqWrapper";
            eventLogService.Log = "WSAdminPaqWrapperLog";
        }

        protected override void OnStart(string[] args)
        {
            eventLogService.WriteEntry("WSAdminPaqWrapper Service started.");
            // Retrieve information
            try {
                Process.Main.Execute();
            }catch(Exception ex){
                eventLogService.WriteEntry("Exception while running process. " + ex.Message, EventLogEntryType.Error);
            }
            Process.Main.Execute();
            // Timer start
            timerDelay.Start();
        }

        protected override void OnStop()
        {
            timerDelay.Stop();
            eventLogService.WriteEntry("WSAdminPaqWrapper Service stoped.");
        }

        private void timerDelay_Tick(object sender, EventArgs e)
        {
            try {
                Process.Main.Execute();
            }catch(Exception ex){
                eventLogService.WriteEntry("Exception while running process. " + ex.Message, EventLogEntryType.Error);
            }
        }
    }
}
