using KeePass.Plugins;
using KeePass.Forms;
using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Net;
using System.Net.Cache;

namespace DatabaseBackup
{
    /// <summary>
    /// This is the main plugin class. It must be named exactly the same as the
    /// namespace and must be derived from the KeePass Plugin class.
    /// </summary>
    public sealed class DatabaseBackupExt : Plugin
    {
        // Host that we use to gain access to the KeePass GUI and all the good
        // database pieces.
        private IPluginHost m_host = null;

        private ToolStripSeparator m_tsSeparator = null;
        private ToolStripMenuItem m_tsmiPopup = null;
        private ToolStripMenuItem m_tsmiBackupNow = null;
        private ToolStripMenuItem m_tsmiAutomaticBackup = null;
        private ToolStripMenuItem m_tsmiConfig = null;

        // This flag makes it so we only make backups when the database has
        // actually been modified.
        private bool m_databaseModified = false;

        /// <summary>
        /// Specifies any kind of backup errors that were critical to the
        /// operation of the backup.
        /// </summary>
        enum BackupError
        {
            /// <summary>
            /// Backup completed successfully.
            /// </summary>
            OK,

            /// <summary>
            /// No database was open when the backup was requested.
            /// </summary>
            DATABASE_CLOSED,

            /// <summary>
            /// No folders are configured for backup.
            /// </summary>
            NO_BACKUP_FOLDERS,

            /// <summary>
            /// None of the configured folders were available for backup.
            /// </summary>
            NO_VALID_FOLDERS,
        }

        /// <summary>
        /// The <c>Initialize</c> function is called by KeePass when
        /// you should initialize your plugin (create menu items, etc.).
        /// </summary>
        /// <param name="host">Plugin host interface. By using this
        /// interface, you can access the KeePass main window and the
        /// currently opened database.</param>
        /// <returns>You must return <c>true</c> in order to signal
        /// successful initialization. If you return <c>false</c>,
        /// KeePass unloads your plugin (without calling the
        /// <c>Terminate</c> function of your plugin).</returns>
        public override bool Initialize(IPluginHost host)
        {
            Debug.Assert(host != null);

            if(host == null) return false;
            m_host = host;

            // Get a reference to the 'Tools' menu item container
            ToolStripItemCollection tsMenu = m_host.MainWindow.ToolsMenu.DropDownItems;

            // Add a separator at the bottom
            m_tsSeparator = new ToolStripSeparator();
            tsMenu.Add(m_tsSeparator);

            // Add the popup menu item
            m_tsmiPopup = new ToolStripMenuItem();
            m_tsmiPopup.Text = "Database Backup";
            tsMenu.Add(m_tsmiPopup);

            // Add menu item 'Backup now'
            m_tsmiBackupNow = new ToolStripMenuItem();
            m_tsmiBackupNow.Text = "Backup Now";
            m_tsmiBackupNow.Click += OnMenuBackupNow;
            m_tsmiBackupNow.Enabled = false;
            m_tsmiPopup.DropDownItems.Add(m_tsmiBackupNow);

            // Add a separator
            m_tsSeparator = new ToolStripSeparator();
            m_tsmiPopup.DropDownItems.Add(m_tsSeparator);

            // Add menu item 'Auto Backup'
            m_tsmiAutomaticBackup = new ToolStripMenuItem();
            m_tsmiAutomaticBackup.Text = "Automatically Backup DB";
            m_tsmiAutomaticBackup.Checked = Properties.Settings.Default.AutoBackup;
            m_tsmiAutomaticBackup.Click += OnMenuAutomaticBackup;
            m_tsmiAutomaticBackup.Enabled = true;
            m_tsmiPopup.DropDownItems.Add(m_tsmiAutomaticBackup);

            // Add menu item 'Configure'
            m_tsmiConfig = new ToolStripMenuItem();
            m_tsmiConfig.Text = "Configure...";
            m_tsmiConfig.Click += OnMenuConfig;
            m_tsmiConfig.Enabled = true;
            m_tsmiPopup.DropDownItems.Add(m_tsmiConfig);

            // hook the events we care about
            m_host.MainWindow.FileOpened += OnFileOpened;
            m_host.MainWindow.FileSaving += OnFileSaving;
            m_host.MainWindow.FileSaved += OnFileSaved;
            m_host.MainWindow.FileClosingPre += OnFileClosing;
            m_host.MainWindow.FileCreated += OnFileCreated;

            return true;
        }

