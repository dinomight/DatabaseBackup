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
			m_tsmiPopup.Text = "DB Backup Plug-in";
			tsMenu.Add(m_tsmiPopup);

			// Add menu item 'Backup now'
            m_tsmiBackupNow = new ToolStripMenuItem();
            m_tsmiBackupNow.Text = "Backup Now";
            m_tsmiBackupNow.Click += OnMenuBackupNow;
            m_tsmiBackupNow.Enabled = true;
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
            m_tsmiConfig.Text = "Configure";
            m_tsmiConfig.Click += OnMenuConfig;
            m_tsmiConfig.Enabled = true;
            m_tsmiPopup.DropDownItems.Add(m_tsmiConfig);

			// hook the events we care about
			m_host.MainWindow.FileSaved += OnFileSaved;

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
			m_host.MainWindow.FileSaved -= OnFileSaved;
		}

        /// <summary>
        /// Do the actual database backup to the configured directories.
        /// </summary>
        private void _BackupDB()
        {
            if (!m_host.Database.IsOpen)
            {
                MessageBox.Show("The database isn't open for backup.  How did we "
                    + "get here?");
                return;
            }

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

            if (Properties.Settings.Default.HistoFolder != null)
            {
                foreach (string it in Properties.Settings.Default.HistoFolder)
                {
                    // read log file
                    string BackupLogFile = it + "/" + SourceFileName + "_log";
                    string[] LogFile = null;
                    if (File.Exists(BackupLogFile))
                    {
                        LogFile = File.ReadAllLines(BackupLogFile);
                    }

                    if (Directory.Exists(it))
                    {
                        // create file
                        BackupFile = it + "/" + SourceFileName + "_" + DateTime.Now.ToString(Properties.Settings.Default.DateFormat) + ".kdbx";
                        File.Copy(SourceFile, BackupFile);

                        // delete extra file
                        if (LogFile != null)
                        {
                            if (LogFile.Length + 1 > Properties.Settings.Default.HistoQty)
                            {
                                for (uint LoopDelete = Properties.Settings.Default.HistoQty - 1; LoopDelete < LogFile.Length; LoopDelete++)
                                {
                                    if (File.Exists(LogFile[LoopDelete]))
                                        File.Delete(LogFile[LoopDelete]);
                                }
                            }
                        }

                        // write log file
                        TextWriter fLog = new StreamWriter(BackupLogFile, false);
                        fLog.WriteLine(BackupFile);
                        if (LogFile != null)
                        {
                            uint LoopMax = (uint)LogFile.Length;
                            if (LoopMax > Properties.Settings.Default.HistoQty)
                                LoopMax = Properties.Settings.Default.HistoQty;
                            for (uint i = 0; i < LoopMax; i++)
                                fLog.WriteLine(LogFile[i]);
                        }

                        fLog.Close();
                        fLog.Dispose();
                        fLog = null;
                    }
                }
            }

            // delete temp remote file
            if (!m_host.Database.IOConnectionInfo.IsLocalFile())
            {
                File.Delete(SourceFile);
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
            _BackupDB();
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
            frmConfig frm = new frmConfig();
            frm.ShowDialog();
            frm.Dispose();
            frm = null;
        }

        /// <summary>
        /// Handler for when the database file is saved from the KeePass GUI.
        /// If we are setup to do automatic backup, we go ahead and do the
        /// backup.
        /// </summary>
        /// <param name="sender">Information about the sender of the event.</param>
        /// <param name="e">Event information.</param>
        private void OnFileSaved(object sender, FileSavedEventArgs e)
        {
            if(Properties.Settings.Default.AutoBackup)
                _BackupDB();
        }
	}
}
