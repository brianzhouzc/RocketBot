using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using POGOProtos.Enums;

namespace PoGo.NecroBot.Logic.Model.Settings
{
    [JsonObject(Description = "", ItemRequired = Required.DisallowNull)] //Dont set Title
    public class SnipeFilter    : BaseConfig
    {
        public SnipeFilter(): base()
        {
            this.Priority = 5;
            Moves = new List<List<PokemonMove>>();
        }

        public SnipeFilter(int snipeMinIV, List<List<PokemonMove>> moves = null)   :base()
        {
            
            this.Operator = "or";
            this.SnipeIV = snipeMinIV;
            this.Moves = moves;
            this.VerifiedOnly = false;
            this.Priority = 5;
        }

        [JsonIgnore]
        [ExcelConfig(IsPrimaryKey = true,Key = "Enable Snipe", Description = "Enable snipe filter for this, if not set it will apply global setting", Position = 1)]
        [DefaultValue(false)]
        public bool EnableSnipe { get; set; }

        [ExcelConfig(Key = "Snipe Min IV" , Description ="Min Pokemon IV for auto snipe", Position =2)]
        [DefaultValue(76)]
        [Range(0, 100)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 1)]
        public int SnipeIV {get; set;}

        [ExcelConfig(Key = "Moves" , Description ="Defined list of moves that you want snipe", Position =3)]
        [DefaultValue(null)]
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Populate, Order = 2)]
        public List<List<PokemonMove>> Moves { get; set; }

        [ExcelConfig(Key = "Operator", Description = "Operator logic check between move and IV", Position = 4)]
        [EnumDataType(typeof(Operator))]
        [DefaultValue("or")]
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Populate, Order = 4)]
        public string Operator { get; set; }

        [ExcelConfig(Key = "Verified Only", Description = "Only catch pokemon that has been verified", Position = 5)]
        [DefaultValue(false)]
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Populate, Order = 5)]
        public bool VerifiedOnly { get; set; }

        [ExcelConfig(Key = "Auto Snipe Priority", Description = "Set autosnipe priority", Position = 6)]
        [DefaultValue(5)]
        [Range(1,10)]
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Populate, Order = 6)]
        public int Priority { get; set; }

        [ExcelConfig(Key = "Auto Snipe Candy", Description = "Set number of candy you want bot snipe for this pokemon", Position = 7)]
        [DefaultValue(2000)]
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Populate, Order = 7)]
        public int AustoSnipeCandy{ get; set; }

        internal static Dictionary<PokemonId, SnipeFilter> SniperFilterDefault()
        {
            return new Dictionary<PokemonId, SnipeFilter>
            {
                { PokemonId.Lapras, new SnipeFilter(0, new List<List<PokemonMove>>() { }) },
                { PokemonId.Dragonite, new SnipeFilter(0, new List<List<PokemonMove>>() { }) },
                { PokemonId.Snorlax, new SnipeFilter(0, new List<List<PokemonMove>>() { }) },
                { PokemonId.Dratini, new SnipeFilter(0, new List<List<PokemonMove>>() { }) },
                { PokemonId.Rhyhorn, new SnipeFilter(0, new List<List<PokemonMove>>() { }) },
                { PokemonId.Abra, new SnipeFilter(0, new List<List<PokemonMove>>() { }) }
            };
        }
    }
}