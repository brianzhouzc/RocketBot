using System.Drawing;
using GMap.NET;
using GMap.NET.WindowsForms;
using System.Runtime.Serialization;
using System;
using System.Security.Permissions;

namespace RocketBot2.Models
{
    [Serializable]
    public class GMapMarkerTrainer : GMapMarker, ISerializable
    {
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        protected virtual new void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            GetObjectData(info, context);
        }

        protected GMapMarkerTrainer(SerializationInfo info, StreamingContext context)
           :base(info, context) 
        {
            //not implanted
        }
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="p">The position of the marker</param>
        public GMapMarkerTrainer(PointLatLng p, Image image)
            : base(p)
        {
            MarkerImage = image;
            Size = MarkerImage.Size;
            Offset = new Point(-Size.Width, -Size.Height);
        }

        /// <summary>
        ///     The image to display as a marker.
        /// </summary>
        public Image MarkerImage { get; set; }

        public override void OnRender(Graphics g)
        {
            g.DrawImage(MarkerImage, LocalPosition.X + (Size.Width / 2), LocalPosition.Y + (Size.Height / 2), Size.Width, Size.Height);
        }
    }
}