        /// <summary>
        /// The <c>Terminate</c> function is called by KeePass when
        /// you should free all resources, close open files/streams,
        /// etc. It is also recommended that you remove all your
        /// plugin menu items from the KeePass menu.
        /// </summary>
        public override void Terminate()
        {
            // Remove all of our menu items
            ToolStripItemCollection tsMenu = m_host.MainWindow.ToolsMenu.DropDownItems;
            tsMenu.Remove(m_tsSeparator);
            tsMenu.Remove(m_tsmiPopup);
            tsMenu.Remove(m_tsmiBackupNow);
            tsMenu.Remove(m_tsmiAutomaticBackup);
            tsMenu.Remove(m_tsmiConfig);

            // Important! Remove event handlers!
            m_host.MainWindow.FileOpened -= OnFileOpened;
            m_host.MainWindow.FileSaving -= OnFileSaving;
            m_host.MainWindow.FileSaved -= OnFileSaved;
            m_host.MainWindow.FileClosingPre -= OnFileClosing;
            m_host.MainWindow.FileCreated -= OnFileCreated;
        }

        /// <summary>
        /// Do the actual database backup to the configured directories.
        /// </summary>
        private BackupError BackupDB()
        {
            if (!m_host.Database.IsOpen)
                return BackupError.DATABASE_CLOSED;

            // make sure we have some folders to actually backup to
            if (Properties.Settings.Default.BackupFolders == null ||
                Properties.Settings.Default.BackupFolders.Count == 0)
                return BackupError.NO_BACKUP_FOLDERS;

            string SourceFile = "";
            string SourceFileName = "";
            string BackupFile = "";

            // get ahold of the password database
            if (m_host.Database.IOConnectionInfo.IsLocalFile())
            {
                // handle local files
                SourceFile = m_host.Database.IOConnectionInfo.Path;
                FileInfo f = new FileInfo(m_host.Database.IOConnectionInfo.Path);
                SourceFileName = f.Name;
                f = null;
            }
            else
            {
                // handle remote files
                SourceFileName = "";
                if (m_host.MainWindow.Text.IndexOf("-") >= 0)
                {
                    SourceFileName = m_host.MainWindow.Text.Substring(0, m_host.MainWindow.Text.IndexOf("-"));
                    SourceFileName = SourceFileName.Trim();
                    if (SourceFileName.EndsWith("*"))
                        SourceFileName = SourceFileName.Substring(0, SourceFileName.Length - 1);

                }

                SourceFileName = _RemoveSpecialChars(SourceFileName);
                SourceFile = Path.GetTempFileName();

                WebClient wc = new WebClient();

                wc.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);

                if ((m_host.Database.IOConnectionInfo.UserName.Length > 0) || (m_host.Database.IOConnectionInfo.Password.Length > 0))
                    wc.Credentials = new NetworkCredential(m_host.Database.IOConnectionInfo.UserName, m_host.Database.IOConnectionInfo.Password);

                wc.DownloadFile(m_host.Database.IOConnectionInfo.Path, SourceFile);
                wc.Dispose();
                wc = null;
            }

