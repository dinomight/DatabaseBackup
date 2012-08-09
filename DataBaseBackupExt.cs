using KeePass.Plugins;
using KeePass.Forms;
using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;



namespace DatabaseBackup
{
	/// <summary>
	/// This is the main plugin class. It must be named exactly
	/// like the namespace and must be derived from
	/// <c>KeePassPlugin</c>.
	/// </summary>
    public sealed class DatabaseBackupExt : Plugin
	{
		// The sample plugin remembers its host in this variable.
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
			m_tsmiPopup.Text = "DB Backup plug-in";
			tsMenu.Add(m_tsmiPopup);

			// Add menu item 'Backup now'
            m_tsmiBackupNow = new ToolStripMenuItem();
            m_tsmiBackupNow.Text = "Backup DB NOW !";
            m_tsmiBackupNow.Click += OnMenuBackupNow;
            m_tsmiBackupNow.Enabled = true;
            m_tsmiPopup.DropDownItems.Add(m_tsmiBackupNow);

            // Add a separator
            m_tsSeparator = new ToolStripSeparator();
            m_tsmiPopup.DropDownItems.Add(m_tsSeparator);

            // Add menu item 'Backup now'
            m_tsmiAutomaticBackup = new ToolStripMenuItem();
            m_tsmiAutomaticBackup.Text = "Automatically Backup DB";
            m_tsmiAutomaticBackup.Checked = Properties.Settings.Default.AutoBackup;
            m_tsmiAutomaticBackup.Click += OnMenuAutomaticBackup;
            m_tsmiAutomaticBackup.Enabled = true;
            m_tsmiPopup.DropDownItems.Add(m_tsmiAutomaticBackup);

            // Add menu item 'Backup now'
            m_tsmiConfig = new ToolStripMenuItem();
            m_tsmiConfig.Text = "Configure";
            m_tsmiConfig.Click += OnMenuConfig;
            m_tsmiConfig.Enabled = true;
            m_tsmiPopup.DropDownItems.Add(m_tsmiConfig);

         
		

			// We want a notification when the user tried to save the
			// current database
			m_host.MainWindow.FileSaved += OnFileSaved;

			return true; // Initialization successful
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
        /// Backup db to all configure directory
        /// </summary>
        private void _BackupDB()
        {
            if (m_host.Database.IsOpen)
            {
                string SourceFile = "";
                string SourceFileName = "";
                string BackupFile = "";
                if (m_host.Database.IOConnectionInfo.IsLocalFile()) // local file
                {
                    SourceFile = m_host.Database.IOConnectionInfo.Path;
                    FileInfo f = new FileInfo(m_host.Database.IOConnectionInfo.Path);
                    SourceFileName = f.Name;
                    f = null;
                }
                else //remote file
                {
                    SourceFileName = "";
                    if (m_host.MainWindow.Text.IndexOf("-") >= 0)
                    {
                        SourceFileName = m_host.MainWindow.Text.Substring(0, m_host.MainWindow.Text.IndexOf("-"));
                        SourceFileName = SourceFileName.Trim();
                        if (SourceFileName.EndsWith("*"))
                            SourceFileName = SourceFileName.Substring(0, SourceFileName.Length - 1);

                    }
                    SourceFileName=_RemoveSpecialChars(SourceFileName);
                    /*if (!Directory.Exists(Application.StartupPath + "/temp/"))
                        Directory.CreateDirectory(Application.StartupPath + "/temp/");

                    SourceFile = Application.StartupPath + "/temp/" + Guid.NewGuid().ToString() + ".tmp";*/
                    SourceFile = Path.GetTempFileName();
                   
                    System.Net.WebClient wc = new System.Net.WebClient();

                    wc.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);

                    if ((m_host.Database.IOConnectionInfo.UserName.Length > 0) || (m_host.Database.IOConnectionInfo.Password.Length > 0))
                        wc.Credentials = new System.Net.NetworkCredential(m_host.Database.IOConnectionInfo.UserName, m_host.Database.IOConnectionInfo.Password);
                    wc.DownloadFile(m_host.Database.IOConnectionInfo.Path, SourceFile);
                    wc.Dispose();
                    wc = null;
                }
                

                if (Properties.Settings.Default.HistoFolder != null)
                {
                    foreach (string it in Properties.Settings.Default.HistoFolder)
                    {
                        //read log file
                        string BackupLogFile = it + "/" + SourceFileName + "_log";
                        string[] LogFile = null;
                        if (File.Exists(BackupLogFile))
                        {
                            LogFile = File.ReadAllLines(BackupLogFile);
                        }

                        if (Directory.Exists(it))
                        {
                            //create file
                            BackupFile = it + "/" + SourceFileName + "_" + DateTime.Now.ToString(Properties.Settings.Default.DateFormat) + ".kdbx";
                            File.Copy(SourceFile, BackupFile);
                            //delete extra file
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

                            //write log file
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
                //delete temp remote file
               if (!m_host.Database.IOConnectionInfo.IsLocalFile())
                {
                    File.Delete(SourceFile);
                }
            }
            else
            {
                MessageBox.Show("Database is not open.");
            }
        }

        private string _RemoveSpecialChars(string input)
        {
            return Regex.Replace(input, @"[^0-9a-zA-Z\._]", string.Empty);
        }


        private void OnMenuBackupNow(object sender, EventArgs e)
        {
            _BackupDB();
        }

        private void OnMenuAutomaticBackup(object sender, EventArgs e)
		{
            Properties.Settings.Default.AutoBackup  = !Properties.Settings.Default.AutoBackup;
            Properties.Settings.Default.Save();
            ((ToolStripMenuItem)sender).Checked = Properties.Settings.Default.AutoBackup;
		}

        private void OnMenuConfig(object sender, EventArgs e)
        {
            frmConfig frm = new frmConfig();
            frm.ShowDialog();
            frm.Dispose();
            frm = null;
        }

        private void OnFileSaved(object sender, FileSavedEventArgs e)
        {
            if(Properties.Settings.Default.AutoBackup)
                _BackupDB();
        }
	}
}
