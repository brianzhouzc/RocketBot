using System.Drawing;
using GMap.NET;
using GMap.NET.WindowsForms;
using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace RocketBot2.Models
{
    [Serializable]
    public class GMapMarkerPokestops : GMapMarker, ISerializable
    {
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        protected virtual new void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            GetObjectData(info, context);
        }
 
        protected GMapMarkerPokestops(SerializationInfo info, StreamingContext context)
           :base(info, context) 
        {
            //not implanted
        }
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="p">The position of the marker</param>
        public GMapMarkerPokestops(PointLatLng p, Image image)
            : base(p)
        {
            MarkerImage = image;
            Size = MarkerImage.Size;
            Offset = new Point(-Size.Width/2, -Size.Height);
        }

        /// <summary>
        ///     The image to display as a marker.
        /// </summary>
        public Image MarkerImage { get; set; }

        public override void OnRender(Graphics g)
        {
            g.DrawImage(MarkerImage, LocalPosition.X + (Size.Width / 4 ), LocalPosition.Y + (Size.Height / 4 * 3 ), Size.Width / 2, Size.Height / 2);
        }
    }
}
