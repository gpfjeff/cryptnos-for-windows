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
            this.rbExportAll = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.rbExportSome = new System.Windows.Forms.RadioButton();
            this.lbSiteList = new System.Windows.Forms.ListBox();
            this.btnExport = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // rbExportAll
            // 
            this.rbExportAll.AutoSize = true;
            this.rbExportAll.Checked = true;
            this.rbExportAll.Location = new System.Drawing.Point(12, 34);
            this.rbExportAll.Name = "rbExportAll";
            this.rbExportAll.Size = new System.Drawing.Size(174, 17);
            this.rbExportAll.TabIndex = 0;
            this.rbExportAll.TabStop = true;
            this.rbExportAll.Text = "Export all saved site parameters";
            this.rbExportAll.UseVisualStyleBackColor = true;
            this.rbExportAll.CheckedChanged += new System.EventHandler(this.rbExportAll_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(266, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Please select which site parameters you wish to export:";
            // 
            // rbExportSome
            // 
            this.rbExportSome.AutoSize = true;
            this.rbExportSome.Location = new System.Drawing.Point(12, 57);
            this.rbExportSome.Name = "rbExportSome";
            this.rbExportSome.Size = new System.Drawing.Size(166, 17);
            this.rbExportSome.TabIndex = 1;
            this.rbExportSome.Text = "Export only the following sites:";
            this.rbExportSome.UseVisualStyleBackColor = true;
            this.rbExportSome.CheckedChanged += new System.EventHandler(this.rbExportSome_CheckedChanged);
            // 
            // lbSiteList
            // 
            this.lbSiteList.FormattingEnabled = true;
            this.lbSiteList.Location = new System.Drawing.Point(12, 80);
            this.lbSiteList.Name = "lbSiteList";
            this.lbSiteList.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.lbSiteList.Size = new System.Drawing.Size(263, 95);
            this.lbSiteList.TabIndex = 2;
            this.lbSiteList.SelectedIndexChanged += new System.EventHandler(this.lbSiteList_SelectedIndexChanged);
            // 
            // btnExport
            // 
            this.btnExport.Location = new System.Drawing.Point(68, 181);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(75, 23);
            this.btnExport.TabIndex = 3;
            this.btnExport.Text = "Export";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(149, 181);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // ExportSitesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 215);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnExport);
            this.Controls.Add(this.lbSiteList);
            this.Controls.Add(this.rbExportSome);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.rbExportAll);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ExportSitesForm";
            this.ShowInTaskbar = false;
            this.Text = "Export";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ExportSitesForm_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton rbExportAll;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton rbExportSome;
        private System.Windows.Forms.ListBox lbSiteList;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Button btnCancel;
    }
}