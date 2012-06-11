namespace com.gpfcomics.Cryptnos
{
    partial class ImportDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImportDialog));
            this.label1 = new System.Windows.Forms.Label();
            this.btnImport = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.chkSelectAll = new System.Windows.Forms.CheckBox();
            this.listSitesInFile = new System.Windows.Forms.ListView();
            this.colSiteName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(254, 52);
            this.label1.TabIndex = 0;
            this.label1.Text = resources.GetString("label1.Text");
            // 
            // btnImport
            // 
            this.btnImport.Location = new System.Drawing.Point(68, 238);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(75, 23);
            this.btnImport.TabIndex = 2;
            this.btnImport.Text = "Import";
            this.toolTip1.SetToolTip(this.btnImport, "Import the selected sites");
            this.btnImport.UseVisualStyleBackColor = true;
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(149, 238);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.toolTip1.SetToolTip(this.btnCancel, "Cancel this import operation");
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // chkSelectAll
            // 
            this.chkSelectAll.AutoSize = true;
            this.chkSelectAll.Location = new System.Drawing.Point(15, 68);
            this.chkSelectAll.Name = "chkSelectAll";
            this.chkSelectAll.Size = new System.Drawing.Size(120, 17);
            this.chkSelectAll.TabIndex = 4;
            this.chkSelectAll.Text = "Select all sites in file";
            this.toolTip1.SetToolTip(this.chkSelectAll, "Check this box to select all sites from\r\nthe import file at once.  This will disa" +
                    "ble\r\nthe list of individual sites.  Clear this\r\nbox to select sites individually" +
                    ".");
            this.chkSelectAll.UseVisualStyleBackColor = true;
            this.chkSelectAll.CheckedChanged += new System.EventHandler(this.chkSelectAll_CheckedChanged);
            // 
            // listSitesInFile
            // 
            this.listSitesInFile.CheckBoxes = true;
            this.listSitesInFile.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colSiteName});
            this.listSitesInFile.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listSitesInFile.Location = new System.Drawing.Point(15, 91);
            this.listSitesInFile.Name = "listSitesInFile";
            this.listSitesInFile.Size = new System.Drawing.Size(265, 141);
            this.listSitesInFile.TabIndex = 5;
            this.toolTip1.SetToolTip(this.listSitesInFile, resources.GetString("listSitesInFile.ToolTip"));
            this.listSitesInFile.UseCompatibleStateImageBehavior = false;
            this.listSitesInFile.View = System.Windows.Forms.View.Details;
            this.listSitesInFile.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.listSitesInFile_ItemChecked);
            // 
            // colSiteName
            // 
            this.colSiteName.Text = "Site Name";
            this.colSiteName.Width = 255;
            // 
            // toolTip1
            // 
            this.toolTip1.IsBalloon = true;
            // 
            // ImportDialog
            // 
            this.AcceptButton = this.btnImport;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(292, 273);
            this.Controls.Add(this.listSitesInFile);
            this.Controls.Add(this.chkSelectAll);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnImport);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ImportDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Import";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ImportDialog_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnImport;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.CheckBox chkSelectAll;
        private System.Windows.Forms.ListView listSitesInFile;
        private System.Windows.Forms.ColumnHeader colSiteName;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}