namespace DatabaseBackup
{
    partial class ConfigDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigDialog));
            this.btnBrowse = new System.Windows.Forms.Button();
            this.txtDestination = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lbFolders = new System.Windows.Forms.ListBox();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.txtNumBackup = new System.Windows.Forms.NumericUpDown();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.fbdBrowse = new System.Windows.Forms.FolderBrowserDialog();
            this.picBannerImage = new System.Windows.Forms.PictureBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnHelpDateFormat = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.txtDateFormat = new System.Windows.Forms.TextBox();
            this.tabContainer = new System.Windows.Forms.TabControl();
            this.tabDirectories = new System.Windows.Forms.TabPage();
            this.tabGeneral = new System.Windows.Forms.TabPage();
            this.tabTriggers = new System.Windows.Forms.TabPage();
            this.chkBackupClosed = new System.Windows.Forms.CheckBox();
            this.chkBackupSaved = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.txtNumBackup)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picBannerImage)).BeginInit();
            this.tabContainer.SuspendLayout();
            this.tabDirectories.SuspendLayout();
            this.tabGeneral.SuspendLayout();
            this.tabTriggers.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnBrowse
            // 
            this.btnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowse.Location = new System.Drawing.Point(478, 6);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(30, 32);
            this.btnBrowse.TabIndex = 2;
            this.btnBrowse.Text = "...";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // txtDestination
            // 
            this.txtDestination.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDestination.Location = new System.Drawing.Point(91, 11);
            this.txtDestination.Name = "txtDestination";
            this.txtDestination.Size = new System.Drawing.Size(381, 23);
            this.txtDestination.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.label1.Location = new System.Drawing.Point(6, 47);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(127, 17);
            this.label1.TabIndex = 3;
            this.label1.Text = "Backup Directories";
            // 
            // lbFolders
            // 
            this.lbFolders.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.lbFolders.FormattingEnabled = true;
            this.lbFolders.ItemHeight = 17;
            this.lbFolders.Location = new System.Drawing.Point(9, 67);
            this.lbFolders.Name = "lbFolders";
            this.lbFolders.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbFolders.Size = new System.Drawing.Size(499, 174);
            this.lbFolders.TabIndex = 4;
            // 
            // btnAdd
            // 
            this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAdd.Location = new System.Drawing.Point(514, 6);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(68, 32);
            this.btnAdd.TabIndex = 5;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRemove.Enabled = false;
            this.btnRemove.Location = new System.Drawing.Point(514, 67);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(68, 32);
            this.btnRemove.TabIndex = 6;
            this.btnRemove.Text = "Remove";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(185, 17);
            this.label2.TabIndex = 10;
            this.label2.Text = "Number of Backups to Keep";
            // 
            // txtNumBackup
            // 
            this.txtNumBackup.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.txtNumBackup.Location = new System.Drawing.Point(429, 40);
            this.txtNumBackup.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.txtNumBackup.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.txtNumBackup.Name = "txtNumBackup";
            this.txtNumBackup.Size = new System.Drawing.Size(79, 23);
            this.txtNumBackup.TabIndex = 11;
            this.txtNumBackup.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // btnOK
            // 
            this.btnOK.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnOK.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOK.Location = new System.Drawing.Point(400, 356);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(101, 32);
            this.btnOK.TabIndex = 12;
            this.btnOK.Text = "&OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnCancel.Location = new System.Drawing.Point(507, 356);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(101, 32);
            this.btnCancel.TabIndex = 13;
            this.btnCancel.Text = "C&ancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // picBannerImage
            // 
            this.picBannerImage.Dock = System.Windows.Forms.DockStyle.Top;
            this.picBannerImage.Location = new System.Drawing.Point(0, 0);
            this.picBannerImage.Name = "picBannerImage";
            this.picBannerImage.Size = new System.Drawing.Size(620, 60);
            this.picBannerImage.TabIndex = 9;
            this.picBannerImage.TabStop = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.label3.Location = new System.Drawing.Point(6, 14);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(79, 17);
            this.label3.TabIndex = 0;
            this.label3.Text = "Destination";
            // 
            // btnHelpDateFormat
            // 
            this.btnHelpDateFormat.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnHelpDateFormat.CausesValidation = false;
            this.btnHelpDateFormat.Location = new System.Drawing.Point(514, 6);
            this.btnHelpDateFormat.Name = "btnHelpDateFormat";
            this.btnHelpDateFormat.Size = new System.Drawing.Size(68, 32);
            this.btnHelpDateFormat.TabIndex = 9;
            this.btnHelpDateFormat.Text = "Help...";
            this.btnHelpDateFormat.UseVisualStyleBackColor = true;
            this.btnHelpDateFormat.Click += new System.EventHandler(this.btnHelpDateFormat_Click);
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 14);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(82, 17);
            this.label4.TabIndex = 7;
            this.label4.Text = "Date format";
            // 
            // txtDateFormat
            // 
            this.txtDateFormat.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.txtDateFormat.Location = new System.Drawing.Point(94, 11);
            this.txtDateFormat.Name = "txtDateFormat";
            this.txtDateFormat.Size = new System.Drawing.Size(414, 23);
            this.txtDateFormat.TabIndex = 8;
            // 
            // tabContainer
            // 
            this.tabContainer.Controls.Add(this.tabDirectories);
            this.tabContainer.Controls.Add(this.tabGeneral);
            this.tabContainer.Controls.Add(this.tabTriggers);
            this.tabContainer.Location = new System.Drawing.Point(12, 66);
            this.tabContainer.Name = "tabContainer";
            this.tabContainer.SelectedIndex = 0;
            this.tabContainer.Size = new System.Drawing.Size(596, 284);
            this.tabContainer.TabIndex = 14;
            // 
            // tabDirectories
            // 
            this.tabDirectories.Controls.Add(this.txtDestination);
            this.tabDirectories.Controls.Add(this.label3);
            this.tabDirectories.Controls.Add(this.btnBrowse);
            this.tabDirectories.Controls.Add(this.btnAdd);
            this.tabDirectories.Controls.Add(this.label1);
            this.tabDirectories.Controls.Add(this.lbFolders);
            this.tabDirectories.Controls.Add(this.btnRemove);
            this.tabDirectories.Location = new System.Drawing.Point(4, 26);
            this.tabDirectories.Name = "tabDirectories";
            this.tabDirectories.Padding = new System.Windows.Forms.Padding(3);
            this.tabDirectories.Size = new System.Drawing.Size(588, 254);
            this.tabDirectories.TabIndex = 0;
            this.tabDirectories.Text = "Directories";
            this.tabDirectories.UseVisualStyleBackColor = true;
            // 
            // tabGeneral
            // 
            this.tabGeneral.Controls.Add(this.label4);
            this.tabGeneral.Controls.Add(this.btnHelpDateFormat);
            this.tabGeneral.Controls.Add(this.txtDateFormat);
            this.tabGeneral.Controls.Add(this.txtNumBackup);
            this.tabGeneral.Controls.Add(this.label2);
            this.tabGeneral.Location = new System.Drawing.Point(4, 26);
            this.tabGeneral.Name = "tabGeneral";
            this.tabGeneral.Padding = new System.Windows.Forms.Padding(3);
            this.tabGeneral.Size = new System.Drawing.Size(588, 254);
            this.tabGeneral.TabIndex = 1;
            this.tabGeneral.Text = "General";
            this.tabGeneral.UseVisualStyleBackColor = true;
            // 
            // tabTriggers
            // 
            this.tabTriggers.Controls.Add(this.chkBackupClosed);
            this.tabTriggers.Controls.Add(this.chkBackupSaved);
            this.tabTriggers.Location = new System.Drawing.Point(4, 26);
            this.tabTriggers.Name = "tabTriggers";
            this.tabTriggers.Padding = new System.Windows.Forms.Padding(3);
            this.tabTriggers.Size = new System.Drawing.Size(588, 254);
            this.tabTriggers.TabIndex = 2;
            this.tabTriggers.Text = "Triggers";
            this.tabTriggers.UseVisualStyleBackColor = true;
            // 
            // chkBackupClosed
            // 
            this.chkBackupClosed.AutoSize = true;
            this.chkBackupClosed.Location = new System.Drawing.Point(6, 40);
            this.chkBackupClosed.Name = "chkBackupClosed";
            this.chkBackupClosed.Size = new System.Drawing.Size(440, 21);
            this.chkBackupClosed.TabIndex = 1;
            this.chkBackupClosed.Text = "Backup when database is closed (this includes closing KeePass).";
            this.chkBackupClosed.UseVisualStyleBackColor = true;
            // 
            // chkBackupSaved
            // 
            this.chkBackupSaved.AutoSize = true;
            this.chkBackupSaved.Location = new System.Drawing.Point(6, 13);
            this.chkBackupSaved.Name = "chkBackupSaved";
            this.chkBackupSaved.Size = new System.Drawing.Size(237, 21);
            this.chkBackupSaved.TabIndex = 0;
            this.chkBackupSaved.Text = "Backup when database is saved.";
            this.chkBackupSaved.UseVisualStyleBackColor = true;
            // 
            // ConfigDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(620, 400);
            this.Controls.Add(this.tabContainer);
            this.Controls.Add(this.picBannerImage);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "ConfigDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "DatabaseBackup Configuration";
            this.Load += new System.EventHandler(this.frmConfig_Load);
            ((System.ComponentModel.ISupportInitialize)(this.txtNumBackup)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picBannerImage)).EndInit();
            this.tabContainer.ResumeLayout(false);
            this.tabDirectories.ResumeLayout(false);
            this.tabDirectories.PerformLayout();
            this.tabGeneral.ResumeLayout(false);
            this.tabGeneral.PerformLayout();
            this.tabTriggers.ResumeLayout(false);
            this.tabTriggers.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.TextBox txtDestination;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox lbFolders;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown txtNumBackup;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.FolderBrowserDialog fbdBrowse;
        private System.Windows.Forms.PictureBox picBannerImage;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtDateFormat;
        private System.Windows.Forms.Button btnHelpDateFormat;
        private System.Windows.Forms.TabControl tabContainer;
        private System.Windows.Forms.TabPage tabDirectories;
        private System.Windows.Forms.TabPage tabGeneral;
        private System.Windows.Forms.TabPage tabTriggers;
        private System.Windows.Forms.CheckBox chkBackupClosed;
        private System.Windows.Forms.CheckBox chkBackupSaved;
    }
}