using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using KeePass.UI;

namespace DataBaseBackup
{
    public partial class frmConfig : Form
    {
        public frmConfig()
        {
            InitializeComponent();
        }

        private void frmConfig_Load(object sender, EventArgs e)
        {
            //banner
            picBannerImage.Image = BannerFactory.CreateBanner(picBannerImage.Width,
                picBannerImage.Height, BannerStyle.Default,
                Properties.Resources.hd2_backup48x48, "Configuration", "Configure option for DBBackup");
            this.Icon = Properties.Resources.hd2_backup;

            txtQtyBackup.Value = Properties.Settings.Default.HistoQty;
            if (Properties.Settings.Default.HistoFolder != null)
            {
                foreach (string it in Properties.Settings.Default.HistoFolder)
                    if (it != null)
                        lbFolder.Items.Add(it);
            }
            txtDateFormat.Text = Properties.Settings.Default.DateFormat;
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if (fbdBrowse.ShowDialog() == DialogResult.OK)
            {
                txtDestination.Text = fbdBrowse.SelectedPath;
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if(txtDestination.Text!=""&& System.IO.Directory.Exists(txtDestination.Text))
            {
                lbFolder.Items.Add(txtDestination.Text);
                txtDestination.Text = "";
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
           List<string>  SelectItem = new List<string>();

            foreach (string  it in lbFolder.SelectedItems)
                SelectItem.Add(it);
            foreach (string it in SelectItem)
                lbFolder.Items.Remove(it);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        /// <summary>
        /// function de validation avant enregistrement
        /// </summary>
        /// <returns></returns>
        private bool _Valid()
        {

            return true;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (_Valid())
            {
                Properties.Settings.Default.HistoQty = (uint)txtQtyBackup.Value;
                if (Properties.Settings.Default.HistoFolder == null)
                    Properties.Settings.Default.HistoFolder = new System.Collections.Specialized.StringCollection();
                Properties.Settings.Default.HistoFolder.Clear();
                foreach (string it in lbFolder.Items)
                    Properties.Settings.Default.HistoFolder.Add(it);

                Properties.Settings.Default.DateFormat = txtDateFormat.Text;
                Properties.Settings.Default.Save();
                this.Close();
            }
        }

        

        private void lbFolder_SelectedIndexChanged(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(lbFolder, "");
            if (lbFolder.SelectedItem != null)
                toolTip1.SetToolTip(lbFolder, lbFolder.SelectedItem.ToString());
        }

        private void btnHelpDateFormat_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://msdn.microsoft.com/en-us/library/8kb3ddd4.aspx");
        }

      
    }
}
