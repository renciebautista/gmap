using System;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Drawing;

namespace gmap
{
    public partial class frmFilter : Form
    {
        private DataTable dtSource;
        private string strSearch;
        private string fid;
        private string[] fields;
        private int[] fieldsSize;
        private int returnValue;
        public frmFilter()
        {
            InitializeComponent();
        }
        public DataTable DataSource
        {
            set { dtSource = value; }
        }
        public string SearchFor
        {
            set { strSearch = value; }
        }
        public string[] Fields
        {
            set { fields = value; }
        }
        public int[] FieldsSize
        {
            set { fieldsSize = value; }
        }
        public string FieldId
        {
            set { fid = value; }
        }
        public int FilterValue
        {
            get { return returnValue; }
        }

        private void txtSearch_Enter(object sender, EventArgs e)
        {
            txtSearch.ForeColor = Color.Red;
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            BindGridView();
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }

            if (e.KeyCode == Keys.Up)
            {
                int index = dataGridView.CurrentRow.Index;
                if (index > 0)
                {
                    --index;
                    dataGridView.CurrentCell = dataGridView.Rows[index].Cells[0];
                    dataGridView.Rows[index].Selected = true;
                }

            }
            if (e.KeyCode == Keys.Down)
            {
                int rowCount = dataGridView.Rows.Count;
                int index = dataGridView.CurrentRow.Index;
                --rowCount;
                if (index < rowCount)
                {
                    ++index;
                    dataGridView.CurrentCell = dataGridView.Rows[index].Cells[0];
                    dataGridView.Rows[index].Selected = true;
                }

            }

            if (e.KeyCode == Keys.Enter)
            {
                ReturnValue();
            }

            

        }

        private void frmFilter_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
            }
        }

        private void dataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            ReturnValue();
        }
      

        private void Init()
        {
            this.Text = string.Format("Search {0}", strSearch);

           
            //dataGridView.AutoGenerateColumns = false;


            //dataGridView.Columns[0].HeaderText = "Id";
            //dataGridView.Columns[0].DataPropertyName = fid;
            //dataGridView.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            //dataGridView.Columns[0].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            //dataGridView.Columns[1].HeaderText = strSearch;
            //dataGridView.Columns[1].DataPropertyName = fdesc;
            //dataGridView.Columns[1].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

        }

        private void frmFilter_Load(object sender, EventArgs e)
        {
            Init();
            BindGridView();
        }

        private string generateQuery(string from, string search, bool first)
        {
            if(first){
                return string.Format("{0} LIKE '%{1}%'", from, search.Trim().Replace("'", "''"));
            }
            else
            {
                return string.Format(" OR {0} LIKE '%{1}%'", from, search.Trim().Replace("'", "''"));
            }
            
        }


        private void BindGridView()
        {
            string search ="";
            bool first = true;
            foreach (DataColumn column in dtSource.Columns)
            {
                if(column.ColumnName != "id"){
                    search += generateQuery(column.ColumnName, txtSearch.Text, first);
                    if(first){
                        first = false;
                    }
                }
                
            }

            dtSource.DefaultView.RowFilter = search;
            dataGridView.DataSource = dtSource;
            for (int i = 0; i < fields.Length; i++)
            {
                dataGridView.Columns[i].HeaderText = fields[i].ToString();
                dataGridView.Columns[i].Width = fieldsSize[i];
            }
        }

        private void ReturnValue()
        {
            if (dataGridView.Rows.Count > 0)
            {
                returnValue = Convert.ToInt32(dataGridView.CurrentRow.Cells[0].Value.ToString());
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                returnValue = 0;
                this.DialogResult = DialogResult.Cancel;
            }


        }

        private void dataGridView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ReturnValue();
            }
        }

        private void frmFilter_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
            }
        }

        public void MoveCursor(int grid_id, DataGridView dgView)
        {
            int index = GetIndex(dgView, grid_id);
            dgView.Rows[index].Selected = true;
            try
            {
                dgView.CurrentCell = dgView.Rows[index].Cells[0];
            }
            catch
            {
                dgView.CurrentCell = dgView.Rows[index].Cells[1];
            }
            dgView.FirstDisplayedScrollingRowIndex = index;
            dgView.Update();
        }

        private int GetIndex(DataGridView dgrid, int data_id)
        {
            int rowIndex = -1;
            foreach (DataGridViewRow row in dgrid.Rows)
            {
                if (row.Cells[0].Value.ToString().Equals(data_id.ToString()))
                {
                    rowIndex = row.Index;
                    break;
                }
            }

            return rowIndex;

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }


    }
}