            bool backupPerformed = false;
            foreach (var folder in Properties.Settings.Default.BackupFolders)
            {
                if (!Directory.Exists(folder))
                    continue;

                backupPerformed = true;

                // create backup file
                BackupFile = folder + "/" + SourceFileName + "_" +
                    DateTime.Now.ToString(Properties.Settings.Default.DateFormat) + ".kdbx";

                bool backupExists = File.Exists(BackupFile);
                if (backupExists && !Properties.Settings.Default.OverwriteBackup)
                    continue;

                File.Copy(SourceFile, BackupFile, true);

                // if the backup existed, we just overwrote it.  There's no
                // reason to do a purge or update the log.
                if (backupExists)
                    continue;

                // read log file
                string BackupLogFile = folder + "/" + SourceFileName + "_log";
                var LogFile = new string[] { };
                if (File.Exists(BackupLogFile))
                    LogFile = File.ReadAllLines(BackupLogFile);

                // record the newest backup at the top of the file
                var newLog = new StreamWriter(BackupLogFile, false);
                newLog.WriteLine(BackupFile);

                // now go through the set of older backups and remove any that
                // take us over our limit
                for (uint i = 0, backupCount = 1; i < LogFile.Length; ++i, ++backupCount)
                {
                    var oldBackup = LogFile[i];
                    if (backupCount >= Properties.Settings.Default.BackupCount)
                    {
                        // this backup is one more than we need, so get rid of it
                        if (File.Exists(oldBackup))
                            File.Delete(oldBackup);
                    }
                    else
                    {
                        // backup is valid, so keep and record it
                        newLog.WriteLine(oldBackup);
                    }
                }

                newLog.Close();
                newLog.Dispose();
                newLog = null;
            }

            // delete temp remote file
            if (!m_host.Database.IOConnectionInfo.IsLocalFile())
            {
                File.Delete(SourceFile);
            }

            BackupError ret = BackupError.OK;
            if (!backupPerformed)
                ret = BackupError.NO_VALID_FOLDERS;

            return ret;
        }

        /// <summary>
        /// Function to call when an automated backup is triggered.
        /// </summary>
        /// The whole idea is that any issue during an automated backup should
        /// be eaten and not presented to the user.  The reasoning for this is
        /// that these errors are generally benign enough to be ignored anyways.
        private void AutoBackup()
        {
            BackupDB();
        }

