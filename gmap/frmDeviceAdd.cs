using System;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Collections.Generic;

namespace gmap
{
    public partial class frmDeviceAdd : Form
    {
        public frmDeviceAdd()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            using (SQLiteConnection conn = new SQLiteConnection(config.DataSource))
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    conn.Open();
                    cmd.Connection = conn;

                    SQLiteHelper sh = new SQLiteHelper(cmd);

                    try
                    {
                        var device = new Dictionary<string, object>();
                        device["mcc"] = txtMcc.Text;
                        device["mnc"] = txtMnc.Text;
                        device["ssi"] = txtSsi.Text;

                        sh.Insert("devices", device);

                        this.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }

                    conn.Close();
                }
            }

            this.DialogResult = DialogResult.OK;
        }
    }
}
