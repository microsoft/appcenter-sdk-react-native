namespace Contoso.WinForms.Puppet
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
            this.mobileCenter = new System.Windows.Forms.TabPage();
            this.logBox = new System.Windows.Forms.GroupBox();
            this.writeLog = new System.Windows.Forms.Button();
            this.logTag = new System.Windows.Forms.TextBox();
            this.logMessage = new System.Windows.Forms.TextBox();
            this.logMessageLabel = new System.Windows.Forms.Label();
            this.logTagLabel = new System.Windows.Forms.Label();
            this.logLevelLabel = new System.Windows.Forms.Label();
            this.logLevel = new System.Windows.Forms.ComboBox();
            this.mobileCenterLogLevelLabel = new System.Windows.Forms.Label();
            this.mobileCenterLogLevel = new System.Windows.Forms.ComboBox();
            this.mobileCenterEnabled = new System.Windows.Forms.CheckBox();
            this.analytics = new System.Windows.Forms.TabPage();
            this.eventBox = new System.Windows.Forms.GroupBox();
            this.trackEvent = new System.Windows.Forms.Button();
            this.eventProperties = new System.Windows.Forms.DataGridView();
            this.Key = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Value = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.eventName = new System.Windows.Forms.TextBox();
            this.eventNameLabel = new System.Windows.Forms.Label();
            this.analyticsEnabled = new System.Windows.Forms.CheckBox();
            this.crashes = new System.Windows.Forms.TabPage();
            this.others = new System.Windows.Forms.TabPage();
            this.tabs.SuspendLayout();
            this.mobileCenter.SuspendLayout();
            this.logBox.SuspendLayout();
            this.analytics.SuspendLayout();
            this.eventBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.eventProperties)).BeginInit();
            this.SuspendLayout();
            // 
            // tabs
            // 
            this.tabs.Controls.Add(this.mobileCenter);
            this.tabs.Controls.Add(this.analytics);
            this.tabs.Controls.Add(this.crashes);
            this.tabs.Controls.Add(this.others);
            this.tabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabs.Location = new System.Drawing.Point(0, 0);
            this.tabs.Name = "tabs";
            this.tabs.SelectedIndex = 0;
            this.tabs.Size = new System.Drawing.Size(384, 261);
            this.tabs.TabIndex = 0;
            this.tabs.SelectedIndexChanged += new System.EventHandler(this.tabs_SelectedIndexChanged);
            // 
            // mobileCenter
            // 
            this.mobileCenter.Controls.Add(this.logBox);
            this.mobileCenter.Controls.Add(this.mobileCenterLogLevelLabel);
            this.mobileCenter.Controls.Add(this.mobileCenterLogLevel);
            this.mobileCenter.Controls.Add(this.mobileCenterEnabled);
            this.mobileCenter.Location = new System.Drawing.Point(4, 22);
            this.mobileCenter.Name = "mobileCenter";
            this.mobileCenter.Padding = new System.Windows.Forms.Padding(3);
            this.mobileCenter.Size = new System.Drawing.Size(376, 235);
            this.mobileCenter.TabIndex = 0;
            this.mobileCenter.Text = "Mobile Center";
            this.mobileCenter.UseVisualStyleBackColor = true;
            // 
            // logBox
            // 
            this.logBox.Controls.Add(this.writeLog);
            this.logBox.Controls.Add(this.logTag);
            this.logBox.Controls.Add(this.logMessage);
            this.logBox.Controls.Add(this.logMessageLabel);
            this.logBox.Controls.Add(this.logTagLabel);
            this.logBox.Controls.Add(this.logLevelLabel);
            this.logBox.Controls.Add(this.logLevel);
            this.logBox.Location = new System.Drawing.Point(8, 63);
            this.logBox.Name = "logBox";
            this.logBox.Size = new System.Drawing.Size(360, 130);
            this.logBox.TabIndex = 5;
            this.logBox.TabStop = false;
            this.logBox.Text = "Log";
            // 
            // writeLog
            // 
            this.writeLog.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.writeLog.Location = new System.Drawing.Point(9, 101);
            this.writeLog.Name = "writeLog";
            this.writeLog.Size = new System.Drawing.Size(342, 23);
            this.writeLog.TabIndex = 11;
            this.writeLog.Text = "Write Log";
            this.writeLog.UseVisualStyleBackColor = true;
            this.writeLog.Click += new System.EventHandler(this.writeLog_Click);
            // 
            // logTag
            // 
            this.logTag.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.logTag.Location = new System.Drawing.Point(106, 19);
            this.logTag.Name = "logTag";
            this.logTag.Size = new System.Drawing.Size(245, 20);
            this.logTag.TabIndex = 10;
            // 
            // logMessage
            // 
            this.logMessage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.logMessage.Location = new System.Drawing.Point(106, 42);
            this.logMessage.Name = "logMessage";
            this.logMessage.Size = new System.Drawing.Size(245, 20);
            this.logMessage.TabIndex = 9;
            // 
            // logMessageLabel
            // 
            this.logMessageLabel.Location = new System.Drawing.Point(6, 40);
            this.logMessageLabel.Name = "logMessageLabel";
            this.logMessageLabel.Size = new System.Drawing.Size(94, 23);
            this.logMessageLabel.TabIndex = 8;
            this.logMessageLabel.Text = "Log Message";
            this.logMessageLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // logTagLabel
            // 
            this.logTagLabel.Location = new System.Drawing.Point(6, 17);
            this.logTagLabel.Name = "logTagLabel";
            this.logTagLabel.Size = new System.Drawing.Size(94, 23);
            this.logTagLabel.TabIndex = 7;
            this.logTagLabel.Text = "Log Tag";
            this.logTagLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // logLevelLabel
            // 
            this.logLevelLabel.Location = new System.Drawing.Point(6, 66);
            this.logLevelLabel.Name = "logLevelLabel";
            this.logLevelLabel.Size = new System.Drawing.Size(94, 23);
            this.logLevelLabel.TabIndex = 6;
            this.logLevelLabel.Text = "Log Level";
            this.logLevelLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // logLevel
            // 
            this.logLevel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.logLevel.FormattingEnabled = true;
            this.logLevel.Items.AddRange(new object[] {
            "Verbose",
            "Debug",
            "Info",
            "Warning",
            "Error"});
            this.logLevel.Location = new System.Drawing.Point(106, 68);
            this.logLevel.Name = "logLevel";
            this.logLevel.Size = new System.Drawing.Size(245, 21);
            this.logLevel.TabIndex = 5;
            // 
            // mobileCenterLogLevelLabel
            // 
            this.mobileCenterLogLevelLabel.Location = new System.Drawing.Point(20, 36);
            this.mobileCenterLogLevelLabel.Name = "mobileCenterLogLevelLabel";
            this.mobileCenterLogLevelLabel.Size = new System.Drawing.Size(88, 23);
            this.mobileCenterLogLevelLabel.TabIndex = 4;
            this.mobileCenterLogLevelLabel.Text = "Log Level";
            this.mobileCenterLogLevelLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // mobileCenterLogLevel
            // 
            this.mobileCenterLogLevel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mobileCenterLogLevel.FormattingEnabled = true;
            this.mobileCenterLogLevel.Items.AddRange(new object[] {
            "Verbose",
            "Debug",
            "Info",
            "Warning",
            "Error"});
            this.mobileCenterLogLevel.Location = new System.Drawing.Point(114, 38);
            this.mobileCenterLogLevel.Name = "mobileCenterLogLevel";
            this.mobileCenterLogLevel.Size = new System.Drawing.Size(245, 21);
            this.mobileCenterLogLevel.TabIndex = 3;
            this.mobileCenterLogLevel.SelectedIndexChanged += new System.EventHandler(this.mobileCenterLogLevel_SelectedIndexChanged);
            // 
            // mobileCenterEnabled
            // 
            this.mobileCenterEnabled.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mobileCenterEnabled.Location = new System.Drawing.Point(8, 6);
            this.mobileCenterEnabled.Name = "mobileCenterEnabled";
            this.mobileCenterEnabled.Size = new System.Drawing.Size(360, 24);
            this.mobileCenterEnabled.TabIndex = 1;
            this.mobileCenterEnabled.Text = "Mobile Center Enabled";
            this.mobileCenterEnabled.UseVisualStyleBackColor = true;
            this.mobileCenterEnabled.CheckedChanged += new System.EventHandler(this.mobileCenterEnabled_CheckedChanged);
            // 
            // analytics
            // 
            this.analytics.Controls.Add(this.eventBox);
            this.analytics.Controls.Add(this.analyticsEnabled);
            this.analytics.Location = new System.Drawing.Point(4, 22);
            this.analytics.Name = "analytics";
            this.analytics.Padding = new System.Windows.Forms.Padding(3);
            this.analytics.Size = new System.Drawing.Size(376, 235);
            this.analytics.TabIndex = 1;
            this.analytics.Text = "Analytics";
            this.analytics.UseVisualStyleBackColor = true;
            // 
            // eventBox
            // 
            this.eventBox.Controls.Add(this.trackEvent);
            this.eventBox.Controls.Add(this.eventProperties);
            this.eventBox.Controls.Add(this.eventName);
            this.eventBox.Controls.Add(this.eventNameLabel);
            this.eventBox.Location = new System.Drawing.Point(8, 36);
            this.eventBox.Name = "eventBox";
            this.eventBox.Size = new System.Drawing.Size(360, 191);
            this.eventBox.TabIndex = 3;
            this.eventBox.TabStop = false;
            this.eventBox.Text = "Event";
            // 
            // trackEvent
            // 
            this.trackEvent.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.trackEvent.Location = new System.Drawing.Point(9, 162);
            this.trackEvent.Name = "trackEvent";
            this.trackEvent.Size = new System.Drawing.Size(342, 23);
            this.trackEvent.TabIndex = 14;
            this.trackEvent.Text = "Track Event";
            this.trackEvent.UseVisualStyleBackColor = true;
            this.trackEvent.Click += new System.EventHandler(this.trackEvent_Click);
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
            this.eventProperties.Location = new System.Drawing.Point(9, 44);
            this.eventProperties.Name = "eventProperties";
            this.eventProperties.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.eventProperties.Size = new System.Drawing.Size(342, 110);
            this.eventProperties.TabIndex = 13;
            // 
            // Key
            // 
            this.Key.HeaderText = "Key";
            this.Key.MaxInputLength = 64;
            this.Key.Name = "Key";
            this.Key.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // Value
            // 
            this.Value.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Value.HeaderText = "Value";
            this.Value.MaxInputLength = 64;
            this.Value.Name = "Value";
            this.Value.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // eventName
            // 
            this.eventName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.eventName.Location = new System.Drawing.Point(106, 18);
            this.eventName.MaxLength = 256;
            this.eventName.Name = "eventName";
            this.eventName.Size = new System.Drawing.Size(245, 20);
            this.eventName.TabIndex = 12;
            // 
            // eventNameLabel
            // 
            this.eventNameLabel.Location = new System.Drawing.Point(6, 16);
            this.eventNameLabel.Name = "eventNameLabel";
            this.eventNameLabel.Size = new System.Drawing.Size(94, 23);
            this.eventNameLabel.TabIndex = 11;
            this.eventNameLabel.Text = "Event Name";
            this.eventNameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // analyticsEnabled
            // 
            this.analyticsEnabled.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.analyticsEnabled.Location = new System.Drawing.Point(8, 6);
            this.analyticsEnabled.Name = "analyticsEnabled";
            this.analyticsEnabled.Size = new System.Drawing.Size(360, 24);
            this.analyticsEnabled.TabIndex = 2;
            this.analyticsEnabled.Text = "Analytics Enabled";
            this.analyticsEnabled.UseVisualStyleBackColor = true;
            this.analyticsEnabled.CheckedChanged += new System.EventHandler(this.analyticsEnabled_CheckedChanged);
            // 
            // crashes
            // 
            this.crashes.Location = new System.Drawing.Point(4, 22);
            this.crashes.Name = "crashes";
            this.crashes.Size = new System.Drawing.Size(376, 235);
            this.crashes.TabIndex = 2;
            this.crashes.Text = "Crashes";
            this.crashes.UseVisualStyleBackColor = true;
            // 
            // others
            // 
            this.others.Location = new System.Drawing.Point(4, 22);
            this.others.Name = "others";
            this.others.Size = new System.Drawing.Size(376, 235);
            this.others.TabIndex = 3;
            this.others.Text = "Others";
            this.others.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 261);
            this.Controls.Add(this.tabs);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "Mobile Center Puppet App";
            this.tabs.ResumeLayout(false);
            this.mobileCenter.ResumeLayout(false);
            this.logBox.ResumeLayout(false);
            this.logBox.PerformLayout();
            this.analytics.ResumeLayout(false);
            this.eventBox.ResumeLayout(false);
            this.eventBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.eventProperties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabs;
        private System.Windows.Forms.TabPage mobileCenter;
        private System.Windows.Forms.TabPage analytics;
        private System.Windows.Forms.TabPage crashes;
        private System.Windows.Forms.TabPage others;
        private System.Windows.Forms.CheckBox mobileCenterEnabled;
        private System.Windows.Forms.Label mobileCenterLogLevelLabel;
        private System.Windows.Forms.ComboBox mobileCenterLogLevel;
        private System.Windows.Forms.GroupBox logBox;
        private System.Windows.Forms.Button writeLog;
        private System.Windows.Forms.TextBox logTag;
        private System.Windows.Forms.TextBox logMessage;
        private System.Windows.Forms.Label logMessageLabel;
        private System.Windows.Forms.Label logTagLabel;
        private System.Windows.Forms.Label logLevelLabel;
        private System.Windows.Forms.ComboBox logLevel;
        private System.Windows.Forms.GroupBox eventBox;
        private System.Windows.Forms.CheckBox analyticsEnabled;
        private System.Windows.Forms.TextBox eventName;
        private System.Windows.Forms.Label eventNameLabel;
        private System.Windows.Forms.DataGridView eventProperties;
        private System.Windows.Forms.DataGridViewTextBoxColumn Key;
        private System.Windows.Forms.DataGridViewTextBoxColumn Value;
        private System.Windows.Forms.Button trackEvent;
    }
}

