using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace PoGo.NecroBot.Logic.Model.Settings
{
    [JsonObject(Title = "Human Walk Snipe Config", Description = "This feature allow bot pull data from pokemap site, if pokemon match with your config. bot will walk to pokemon's location to catch him.", ItemRequired = Required.DisallowNull)]
    public class HumanWalkSnipeConfig
    {
        [ExcelConfig (Position =1, Description ="Allow bot using human walk sniper feature")]
        [DefaultValue(true)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate)]
        public bool Enable = true;

        [ExcelConfig(Position = 2, Description = "Display list pokemon snipeable in console window")]
        [DefaultValue(true)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate)]
        public bool DisplayPokemonList = true;

        [ExcelConfig(Position = 3, Description = "Max distance that you want bot travel for snipe")]
        [DefaultValue(1500.0)]
        [Range(0, 9999)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate)]
        public double MaxDistance = 1500.0;

        [ExcelConfig(Position = 4, Description = "Max walking time you want bot travel to snipe")]
        [DefaultValue(900.0)]
        [Range(0, 9999)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate)]
        public double MaxEstimateTime = 900.0;

        [ExcelConfig(Position = 5, Description = "Minimun ball available in bag for catch em all mode. this mean continuously snipping if pokemon available.")]
        [DefaultValue(50)]
        [Range(0, 999)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate)]
        public int CatchEmAllMinBalls = 50;

        [ExcelConfig(Position = 6, Description = "Try to catch em all - confused")]
        [DefaultValue(true)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate)]
        public bool TryCatchEmAll = true;

        [ExcelConfig(Position = 7, Description = "Allow catch pokemon when walking to target - overwrite by pokemon filter")]
        [DefaultValue(true)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate)]
        public bool CatchPokemonWhileWalking = true;

        [ExcelConfig(Position = 8, Description = "Allow farm pokestop when walking to target - overwrite by pokemon filter")]
        [DefaultValue(true)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate)]
        public bool SpinWhileWalking = true;

        [ExcelConfig(Position = 9, Description = "Set to make bot return to farm zone define in MaxTravelDistance in location config")]
        [DefaultValue(false)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate)]
        public bool AlwaysWalkback = false;

        [ExcelConfig(Position = 10, Description = "The area the bot looking for pokemon from data service.")]
        [DefaultValue(0.025)]
        [Range(0, 1)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate)]
        public double SnipingScanOffset = 0.025;

        [ExcelConfig(Position = 11, Description = "The max distance bot will always walk back regardless AlwaysWalkback")]
        [DefaultValue(300.0)]
        [Range(0, 9999)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate)]
        public double WalkbackDistanceLimit = 300.0;

        [ExcelConfig(Position = 12, Description = "Turn it on will always looking for pokemon at default location no matter what how far from current location")]
        [DefaultValue(true)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate)]
        public bool IncludeDefaultLocation = true;

        [ExcelConfig(Position = 13, Description = "Use list pokemon pokemon to snipe")] 
        [DefaultValue(true)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate)]
        public bool UseSnipePokemonList = true;

        [ExcelConfig(Position = 14, Description = "The maximun speed up that bot travel when snipe - overwrite by pokemon setting")]
        [DefaultValue(60.0)]
        [Range(0, 999)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate)]
        public double MaxSpeedUpSpeed = 60.0;

        [ExcelConfig(Position = 15, Description = "Allow bot speed up for snipe with the max speed defined above - overwrite by pokemon setting")]
        [DefaultValue(true)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate)]
        public bool AllowSpeedUp = true;

        [ExcelConfig(Position = 16, Description = "Delay time at destination before looking for pokemon - overwrite by pokemon setting")]
        [DefaultValue(10000)]
        [Range(0, 999999)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate)]
        public int DelayTimeAtDestination = 10000;//  10 sec

        [ExcelConfig(Position = 17, Description = "Datasource from pokeradar.info")]
        [DefaultValue(true)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate)]
        public bool UsePokeRadar = false;

        [ExcelConfig(Position = 18, Description = "Datasource from UseSkiplagged.info")]
        [DefaultValue(true)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate)]
        public bool UseSkiplagged = false;

        [ExcelConfig(Position = 19, Description = "Datasource from pokekcrew")]
        [DefaultValue(true)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate)]
        public bool UsePokecrew = false;

        [ExcelConfig(Position = 20, Description = "Datasource from pokesnipers")]
        [DefaultValue(true)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate)]
        public bool UsePokesnipers = false;

        [ExcelConfig(Position = 21, Description = "Datasource from pokezz.info")]
        [DefaultValue(true)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate)]
        public bool UsePokeZZ = false;

        [ExcelConfig(Position = 22, Description = "Datasource from pokewatcher")]
        [DefaultValue(true)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate)]
        public bool UsePokeWatcher = false;

        [ExcelConfig(Position = 23, Description = "Datasource from FPM")]
        [DefaultValue(true)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate)]
        public bool UseFastPokemap = false;

        [ExcelConfig(Position = 24, Description = "Datasource from location feeder")]
        [DefaultValue(false)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate)]
        public bool UsePogoLocationFeeder = false;

        [ExcelConfig(Position = 25, Description = "Allow bot transfer while working to target  - overriteable")]
        [DefaultValue(false)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate)]
        public bool AllowTransferWhileWalking = false;
    }
}
