using System;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;

namespace gmap
{
    public partial class frmDevice : Form
    {
        public frmDevice()
        {
            InitializeComponent();
        }

        private void frmDevice_Activated(object sender, EventArgs e)
        {
            if (this.MdiParent != null)
            {
                ((frmMain)this.MdiParent).menuControl(false);
            }
        }

        private void frmDevice_FormClosing(object sender, FormClosingEventArgs e)
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

        private void btnAdd_Click(object sender, EventArgs e)
        {
            using (frmDeviceAdd add = new frmDeviceAdd(frmDeviceAdd.FormMode.Add))
            {
                DialogResult result = add.ShowDialog();
                if (result == DialogResult.OK)
                {
                    bind_Devices();
                }
            }
        }
        private void btnEdit_Click(object sender, EventArgs e)
        {
            using (frmDeviceAdd edit = new frmDeviceAdd(frmDeviceAdd.FormMode.Edit))
            {
                edit.DeviceId = Convert.ToInt32(dataGridView1.CurrentRow.Cells["id"].Value.ToString());
                DialogResult result = edit.ShowDialog();
                if (result == DialogResult.OK)
                {
                    bind_Devices();
                }
            }
        }


        private void frmDevice_Load(object sender, EventArgs e)
        {
            // Add a DataGridViewImageColumn to display the images
            DataGridViewImageColumn img = new DataGridViewImageColumn();
            img.Name = "Image";
            dataGridView1.Columns.Insert(1, img);
            dataGridView1.Columns[1].Width = 60;
            dataGridView1.Columns[1].HeaderText = "Icon";

            bind_Devices();
        }

        private void bind_Devices()
        {
            DataTable dt = SqliteDal.getData("SELECT * FROM devices");
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.DataSource = dt;

            
            ToggleAdd();

        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            int result;
            DataTable dt = SqliteDal.getData("SELECT id, mcc, mnc, ssi FROM devices");

            string[] fields = new string[] { "ID", "MCC", "MNC", "SSI" };
            int[] fieldsSize = new int[] { 50, 100 , 100, 300};

            frmFilter filter = new frmFilter();
            filter.DataSource = dt;
            filter.SearchFor = "Devices";
            filter.FieldId = "id";
            filter.Fields = fields;
            filter.FieldsSize = fieldsSize;
            if (filter.ShowDialog() == DialogResult.OK)
            {
                result = filter.FilterValue;
                bind_Devices();
                filter.MoveCursor(result, dataGridView1);
            }
            filter.Dispose();
            btnAdd.Focus();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to delete this  record?", this.Text,
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                int id = Int32.Parse( dataGridView1.CurrentRow.Cells["id"].Value.ToString());
                int retVal = SqliteDal.execNQ("DELETE FROM devices where id ='" + id + "'");
                if (retVal == 1)
                {
                    bind_Devices();
                }
                else
                {
                    MessageBox.Show("Erorr deleting record.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ToggleAdd()
        {
            if (dataGridView1.Rows.Count > 0)
            {
                btnAdd.Enabled = true;
                btnEdit.Enabled = true;
                btnDelete.Enabled = true;
                btnFind.Enabled = true;

            }
            else
            {
                btnEdit.Enabled = false;
                btnDelete.Enabled = false;
                btnFind.Enabled = false;
            }
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (this.dataGridView1.Columns[e.ColumnIndex].Name == "Image")
            {
                if (dataGridView1["image_path", e.RowIndex].Value.ToString() != "")
                {
                    e.Value = Image.FromFile(config.MyDirectory() + @"\marker\" + dataGridView1["image_path", e.RowIndex].Value.ToString());
                }
                
            }
            //if (e.RowIndex > -1 && e.ColumnIndex == this.dataGridView1.Columns["Image"].Index)
            //{

            //    if (this.dataGridView1["image_path", e.RowIndex].Value != null)
            //    {
            //        string s = this.dataGridView1["image_path", e.RowIndex].Value.ToString();

            //        

            //    }

            //}
        }
    }
}
