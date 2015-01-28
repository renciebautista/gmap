using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace gmap
{

    public partial class frmMap : Form
    {
        internal readonly GMapOverlay objects = new GMapOverlay("objects");
        public frmMap()
        {
            InitializeComponent();

            // config map         
            MainMap.MapProvider = GMapProviders.OpenCycleTransportMap;
            string[] center = settingsClass.GetValue("center").Split(',');
            MainMap.Position = new PointLatLng(Convert.ToDouble(center[0].ToString()), Convert.ToDouble(center[1].ToString()));
            MainMap.MinZoom = 0;
            MainMap.MaxZoom = 24;
            MainMap.Zoom = 13;
            MainMap.DragButton = MouseButtons.Left;

            MainMap.Overlays.Add(objects);

            // set cache mode only if no internet avaible
            if (!Stuff.PingNetwork("pingtest.com"))
            {
                MainMap.Manager.Mode = AccessMode.CacheOnly;
                MessageBox.Show("No internet connection available, going to CacheOnly mode.", "GMap.NET - Demo.WindowsForms", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }

        private List<int> selected()
        {
            List<int> list = new List<int>();
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (Convert.ToBoolean(row.Cells[1].Value))
                {
                    list.Add(Int32.Parse(row.Cells[0].Value.ToString()));
                }
            }

            return list;
        }

        private void plotTrain()
        {
            this.Text = DateTime.Now.ToString();
            timer1.Enabled = false;

            List<PointLatLng> positions = new List<PointLatLng>();
            List<int> list = selected();

            #region Read File Data
            
            if (list.Count > 0)
            {
                string query = "SELECT * FROM logs" +
                    " inner join devices on devices.id = logs.device_id" +
                    " WHERE device_id IN(" + String.Join(",", list.ToArray()) +  ") " +
                    " GROUP BY device_id ORDER BY id DESC";
                DataTable dt = SqliteDal.getData(query);

                foreach (DataRow row in dt.Rows) // Loop over the rows.
                {
                    PointLatLng p = new PointLatLng
                    {
                        Lat = float.Parse(row["lat"].ToString()),
                        Lng = float.Parse(row["lng"].ToString())
                    };
                    
                    Image markerImage = Image.FromFile(config.MyDirectory() + @"\marker\" + row["image"].ToString());


                   /* Bitmap bmp = new Bitmap(markerImage.Width, markerImage.Height);
                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        g.Clear(Color.SkyBlue);
                        g.InterpolationMode = InterpolationMode.NearestNeighbor;
                        g.PixelOffsetMode = PixelOffsetMode.None;
                        g.DrawImage(markerImage, Point.Empty);
                    }
                    */

                    GMapMarkerImage marker = new GMapMarkerImage(p, markerImage);
                    objects.Markers.Add(marker);

                    // marker.ToolTipMode = MarkerTooltipMode.Always; enable tooltip
                    marker.ToolTipText = row["subscriber_name"].ToString();

                }
            }
            MainMap.Refresh();
            #endregion
            timer1.Enabled = true;
        }
        private void plotTrain_old()
        {
            this.Text = DateTime.Now.ToString();
            timer1.Enabled = false;

            List<PointLatLng> positions = new List<PointLatLng>();
            List<int> list = selected();

            #region Read File Data

            if (list.Count > 0)
            {
                string query = "SELECT * FROM logs" +
                    " inner join devices on devices.id = logs.device_id" +
                    " WHERE device_id IN(" + String.Join(",", list.ToArray()) + ") " +
                    " GROUP BY device_id ORDER BY id DESC";
                DataTable dt = SqliteDal.getData(query);

                foreach (DataRow row in dt.Rows) // Loop over the rows.
                {
                    PointLatLng p = new PointLatLng
                    {
                        Lat = float.Parse(row["lat"].ToString()),
                        Lng = float.Parse(row["lng"].ToString())
                    };
                    //positions.Add(p);
                    //GMapCustomImageMarker marker = new GMapCustomImageMarker(markerImage, p);
                    Image markerImage = Image.FromFile(config.MyDirectory() + @"\marker\" + row["image"]);
                    GMapMarkerImage marker = new GMapMarkerImage(p, markerImage);
                    objects.Markers.Add(marker);

                    //add marker


                }
            }

            /*TextReader file = new StreamReader(config.MyDirectory() + @"\data\data.csv");
            string aLine;
            while ((aLine = file.ReadLine()) != null)
            {
                string[] pos = aLine.Split(',');
                PointLatLng p = new PointLatLng
                {
                    Lat = float.Parse(pos[1]),
                    Lng = float.Parse(pos[2])
                };
                //positions.Add(p);
                //GMarkerGoogle m = new GMarkerGoogle(p, GMarkerGoogleType.green);
                Image markerImage = Image.FromFile(config.MyDirectory() + @"\marker\train_red.png");
                //GMapCustomImageMarker marker = new GMapCustomImageMarker(markerImage, p);
                GMapMarkerImage marker = new GMapMarkerImage(p, markerImage);
                objects.Markers.Add(marker);

                //add marker


            }*/

            MainMap.Refresh();



            #endregion



            //Random rnd = new Random();
            //int index = rnd.Next(1, positions.Count); // creates a number between 1 and 12

            //GMarkerGoogle m = new GMarkerGoogle(positions[index], GMarkerGoogleType.orange);
            //objects.Markers.Add(m);
            timer1.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            objects.Markers.Clear();
            plotTrain();

        }

        private void frmMap_FormClosing(object sender, FormClosingEventArgs e)
        {
            ((frmMain)this.MdiParent).menuControl(true);
        }

        private void frmMap_Activated(object sender, EventArgs e)
        {
            ((frmMain)this.MdiParent).menuControl(false);
        }

        private void frmMap_Load(object sender, EventArgs e)
        {
            // Add a DataGridViewImageColumn to display the images
            DataGridViewImageColumn img = new DataGridViewImageColumn();
            img.Name = "Image";
            dataGridView1.Columns.Insert(2, img);
            dataGridView1.Columns[2].Width = 60;
            dataGridView1.Columns[2].HeaderText = "Icon";

            DataTable dt = SqliteDal.getData("SELECT * FROM devices");
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.DataSource = dt;
        }

        private void dataGridView1_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dataGridView1.IsCurrentCellDirty)
            {
                dataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit);
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
        }

    }
}
