using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace gmap
{
    public partial class frmSettings : Form
    {
        public frmSettings()
        {
            InitializeComponent();
        }

        private void frmSettings_Activated(object sender, EventArgs e)
        {
            if (this.MdiParent != null)
            {
                ((frmMain)this.MdiParent).menuControl(false);
            }
        }

        private void frmSettings_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.MdiParent != null)
            {
                ((frmMain)this.MdiParent).menuControl(true);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmSettings_Load(object sender, EventArgs e)
        {
            txtUsername.Text = settingsClass.GetValue("tig_username");
            txtVersion.Text = settingsClass.GetValue("tig_version");
            txtServer.Text = settingsClass.GetValue("tig_server");
            txtPort.Text = settingsClass.GetValue("server_port");
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (btnSave.Text == "&Edit")
            {
                btnSave.Text = "&Save";
                txtUsername.Enabled = true;
                txtVersion.Enabled = true;
                txtServer.Enabled = true;
                txtPort.Enabled = true;
            }
            else
            {
                settingsClass.AddValue("tig_username", txtUsername.Text);
                settingsClass.AddValue("tig_version", txtVersion.Text);
                settingsClass.AddValue("tig_server", txtServer.Text);
                settingsClass.AddValue("server_port", txtPort.Text);
                this.Close();
                DialogResult result = MessageBox.Show("Please restart the application to initialize the settings", "System Settings",
                  MessageBoxButtons.OK, MessageBoxIcon.Question);
                if (result == DialogResult.OK)
                {
                    Application.Restart();
                }
            }
           
        }
    }
}
