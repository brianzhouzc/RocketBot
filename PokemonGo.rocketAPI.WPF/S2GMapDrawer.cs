using System.Collections.Generic;
using System.Drawing;
using GMap.NET;
using GMap.NET.WindowsPresentation;
using Google.Common.Geometry;

namespace PokemonGo.rocketAPI.WPF
{
    class S2GMapDrawer
    {
        public static void DrawS2Cells(List<ulong> cellsIds, GMapMarker mapLayer)
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
                GMapPolygon polygon = new GMapPolygon(points);
                //polygon.Fill = new SolidBrush(Color.FromArgb(50, Color.Red));
                //polygon.Stroke = new Pen(Color.Red, 1);
                //mapLayer.Markers.Add(polygon);
            }
        }
    }
}
