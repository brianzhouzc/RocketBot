using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using POGOProtos.Enums;

namespace PoGo.NecroBot.Logic.Model.Settings
{
    [JsonObject(Description = "", ItemRequired = Required.DisallowNull)] //Dont set Title
    public class CatchFilter
    {
        public CatchFilter()
        {
            Moves = new List<List<PokemonMove>>();
            EnableCatchFilter = true;
        }


        public CatchFilter(int minIV, int minLV, int minCP, string op = "or", List<List<PokemonMove>> moves = null)
        {
            EnableCatchFilter = true;
            this.MinIV = minIV;
            this.Moves = moves == null ? new List<List<PokemonMove>>() : moves;
            this.MinLV = minLV;
            this.MinCP = MinCP;
            this.Operator = op;
        }

        [ExcelConfig(IsPrimaryKey = true, Key = "Enable Catch filter", Description = "Allow bot check for filter for catch this pokemon", Position = 1)]
        [DefaultValue(false)]
        [JsonIgnore]
        public bool EnableCatchFilter { get; set; }

        [ExcelConfig(Key = "Min IV", Description = "Min IV for catch", Position = 2)]
        [DefaultValue(95)]
        [Range(0, 100)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 1)]
        public int MinIV {get; set;}

        [ExcelConfig(Key = "Min LV", Description = "Min LV for auto catch", Position = 3)]
        [DefaultValue(95)]
        [Range(0, 999)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 1)]
        public int MinLV { get; set; }

        [ExcelConfig(Key = "Min CP", Description = "Min CP for auto catch", Position = 4)]
        [DefaultValue(10)]
        [Range(0, 9999)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 1)]
        public int MinCP { get; set; }

        [ExcelConfig(Key = "Moves", Description = "Define list of desire move for catch", Position = 5)]
        [DefaultValue(null)]
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Populate, Order = 2)]
        public List<List<PokemonMove>> Moves { get; set; }

        [ExcelConfig(Key = "Operator", Position = 6, Description = "The operator logic use to check for catch")]
        [DefaultValue("or")]
        [EnumDataType(typeof(Operator))]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 5)]
        public string Operator { get; set; }

        internal static Dictionary<PokemonId, CatchFilter> Default()
        {
            return new Dictionary<PokemonId, CatchFilter>
            {
                {PokemonId.Lapras, new CatchFilter(0, 0, 0)},
                {PokemonId.Dratini, new CatchFilter(0, 0, 0)},
                {PokemonId.Dragonite, new CatchFilter(0, 0, 0)},
                {PokemonId.Snorlax, new CatchFilter(0, 0, 0)},
                {PokemonId.Zubat, new CatchFilter(100, 100, 100, "and", new List<List<PokemonMove>>() { })}
            };
        }
    }
}