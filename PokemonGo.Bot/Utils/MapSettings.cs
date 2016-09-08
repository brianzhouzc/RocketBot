namespace PokemonGo.Bot.Utils
{
    /// <summary>
    /// Contains the downloaded map settings from the PoGo servers.
    /// </summary>
    public class MapSettings
    {
        public MapSettings()
        {
        }

        public MapSettings(POGOProtos.Settings.MapSettings mapSettings)
        {
            EncounterRangeMeters = mapSettings.EncounterRangeMeters;
            GetMapObjectsMaxRefreshSeconds = mapSettings.GetMapObjectsMaxRefreshSeconds;
            GetMapObjectsMinDistanceMeters = mapSettings.GetMapObjectsMinDistanceMeters;
            GetMapObjectsMinRefreshSeconds = mapSettings.GetMapObjectsMinRefreshSeconds;
            GoogleMapsApiKey = mapSettings.GoogleMapsApiKey;
            PokemonVisibleRange = mapSettings.PokemonVisibleRange;
            PokeNavRangeMeters = mapSettings.PokeNavRangeMeters;
        }

        public double EncounterRangeMeters { get; }
        public float GetMapObjectsMaxRefreshSeconds { get; }
        public float GetMapObjectsMinDistanceMeters { get; }
        public float GetMapObjectsMinRefreshSeconds { get; }
        public string GoogleMapsApiKey { get; }
        public double PokemonVisibleRange { get; }
        public double PokeNavRangeMeters { get; }
    }
}