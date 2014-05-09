namespace WSAdminPaqWrapper
{
    partial class WSAdminPaqWrapper
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.eventLogService = new System.Diagnostics.EventLog();
            this.timerDelay = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.eventLogService)).BeginInit();
            // 
            // timerDelay
            // 
            this.timerDelay.Interval = 1800000;
            this.timerDelay.Tick += new System.EventHandler(this.timerDelay_Tick);
            // 
            // WSAdminPaqWrapper
            // 
            this.CanHandlePowerEvent = true;
            this.CanHandleSessionChangeEvent = true;
            this.CanPauseAndContinue = true;
            this.CanShutdown = true;
            this.ServiceName = "WSAdminPaqWrapper";
            ((System.ComponentModel.ISupportInitialize)(this.eventLogService)).EndInit();

        }

        #endregion

        private System.Diagnostics.EventLog eventLogService;
        private System.Windows.Forms.Timer timerDelay;
    }
}
