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
using System.Linq;
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

            MainMap.Overlays.Add(objects);

            // set cache mode only if no internet avaible
            if (!Stuff.PingNetwork("pingtest.com"))
            {
                MainMap.Manager.Mode = AccessMode.CacheOnly;
                MessageBox.Show("No internet connection available, going to CacheOnly mode.", "GMap.NET - Demo.WindowsForms", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }
        private string MyDirectory()
        {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }

        private void plotTrain()
        {
            objects.Markers.Clear();

            List<PointLatLng> positions = new List<PointLatLng>();
            string aLine;

            #region Read File Data
            TextReader file = new StreamReader(MyDirectory() + @"\data\data.csv");

            while ((aLine = file.ReadLine()) != null)
            {
                string[] pos = aLine.Split(',');
                PointLatLng p = new PointLatLng
                {
                    Lat = float.Parse(pos[1]),
                    Lng = float.Parse(pos[2])
                };
                positions.Add(p);
                //GMarkerGoogle m = new GMarkerGoogle(p, GMarkerGoogleType.orange);
                //objects.Markers.Add(m);

            }
            #endregion

            

            Random rnd = new Random();
            int index = rnd.Next(1, positions.Count); // creates a number between 1 and 12

            GMarkerGoogle m = new GMarkerGoogle(positions[index], GMarkerGoogleType.orange);
            objects.Markers.Add(m);

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
    }
}
