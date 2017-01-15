using GeoCoordinatePortable;
using POGOProtos.Data;
using POGOProtos.Map.Fort;
using POGOProtos.Networking.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoGo.NecroBot.Logic.Model
{
    public interface IGeoLocation
    {
        double Latitude { get; set; }
        double Longitude { get; set; }
        double Altitude { get; set; }
        string Name { get; set; }
        GeoCoordinate ToGeoCoordinate();
    }
    public class FortLocation : MapLocation
    {
        public FortData FortData { get; set; }
        public FortDetailsResponse FortInfo { get; set; }
        public FortLocation(double lat, double lng, double alt, FortData fortData, FortDetailsResponse fortInfo)  :base(lat, lng, alt)
        {
            this.FortData = fortData;

            if (fortInfo != null)
            {
                this.FortInfo = fortInfo;
                this.Name = fortInfo.Name;
            }
        }
    }
    public class GPXPointLocation : MapLocation
    {
        public  GPXPointLocation(double lat, double lng, double alt)  :base(lat, lng, alt)
        {

        }
    }
    public class SnipeLocation :MapLocation
    {
        public SnipeLocation(double lat, double lng, double alt)  :base(lat, lng, alt)
        {
        }
        public PokemonData Pokemon { get; set; }
    }
    public class MapLocation : IGeoLocation
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public double Altitude { get; set; }
        public string Name { get; set; }

        public MapLocation(double lat, double lng, double alt)
        {
            this.Latitude = lat;
            this.Longitude = lng;
            this.Altitude = alt;
        }

        public GeoCoordinate ToGeoCoordinate()
        {
            return new GeoCoordinate(this.Latitude, this.Longitude, this.Altitude);
        }
    }

} 