using GMap.NET;
using GMap.NET.WindowsForms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gmap
{
    class GMapCustomImageMarker : GMapMarker
    {
        private Image _image;

        public GMapCustomImageMarker(Image Image, PointLatLng p)
            : base(p)
        {
            _image = Image;
        }

        public override void OnRender(Graphics g)
        {
            g.DrawImage(_image, new Point(LocalPosition.X - _image.Width / 2, LocalPosition.Y - _image.Height / 2));
        }
    }
}
