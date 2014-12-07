using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
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
            MainMap.Position = new PointLatLng(14.6, 121);
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
       

        private void plotTrain()
        {
            objects.Markers.Clear();

            List<PointLatLng> positions = new List<PointLatLng>();
            string aLine;

            #region Read File Data
            TextReader file = new StreamReader(config.MyDirectory() + @"\data\data.csv");

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
                Image markerImage = Image.FromFile(config.MyDirectory() + @"\marker\train.png");
                //GMapCustomImageMarker marker = new GMapCustomImageMarker(markerImage, p);
                GMapMarkerImage marker = new GMapMarkerImage(p, markerImage);
                objects.Markers.Add(marker);

                //add marker


            }
            #endregion

            

            //Random rnd = new Random();
            //int index = rnd.Next(1, positions.Count); // creates a number between 1 and 12

            //GMarkerGoogle m = new GMarkerGoogle(positions[index], GMarkerGoogleType.orange);
            //objects.Markers.Add(m);

        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            //if (!Stuff.PingNetwork("pingtest.com"))
            //{
            //    MainMap.Manager.Mode = AccessMode.CacheOnly;
            //}
            //else
            //{
            //    MainMap.Manager.Mode = AccessMode.ServerAndCache;
               
            //}
            //MainMap.ReloadMap();
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
            DataTable dt = SqliteDal.getData("SELECT * FROM devices");
            dataGridView1.DataSource = dt;
        }
    }
}
