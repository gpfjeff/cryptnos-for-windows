namespace com.gpfcomics.Cryptnos
{
    partial class AdvancedSettingsDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AdvancedSettingsDialog));
            this.lblWarning = new System.Windows.Forms.Label();
            this.lblTextEncoding = new System.Windows.Forms.Label();
            this.cmbTextEncodings = new System.Windows.Forms.ComboBox();
            this.lblDefaultEncoding = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.chkDebug = new System.Windows.Forms.CheckBox();
            this.chkDisableUpdateCheck = new System.Windows.Forms.CheckBox();
            this.chkKeepOnTop = new System.Windows.Forms.CheckBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.chkShowMasterPassphrase = new System.Windows.Forms.CheckBox();
            this.chkClearPasswordOnFocusLoss = new System.Windows.Forms.CheckBox();
            this.btnCheckForUpdates = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblWarning
            // 
            this.lblWarning.Location = new System.Drawing.Point(12, 9);
            this.lblWarning.Name = "lblWarning";
            this.lblWarning.Size = new System.Drawing.Size(268, 69);
            this.lblWarning.TabIndex = 0;
            this.lblWarning.Text = resources.GetString("lblWarning.Text");
            // 
            // lblTextEncoding
            // 
            this.lblTextEncoding.Location = new System.Drawing.Point(12, 87);
            this.lblTextEncoding.Name = "lblTextEncoding";
            this.lblTextEncoding.Size = new System.Drawing.Size(268, 30);
            this.lblTextEncoding.TabIndex = 1;
            this.lblTextEncoding.Text = "Text encoding (UTF-8 is strongly recommended for cross-platform compatibility):";
            this.toolTip1.SetToolTip(this.lblTextEncoding, resources.GetString("lblTextEncoding.ToolTip"));
            // 
            // cmbTextEncodings
            // 
            this.cmbTextEncodings.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTextEncodings.FormattingEnabled = true;
            this.cmbTextEncodings.Location = new System.Drawing.Point(15, 120);
            this.cmbTextEncodings.Name = "cmbTextEncodings";
            this.cmbTextEncodings.Size = new System.Drawing.Size(265, 21);
            this.cmbTextEncodings.TabIndex = 2;
            this.toolTip1.SetToolTip(this.cmbTextEncodings, resources.GetString("cmbTextEncodings.ToolTip"));
            // 
            // lblDefaultEncoding
            // 
            this.lblDefaultEncoding.AutoSize = true;
            this.lblDefaultEncoding.Location = new System.Drawing.Point(12, 144);
            this.lblDefaultEncoding.Name = "lblDefaultEncoding";
            this.lblDefaultEncoding.Size = new System.Drawing.Size(129, 13);
            this.lblDefaultEncoding.TabIndex = 3;
            this.lblDefaultEncoding.Text = "System default encoding: ";
            this.toolTip1.SetToolTip(this.lblDefaultEncoding, "This label displays the default text encoding\r\nfor your system.  This is what you" +
        "r computer\r\nuses to convert text to binary data if you\r\ndon\'t specify anything e" +
        "lse.");
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(68, 319);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 4;
            this.btnOK.Text = "OK";
            this.toolTip1.SetToolTip(this.btnOK, "Click this button to save your current\r\nsettings and return to the main\r\nCryptnos" +
        " window.");
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(149, 319);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.toolTip1.SetToolTip(this.btnCancel, "Click this button to abandon any changes\r\nto your settings and return to the main" +
        "\r\nCryptnos window.");
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // chkDebug
            // 
            this.chkDebug.AutoSize = true;
            this.chkDebug.Location = new System.Drawing.Point(15, 173);
            this.chkDebug.Name = "chkDebug";
            this.chkDebug.Size = new System.Drawing.Size(267, 17);
            this.chkDebug.TabIndex = 6;
            this.chkDebug.Text = "Enable debug mode (more detailed error messages)";
            this.toolTip1.SetToolTip(this.chkDebug, "Enable this checkbox to get more detailed\r\nerror messages when something goes\r\nwr" +
        "ong.  This may be more helpful when\r\ndiscussing problems with the Cryptnos\r\ndeve" +
        "lopment team.");
            this.chkDebug.UseVisualStyleBackColor = true;
            this.chkDebug.CheckedChanged += new System.EventHandler(this.chkDebug_CheckedChanged);
            // 
            // chkDisableUpdateCheck
            // 
            this.chkDisableUpdateCheck.AutoSize = true;
            this.chkDisableUpdateCheck.Location = new System.Drawing.Point(15, 196);
            this.chkDisableUpdateCheck.Name = "chkDisableUpdateCheck";
            this.chkDisableUpdateCheck.Size = new System.Drawing.Size(224, 17);
            this.chkDisableUpdateCheck.TabIndex = 7;
            this.chkDisableUpdateCheck.Text = "Disable update check (not recommended)";
            this.toolTip1.SetToolTip(this.chkDisableUpdateCheck, "Enable this checkbox to prevent Cryptnos\r\nfrom accessing the Cryptnos website to\r" +
        "\ncheck for updates.  We do not recommend\r\nactivating this option unless absolute" +
        "ly\r\nnecessary.");
            this.chkDisableUpdateCheck.UseVisualStyleBackColor = true;
            this.chkDisableUpdateCheck.CheckedChanged += new System.EventHandler(this.chkDisableUpdateCheck_CheckedChanged);
            // 
            // chkKeepOnTop
            // 
            this.chkKeepOnTop.AutoSize = true;
            this.chkKeepOnTop.Location = new System.Drawing.Point(15, 219);
            this.chkKeepOnTop.Name = "chkKeepOnTop";
            this.chkKeepOnTop.Size = new System.Drawing.Size(211, 17);
            this.chkKeepOnTop.TabIndex = 9;
            this.chkKeepOnTop.Text = "Keep Cryptnos on top of other windows";
            this.toolTip1.SetToolTip(this.chkKeepOnTop, resources.GetString("chkKeepOnTop.ToolTip"));
            this.chkKeepOnTop.UseVisualStyleBackColor = true;
            // 
            // toolTip1
            // 
            this.toolTip1.IsBalloon = true;
            // 
            // chkShowMasterPassphrase
            // 
            this.chkShowMasterPassphrase.AutoSize = true;
            this.chkShowMasterPassphrase.Location = new System.Drawing.Point(15, 243);
            this.chkShowMasterPassphrase.Name = "chkShowMasterPassphrase";
            this.chkShowMasterPassphrase.Size = new System.Drawing.Size(144, 17);
            this.chkShowMasterPassphrase.TabIndex = 10;
            this.chkShowMasterPassphrase.Text = "Show master passphrase";
            this.toolTip1.SetToolTip(this.chkShowMasterPassphrase, resources.GetString("chkShowMasterPassphrase.ToolTip"));
            this.chkShowMasterPassphrase.UseVisualStyleBackColor = true;
            // 
            // chkClearPasswordOnFocusLoss
            // 
            this.chkClearPasswordOnFocusLoss.AutoSize = true;
            this.chkClearPasswordOnFocusLoss.Location = new System.Drawing.Point(15, 267);
            this.chkClearPasswordOnFocusLoss.Name = "chkClearPasswordOnFocusLoss";
            this.chkClearPasswordOnFocusLoss.Size = new System.Drawing.Size(191, 17);
            this.chkClearPasswordOnFocusLoss.TabIndex = 11;
            this.chkClearPasswordOnFocusLoss.Text = "Clear passwords when losing focus";
            this.toolTip1.SetToolTip(this.chkClearPasswordOnFocusLoss, resources.GetString("chkClearPasswordOnFocusLoss.ToolTip"));
            this.chkClearPasswordOnFocusLoss.UseVisualStyleBackColor = true;
            // 
            // btnCheckForUpdates
            // 
            this.btnCheckForUpdates.Location = new System.Drawing.Point(68, 290);
            this.btnCheckForUpdates.Name = "btnCheckForUpdates";
            this.btnCheckForUpdates.Size = new System.Drawing.Size(156, 23);
            this.btnCheckForUpdates.TabIndex = 12;
            this.btnCheckForUpdates.Text = "Check for Updates...";
            this.toolTip1.SetToolTip(this.btnCheckForUpdates, "Check to see if you\'re running\r\nthe latest version of Cryptnos.");
            this.btnCheckForUpdates.UseVisualStyleBackColor = true;
            this.btnCheckForUpdates.Click += new System.EventHandler(this.btnCheckForUpdates_Click);
            // 
            // AdvancedSettingsDialog
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(292, 354);
            this.Controls.Add(this.btnCheckForUpdates);
            this.Controls.Add(this.chkClearPasswordOnFocusLoss);
            this.Controls.Add(this.chkShowMasterPassphrase);
            this.Controls.Add(this.chkKeepOnTop);
            this.Controls.Add(this.chkDisableUpdateCheck);
            this.Controls.Add(this.chkDebug);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.lblDefaultEncoding);
            this.Controls.Add(this.cmbTextEncodings);
            this.Controls.Add(this.lblTextEncoding);
            this.Controls.Add(this.lblWarning);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AdvancedSettingsDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Advanced Settings";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblWarning;
        private System.Windows.Forms.Label lblTextEncoding;
        private System.Windows.Forms.ComboBox cmbTextEncodings;
        private System.Windows.Forms.Label lblDefaultEncoding;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.CheckBox chkDebug;
        private System.Windows.Forms.CheckBox chkDisableUpdateCheck;
        private System.Windows.Forms.CheckBox chkKeepOnTop;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.CheckBox chkShowMasterPassphrase;
        private System.Windows.Forms.CheckBox chkClearPasswordOnFocusLoss;
        private System.Windows.Forms.Button btnCheckForUpdates;
    }
}