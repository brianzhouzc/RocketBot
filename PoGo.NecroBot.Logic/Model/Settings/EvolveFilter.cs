using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using POGOProtos.Enums;

namespace PoGo.NecroBot.Logic.Model.Settings
{
    [JsonObject(Description = "", ItemRequired = Required.DisallowNull)] //Dont set Title
    public class EvolveFilter
    {
        public EvolveFilter()
        {
            Moves = new List<List<PokemonMove>>();
            EnableEvolve = true;
            Operator = "or";
        }

        
        public EvolveFilter(int evolveIV, int evolveLV, int minCP, List<List<PokemonMove>> moves = null)
        {
            EnableEvolve = true;
            this.MinIV = evolveIV;
            this.Moves = moves;
            this.MinLV = evolveLV;
            this.MinCP = MinCP;
            this.Operator = "or";
        }

        [ExcelConfig(IsPrimaryKey = true, Key = "Enable Envolve", Description = "Allow bot auto evolve this pokemon", Position = 1)]
        [DefaultValue(false)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 1)]
        public bool EnableEvolve { get; set; }

        [ExcelConfig(Key = "Evolve Min IV" , Description ="Min IV for auto evolve", Position =2)]
        [DefaultValue(95)]
        [Range(0, 100)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 1)]
        public int MinIV {get; set;}

        [ExcelConfig(Key = "Evolve Min LV", Description = "Min LV for auto evolve", Position = 3)]
        [DefaultValue(95)]
        [Range(0, 999)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 1)]
        public int MinLV { get; set; }

        [ExcelConfig(Key = "Evolve Min CP", Description = "Min CP for auto evolve", Position = 4)]
        [DefaultValue(10)]
        [Range(0, 9999)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 1)]
        public int MinCP { get; set; }

        [ExcelConfig(Key = "Moves" , Description ="Define list of desire move for evolve", Position =5)]
        [DefaultValue(null)]
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Populate, Order = 2)]
        public List<List<PokemonMove>> Moves { get; set; }

        [ExcelConfig(Key = "Operator", Position = 6, Description = "The operator logic use to check for evolve")]
        [DefaultValue("or")]
        [EnumDataType(typeof(Operator))]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 5)]
        public string Operator { get; set; }

        internal static Dictionary<PokemonId, EvolveFilter> Default()
        {
            return new Dictionary<PokemonId, EvolveFilter>
            {
                { PokemonId.Zubat, new EvolveFilter(0, 0,0 ,new List<List<PokemonMove>>() { }) }   ,
                { PokemonId.Pidgey, new EvolveFilter(0, 0,0 ,new List<List<PokemonMove>>() { }) } ,
                { PokemonId.Caterpie, new EvolveFilter(0, 0,0 ,new List<List<PokemonMove>>() { }) }
            };
        }
    }
}