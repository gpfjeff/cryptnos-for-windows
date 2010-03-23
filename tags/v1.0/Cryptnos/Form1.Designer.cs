namespace com.gpfcomics.Cryptnos
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtPassphrase = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txtIterations = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.cbHashes = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.cbCharTypes = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtCharLimit = new System.Windows.Forms.TextBox();
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
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.chkLock = new System.Windows.Forms.CheckBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.chkShowTooltips = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
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
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.txtIterations);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.cbHashes);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.cbCharTypes);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txtCharLimit);
            this.groupBox1.Location = new System.Drawing.Point(7, 139);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(278, 118);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Optional Rules:";
            this.toolTip1.SetToolTip(this.groupBox1, resources.GetString("groupBox1.ToolTip"));
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
            this.label6.Size = new System.Drawing.Size(85, 13);
            this.label6.TabIndex = 8;
            this.label6.Text = "Use only the first";
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
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(172, 92);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(57, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "characters";
            // 
            // txtCharLimit
            // 
            this.txtCharLimit.Location = new System.Drawing.Point(115, 89);
            this.txtCharLimit.Name = "txtCharLimit";
            this.txtCharLimit.Size = new System.Drawing.Size(51, 20);
            this.txtCharLimit.TabIndex = 3;
            this.toolTip1.SetToolTip(this.txtCharLimit, resources.GetString("txtCharLimit.ToolTip"));
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
            this.txtPassword.Location = new System.Drawing.Point(61, 100);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.ReadOnly = true;
            this.txtPassword.Size = new System.Drawing.Size(212, 20);
            this.txtPassword.TabIndex = 3;
            this.toolTip1.SetToolTip(this.txtPassword, resources.GetString("txtPassword.ToolTip"));
            // 
            // btnGenerate
            // 
            this.btnGenerate.Location = new System.Drawing.Point(8, 72);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(264, 23);
            this.btnGenerate.TabIndex = 2;
            this.btnGenerate.Text = "Generate";
            this.toolTip1.SetToolTip(this.btnGenerate, "Click this button to generate your\r\npassword.  Note that you must\r\nenter a site n" +
                    "ame and passphrase\r\nbefore this button will work.");
            this.btnGenerate.UseVisualStyleBackColor = true;
            this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
            // 
            // btnClose
            // 
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(228, 370);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(57, 23);
            this.btnClose.TabIndex = 7;
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
            this.btnAbout.Location = new System.Drawing.Point(164, 370);
            this.btnAbout.Name = "btnAbout";
            this.btnAbout.Size = new System.Drawing.Size(58, 23);
            this.btnAbout.TabIndex = 6;
            this.btnAbout.Text = "About...";
            this.toolTip1.SetToolTip(this.btnAbout, "Click this button to see the copyright and\r\nlicensing information about Cryptnos," +
                    " as\r\nwell as a hyperlink to find Cryptnos\r\nonline.");
            this.btnAbout.UseVisualStyleBackColor = true;
            this.btnAbout.Click += new System.EventHandler(this.btnAbout_Click);
            // 
            // btnImport
            // 
            this.btnImport.Location = new System.Drawing.Point(5, 370);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(72, 23);
            this.btnImport.TabIndex = 4;
            this.btnImport.Text = "Import...";
            this.toolTip1.SetToolTip(this.btnImport, resources.GetString("btnImport.ToolTip"));
            this.btnImport.UseVisualStyleBackColor = true;
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // btnForgetAll
            // 
            this.btnForgetAll.Location = new System.Drawing.Point(143, 42);
            this.btnForgetAll.Name = "btnForgetAll";
            this.btnForgetAll.Size = new System.Drawing.Size(120, 23);
            this.btnForgetAll.TabIndex = 3;
            this.btnForgetAll.Text = "Forget All";
            this.toolTip1.SetToolTip(this.btnForgetAll, resources.GetString("btnForgetAll.ToolTip"));
            this.btnForgetAll.UseVisualStyleBackColor = true;
            this.btnForgetAll.Click += new System.EventHandler(this.btnForgetAll_Click);
            // 
            // btnExport
            // 
            this.btnExport.Location = new System.Drawing.Point(83, 370);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(75, 23);
            this.btnExport.TabIndex = 5;
            this.btnExport.Text = "Export...";
            this.toolTip1.SetToolTip(this.btnExport, resources.GetString("btnExport.ToolTip"));
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // btnForget
            // 
            this.btnForget.Location = new System.Drawing.Point(16, 42);
            this.btnForget.Name = "btnForget";
            this.btnForget.Size = new System.Drawing.Size(120, 23);
            this.btnForget.TabIndex = 2;
            this.btnForget.Text = "Forget";
            this.toolTip1.SetToolTip(this.btnForget, resources.GetString("btnForget.ToolTip"));
            this.btnForget.UseVisualStyleBackColor = true;
            this.btnForget.Click += new System.EventHandler(this.btnForget_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cbSites);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.txtPassphrase);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.btnGenerate);
            this.groupBox2.Controls.Add(this.txtPassword);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Location = new System.Drawing.Point(7, 7);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(278, 126);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Core Parameters:";
            this.toolTip1.SetToolTip(this.groupBox2, resources.GetString("groupBox2.ToolTip"));
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.chkLock);
            this.groupBox3.Controls.Add(this.chkRemember);
            this.groupBox3.Controls.Add(this.btnForget);
            this.groupBox3.Controls.Add(this.btnForgetAll);
            this.groupBox3.Location = new System.Drawing.Point(7, 263);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(278, 77);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Remebering Settings:";
            this.toolTip1.SetToolTip(this.groupBox3, resources.GetString("groupBox3.ToolTip"));
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
            this.chkShowTooltips.Location = new System.Drawing.Point(70, 347);
            this.chkShowTooltips.Name = "chkShowTooltips";
            this.chkShowTooltips.Size = new System.Drawing.Size(153, 17);
            this.chkShowTooltips.TabIndex = 3;
            this.chkShowTooltips.Text = "Show verbose tooltips help";
            this.toolTip1.SetToolTip(this.chkShowTooltips, resources.GetString("chkShowTooltips.ToolTip"));
            this.chkShowTooltips.UseVisualStyleBackColor = true;
            this.chkShowTooltips.CheckedChanged += new System.EventHandler(this.chkShowTooltips_CheckedChanged);
            // 
            // Form1
            // 
            this.AcceptButton = this.btnGenerate;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(292, 405);
            this.Controls.Add(this.chkShowTooltips);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.btnExport);
            this.Controls.Add(this.btnImport);
            this.Controls.Add(this.btnAbout);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "Cryptnos";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtPassphrase;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cbCharTypes;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtCharLimit;
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
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox chkLock;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtIterations;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.CheckBox chkShowTooltips;
    }
}

