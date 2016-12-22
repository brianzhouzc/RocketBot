namespace PoGo.NecroBot.Logic.Service.Elevation
{
    public interface IElevationService
    {
        string GetServiceId();
        double GetElevation(double lat, double lng);
    }
}