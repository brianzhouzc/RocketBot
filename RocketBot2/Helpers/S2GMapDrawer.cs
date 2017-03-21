using System.Collections.Generic;
using System.Drawing;
using GMap.NET;
using GMap.NET.WindowsForms;
using Google.Common.Geometry;
using RocketBot2.Forms;

namespace RocketBot2.Helpers
{
    internal class S2GMapDrawer
    {
        public static void DrawS2Cells(List<ulong> cellsIds, GMapOverlay mapLayer)
        {
            for (var i = 0; i < cellsIds.Count; i++)
            {
                var cellId = new S2CellId((ulong) i);
                var cell = new S2Cell(cellId);

                var points = new List<PointLatLng>();
                for (var j = 0; j < 4; j++)
                {
                    var point = new S2LatLng(cell.GetVertex(j));
                    points.Add(new PointLatLng(point.LatDegrees, point.LngDegrees));
                }
                var polygon = new GMapPolygon(points, "mypolygon");
                polygon.Fill = new SolidBrush(Color.FromArgb(50, Color.Red));
                polygon.Stroke = new Pen(Color.Red, 1);
                mapLayer.Polygons.Add(polygon);
            }
        }
    }
}