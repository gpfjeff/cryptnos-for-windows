namespace com.gpfcomics.Cryptnos
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtPassphrase = new System.Windows.Forms.TextBox();
            this.gboxOptionalRules = new System.Windows.Forms.GroupBox();
            this.cbCharLimit = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txtIterations = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.cbHashes = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.cbCharTypes = new System.Windows.Forms.ComboBox();
            this.chkRemember = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.btnGenerate = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.cbSites = new System.Windows.Forms.ComboBox();
            this.btnAbout = new System.Windows.Forms.Button();
            this.btnImport = new System.Windows.Forms.Button();
            this.btnForgetAll = new System.Windows.Forms.Button();
            this.btnExport = new System.Windows.Forms.Button();
            this.btnForget = new System.Windows.Forms.Button();
            this.gboxCoreParameters = new System.Windows.Forms.GroupBox();
            this.gboxRememberingSettings = new System.Windows.Forms.GroupBox();
            this.chkLock = new System.Windows.Forms.CheckBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.chkShowTooltips = new System.Windows.Forms.CheckBox();
            this.chkCopyToClipboard = new System.Windows.Forms.CheckBox();
            this.btnAdvanced = new System.Windows.Forms.Button();
            this.chkDailyMode = new System.Windows.Forms.CheckBox();
            this.gboxOptionalRules.SuspendLayout();
            this.gboxCoreParameters.SuspendLayout();
            this.gboxRememberingSettings.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Site name:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(5, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Passphrase:";
            // 
            // txtPassphrase
            // 
            this.txtPassphrase.Location = new System.Drawing.Point(76, 46);
            this.txtPassphrase.Name = "txtPassphrase";
            this.txtPassphrase.Size = new System.Drawing.Size(197, 20);
            this.txtPassphrase.TabIndex = 1;
            this.toolTip1.SetToolTip(this.txtPassphrase, resources.GetString("txtPassphrase.ToolTip"));
            this.txtPassphrase.UseSystemPasswordChar = true;
            // 
            // gboxOptionalRules
            // 
            this.gboxOptionalRules.Controls.Add(this.cbCharLimit);
            this.gboxOptionalRules.Controls.Add(this.label9);
            this.gboxOptionalRules.Controls.Add(this.txtIterations);
            this.gboxOptionalRules.Controls.Add(this.label8);
            this.gboxOptionalRules.Controls.Add(this.cbHashes);
            this.gboxOptionalRules.Controls.Add(this.label7);
            this.gboxOptionalRules.Controls.Add(this.label6);
            this.gboxOptionalRules.Controls.Add(this.label4);
            this.gboxOptionalRules.Controls.Add(this.cbCharTypes);
            this.gboxOptionalRules.Location = new System.Drawing.Point(7, 162);
            this.gboxOptionalRules.Name = "gboxOptionalRules";
            this.gboxOptionalRules.Size = new System.Drawing.Size(278, 118);
            this.gboxOptionalRules.TabIndex = 1;
            this.gboxOptionalRules.TabStop = false;
            this.gboxOptionalRules.Text = "Optional Rules:";
            this.toolTip1.SetToolTip(this.gboxOptionalRules, resources.GetString("gboxOptionalRules.ToolTip"));
            // 
            // cbCharLimit
            // 
            this.cbCharLimit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbCharLimit.FormattingEnabled = true;
            this.cbCharLimit.Items.AddRange(new object[] {
            "None"});
            this.cbCharLimit.Location = new System.Drawing.Point(130, 89);
            this.cbCharLimit.Name = "cbCharLimit";
            this.cbCharLimit.Size = new System.Drawing.Size(121, 21);
            this.cbCharLimit.TabIndex = 3;
            this.toolTip1.SetToolTip(this.cbCharLimit, resources.GetString("cbCharLimit.ToolTip"));
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(172, 43);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(31, 13);
            this.label9.TabIndex = 6;
            this.label9.Text = "times";
            // 
            // txtIterations
            // 
            this.txtIterations.Location = new System.Drawing.Point(115, 40);
            this.txtIterations.Name = "txtIterations";
            this.txtIterations.Size = new System.Drawing.Size(51, 20);
            this.txtIterations.TabIndex = 1;
            this.txtIterations.Text = "1";
            this.txtIterations.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.toolTip1.SetToolTip(this.txtIterations, resources.GetString("txtIterations.ToolTip"));
            this.txtIterations.TextChanged += new System.EventHandler(this.txtIterations_TextChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(24, 43);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(88, 13);
            this.label8.TabIndex = 5;
            this.label8.Text = "Perform this hash";
            // 
            // cbHashes
            // 
            this.cbHashes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbHashes.FormattingEnabled = true;
            this.cbHashes.Location = new System.Drawing.Point(110, 13);
            this.cbHashes.Name = "cbHashes";
            this.cbHashes.Size = new System.Drawing.Size(141, 21);
            this.cbHashes.TabIndex = 0;
            this.toolTip1.SetToolTip(this.cbHashes, resources.GetString("cbHashes.ToolTip"));
            this.cbHashes.SelectedIndexChanged += new System.EventHandler(this.cbHashes_SelectedIndexChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(24, 16);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(80, 13);
            this.label7.TabIndex = 4;
            this.label7.Text = "Hash algorithm:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(24, 92);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(91, 13);
            this.label6.TabIndex = 8;
            this.label6.Text = "Length restriction:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(24, 68);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(26, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Use";
            // 
            // cbCharTypes
            // 
            this.cbCharTypes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbCharTypes.FormattingEnabled = true;
            this.cbCharTypes.Items.AddRange(new object[] {
            "all generated characters",
            "alphanumerics, change others to underscores",
            "alphanumerics only",
            "alphabetic characters only",
            "numbers only"});
            this.cbCharTypes.Location = new System.Drawing.Point(56, 65);
            this.cbCharTypes.Name = "cbCharTypes";
            this.cbCharTypes.Size = new System.Drawing.Size(195, 21);
            this.cbCharTypes.TabIndex = 2;
            this.toolTip1.SetToolTip(this.cbCharTypes, resources.GetString("cbCharTypes.ToolTip"));
            // 
            // chkRemember
            // 
            this.chkRemember.AutoSize = true;
            this.chkRemember.Checked = true;
            this.chkRemember.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkRemember.Location = new System.Drawing.Point(8, 19);
            this.chkRemember.Name = "chkRemember";
            this.chkRemember.Size = new System.Drawing.Size(132, 17);
            this.chkRemember.TabIndex = 0;
            this.chkRemember.Text = "Remember parameters";
            this.toolTip1.SetToolTip(this.chkRemember, resources.GetString("chkRemember.ToolTip"));
            this.chkRemember.UseVisualStyleBackColor = true;
            this.chkRemember.CheckedChanged += new System.EventHandler(this.chkRemember_CheckedChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(5, 103);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(56, 13);
            this.label5.TabIndex = 6;
            this.label5.Text = "Password:";
            // 
            // txtPassword
            // 
            this.txtPassword.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPassword.Location = new System.Drawing.Point(61, 100);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.ReadOnly = true;
            this.txtPassword.Size = new System.Drawing.Size(212, 20);
            this.txtPassword.TabIndex = 3;
            this.toolTip1.SetToolTip(this.txtPassword, resources.GetString("txtPassword.ToolTip"));
            this.txtPassword.MouseClick += new System.Windows.Forms.MouseEventHandler(this.txtPassword_MouseClick);
            this.txtPassword.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.txtPassword_MouseDoubleClick);
            // 
            // btnGenerate
            // 
            this.btnGenerate.Location = new System.Drawing.Point(8, 72);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(264, 23);
            this.btnGenerate.TabIndex = 2;
            this.btnGenerate.Text = "Generate";
            this.toolTip1.SetToolTip(this.btnGenerate, resources.GetString("btnGenerate.ToolTip"));
            this.btnGenerate.UseVisualStyleBackColor = true;
            this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
            // 
            // btnClose
            // 
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(228, 393);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(57, 23);
            this.btnClose.TabIndex = 4;
            this.btnClose.Text = "Close";
            this.toolTip1.SetToolTip(this.btnClose, resources.GetString("btnClose.ToolTip"));
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // cbSites
            // 
            this.cbSites.FormattingEnabled = true;
            this.cbSites.Location = new System.Drawing.Point(76, 19);
            this.cbSites.Name = "cbSites";
            this.cbSites.Size = new System.Drawing.Size(197, 21);
            this.cbSites.TabIndex = 0;
            this.toolTip1.SetToolTip(this.cbSites, resources.GetString("cbSites.ToolTip"));
            this.cbSites.SelectedIndexChanged += new System.EventHandler(this.cbSites_SelectedIndexChanged);
            this.cbSites.Leave += new System.EventHandler(this.cbSites_Leave);
            // 
            // btnAbout
            // 
            this.btnAbout.Location = new System.Drawing.Point(7, 393);
            this.btnAbout.Name = "btnAbout";
            this.btnAbout.Size = new System.Drawing.Size(58, 23);
            this.btnAbout.TabIndex = 2;
            this.btnAbout.Text = "About...";
            this.toolTip1.SetToolTip(this.btnAbout, "Click this button to see the copyright and\r\nlicensing information about Cryptnos," +
        " as\r\nwell as a hyperlink to find Cryptnos\r\nonline.");
            this.btnAbout.UseVisualStyleBackColor = true;
            this.btnAbout.Click += new System.EventHandler(this.btnAbout_Click);
            // 
            // btnImport
            // 
            this.btnImport.Location = new System.Drawing.Point(141, 42);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(66, 23);
            this.btnImport.TabIndex = 4;
            this.btnImport.Text = "Import...";
            this.toolTip1.SetToolTip(this.btnImport, resources.GetString("btnImport.ToolTip"));
            this.btnImport.UseVisualStyleBackColor = true;
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // btnForgetAll
            // 
            this.btnForgetAll.Location = new System.Drawing.Point(69, 42);
            this.btnForgetAll.Name = "btnForgetAll";
            this.btnForgetAll.Size = new System.Drawing.Size(66, 23);
            this.btnForgetAll.TabIndex = 3;
            this.btnForgetAll.Text = "Forget All";
            this.toolTip1.SetToolTip(this.btnForgetAll, resources.GetString("btnForgetAll.ToolTip"));
            this.btnForgetAll.UseVisualStyleBackColor = true;
            this.btnForgetAll.Click += new System.EventHandler(this.btnForgetAll_Click);
            // 
            // btnExport
            // 
            this.btnExport.Location = new System.Drawing.Point(213, 42);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(57, 23);
            this.btnExport.TabIndex = 5;
            this.btnExport.Text = "Export...";
            this.toolTip1.SetToolTip(this.btnExport, resources.GetString("btnExport.ToolTip"));
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // btnForget
            // 
            this.btnForget.Location = new System.Drawing.Point(9, 42);
            this.btnForget.Name = "btnForget";
            this.btnForget.Size = new System.Drawing.Size(54, 23);
            this.btnForget.TabIndex = 2;
            this.btnForget.Text = "Forget";
            this.toolTip1.SetToolTip(this.btnForget, resources.GetString("btnForget.ToolTip"));
            this.btnForget.UseVisualStyleBackColor = true;
            this.btnForget.Click += new System.EventHandler(this.btnForget_Click);
            // 
            // gboxCoreParameters
            // 
            this.gboxCoreParameters.Controls.Add(this.cbSites);
            this.gboxCoreParameters.Controls.Add(this.label1);
            this.gboxCoreParameters.Controls.Add(this.txtPassphrase);
            this.gboxCoreParameters.Controls.Add(this.label2);
            this.gboxCoreParameters.Controls.Add(this.btnGenerate);
            this.gboxCoreParameters.Controls.Add(this.txtPassword);
            this.gboxCoreParameters.Controls.Add(this.label5);
            this.gboxCoreParameters.Location = new System.Drawing.Point(7, 7);
            this.gboxCoreParameters.Name = "gboxCoreParameters";
            this.gboxCoreParameters.Size = new System.Drawing.Size(278, 126);
            this.gboxCoreParameters.TabIndex = 0;
            this.gboxCoreParameters.TabStop = false;
            this.gboxCoreParameters.Text = "Core Parameters:";
            this.toolTip1.SetToolTip(this.gboxCoreParameters, resources.GetString("gboxCoreParameters.ToolTip"));
            // 
            // gboxRememberingSettings
            // 
            this.gboxRememberingSettings.Controls.Add(this.chkLock);
            this.gboxRememberingSettings.Controls.Add(this.chkRemember);
            this.gboxRememberingSettings.Controls.Add(this.btnForget);
            this.gboxRememberingSettings.Controls.Add(this.btnForgetAll);
            this.gboxRememberingSettings.Controls.Add(this.btnExport);
            this.gboxRememberingSettings.Controls.Add(this.btnImport);
            this.gboxRememberingSettings.Location = new System.Drawing.Point(7, 286);
            this.gboxRememberingSettings.Name = "gboxRememberingSettings";
            this.gboxRememberingSettings.Size = new System.Drawing.Size(278, 77);
            this.gboxRememberingSettings.TabIndex = 2;
            this.gboxRememberingSettings.TabStop = false;
            this.gboxRememberingSettings.Text = "Remembering Settings:";
            this.toolTip1.SetToolTip(this.gboxRememberingSettings, resources.GetString("gboxRememberingSettings.ToolTip"));
            // 
            // chkLock
            // 
            this.chkLock.AutoSize = true;
            this.chkLock.Location = new System.Drawing.Point(167, 19);
            this.chkLock.Name = "chkLock";
            this.chkLock.Size = new System.Drawing.Size(105, 17);
            this.chkLock.TabIndex = 1;
            this.chkLock.Text = "Lock parameters";
            this.toolTip1.SetToolTip(this.chkLock, resources.GetString("chkLock.ToolTip"));
            this.chkLock.UseVisualStyleBackColor = true;
            this.chkLock.CheckedChanged += new System.EventHandler(this.chkLock_CheckedChanged);
            // 
            // toolTip1
            // 
            this.toolTip1.IsBalloon = true;
            // 
            // chkShowTooltips
            // 
            this.chkShowTooltips.AutoSize = true;
            this.chkShowTooltips.Checked = true;
            this.chkShowTooltips.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkShowTooltips.Location = new System.Drawing.Point(7, 370);
            this.chkShowTooltips.Name = "chkShowTooltips";
            this.chkShowTooltips.Size = new System.Drawing.Size(112, 17);
            this.chkShowTooltips.TabIndex = 0;
            this.chkShowTooltips.Text = "Show tooltips help";
            this.toolTip1.SetToolTip(this.chkShowTooltips, resources.GetString("chkShowTooltips.ToolTip"));
            this.chkShowTooltips.UseVisualStyleBackColor = true;
            this.chkShowTooltips.CheckedChanged += new System.EventHandler(this.chkShowTooltips_CheckedChanged);
            // 
            // chkCopyToClipboard
            // 
            this.chkCopyToClipboard.AutoSize = true;
            this.chkCopyToClipboard.Location = new System.Drawing.Point(129, 370);
            this.chkCopyToClipboard.Name = "chkCopyToClipboard";
            this.chkCopyToClipboard.Size = new System.Drawing.Size(156, 17);
            this.chkCopyToClipboard.TabIndex = 1;
            this.chkCopyToClipboard.Text = "Copy password to clipboard";
            this.toolTip1.SetToolTip(this.chkCopyToClipboard, resources.GetString("chkCopyToClipboard.ToolTip"));
            this.chkCopyToClipboard.UseVisualStyleBackColor = true;
            // 
            // btnAdvanced
            // 
            this.btnAdvanced.Location = new System.Drawing.Point(107, 393);
            this.btnAdvanced.Name = "btnAdvanced";
            this.btnAdvanced.Size = new System.Drawing.Size(75, 23);
            this.btnAdvanced.TabIndex = 3;
            this.btnAdvanced.Text = "Advanced...";
            this.toolTip1.SetToolTip(this.btnAdvanced, resources.GetString("btnAdvanced.ToolTip"));
            this.btnAdvanced.UseVisualStyleBackColor = true;
            this.btnAdvanced.Click += new System.EventHandler(this.btnAdvanced_Click);
            // 
            // chkDailyMode
            // 
            this.chkDailyMode.AutoSize = true;
            this.chkDailyMode.Location = new System.Drawing.Point(75, 139);
            this.chkDailyMode.Name = "chkDailyMode";
            this.chkDailyMode.Size = new System.Drawing.Size(142, 17);
            this.chkDailyMode.TabIndex = 5;
            this.chkDailyMode.Text = "Enable \"daily use\" mode";
            this.toolTip1.SetToolTip(this.chkDailyMode, resources.GetString("chkDailyMode.ToolTip"));
            this.chkDailyMode.UseVisualStyleBackColor = true;
            this.chkDailyMode.CheckedChanged += new System.EventHandler(this.chkDailyMode_CheckedChanged);
            // 
            // Form1
            // 
            this.AcceptButton = this.btnGenerate;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(292, 424);
            this.Controls.Add(this.chkDailyMode);
            this.Controls.Add(this.btnAdvanced);
            this.Controls.Add(this.chkCopyToClipboard);
            this.Controls.Add(this.chkShowTooltips);
            this.Controls.Add(this.gboxRememberingSettings);
            this.Controls.Add(this.gboxCoreParameters);
            this.Controls.Add(this.btnAbout);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.gboxOptionalRules);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "Cryptnos";
            this.Deactivate += new System.EventHandler(this.Form1_Deactivate);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.gboxOptionalRules.ResumeLayout(false);
            this.gboxOptionalRules.PerformLayout();
            this.gboxCoreParameters.ResumeLayout(false);
            this.gboxCoreParameters.PerformLayout();
            this.gboxRememberingSettings.ResumeLayout(false);
            this.gboxRememberingSettings.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtPassphrase;
        private System.Windows.Forms.GroupBox gboxOptionalRules;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cbCharTypes;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Button btnGenerate;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox chkRemember;
        private System.Windows.Forms.ComboBox cbHashes;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox cbSites;
        private System.Windows.Forms.Button btnAbout;
        private System.Windows.Forms.Button btnImport;
        private System.Windows.Forms.Button btnForgetAll;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Button btnForget;
        private System.Windows.Forms.GroupBox gboxCoreParameters;
        private System.Windows.Forms.GroupBox gboxRememberingSettings;
        private System.Windows.Forms.CheckBox chkLock;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtIterations;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.CheckBox chkShowTooltips;
        private System.Windows.Forms.CheckBox chkCopyToClipboard;
        private System.Windows.Forms.Button btnAdvanced;
        private System.Windows.Forms.ComboBox cbCharLimit;
        private System.Windows.Forms.CheckBox chkDailyMode;
    }
}