        /// <summary>
        /// Function to call when a user triggers a backup operation.
        /// </summary>
        /// This function actually examines the results of a backup operation
        /// and reports the results to the user so they know when something has
        /// happened and is completed.
        private void UserBackup()
        {
            switch (BackupDB())
            {
                case BackupError.DATABASE_CLOSED:
                    MessageBox.Show("A database must be open for a backup to be performed.",
                        "Database Backup Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                case BackupError.NO_BACKUP_FOLDERS:
                    MessageBox.Show("You have not configured any backup directories yet.",
                        "Database Backup Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                case BackupError.NO_VALID_FOLDERS:
                    MessageBox.Show("Backup could not be done with currently configured directories.",
                        "Database Backup Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                case BackupError.OK:
                    MessageBox.Show("Backup completed successfully.",
                        "Database Backup", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
            }
        }

        /// <summary>
        /// Removes any special characters that may appear in the filename so
        /// that we have a clear name when we try and write the backup to the
        /// filesystem.
        /// </summary>
        /// <param name="input">String to be cleaned up.</param>
        /// <returns>The cleaned string.</returns>
        private string _RemoveSpecialChars(string input)
        {
            return Regex.Replace(input, @"[^0-9a-zA-Z\._]", string.Empty);
        }

        /// <summary>
        /// Handler for when the backup now menu item is selected.
        /// </summary>
        /// <param name="sender">Information about the sender of the event.</param>
        /// <param name="e">Event information.</param>
        private void OnMenuBackupNow(object sender, EventArgs e)
        {
            UserBackup();
        }

        /// <summary>
        /// Handler for when the option for toggling automatic backup is clicked.
        /// All this does is toggle the state of the auto backup property.
        /// </summary>
        /// <param name="sender">Information about the sender of the event.</param>
        /// <param name="e">Event information.</param>
        private void OnMenuAutomaticBackup(object sender, EventArgs e)
        {
            Properties.Settings.Default.AutoBackup = !Properties.Settings.Default.AutoBackup;
            Properties.Settings.Default.Save();
            ((ToolStripMenuItem)sender).Checked = Properties.Settings.Default.AutoBackup;
        }

        /// <summary>
        /// Handler for when the configuration menu option is selected.  This
        /// pops up the configuration dialog.
        /// </summary>
        /// <param name="sender">Information about the sender of the event.</param>
        /// <param name="e">Event information.</param>
        private void OnMenuConfig(object sender, EventArgs e)
        {
            ConfigDialog frm = new ConfigDialog();
            frm.ShowDialog();
            frm.Dispose();
            frm = null;
        }

        /// <summary>
        /// Handler for when the database file is opened by KeePass.  We use
        /// this event to enable the 'Backup Now' option from the tool menu.
        /// </summary>
        /// <param name="sender">Information about the sender.</param>
        /// <param name="e">Event information.</param>
        private void OnFileOpened(object sender, FileOpenedEventArgs e)
        {
            m_tsmiBackupNow.Enabled = true;
        }

        /// <summary>
        /// Handler for when a database file is created by KeePass.  For some
        /// reason, the open event doesn't fire when a new database is created,
        /// so we must enable menu options after a file is created.
        /// </summary>
        /// <param name="sender">Information about the sender.</param>
        /// <param name="e">Event information.</param>
        private void OnFileCreated(object sender, FileCreatedEventArgs e)
        {
            m_tsmiBackupNow.Enabled = true;
        }

        /// <summary>
        /// Handler for when a file is being saved by KeePass.  We take this
        /// opportunity to see if the file is actually modified.  We use this
        /// to control later backup events.
        /// </summary>
        /// <param name="sender">Information about the sender.</param>
        /// <param name="e">Event information.</param>
        private void OnFileSaving(object sender, FileSavingEventArgs e)
        {
            m_databaseModified = e.Database.Modified;
        }

        /// <summary>
        /// Determines if an automatic backup should be triggered.  This uses a
        /// combination of the autobackup setting along with the database
        /// modified flag to determine what should happen.
        /// </summary>
        /// <param name="extraConditions">Additional condition (and) required to enable backup. Default is true.</param>
        /// <returns>True if an automatic backup should happen.</returns>
        private bool ShouldAutoBackup(bool extraConditions = true)
        {
            return (!Properties.Settings.Default.AutoBackupModifiedOnly || m_databaseModified) &&
                Properties.Settings.Default.AutoBackup
                && extraConditions;
        }

        /// <summary>
        /// Handler for when the database file is saved from the KeePass GUI.
        /// If we are setup to do automatic backup, we go ahead and do the
        /// backup.  If we do the backup, we also clear the database modified
        /// flag since we now have the latest database in the backup directory.
        /// </summary>
        /// <param name="sender">Information about the sender of the event.</param>
        /// <param name="e">Event information.</param>
        private void OnFileSaved(object sender, FileSavedEventArgs e)
        {
            if (ShouldAutoBackup(Properties.Settings.Default.BackupOnFileSaved))
            {
                AutoBackup();
                m_databaseModified = false;
            }
        }

        /// <summary>
        /// Handler for when the database file is being closed by KeePass.  If
        /// we are setup for automatic backup on this event, we go ahead and do
        /// so.  We also disable any menu actions that require a database to be
        /// open and clear the modified flag.
        /// </summary>
        /// <param name="sender">Information about the sending object.</param>
        /// <param name="e">Event information.</param>
        private void OnFileClosing(object sender, FileClosingEventArgs e)
        {
            if (ShouldAutoBackup(Properties.Settings.Default.BackupOnFileClosed))
                AutoBackup();

            m_databaseModified = false;
            m_tsmiBackupNow.Enabled = false;
        }
    }
}
