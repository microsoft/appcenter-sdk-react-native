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
            this.Tabs = new System.Windows.Forms.TabControl();
            this.AppCenterTab = new System.Windows.Forms.TabPage();
            this.LogBox = new System.Windows.Forms.GroupBox();
            this.WriteLog = new System.Windows.Forms.Button();
            this.LogTag = new System.Windows.Forms.TextBox();
            this.LogMessage = new System.Windows.Forms.TextBox();
            this.LogMessageLabel = new System.Windows.Forms.Label();
            this.LogTagLabel = new System.Windows.Forms.Label();
            this.LogLevelLabel = new System.Windows.Forms.Label();
            this.LogLevelValue = new System.Windows.Forms.ComboBox();
            this.AppCenterLogLevelLabel = new System.Windows.Forms.Label();
            this.AppCenterLogLevel = new System.Windows.Forms.ComboBox();
            this.AppCenterEnabled = new System.Windows.Forms.CheckBox();
            this.AnalyticsTab = new System.Windows.Forms.TabPage();
            this.EventBox = new System.Windows.Forms.GroupBox();
            this.TrackEvent = new System.Windows.Forms.Button();
            this.EventProperties = new System.Windows.Forms.DataGridView();
            this.Key = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Value = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.EventName = new System.Windows.Forms.TextBox();
            this.EventNameLabel = new System.Windows.Forms.Label();
            this.AnalyticsEnabled = new System.Windows.Forms.CheckBox();
            this.CrashesTab = new System.Windows.Forms.TabPage();
            this.OthersTab = new System.Windows.Forms.TabPage();
            this.CrashBox = new System.Windows.Forms.GroupBox();
            this.CrashInsideAsyncTask = new System.Windows.Forms.Button();
            this.CrashWithNullReference = new System.Windows.Forms.Button();
            this.CrashWithAggregateException = new System.Windows.Forms.Button();
            this.CrashWithDivisionByZero = new System.Windows.Forms.Button();
            this.CrashWithNonSerializableException = new System.Windows.Forms.Button();
            this.CrashWithTestException = new System.Windows.Forms.Button();
            this.CrashesEnabled = new System.Windows.Forms.CheckBox();
            this.Tabs.SuspendLayout();
            this.AppCenterTab.SuspendLayout();
            this.LogBox.SuspendLayout();
            this.AnalyticsTab.SuspendLayout();
            this.EventBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.EventProperties)).BeginInit();
            this.CrashesTab.SuspendLayout();
            this.CrashBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // Tabs
            // 
            this.Tabs.Controls.Add(this.AppCenterTab);
            this.Tabs.Controls.Add(this.AnalyticsTab);
            this.Tabs.Controls.Add(this.CrashesTab);
            this.Tabs.Controls.Add(this.OthersTab);
            this.Tabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Tabs.Location = new System.Drawing.Point(0, 0);
            this.Tabs.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.Tabs.Name = "Tabs";
            this.Tabs.SelectedIndex = 0;
            this.Tabs.Size = new System.Drawing.Size(768, 515);
            this.Tabs.TabIndex = 0;
            this.Tabs.SelectedIndexChanged += new System.EventHandler(this.Tabs_SelectedIndexChanged);
            // 
            // AppCenterTab
            // 
            this.AppCenterTab.Controls.Add(this.LogBox);
            this.AppCenterTab.Controls.Add(this.AppCenterLogLevelLabel);
            this.AppCenterTab.Controls.Add(this.AppCenterLogLevel);
            this.AppCenterTab.Controls.Add(this.AppCenterEnabled);
            this.AppCenterTab.Location = new System.Drawing.Point(8, 39);
            this.AppCenterTab.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.AppCenterTab.Name = "AppCenterTab";
            this.AppCenterTab.Padding = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.AppCenterTab.Size = new System.Drawing.Size(752, 455);
            this.AppCenterTab.TabIndex = 0;
            this.AppCenterTab.Text = "App Center";
            this.AppCenterTab.UseVisualStyleBackColor = true;
            // 
            // LogBox
            // 
            this.LogBox.Controls.Add(this.WriteLog);
            this.LogBox.Controls.Add(this.LogTag);
            this.LogBox.Controls.Add(this.LogMessage);
            this.LogBox.Controls.Add(this.LogMessageLabel);
            this.LogBox.Controls.Add(this.LogTagLabel);
            this.LogBox.Controls.Add(this.LogLevelLabel);
            this.LogBox.Controls.Add(this.LogLevelValue);
            this.LogBox.Location = new System.Drawing.Point(16, 121);
            this.LogBox.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.LogBox.Name = "LogBox";
            this.LogBox.Padding = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.LogBox.Size = new System.Drawing.Size(720, 250);
            this.LogBox.TabIndex = 5;
            this.LogBox.TabStop = false;
            this.LogBox.Text = "Log";
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
            // LogMessageLabel
            // 
            this.LogMessageLabel.Location = new System.Drawing.Point(12, 78);
            this.LogMessageLabel.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.LogMessageLabel.Name = "LogMessageLabel";
            this.LogMessageLabel.Size = new System.Drawing.Size(188, 44);
            this.LogMessageLabel.TabIndex = 8;
            this.LogMessageLabel.Text = "Log Message";
            this.LogMessageLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // LogTagLabel
            // 
            this.LogTagLabel.Location = new System.Drawing.Point(12, 32);
            this.LogTagLabel.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.LogTagLabel.Name = "LogTagLabel";
            this.LogTagLabel.Size = new System.Drawing.Size(188, 44);
            this.LogTagLabel.TabIndex = 7;
            this.LogTagLabel.Text = "Log Tag";
            this.LogTagLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // LogLevelLabel
            // 
            this.LogLevelLabel.Location = new System.Drawing.Point(12, 128);
            this.LogLevelLabel.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.LogLevelLabel.Name = "LogLevelLabel";
            this.LogLevelLabel.Size = new System.Drawing.Size(188, 44);
            this.LogLevelLabel.TabIndex = 6;
            this.LogLevelLabel.Text = "Log Level";
            this.LogLevelLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // LogLevelValue
            // 
            this.LogLevelValue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LogLevelValue.FormattingEnabled = true;
            this.LogLevelValue.Items.AddRange(new object[] {
            "Verbose",
            "Debug",
            "Info",
            "Warning",
            "Error"});
            this.LogLevelValue.Location = new System.Drawing.Point(212, 131);
            this.LogLevelValue.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.LogLevelValue.Name = "LogLevelValue";
            this.LogLevelValue.Size = new System.Drawing.Size(487, 33);
            this.LogLevelValue.TabIndex = 5;
            // 
            // AppCenterLogLevelLabel
            // 
            this.AppCenterLogLevelLabel.Location = new System.Drawing.Point(40, 69);
            this.AppCenterLogLevelLabel.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.AppCenterLogLevelLabel.Name = "AppCenterLogLevelLabel";
            this.AppCenterLogLevelLabel.Size = new System.Drawing.Size(176, 44);
            this.AppCenterLogLevelLabel.TabIndex = 4;
            this.AppCenterLogLevelLabel.Text = "Log Level";
            this.AppCenterLogLevelLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
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
            // AnalyticsTab
            // 
            this.AnalyticsTab.Controls.Add(this.EventBox);
            this.AnalyticsTab.Controls.Add(this.AnalyticsEnabled);
            this.AnalyticsTab.Location = new System.Drawing.Point(8, 39);
            this.AnalyticsTab.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.AnalyticsTab.Name = "AnalyticsTab";
            this.AnalyticsTab.Padding = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.AnalyticsTab.Size = new System.Drawing.Size(752, 455);
            this.AnalyticsTab.TabIndex = 1;
            this.AnalyticsTab.Text = "Analytics";
            this.AnalyticsTab.UseVisualStyleBackColor = true;
            // 
            // EventBox
            // 
            this.EventBox.Controls.Add(this.TrackEvent);
            this.EventBox.Controls.Add(this.EventProperties);
            this.EventBox.Controls.Add(this.EventName);
            this.EventBox.Controls.Add(this.EventNameLabel);
            this.EventBox.Location = new System.Drawing.Point(16, 69);
            this.EventBox.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.EventBox.Name = "EventBox";
            this.EventBox.Padding = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.EventBox.Size = new System.Drawing.Size(720, 368);
            this.EventBox.TabIndex = 3;
            this.EventBox.TabStop = false;
            this.EventBox.Text = "Event";
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
            // EventProperties
            // 
            this.EventProperties.AllowUserToResizeColumns = false;
            this.EventProperties.AllowUserToResizeRows = false;
            this.EventProperties.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.EventProperties.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.EventProperties.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Key,
            this.Value});
            this.EventProperties.Location = new System.Drawing.Point(19, 85);
            this.EventProperties.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.EventProperties.Name = "EventProperties";
            this.EventProperties.RowHeadersWidth = 82;
            this.EventProperties.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.EventProperties.Size = new System.Drawing.Size(684, 211);
            this.EventProperties.TabIndex = 13;
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
            // EventNameLabel
            // 
            this.EventNameLabel.Location = new System.Drawing.Point(12, 31);
            this.EventNameLabel.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.EventNameLabel.Name = "EventNameLabel";
            this.EventNameLabel.Size = new System.Drawing.Size(188, 44);
            this.EventNameLabel.TabIndex = 11;
            this.EventNameLabel.Text = "Event Name";
            this.EventNameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
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
            // CrashesTab
            // 
            this.CrashesTab.Controls.Add(this.CrashBox);
            this.CrashesTab.Controls.Add(this.CrashesEnabled);
            this.CrashesTab.Location = new System.Drawing.Point(8, 39);
            this.CrashesTab.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.CrashesTab.Name = "CrashesTab";
            this.CrashesTab.Size = new System.Drawing.Size(752, 468);
            this.CrashesTab.TabIndex = 2;
            this.CrashesTab.Text = "Crashes";
            this.CrashesTab.UseVisualStyleBackColor = true;
            // 
            // OthersTab
            // 
            this.OthersTab.Location = new System.Drawing.Point(8, 39);
            this.OthersTab.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.OthersTab.Name = "OthersTab";
            this.OthersTab.Size = new System.Drawing.Size(752, 455);
            this.OthersTab.TabIndex = 3;
            this.OthersTab.Text = "Others";
            this.OthersTab.UseVisualStyleBackColor = true;
            // 
            // CrashBox
            // 
            this.CrashBox.Controls.Add(this.CrashInsideAsyncTask);
            this.CrashBox.Controls.Add(this.CrashWithNullReference);
            this.CrashBox.Controls.Add(this.CrashWithAggregateException);
            this.CrashBox.Controls.Add(this.CrashWithDivisionByZero);
            this.CrashBox.Controls.Add(this.CrashWithNonSerializableException);
            this.CrashBox.Controls.Add(this.CrashWithTestException);
            this.CrashBox.Location = new System.Drawing.Point(16, 69);
            this.CrashBox.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.CrashBox.Name = "CrashBox";
            this.CrashBox.Padding = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.CrashBox.Size = new System.Drawing.Size(720, 393);
            this.CrashBox.TabIndex = 6;
            this.CrashBox.TabStop = false;
            this.CrashBox.Text = "Crashes";
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
            this.CrashesEnabled.TabIndex = 5;
            this.CrashesEnabled.Text = "Crashes Enabled";
            this.CrashesEnabled.UseVisualStyleBackColor = true;
            this.CrashesEnabled.CheckedChanged += new System.EventHandler(this.CrashesEnabled_CheckedChanged);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(768, 515);
            this.Controls.Add(this.Tabs);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "App Center Demo App";
            this.Tabs.ResumeLayout(false);
            this.AppCenterTab.ResumeLayout(false);
            this.LogBox.ResumeLayout(false);
            this.LogBox.PerformLayout();
            this.AnalyticsTab.ResumeLayout(false);
            this.EventBox.ResumeLayout(false);
            this.EventBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.EventProperties)).EndInit();
            this.CrashesTab.ResumeLayout(false);
            this.CrashBox.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl Tabs;
        private System.Windows.Forms.TabPage AppCenterTab;
        private System.Windows.Forms.TabPage AnalyticsTab;
        private System.Windows.Forms.TabPage CrashesTab;
        private System.Windows.Forms.TabPage OthersTab;
        private System.Windows.Forms.CheckBox AppCenterEnabled;
        private System.Windows.Forms.Label AppCenterLogLevelLabel;
        private System.Windows.Forms.ComboBox AppCenterLogLevel;
        private System.Windows.Forms.GroupBox LogBox;
        private System.Windows.Forms.Button WriteLog;
        private System.Windows.Forms.TextBox LogTag;
        private System.Windows.Forms.TextBox LogMessage;
        private System.Windows.Forms.Label LogMessageLabel;
        private System.Windows.Forms.Label LogTagLabel;
        private System.Windows.Forms.Label LogLevelLabel;
        private System.Windows.Forms.ComboBox LogLevelValue;
        private System.Windows.Forms.GroupBox EventBox;
        private System.Windows.Forms.CheckBox AnalyticsEnabled;
        private System.Windows.Forms.TextBox EventName;
        private System.Windows.Forms.Label EventNameLabel;
        private System.Windows.Forms.DataGridView EventProperties;
        private System.Windows.Forms.DataGridViewTextBoxColumn Key;
        private System.Windows.Forms.DataGridViewTextBoxColumn Value;
        private System.Windows.Forms.Button TrackEvent;
        private System.Windows.Forms.GroupBox CrashBox;
        private System.Windows.Forms.Button CrashInsideAsyncTask;
        private System.Windows.Forms.Button CrashWithNullReference;
        private System.Windows.Forms.Button CrashWithAggregateException;
        private System.Windows.Forms.Button CrashWithDivisionByZero;
        private System.Windows.Forms.Button CrashWithNonSerializableException;
        private System.Windows.Forms.Button CrashWithTestException;
        private System.Windows.Forms.CheckBox CrashesEnabled;
    }
}

