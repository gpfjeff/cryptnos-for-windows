namespace com.gpfcomics.Cryptnos
{
    partial class AboutDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutDialog));
            this.lblVersion = new System.Windows.Forms.Label();
            this.lblCopyright = new System.Windows.Forms.Label();
            this.lblLink = new System.Windows.Forms.LinkLabel();
            this.btnOK = new System.Windows.Forms.Button();
            this.txtLicense = new System.Windows.Forms.TextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.btnHelp = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // lblVersion
            // 
            this.lblVersion.Location = new System.Drawing.Point(59, 9);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(221, 23);
            this.lblVersion.TabIndex = 0;
            this.lblVersion.Text = "[Version Label]";
            this.lblVersion.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.toolTip1.SetToolTip(this.lblVersion, "The version of Cryptnos you\r\nare currently using.");
            // 
            // lblCopyright
            // 
            this.lblCopyright.Location = new System.Drawing.Point(59, 32);
            this.lblCopyright.Name = "lblCopyright";
            this.lblCopyright.Size = new System.Drawing.Size(221, 23);
            this.lblCopyright.TabIndex = 1;
            this.lblCopyright.Text = "[Copyright label]";
            this.lblCopyright.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.toolTip1.SetToolTip(this.lblCopyright, "The copyright date for this version\r\nof Cryptnos, so you can determine\r\nits fresh" +
        "ness.");
            // 
            // lblLink
            // 
            this.lblLink.Location = new System.Drawing.Point(59, 55);
            this.lblLink.Name = "lblLink";
            this.lblLink.Size = new System.Drawing.Size(221, 23);
            this.lblLink.TabIndex = 2;
            this.lblLink.TabStop = true;
            this.lblLink.Text = "http://www.cryptnos.com/";
            this.lblLink.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.toolTip1.SetToolTip(this.lblLink, "Click this link to find out more\r\ninformation about Cryptnos\r\nonline.");
            this.lblLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblLink_LinkClicked);
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnOK.Location = new System.Drawing.Point(149, 166);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 5;
            this.btnOK.Text = "OK";
            this.toolTip1.SetToolTip(this.btnOK, "Click this button to return to the\r\nwonder and joy that is Cryptnos.");
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // txtLicense
            // 
            this.txtLicense.Location = new System.Drawing.Point(15, 81);
            this.txtLicense.Multiline = true;
            this.txtLicense.Name = "txtLicense";
            this.txtLicense.ReadOnly = true;
            this.txtLicense.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLicense.Size = new System.Drawing.Size(265, 79);
            this.txtLicense.TabIndex = 3;
            this.txtLicense.Text = resources.GetString("txtLicense.Text");
            this.toolTip1.SetToolTip(this.txtLicense, "The license under which Cryptnos has\r\nbeen released.  A full copy of this\r\nlicens" +
        "e can be found in the Cryptnos\r\ninstallation folder.");
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(15, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(38, 50);
            this.pictureBox1.TabIndex = 5;
            this.pictureBox1.TabStop = false;
            this.toolTip1.SetToolTip(this.pictureBox1, "Our icon.\r\nIsn\'t it nifty?");
            // 
            // toolTip1
            // 
            this.toolTip1.IsBalloon = true;
            // 
            // btnHelp
            // 
            this.btnHelp.Location = new System.Drawing.Point(68, 166);
            this.btnHelp.Name = "btnHelp";
            this.btnHelp.Size = new System.Drawing.Size(75, 23);
            this.btnHelp.TabIndex = 4;
            this.btnHelp.Text = "Help...";
            this.toolTip1.SetToolTip(this.btnHelp, "Click this button to launch\r\nthe Cryptnos HTML help\r\nfile in your default browser" +
        ".");
            this.btnHelp.UseVisualStyleBackColor = true;
            this.btnHelp.Click += new System.EventHandler(this.btnHelp_Click);
            // 
            // AboutDialog
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnOK;
            this.ClientSize = new System.Drawing.Size(292, 199);
            this.Controls.Add(this.btnHelp);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.txtLicense);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.lblLink);
            this.Controls.Add(this.lblCopyright);
            this.Controls.Add(this.lblVersion);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "AboutDialog";
            this.toolTip1.SetToolTip(this, "Our about box.\r\nIsn\'t it nifty?");
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.Label lblCopyright;
        private System.Windows.Forms.LinkLabel lblLink;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.TextBox txtLicense;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button btnHelp;
    }
}