using Google.Common.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using GMap.NET;
using System.Drawing;

namespace PokemonGo.RocketAPI.Window
{
    class S2GMapDrawer
    {
        public static void DrawS2Cells(List<ulong> cellsIds, GMapOverlay mapLayer)
        {
            for (int i=0; i<cellsIds.Count; i++)
            {
                S2CellId cellId = new S2CellId(cellsIds[i]);
                S2Cell cell = new S2Cell(cellId);
                
                List<PointLatLng> points = new List<PointLatLng>();
                for (int j=0; j<4; j++)
                {
                    S2LatLng point = new S2LatLng(cell.GetVertex(j));
                    points.Add(new PointLatLng(point.LatDegrees, point.LngDegrees));
                }
                GMapPolygon polygon = new GMapPolygon(points, "mypolygon");
                polygon.Fill = new SolidBrush(Color.FromArgb(50, Color.Red));
                polygon.Stroke = new Pen(Color.Red, 1);
                mapLayer.Polygons.Add(polygon);
            }
        }
    }
}
