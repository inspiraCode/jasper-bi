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
        }

        protected override void OnStart(string[] args)
        {
            eventLogService.WriteEntry("WSAdminPaqWrapper Service started.", EventLogEntryType.Information, 0, 0);
            // Timer start
            tmrDelay = new Timer(30000);
            tmrDelay.Elapsed += new ElapsedEventHandler(timerDelay_Tick);
            tmrDelay.Enabled = true;
            tmrDelay.Start();
        }

        protected override void OnStop()
        {
            tmrDelay.Stop();
            eventLogService.WriteEntry("WSAdminPaqWrapper Service stoped.", EventLogEntryType.Information, 1, 0);
        }

        private void timerDelay_Tick(object sender, EventArgs e)
        {
            try {
                if (tmrDelay.Interval == 30000)
                {
                    tmrDelay.Stop();
                    tmrDelay.Interval = 300000; // 5 minutos
                    tmrDelay.Start();
                }

                eventLogService.WriteEntry("SALES ETL BEGIN.", EventLogEntryType.Information, 2, 1);
                System.Threading.Thread.Sleep(15000);
                eventLogService.WriteEntry("DUE ACCOUNTS COLLECTABLE ETL BEGIN.", EventLogEntryType.Information, 2, 1);
                System.Threading.Thread.Sleep(15000);
                eventLogService.WriteEntry("TO DUE ACCOUNTS COLLECTABLE ETL BEGIN.", EventLogEntryType.Information, 2, 1);
                System.Threading.Thread.Sleep(15000);
                eventLogService.WriteEntry("COLLECTED ETL BEGIN.", EventLogEntryType.Information, 2, 1);
            }catch(Exception ex){
                eventLogService.WriteEntry("Exception while running process. " + ex.Message + "::" + ex.StackTrace, EventLogEntryType.Error, 4, 1);
            }
        }
    }
}
