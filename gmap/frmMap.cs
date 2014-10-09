using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
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
        }
    }
}
