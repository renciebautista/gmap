using System;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Collections.Generic;
using System.IO;

namespace gmap
{
    public partial class frmDeviceAdd : Form
    {
        private FormMode form_mode;
        private int device_id;
        private string oldmcc;
        private string oldmnc;
        private string oldssi;
        public enum FormMode
        {
            Add, Edit
        };

        public int DeviceId
        {
            set { device_id = value; }
        }

        public frmDeviceAdd(FormMode mode)
        {
            form_mode = mode;
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (this.Text == "&Save")
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
                            device["mcc"] = txtMcc.Text.Trim();
                            device["mnc"] = txtMnc.Text.Trim();
                            device["ssi"] = txtSsi.Text.Trim();
                            device["image"] = txtImage.Text.Trim();
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
            }
            else
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
                            device["mcc"] = txtMcc.Text.Trim();
                            device["mnc"] = txtMnc.Text.Trim();
                            device["ssi"] = txtSsi.Text.Trim();
                            device["image"] = txtImage.Text.Trim();
                            sh.Update("devices", device, "id", device_id);

                            this.Close();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.ToString());
                        }

                        conn.Close();
                    }
                }
            }
            

            this.DialogResult = DialogResult.OK;
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog fdlg = new OpenFileDialog();
            fdlg.Title = "Device image";
            fdlg.InitialDirectory = Path.Combine(Application.StartupPath,@"marker");
            fdlg.Filter = "PNG Files|*.png";
            fdlg.FilterIndex = 2;
            fdlg.RestoreDirectory = true;
            if (fdlg.ShowDialog() == DialogResult.OK)
            {
                txtImage.Text = fdlg.SafeFileName;
            }
        }

        private void frmDeviceAdd_Load(object sender, EventArgs e)
        {
            if (form_mode == FormMode.Add)
            {
                lblDesc.Text = "New Device";
                btnSave.Text = "&Save";
            }
            else
            {
                lblDesc.Text = "Edit Device";
                btnSave.Text = "&Update";

                string query = "SELECT * FROM devices" +
                    " WHERE id = '" + device_id +"'";
                DataTable dt = SqliteDal.getData(query);

                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        oldmcc = dr["mcc"].ToString();
                        oldmnc = dr["mnc"].ToString();
                        oldssi = dr["ssi"].ToString();
                        txtMcc.Text = oldmcc;
                        txtMnc.Text = oldmnc;
                        txtSsi.Text = oldssi;
                        txtImage.Text = dr["image"].ToString();
                    }
                }
            }
        }
    }
}
