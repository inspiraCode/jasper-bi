using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Timers;

namespace WSAdminPaqWrapper
{
    public partial class WSAdminPaqWrapper : ServiceBase
    {
        private Timer tmrDelay;

        public WSAdminPaqWrapper()
        {
            InitializeComponent();
            if (!System.Diagnostics.EventLog.SourceExists("WSAdminPaqWrapperService"))
            {
                System.Diagnostics.EventLog.CreateEventSource("WSAdminPaqWrapperService", "WSAdminPaqWrapperLog");
            }

            eventLogService.Source = "WSAdminPaqWrapperService";
            eventLogService.Log = "WSAdminPaqWrapperLog";
            CommonAdminPaq.AdminPaqLib apl = new CommonAdminPaq.AdminPaqLib();
            apl.SetDllFolder();
        }

        protected override void OnStart(string[] args)
        {
            eventLogService.WriteEntry("WSAdminPaqWrapper Service started.");
            // Timer start
            tmrDelay = new Timer(30000);
            tmrDelay.Elapsed += new ElapsedEventHandler(timerDelay_Tick);
            tmrDelay.Enabled = true;
            tmrDelay.Start();
        }

        protected override void OnStop()
        {
            tmrDelay.Stop();
            eventLogService.WriteEntry("WSAdminPaqWrapper Service stoped.");
        }

        private void timerDelay_Tick(object sender, EventArgs e)
        {
            try {
                tmrDelay.Interval = 1800000;
                eventLogService.WriteEntry("PERIODICAL ETL Process Execution BEGIN.");
                Process.Main.Execute();
                eventLogService.WriteEntry("PERIODICAL ETL Process Execution END.");
            }catch(Exception ex){
                eventLogService.WriteEntry("Exception while running process. " + ex.Message + "::" + ex.StackTrace, EventLogEntryType.Error);
            }
        }
    }
}
