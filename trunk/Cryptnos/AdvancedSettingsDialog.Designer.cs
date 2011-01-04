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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AdvancedSettingsDialog));
            this.lblWarning = new System.Windows.Forms.Label();
            this.lblTextEncoding = new System.Windows.Forms.Label();
            this.cmbTextEncodings = new System.Windows.Forms.ComboBox();
            this.lblDefaultEncoding = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.chkDebug = new System.Windows.Forms.CheckBox();
            this.chkDisableUpdateCheck = new System.Windows.Forms.CheckBox();
            this.chkForceUpdateCheck = new System.Windows.Forms.CheckBox();
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
            // 
            // cmbTextEncodings
            // 
            this.cmbTextEncodings.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTextEncodings.FormattingEnabled = true;
            this.cmbTextEncodings.Location = new System.Drawing.Point(15, 120);
            this.cmbTextEncodings.Name = "cmbTextEncodings";
            this.cmbTextEncodings.Size = new System.Drawing.Size(265, 21);
            this.cmbTextEncodings.TabIndex = 2;
            // 
            // lblDefaultEncoding
            // 
            this.lblDefaultEncoding.AutoSize = true;
            this.lblDefaultEncoding.Location = new System.Drawing.Point(12, 144);
            this.lblDefaultEncoding.Name = "lblDefaultEncoding";
            this.lblDefaultEncoding.Size = new System.Drawing.Size(129, 13);
            this.lblDefaultEncoding.TabIndex = 3;
            this.lblDefaultEncoding.Text = "System default encoding: ";
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(68, 242);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 4;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(149, 242);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
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
            this.chkDebug.UseVisualStyleBackColor = true;
            // 
            // chkDisableUpdateCheck
            // 
            this.chkDisableUpdateCheck.AutoSize = true;
            this.chkDisableUpdateCheck.Location = new System.Drawing.Point(15, 196);
            this.chkDisableUpdateCheck.Name = "chkDisableUpdateCheck";
            this.chkDisableUpdateCheck.Size = new System.Drawing.Size(224, 17);
            this.chkDisableUpdateCheck.TabIndex = 7;
            this.chkDisableUpdateCheck.Text = "Disable update check (not recommended)";
            this.chkDisableUpdateCheck.UseVisualStyleBackColor = true;
            this.chkDisableUpdateCheck.CheckedChanged += new System.EventHandler(this.chkDisableUpdateCheck_CheckedChanged);
            // 
            // chkForceUpdateCheck
            // 
            this.chkForceUpdateCheck.AutoSize = true;
            this.chkForceUpdateCheck.Location = new System.Drawing.Point(15, 219);
            this.chkForceUpdateCheck.Name = "chkForceUpdateCheck";
            this.chkForceUpdateCheck.Size = new System.Drawing.Size(195, 17);
            this.chkForceUpdateCheck.TabIndex = 8;
            this.chkForceUpdateCheck.Text = "Force update check on next launch";
            this.chkForceUpdateCheck.UseVisualStyleBackColor = true;
            // 
            // AdvancedSettingsDialog
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(292, 276);
            this.Controls.Add(this.chkForceUpdateCheck);
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
        private System.Windows.Forms.CheckBox chkForceUpdateCheck;
    }
}