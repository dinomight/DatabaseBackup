using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using KeePass.UI;
using System.Collections.Specialized;

namespace DatabaseBackup
{
    public partial class ConfigDialog : Form
    {
        /// <summary>
        /// Constructor for the form that goes out and initializes using the
        /// designer settings.
        /// </summary>
        public ConfigDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handler for when the dialog is loaded.  From here, we create the
        /// banner and populate the form with the current settings for the
        /// plugin.
        /// </summary>
        /// <param name="sender">Information about the sender.</param>
        /// <param name="e">Event information.</param>
        private void frmConfig_Load(object sender, EventArgs e)
        {
            // banner
            picBannerImage.Image = BannerFactory.CreateBanner(picBannerImage.Width,
                picBannerImage.Height, BannerStyle.Default,
                Properties.Resources.hd2_backup32x32, "Configuration", "Configure your Database Backups");
            this.Icon = Properties.Resources.hd2_backup;

            if (Properties.Settings.Default.BackupFolders != null)
            {
                foreach (string it in Properties.Settings.Default.BackupFolders)
                    if (it != null)
                        lbFolders.Items.Add(it);
            }

            txtNumBackup.Value = Properties.Settings.Default.BackupCount;
            txtDateFormat.Text = Properties.Settings.Default.DateFormat;

            chkBackupClosed.Checked = Properties.Settings.Default.BackupOnFileClosed;
            chkBackupSaved.Checked = Properties.Settings.Default.BackupOnFileSaved;

            lbFolders.SelectedIndexChanged += lbFolders_SelectedIndexChanged;
        }

        /// <summary>
        /// Handler for when the browse button is clicked.  From here, we show a
        /// directory browser so the user can select where the backups are placed.
        /// </summary>
        /// <param name="sender">Information about the sender.</param>
        /// <param name="e">Event information.</param>
        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if (fbdBrowse.ShowDialog() == DialogResult.OK)
            {
                txtDestination.Text = fbdBrowse.SelectedPath;
            }
        }

        /// <summary>
        /// Handler for when the add button is clicked.  If there is text in the
        /// destination field and the directory exists, we go ahead and add it
        /// to the list of configured directories.
        /// </summary>
        /// <param name="sender">Information about the sender.</param>
        /// <param name="e">Event information.</param>
        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (txtDestination.Text != "" && System.IO.Directory.Exists(txtDestination.Text))
            {
                lbFolders.Items.Add(txtDestination.Text);
                txtDestination.Text = "";
            }
        }

        /// <summary>
        /// Handler for when the selected index changes in the folders list box.
        /// This is used to enable or disable the remove button as items are
        /// selected or deselected.
        /// </summary>
        /// <param name="sender">Information about the sender.</param>
        /// <param name="e">Event information.</param>
        private void lbFolders_SelectedIndexChanged(object sender, EventArgs e)
        {
            var items = lbFolders.SelectedItems;
            if (items != null && items.Count > 0)
                btnRemove.Enabled = true;
            else
                btnRemove.Enabled = false;
        }

        /// <summary>
        /// Handler for when the remove button is clicked.  The list of selected
        /// items is copied over and we proceed to remove each of those from the
        /// list of directories where the backups will be stored.
        /// </summary>
        /// <param name="sender">Information about the sender.</param>
        /// <param name="e">Event information.</param>
        private void btnRemove_Click(object sender, EventArgs e)
        {
            List<string> SelectItem = new List<string>();

            foreach (string it in lbFolders.SelectedItems)
                SelectItem.Add(it);

            foreach (string it in SelectItem)
                lbFolders.Items.Remove(it);
        }

        /// <summary>
        /// Handler for when the cancel button is clicked.  We just close down
        /// the dialog as none of the changes need to be saved.
        /// </summary>
        /// <param name="sender">Information about the sender.</param>
        /// <param name="e">Event information.</param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Validates that the form has been filled out properly.
        /// </summary>
        /// <returns>Always returns true.</returns>
        private bool _Valid()
        {
            return true;
        }

        /// <summary>
        /// Handler for when the OK button is clicked.  All of the settings from
        /// the dialog are copied into the saved settings for the plugin.  After
        /// that, the dialog is closed down.
        /// </summary>
        /// <param name="sender">Information about the sender.</param>
        /// <param name="e">Event information.</param>
        private void btnOK_Click(object sender, EventArgs e)
        {
            if (!_Valid())
                return;

            Properties.Settings.Default.BackupCount = (uint)txtNumBackup.Value;
            if (Properties.Settings.Default.BackupFolders == null)
                Properties.Settings.Default.BackupFolders = new StringCollection();

            Properties.Settings.Default.BackupFolders.Clear();
            foreach (string it in lbFolders.Items)
                Properties.Settings.Default.BackupFolders.Add(it);

            Properties.Settings.Default.DateFormat = txtDateFormat.Text;
            Properties.Settings.Default.BackupOnFileClosed = chkBackupClosed.Checked;
            Properties.Settings.Default.BackupOnFileSaved = chkBackupSaved.Checked;

            Properties.Settings.Default.Save();

            this.Close();
        }

        /// <summary>
        /// Handler for when the help button for date format is clicked.  From
        /// here we direct them to the MSDN library page about custom date
        /// format strings.
        /// </summary>
        /// <param name="sender">Information about the sender.</param>
        /// <param name="e">Event information.</param>
        private void btnHelpDateFormat_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://msdn.microsoft.com/en-us/library/8kb3ddd4.aspx");
        }
    }
}
