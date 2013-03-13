namespace com.gpfcomics.Cryptnos
{
    partial class ExportSitesForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExportSitesForm));
            this.rbExportAll = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.rbExportSome = new System.Windows.Forms.RadioButton();
            this.lbSiteList = new System.Windows.Forms.ListBox();
            this.btnExport = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.tabExportFile = new System.Windows.Forms.TabPage();
            this.tabExportQRCode = new System.Windows.Forms.TabPage();
            this.btnExportClose = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.cmboExportSiteQR = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabExportFile.SuspendLayout();
            this.tabExportQRCode.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // rbExportAll
            // 
            this.rbExportAll.AutoSize = true;
            this.rbExportAll.Checked = true;
            this.rbExportAll.Location = new System.Drawing.Point(6, 51);
            this.rbExportAll.Name = "rbExportAll";
            this.rbExportAll.Size = new System.Drawing.Size(174, 17);
            this.rbExportAll.TabIndex = 0;
            this.rbExportAll.TabStop = true;
            this.rbExportAll.Text = "Export all saved site parameters";
            this.toolTip1.SetToolTip(this.rbExportAll, "Select this radio button to export all of your\r\nsite parameters in one file.  Thi" +
        "nk of this as\r\na shortcut to selecting all of the sites in the\r\nlist below.");
            this.rbExportAll.UseVisualStyleBackColor = true;
            this.rbExportAll.CheckedChanged += new System.EventHandler(this.rbExportAll_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(231, 26);
            this.label1.TabIndex = 5;
            this.label1.Text = "Please select which site parameters you wish to\r\nexport:";
            // 
            // rbExportSome
            // 
            this.rbExportSome.AutoSize = true;
            this.rbExportSome.Location = new System.Drawing.Point(6, 74);
            this.rbExportSome.Name = "rbExportSome";
            this.rbExportSome.Size = new System.Drawing.Size(166, 17);
            this.rbExportSome.TabIndex = 1;
            this.rbExportSome.Text = "Export only the following sites:";
            this.toolTip1.SetToolTip(this.rbExportSome, "Select this radio button if you would like to\r\nexport only certain site parameter" +
        "s, but not\r\nall of them.  The site list below will become\r\nenabled.");
            this.rbExportSome.UseVisualStyleBackColor = true;
            this.rbExportSome.CheckedChanged += new System.EventHandler(this.rbExportSome_CheckedChanged);
            // 
            // lbSiteList
            // 
            this.lbSiteList.FormattingEnabled = true;
            this.lbSiteList.Location = new System.Drawing.Point(6, 97);
            this.lbSiteList.Name = "lbSiteList";
            this.lbSiteList.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.lbSiteList.Size = new System.Drawing.Size(249, 173);
            this.lbSiteList.TabIndex = 2;
            this.toolTip1.SetToolTip(this.lbSiteList, resources.GetString("lbSiteList.ToolTip"));
            this.lbSiteList.SelectedIndexChanged += new System.EventHandler(this.lbSiteList_SelectedIndexChanged);
            // 
            // btnExport
            // 
            this.btnExport.Location = new System.Drawing.Point(52, 281);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(75, 23);
            this.btnExport.TabIndex = 3;
            this.btnExport.Text = "Export";
            this.toolTip1.SetToolTip(this.btnExport, resources.GetString("btnExport.ToolTip"));
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(133, 281);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.toolTip1.SetToolTip(this.btnCancel, "Click this button to cancel the export\r\nprocess.  You will be returned to the\r\nma" +
        "in Cryptnos window.");
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // toolTip1
            // 
            this.toolTip1.IsBalloon = true;
            // 
            // tabExportFile
            // 
            this.tabExportFile.Controls.Add(this.rbExportAll);
            this.tabExportFile.Controls.Add(this.btnCancel);
            this.tabExportFile.Controls.Add(this.label1);
            this.tabExportFile.Controls.Add(this.btnExport);
            this.tabExportFile.Controls.Add(this.rbExportSome);
            this.tabExportFile.Controls.Add(this.lbSiteList);
            this.tabExportFile.Location = new System.Drawing.Point(4, 22);
            this.tabExportFile.Name = "tabExportFile";
            this.tabExportFile.Padding = new System.Windows.Forms.Padding(3);
            this.tabExportFile.Size = new System.Drawing.Size(261, 310);
            this.tabExportFile.TabIndex = 0;
            this.tabExportFile.Text = "To File";
            this.toolTip1.SetToolTip(this.tabExportFile, "This tab will allow you to export any or all of\r\nyour site parameters to an encry" +
        "pted file.\r\nThis file can then be imported into another\r\ncopy of Cryptnos on a d" +
        "ifferent device.\r\n");
            this.tabExportFile.UseVisualStyleBackColor = true;
            // 
            // tabExportQRCode
            // 
            this.tabExportQRCode.Controls.Add(this.btnExportClose);
            this.tabExportQRCode.Controls.Add(this.pictureBox1);
            this.tabExportQRCode.Controls.Add(this.cmboExportSiteQR);
            this.tabExportQRCode.Controls.Add(this.label2);
            this.tabExportQRCode.Location = new System.Drawing.Point(4, 22);
            this.tabExportQRCode.Name = "tabExportQRCode";
            this.tabExportQRCode.Padding = new System.Windows.Forms.Padding(3);
            this.tabExportQRCode.Size = new System.Drawing.Size(261, 310);
            this.tabExportQRCode.TabIndex = 1;
            this.tabExportQRCode.Text = "To QR Code";
            this.toolTip1.SetToolTip(this.tabExportQRCode, resources.GetString("tabExportQRCode.ToolTip"));
            this.tabExportQRCode.UseVisualStyleBackColor = true;
            // 
            // btnExportClose
            // 
            this.btnExportClose.Location = new System.Drawing.Point(93, 278);
            this.btnExportClose.Name = "btnExportClose";
            this.btnExportClose.Size = new System.Drawing.Size(75, 23);
            this.btnExportClose.TabIndex = 3;
            this.btnExportClose.Text = "Close";
            this.toolTip1.SetToolTip(this.btnExportClose, "Close the Export dialog");
            this.btnExportClose.UseVisualStyleBackColor = true;
            this.btnExportClose.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(30, 72);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(200, 200);
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            this.toolTip1.SetToolTip(this.pictureBox1, resources.GetString("pictureBox1.ToolTip"));
            // 
            // cmboExportSiteQR
            // 
            this.cmboExportSiteQR.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmboExportSiteQR.FormattingEnabled = true;
            this.cmboExportSiteQR.Location = new System.Drawing.Point(9, 43);
            this.cmboExportSiteQR.Name = "cmboExportSiteQR";
            this.cmboExportSiteQR.Size = new System.Drawing.Size(246, 21);
            this.cmboExportSiteQR.TabIndex = 1;
            this.toolTip1.SetToolTip(this.cmboExportSiteQR, "To export a site via QRCode, select the\r\nsite name from this drop-down list.  The" +
        "\r\nQRCode will be generated below.");
            this.cmboExportSiteQR.SelectedIndexChanged += new System.EventHandler(this.cmboExportSiteQR_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 14);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(232, 26);
            this.label2.TabIndex = 0;
            this.label2.Text = "Select a site to export.  Note that QR Codes are\r\nNOT encrypted!";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabExportFile);
            this.tabControl1.Controls.Add(this.tabExportQRCode);
            this.tabControl1.Location = new System.Drawing.Point(11, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(269, 336);
            this.tabControl1.TabIndex = 6;
            // 
            // ExportSitesForm
            // 
            this.AcceptButton = this.btnExport;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(292, 358);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ExportSitesForm";
            this.ShowInTaskbar = false;
            this.Text = "Export";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ExportSitesForm_FormClosing);
            this.tabExportFile.ResumeLayout(false);
            this.tabExportFile.PerformLayout();
            this.tabExportQRCode.ResumeLayout(false);
            this.tabExportQRCode.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RadioButton rbExportAll;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton rbExportSome;
        private System.Windows.Forms.ListBox lbSiteList;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabExportFile;
        private System.Windows.Forms.TabPage tabExportQRCode;
        private System.Windows.Forms.ComboBox cmboExportSiteQR;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnExportClose;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}