namespace Contoso.WinForms.Demo
{
    partial class MainForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tabs = new System.Windows.Forms.TabControl();
            this.appCenter = new System.Windows.Forms.TabPage();
            this.logBox = new System.Windows.Forms.GroupBox();
            this.WriteLog = new System.Windows.Forms.Button();
            this.LogTag = new System.Windows.Forms.TextBox();
            this.LogMessage = new System.Windows.Forms.TextBox();
            this.logMessageLabel = new System.Windows.Forms.Label();
            this.logTagLabel = new System.Windows.Forms.Label();
            this.logLevelLabel = new System.Windows.Forms.Label();
            this.LogLevel = new System.Windows.Forms.ComboBox();
            this.appCenterLogLevelLabel = new System.Windows.Forms.Label();
            this.AppCenterLogLevel = new System.Windows.Forms.ComboBox();
            this.AppCenterEnabled = new System.Windows.Forms.CheckBox();
            this.analytics = new System.Windows.Forms.TabPage();
            this.eventBox = new System.Windows.Forms.GroupBox();
            this.TrackEvent = new System.Windows.Forms.Button();
            this.eventProperties = new System.Windows.Forms.DataGridView();
            this.Key = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Value = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.EventName = new System.Windows.Forms.TextBox();
            this.eventNameLabel = new System.Windows.Forms.Label();
            this.AnalyticsEnabled = new System.Windows.Forms.CheckBox();
            this.crashes = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.CrashInsideAsyncTask = new System.Windows.Forms.Button();
            this.CrashWithNullReference = new System.Windows.Forms.Button();
            this.CrashWithAggregateException = new System.Windows.Forms.Button();
            this.CrashWithDivisionByZero = new System.Windows.Forms.Button();
            this.CrashWithNonSerializableException = new System.Windows.Forms.Button();
            this.CrashWithTestException = new System.Windows.Forms.Button();
            this.CrashesEnabled = new System.Windows.Forms.CheckBox();
            this.others = new System.Windows.Forms.TabPage();
            this.tabs.SuspendLayout();
            this.appCenter.SuspendLayout();
            this.logBox.SuspendLayout();
            this.analytics.SuspendLayout();
            this.eventBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.eventProperties)).BeginInit();
            this.crashes.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabs
            // 
            this.tabs.Controls.Add(this.appCenter);
            this.tabs.Controls.Add(this.analytics);
            this.tabs.Controls.Add(this.crashes);
            this.tabs.Controls.Add(this.others);
            this.tabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabs.Location = new System.Drawing.Point(0, 0);
            this.tabs.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.tabs.Name = "tabs";
            this.tabs.SelectedIndex = 0;
            this.tabs.Size = new System.Drawing.Size(768, 528);
            this.tabs.TabIndex = 0;
            this.tabs.SelectedIndexChanged += new System.EventHandler(this.Tabs_SelectedIndexChanged);
            // 
            // appCenter
            // 
            this.appCenter.Controls.Add(this.logBox);
            this.appCenter.Controls.Add(this.appCenterLogLevelLabel);
            this.appCenter.Controls.Add(this.AppCenterLogLevel);
            this.appCenter.Controls.Add(this.AppCenterEnabled);
            this.appCenter.Location = new System.Drawing.Point(8, 39);
            this.appCenter.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.appCenter.Name = "appCenter";
            this.appCenter.Padding = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.appCenter.Size = new System.Drawing.Size(752, 481);
            this.appCenter.TabIndex = 0;
            this.appCenter.Text = "App Center";
            this.appCenter.UseVisualStyleBackColor = true;
            // 
            // logBox
            // 
            this.logBox.Controls.Add(this.WriteLog);
            this.logBox.Controls.Add(this.LogTag);
            this.logBox.Controls.Add(this.LogMessage);
            this.logBox.Controls.Add(this.logMessageLabel);
            this.logBox.Controls.Add(this.logTagLabel);
            this.logBox.Controls.Add(this.logLevelLabel);
            this.logBox.Controls.Add(this.LogLevel);
            this.logBox.Location = new System.Drawing.Point(16, 121);
            this.logBox.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.logBox.Name = "logBox";
            this.logBox.Padding = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.logBox.Size = new System.Drawing.Size(720, 250);
            this.logBox.TabIndex = 5;
            this.logBox.TabStop = false;
            this.logBox.Text = "Log";
            // 
            // WriteLog
            // 
            this.WriteLog.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.WriteLog.Location = new System.Drawing.Point(19, 194);
            this.WriteLog.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.WriteLog.Name = "WriteLog";
            this.WriteLog.Size = new System.Drawing.Size(684, 44);
            this.WriteLog.TabIndex = 11;
            this.WriteLog.Text = "Write Log";
            this.WriteLog.UseVisualStyleBackColor = true;
            this.WriteLog.Click += new System.EventHandler(this.WriteLog_Click);
            // 
            // LogTag
            // 
            this.LogTag.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LogTag.Location = new System.Drawing.Point(212, 36);
            this.LogTag.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.LogTag.Name = "LogTag";
            this.LogTag.Size = new System.Drawing.Size(487, 31);
            this.LogTag.TabIndex = 10;
            // 
            // LogMessage
            // 
            this.LogMessage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LogMessage.Location = new System.Drawing.Point(212, 81);
            this.LogMessage.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.LogMessage.Name = "LogMessage";
            this.LogMessage.Size = new System.Drawing.Size(487, 31);
            this.LogMessage.TabIndex = 9;
            // 
            // logMessageLabel
            // 
            this.logMessageLabel.Location = new System.Drawing.Point(12, 78);
            this.logMessageLabel.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.logMessageLabel.Name = "logMessageLabel";
            this.logMessageLabel.Size = new System.Drawing.Size(188, 44);
            this.logMessageLabel.TabIndex = 8;
            this.logMessageLabel.Text = "Log Message";
            this.logMessageLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // logTagLabel
            // 
            this.logTagLabel.Location = new System.Drawing.Point(12, 32);
            this.logTagLabel.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.logTagLabel.Name = "logTagLabel";
            this.logTagLabel.Size = new System.Drawing.Size(188, 44);
            this.logTagLabel.TabIndex = 7;
            this.logTagLabel.Text = "Log Tag";
            this.logTagLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // logLevelLabel
            // 
            this.logLevelLabel.Location = new System.Drawing.Point(12, 128);
            this.logLevelLabel.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.logLevelLabel.Name = "logLevelLabel";
            this.logLevelLabel.Size = new System.Drawing.Size(188, 44);
            this.logLevelLabel.TabIndex = 6;
            this.logLevelLabel.Text = "Log Level";
            this.logLevelLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // LogLevel
            // 
            this.LogLevel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LogLevel.FormattingEnabled = true;
            this.LogLevel.Items.AddRange(new object[] {
            "Verbose",
            "Debug",
            "Info",
            "Warning",
            "Error"});
            this.LogLevel.Location = new System.Drawing.Point(212, 131);
            this.LogLevel.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.LogLevel.Name = "LogLevel";
            this.LogLevel.Size = new System.Drawing.Size(487, 33);
            this.LogLevel.TabIndex = 5;
            // 
            // appCenterLogLevelLabel
            // 
            this.appCenterLogLevelLabel.Location = new System.Drawing.Point(40, 69);
            this.appCenterLogLevelLabel.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.appCenterLogLevelLabel.Name = "appCenterLogLevelLabel";
            this.appCenterLogLevelLabel.Size = new System.Drawing.Size(176, 44);
            this.appCenterLogLevelLabel.TabIndex = 4;
            this.appCenterLogLevelLabel.Text = "Log Level";
            this.appCenterLogLevelLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // AppCenterLogLevel
            // 
            this.AppCenterLogLevel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.AppCenterLogLevel.FormattingEnabled = true;
            this.AppCenterLogLevel.Items.AddRange(new object[] {
            "Verbose",
            "Debug",
            "Info",
            "Warning",
            "Error"});
            this.AppCenterLogLevel.Location = new System.Drawing.Point(228, 72);
            this.AppCenterLogLevel.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.AppCenterLogLevel.Name = "AppCenterLogLevel";
            this.AppCenterLogLevel.Size = new System.Drawing.Size(487, 33);
            this.AppCenterLogLevel.TabIndex = 3;
            this.AppCenterLogLevel.SelectedIndexChanged += new System.EventHandler(this.AppCenterLogLevel_SelectedIndexChanged);
            // 
            // AppCenterEnabled
            // 
            this.AppCenterEnabled.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.AppCenterEnabled.Location = new System.Drawing.Point(16, 11);
            this.AppCenterEnabled.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.AppCenterEnabled.Name = "AppCenterEnabled";
            this.AppCenterEnabled.Size = new System.Drawing.Size(720, 46);
            this.AppCenterEnabled.TabIndex = 1;
            this.AppCenterEnabled.Text = "App Center Enabled";
            this.AppCenterEnabled.UseVisualStyleBackColor = true;
            this.AppCenterEnabled.CheckedChanged += new System.EventHandler(this.AppCenterEnabled_CheckedChanged);
            // 
            // analytics
            // 
            this.analytics.Controls.Add(this.eventBox);
            this.analytics.Controls.Add(this.AnalyticsEnabled);
            this.analytics.Location = new System.Drawing.Point(8, 39);
            this.analytics.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.analytics.Name = "analytics";
            this.analytics.Padding = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.analytics.Size = new System.Drawing.Size(752, 481);
            this.analytics.TabIndex = 1;
            this.analytics.Text = "Analytics";
            this.analytics.UseVisualStyleBackColor = true;
            // 
            // eventBox
            // 
            this.eventBox.Controls.Add(this.TrackEvent);
            this.eventBox.Controls.Add(this.eventProperties);
            this.eventBox.Controls.Add(this.EventName);
            this.eventBox.Controls.Add(this.eventNameLabel);
            this.eventBox.Location = new System.Drawing.Point(16, 69);
            this.eventBox.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.eventBox.Name = "eventBox";
            this.eventBox.Padding = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.eventBox.Size = new System.Drawing.Size(720, 368);
            this.eventBox.TabIndex = 3;
            this.eventBox.TabStop = false;
            this.eventBox.Text = "Event";
            // 
            // TrackEvent
            // 
            this.TrackEvent.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TrackEvent.Location = new System.Drawing.Point(19, 311);
            this.TrackEvent.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.TrackEvent.Name = "TrackEvent";
            this.TrackEvent.Size = new System.Drawing.Size(684, 44);
            this.TrackEvent.TabIndex = 14;
            this.TrackEvent.Text = "Track Event";
            this.TrackEvent.UseVisualStyleBackColor = true;
            this.TrackEvent.Click += new System.EventHandler(this.TrackEvent_Click);
            // 
            // eventProperties
            // 
            this.eventProperties.AllowUserToResizeColumns = false;
            this.eventProperties.AllowUserToResizeRows = false;
            this.eventProperties.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.eventProperties.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.eventProperties.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Key,
            this.Value});
            this.eventProperties.Location = new System.Drawing.Point(19, 85);
            this.eventProperties.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.eventProperties.Name = "eventProperties";
            this.eventProperties.RowHeadersWidth = 82;
            this.eventProperties.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.eventProperties.Size = new System.Drawing.Size(684, 211);
            this.eventProperties.TabIndex = 13;
            // 
            // Key
            // 
            this.Key.HeaderText = "Key";
            this.Key.MaxInputLength = 64;
            this.Key.MinimumWidth = 10;
            this.Key.Name = "Key";
            this.Key.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Key.Width = 200;
            // 
            // Value
            // 
            this.Value.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Value.HeaderText = "Value";
            this.Value.MaxInputLength = 64;
            this.Value.MinimumWidth = 10;
            this.Value.Name = "Value";
            this.Value.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // EventName
            // 
            this.EventName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.EventName.Location = new System.Drawing.Point(212, 35);
            this.EventName.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.EventName.MaxLength = 256;
            this.EventName.Name = "EventName";
            this.EventName.Size = new System.Drawing.Size(487, 31);
            this.EventName.TabIndex = 12;
            // 
            // eventNameLabel
            // 
            this.eventNameLabel.Location = new System.Drawing.Point(12, 31);
            this.eventNameLabel.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.eventNameLabel.Name = "eventNameLabel";
            this.eventNameLabel.Size = new System.Drawing.Size(188, 44);
            this.eventNameLabel.TabIndex = 11;
            this.eventNameLabel.Text = "Event Name";
            this.eventNameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // AnalyticsEnabled
            // 
            this.AnalyticsEnabled.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.AnalyticsEnabled.Location = new System.Drawing.Point(16, 11);
            this.AnalyticsEnabled.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.AnalyticsEnabled.Name = "AnalyticsEnabled";
            this.AnalyticsEnabled.Size = new System.Drawing.Size(720, 46);
            this.AnalyticsEnabled.TabIndex = 2;
            this.AnalyticsEnabled.Text = "Analytics Enabled";
            this.AnalyticsEnabled.UseVisualStyleBackColor = true;
            this.AnalyticsEnabled.CheckedChanged += new System.EventHandler(this.AnalyticsEnabled_CheckedChanged);
            // 
            // crashes
            // 
            this.crashes.Controls.Add(this.groupBox1);
            this.crashes.Controls.Add(this.CrashesEnabled);
            this.crashes.Location = new System.Drawing.Point(8, 39);
            this.crashes.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.crashes.Name = "crashes";
            this.crashes.Size = new System.Drawing.Size(752, 481);
            this.crashes.TabIndex = 2;
            this.crashes.Text = "Crashes";
            this.crashes.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.CrashInsideAsyncTask);
            this.groupBox1.Controls.Add(this.CrashWithNullReference);
            this.groupBox1.Controls.Add(this.CrashWithAggregateException);
            this.groupBox1.Controls.Add(this.CrashWithDivisionByZero);
            this.groupBox1.Controls.Add(this.CrashWithNonSerializableException);
            this.groupBox1.Controls.Add(this.CrashWithTestException);
            this.groupBox1.Location = new System.Drawing.Point(16, 69);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.groupBox1.Size = new System.Drawing.Size(720, 393);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Crashes";
            // 
            // CrashInsideAsyncTask
            // 
            this.CrashInsideAsyncTask.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.CrashInsideAsyncTask.Location = new System.Drawing.Point(15, 322);
            this.CrashInsideAsyncTask.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.CrashInsideAsyncTask.Name = "CrashInsideAsyncTask";
            this.CrashInsideAsyncTask.Size = new System.Drawing.Size(684, 44);
            this.CrashInsideAsyncTask.TabIndex = 19;
            this.CrashInsideAsyncTask.Text = "Async task crash";
            this.CrashInsideAsyncTask.UseVisualStyleBackColor = true;
            this.CrashInsideAsyncTask.Click += new System.EventHandler(this.CrashInsideAsyncTask_Click);
            // 
            // CrashWithNullReference
            // 
            this.CrashWithNullReference.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.CrashWithNullReference.Location = new System.Drawing.Point(15, 266);
            this.CrashWithNullReference.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.CrashWithNullReference.Name = "CrashWithNullReference";
            this.CrashWithNullReference.Size = new System.Drawing.Size(684, 44);
            this.CrashWithNullReference.TabIndex = 18;
            this.CrashWithNullReference.Text = "Crash with null reference";
            this.CrashWithNullReference.UseVisualStyleBackColor = true;
            this.CrashWithNullReference.Click += new System.EventHandler(this.CrashWithNullReference_Click);
            // 
            // CrashWithAggregateException
            // 
            this.CrashWithAggregateException.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.CrashWithAggregateException.Location = new System.Drawing.Point(15, 210);
            this.CrashWithAggregateException.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.CrashWithAggregateException.Name = "CrashWithAggregateException";
            this.CrashWithAggregateException.Size = new System.Drawing.Size(684, 44);
            this.CrashWithAggregateException.TabIndex = 17;
            this.CrashWithAggregateException.Text = "Aggregate Exception";
            this.CrashWithAggregateException.UseVisualStyleBackColor = true;
            this.CrashWithAggregateException.Click += new System.EventHandler(this.CrashWithAggregateException_Click);
            // 
            // CrashWithDivisionByZero
            // 
            this.CrashWithDivisionByZero.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.CrashWithDivisionByZero.Location = new System.Drawing.Point(15, 154);
            this.CrashWithDivisionByZero.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.CrashWithDivisionByZero.Name = "CrashWithDivisionByZero";
            this.CrashWithDivisionByZero.Size = new System.Drawing.Size(684, 44);
            this.CrashWithDivisionByZero.TabIndex = 16;
            this.CrashWithDivisionByZero.Text = "Divide by zero";
            this.CrashWithDivisionByZero.UseVisualStyleBackColor = true;
            this.CrashWithDivisionByZero.Click += new System.EventHandler(this.CrashWithDivisionByZero_Click);
            // 
            // CrashWithNonSerializableException
            // 
            this.CrashWithNonSerializableException.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.CrashWithNonSerializableException.Location = new System.Drawing.Point(15, 98);
            this.CrashWithNonSerializableException.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.CrashWithNonSerializableException.Name = "CrashWithNonSerializableException";
            this.CrashWithNonSerializableException.Size = new System.Drawing.Size(684, 44);
            this.CrashWithNonSerializableException.TabIndex = 15;
            this.CrashWithNonSerializableException.Text = "Generate non serializable Exception";
            this.CrashWithNonSerializableException.UseVisualStyleBackColor = true;
            this.CrashWithNonSerializableException.Click += new System.EventHandler(this.CrashWithNonSerializableException_Click);
            // 
            // CrashWithTestException
            // 
            this.CrashWithTestException.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.CrashWithTestException.Location = new System.Drawing.Point(15, 42);
            this.CrashWithTestException.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.CrashWithTestException.Name = "CrashWithTestException";
            this.CrashWithTestException.Size = new System.Drawing.Size(684, 44);
            this.CrashWithTestException.TabIndex = 14;
            this.CrashWithTestException.Text = "Call Crashes.GenerateTestCrash (debug only)";
            this.CrashWithTestException.UseVisualStyleBackColor = true;
            this.CrashWithTestException.Click += new System.EventHandler(this.CrashWithTestException_Click);
            // 
            // CrashesEnabled
            // 
            this.CrashesEnabled.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.CrashesEnabled.Location = new System.Drawing.Point(16, 11);
            this.CrashesEnabled.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.CrashesEnabled.Name = "CrashesEnabled";
            this.CrashesEnabled.Size = new System.Drawing.Size(720, 46);
            this.CrashesEnabled.TabIndex = 3;
            this.CrashesEnabled.Text = "Crashes Enabled";
            this.CrashesEnabled.UseVisualStyleBackColor = true;
            this.CrashesEnabled.CheckedChanged += new System.EventHandler(this.CrashesEnabled_CheckedChanged);
            // 
            // others
            // 
            this.others.Location = new System.Drawing.Point(8, 39);
            this.others.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.others.Name = "others";
            this.others.Size = new System.Drawing.Size(752, 481);
            this.others.TabIndex = 3;
            this.others.Text = "Others";
            this.others.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(768, 528);
            this.Controls.Add(this.tabs);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "App Center Puppet App";
            this.tabs.ResumeLayout(false);
            this.appCenter.ResumeLayout(false);
            this.logBox.ResumeLayout(false);
            this.logBox.PerformLayout();
            this.analytics.ResumeLayout(false);
            this.eventBox.ResumeLayout(false);
            this.eventBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.eventProperties)).EndInit();
            this.crashes.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabs;
        private System.Windows.Forms.TabPage appCenter;
        private System.Windows.Forms.TabPage analytics;
        private System.Windows.Forms.TabPage crashes;
        private System.Windows.Forms.TabPage others;
        private System.Windows.Forms.CheckBox AppCenterEnabled;
        private System.Windows.Forms.Label appCenterLogLevelLabel;
        private System.Windows.Forms.ComboBox AppCenterLogLevel;
        private System.Windows.Forms.GroupBox logBox;
        private System.Windows.Forms.Button WriteLog;
        private System.Windows.Forms.TextBox LogTag;
        private System.Windows.Forms.TextBox LogMessage;
        private System.Windows.Forms.Label logMessageLabel;
        private System.Windows.Forms.Label logTagLabel;
        private System.Windows.Forms.Label logLevelLabel;
        private System.Windows.Forms.ComboBox LogLevel;
        private System.Windows.Forms.GroupBox eventBox;
        private System.Windows.Forms.CheckBox AnalyticsEnabled;
        private System.Windows.Forms.TextBox EventName;
        private System.Windows.Forms.Label eventNameLabel;
        private System.Windows.Forms.DataGridView eventProperties;
        private System.Windows.Forms.DataGridViewTextBoxColumn Key;
        private System.Windows.Forms.DataGridViewTextBoxColumn Value;
        private System.Windows.Forms.Button TrackEvent;
        private System.Windows.Forms.CheckBox CrashesEnabled;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button CrashWithTestException;
        private System.Windows.Forms.Button CrashWithNonSerializableException;
        private System.Windows.Forms.Button CrashWithDivisionByZero;
        private System.Windows.Forms.Button CrashWithAggregateException;
        private System.Windows.Forms.Button CrashWithNullReference;
        private System.Windows.Forms.Button CrashInsideAsyncTask;
    }
}

