using System;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data.SQLite;
namespace gmap
{
    public partial class frmMain : Form
    {
        TigClass tig = new TigClass();
        public frmMain()
        {
            InitializeComponent();
        }

        private void mapViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmMap map = new frmMap();
            map.MdiParent = this;
            map.Show();
            map.WindowState = FormWindowState.Maximized;
        }

        private void deviceMaintenanceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmDevice device = new frmDevice();
            device.MdiParent = this;
            device.Show();
        }

        public void menuControl(bool IsEnabled)
        {
            fileToolStripMenuItem.Enabled = IsEnabled;
            maintenanceToolStripMenuItem.Enabled = IsEnabled;
            helpToolStripMenuItem.Enabled = IsEnabled;
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            config.DatabaseFile = config.MyDirectory() + @"\data\db";

            using (SQLiteConnection conn = new SQLiteConnection(config.DataSource))
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    cmd.Connection = conn;
                    conn.Open();

                    SQLiteHelper sh = new SQLiteHelper(cmd);

                    //Update structure
                    SQLiteTable tb = new SQLiteTable();
                    tb.Columns.Add(new SQLiteColumn("id", true));
                    tb.Columns.Add(new SQLiteColumn("mcc"));
                    tb.Columns.Add(new SQLiteColumn("mnc"));
                    tb.Columns.Add(new SQLiteColumn("ssi"));
                    tb.Columns.Add(new SQLiteColumn("image"));

                    sh.UpdateTableStructure("devices", tb);

                    conn.Close();
                }
            }
            

            
            tig.connect();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmAbout about = new frmAbout();
            about.MdiParent = this;
            about.Show();
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmSettings settings = new frmSettings();
            settings.MdiParent = this;
            settings.Show();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            tig.connect();
        }

    }
}
