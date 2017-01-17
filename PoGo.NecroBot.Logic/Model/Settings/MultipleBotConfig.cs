using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using PoGo.NecroBot.Logic.Interfaces.Configuration;
using POGOProtos.Enums;

namespace PoGo.NecroBot.Logic.Model.Settings
{
    public class BotSwitchPokemonFilter
    {
        [JsonIgnore]
        [ExcelConfig(IsPrimaryKey = true, Key = "Allow Switch", Description = "Allow bot use invidual filter for switch", Position = 1)]
        public bool AllowBotSwitch { get; set; }

        [ExcelConfig(Key = "Min IV", Description = "When this pokemon has IV > this value, bot will switch account", Position = 2)]
        [Range(0, 100)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 1)]
        public int IV { get; set; }

        [ExcelConfig(Key = "Min LV", Description = "When this pokemon has LV > this value, bot will switch account", Position = 3)]
        [Range(0, 999)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 2)]
        public int LV { get; set; }

        [ExcelConfig(Key = "Move", Description = "When wild pokemon has the move match , bot will change account to catch", Position = 4)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 3)]
        public List<List<PokemonMove>> Moves { get; set; }


        [ExcelConfig(Key = "Remain times", Description = "Number of second since pokemon disappear ", Position = 5)]
        [Range(0, 900)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 1)]

        public int RemainTimes { get; set; }

        [ExcelConfig(Key = "Operator", Description = "The operator to check ", Position = 6)]
        [EnumDataType(typeof(Operator))]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 1)]
        public string Operator { get; set; }

        public BotSwitchPokemonFilter()
        {
            this.Moves = new List<List<PokemonMove>>();
        }

        public BotSwitchPokemonFilter(int iv, int lv, int remain)
        {
            this.Operator = "or";
            this.Moves = new List<List<PokemonMove>>();
            this.IV = iv;
            this.LV = lv;
            this.RemainTimes = remain;
        }

        public static Dictionary<PokemonId, BotSwitchPokemonFilter> Default()
        {
            return new Dictionary<PokemonId, BotSwitchPokemonFilter>()
            {
                {PokemonId.Lickitung, new BotSwitchPokemonFilter(30, 0, 60)},
                {PokemonId.Dragonite, new BotSwitchPokemonFilter(10, 0, 60)},
                {PokemonId.Lapras, new BotSwitchPokemonFilter(10, 0, 60)},
                {PokemonId.Exeggutor, new BotSwitchPokemonFilter(10, 0, 60)},
                {PokemonId.Magmar, new BotSwitchPokemonFilter(70, 0, 60)},
                {PokemonId.Arcanine, new BotSwitchPokemonFilter(10, 0, 60)},
                {PokemonId.Beedrill, new BotSwitchPokemonFilter(10, 0, 60)},
                {PokemonId.Blastoise, new BotSwitchPokemonFilter(10, 0, 60)},
                {PokemonId.Charizard, new BotSwitchPokemonFilter(10, 0, 60)},
                {PokemonId.Venusaur, new BotSwitchPokemonFilter(10, 100, 60)},
                {PokemonId.Vileplume, new BotSwitchPokemonFilter(10, 0, 60)},
                {PokemonId.Vaporeon, new BotSwitchPokemonFilter(10, 0, 60)},
                {PokemonId.Dragonair, new BotSwitchPokemonFilter(70, 0, 60)},
                {PokemonId.Dratini, new BotSwitchPokemonFilter(90, 100, 60)},
                {PokemonId.Snorlax, new BotSwitchPokemonFilter(30, 0, 60)},
                {PokemonId.Kangaskhan, new BotSwitchPokemonFilter(80, 0, 60)},
                {PokemonId.Ninetales, new BotSwitchPokemonFilter(10, 0, 60)},
                {PokemonId.Electabuzz, new BotSwitchPokemonFilter(10, 0, 60)},
                {PokemonId.Magikarp, new BotSwitchPokemonFilter(95, 0, 60)},
            };
        }
    }

    [JsonObject(Title = "Multiple Bot Config", Description = "Use this to setup the condition when we switch to next bot", ItemRequired = Required.DisallowNull)]
    public class MultipleBotConfig   : BaseConfig
    {
        public MultipleBotConfig() : base()
        {
        }

        [ExcelConfig (Description = "Bot will switch to new account after x minutes ", Position = 1)]
        [DefaultValue(55)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 1)]
        public int RuntimeSwitch = 55;

        [ExcelConfig(Description = "X minitues to block this bot when reach daily limit ", Position = 1)]
        [DefaultValue(15)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 1)]

        public int OnLimitPauseTimes { get; set; }

        [ExcelConfig(Description = "Allow bot switch account when encountered with a rare pokemon that you definied in the list", Position = 2)]
        [DefaultValue(true)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 2)]
        public bool OnRarePokemon { get; set; }

        [ExcelConfig(Description = "Allow bot switch account when encountered with pokemon IV higher than this value", Position = 3)]
        [DefaultValue(90.0)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 3)]
        public double MinIVToSwitch { get; set; }

        [ExcelConfig(Description = "Bot will switch to new account after collect this EXP in one login session ", Position = 4)]
        [DefaultValue(25000)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 3)]
        public int EXPSwitch { get; set; }

        [ExcelConfig(Description = "Bot will switch to new account after x  pokestop farm", Position = 5)]
        [DefaultValue(500)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 4)]
        public int PokestopSwitch { get; set; }

        [ExcelConfig(Description = "Bot will switch to new account after x  pokemon catch ", Position = 6)]
        [DefaultValue(200)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 5)]
        public int PokemonSwitch { get; set; }

        [ExcelConfig(Description = "Bot will switch to new account after x pokemon catch in 1 hours - not being used atm ", Position = 7)]
        [DefaultValue(100)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 6)]
        public int PokemonPerHourSwitch { get; set; } //only apply if runtime > 1h. 

        [ExcelConfig(Description = "Tell bot to start at default location", Position = 8)]
        [DefaultValue(true)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 7)]
        public bool StartFromDefaultLocation { get; set; } //only apply if runtime > 1h. 

        [ExcelConfig(Description = "How many time pokestop softban triger bot switch, 0 is mean doesn't not switch", Position = 9)]
        [DefaultValue(5)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 7)]
        [Range(0, 100)]
        public int PokestopSoftbanCount { get; set; } //only apply if runtime > 1h. 


        [ExcelConfig(Description = "Display bot list (include ran time) on switch", Position = 10)]
        [DefaultValue(true)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 9)]
        public bool DisplayList { get; set; }

        [ExcelConfig(Description = "Bot will display a list of account that you setup in auth.config then ask you to select which account you want to start with.", Position = 11)]
        [DefaultValue(false)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 9)]
        public bool SelectAccountOnStartUp { get; set; }

        [ExcelConfig(Description = "Number of continuously catch flee before switch", Position = 12)]
        [DefaultValue(5)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 12)]
        public int CatchFleeCount{ get; set; }

        [ExcelConfig(Description = "Switch on catch limit", Position = 13)]
        [DefaultValue(true)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 13)]
        public bool SwitchOnCatchLimit { get; set; }


        [ExcelConfig(Description = "Switch on pokestop limit", Position = 14)]
        [DefaultValue(true)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 14)]
        public bool SwitchOnPokestopLimit { get; set; }


        public static MultipleBotConfig Default()
        {
            return new MultipleBotConfig();
        }

        public static bool IsMultiBotActive(ILogicSettings logicSettings)
        {
            return logicSettings.AllowMultipleBot && logicSettings.Bots != null && logicSettings.Bots.Count >= 1;
        }
    }
}