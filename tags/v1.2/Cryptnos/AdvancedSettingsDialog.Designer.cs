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
            this.lblWarning = new System.Windows.Forms.Label();
            this.lblTextEncoding = new System.Windows.Forms.Label();
            this.cmbTextEncodings = new System.Windows.Forms.ComboBox();
            this.lblDefaultEncoding = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblWarning
            // 
            this.lblWarning.Location = new System.Drawing.Point(12, 9);
            this.lblWarning.Name = "lblWarning";
            this.lblWarning.Size = new System.Drawing.Size(268, 60);
            this.lblWarning.TabIndex = 0;
            this.lblWarning.Text = "Modifying advanced settings may \"break\" your existing passwords.  Please do not m" +
                "odify these settings unless you know what you\'re doing or you\'ve been directed t" +
                "o do so by Cryptnos support.";
            // 
            // lblTextEncoding
            // 
            this.lblTextEncoding.Location = new System.Drawing.Point(12, 78);
            this.lblTextEncoding.Name = "lblTextEncoding";
            this.lblTextEncoding.Size = new System.Drawing.Size(268, 30);
            this.lblTextEncoding.TabIndex = 1;
            this.lblTextEncoding.Text = "Text encoding (UTF-8 is strongly recommended for cross-platform compatibility):";
            // 
            // cmbTextEncodings
            // 
            this.cmbTextEncodings.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTextEncodings.FormattingEnabled = true;
            this.cmbTextEncodings.Location = new System.Drawing.Point(15, 111);
            this.cmbTextEncodings.Name = "cmbTextEncodings";
            this.cmbTextEncodings.Size = new System.Drawing.Size(265, 21);
            this.cmbTextEncodings.TabIndex = 2;
            // 
            // lblDefaultEncoding
            // 
            this.lblDefaultEncoding.AutoSize = true;
            this.lblDefaultEncoding.Location = new System.Drawing.Point(12, 135);
            this.lblDefaultEncoding.Name = "lblDefaultEncoding";
            this.lblDefaultEncoding.Size = new System.Drawing.Size(129, 13);
            this.lblDefaultEncoding.TabIndex = 3;
            this.lblDefaultEncoding.Text = "System default encoding: ";
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(68, 163);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 4;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(149, 163);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // AdvancedSettingsDialog
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(292, 199);
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
    }
}