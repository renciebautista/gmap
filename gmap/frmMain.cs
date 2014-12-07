using System;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
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
