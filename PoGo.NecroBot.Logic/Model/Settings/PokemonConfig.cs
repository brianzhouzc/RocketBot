using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace PoGo.NecroBot.Logic.Model.Settings
{
    [JsonObject(Title = "Pokemon Config", Description = "Set your pokemon settings.", ItemRequired = Required.DisallowNull)]
    public class PokemonConfig
    {
        internal enum Operator
        {
            or,
            and
        }

        internal enum CpIv
        {
            cp,
            iv
        }

        [ExcelConfig (Description ="Allow bot catch pokemon", Position =1) ]
        /*Catch*/
        [DefaultValue(true)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 1)]
        public bool CatchPokemon = true;

        [ExcelConfig(Description = "Delay time between 2 time catch pokemon ", Position = 2)]
        [DefaultValue(2000)]
        [Range(0, 99999)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 2)]
        public int DelayBetweenPokemonCatch = 2000;

        /*CatchLimit*/
        [ExcelConfig(Description = "Check for daily limit catch rate - 1000/24h", Position = 3)]
        [DefaultValue(true)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 3)]
        public bool UseCatchLimit = true;

        [ExcelConfig(Description = "Number of pokemon allow for catch duration", Position = 4)]
        [DefaultValue(998)]
        [Range(0, 9999)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 4)]
        public int CatchPokemonLimit = 998;

        [ExcelConfig(Description = "Catch duration apply for catch limit  & number", Position = 5)]
        [DefaultValue(60 * 24 + 30)]
        [Range(0, 9999)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 5)]
        public int CatchPokemonLimitMinutes = 60 * 24 + 30;

        /*Incense*/
        [ExcelConfig(Description = "Allow bot use Incense ", Position = 6)]
        [DefaultValue(false)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 6)]
        public bool UseIncenseConstantly;

        /*Egg*/
        [ExcelConfig(Description = "Allow bot put egg in Incubator for hatching", Position = 7)]
        [DefaultValue(true)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 7)]
        public bool UseEggIncubators = true;

        [ExcelConfig(Description = "TUrn this on bot only put 10km egg in to non infinity incubator", Position = 8)]
        [DefaultValue(true)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 8)]
        public bool UseLimitedEggIncubators = true;

        [ExcelConfig(Description = "Turn on to allow bot always use lucky egg when they are available in bag", Position = 9)]
        [DefaultValue(false)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 9)]
        public bool UseLuckyEggConstantly;

        [ExcelConfig(Description = "Number of pokemon ready for evolve that can use lucky egg", Position = 10)]
        [DefaultValue(30)]
        [Range(0, 999)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 10)]
        public int UseLuckyEggsMinPokemonAmount = 30;

        [ExcelConfig(Description = "Allow bot use lucky egg when evolve pokemon", Position = 11)]
        [DefaultValue(false)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 11)]
        public bool UseLuckyEggsWhileEvolving;

        /*Berries*/
        [ExcelConfig(Description = "Specify min CP will be use berries when catch", Position = 12)]
        [DefaultValue(1000)]
        [Range(0, 9999)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 12)]
        public int UseBerriesMinCp = 1000;

        [ExcelConfig(Description = "Specify min IV will be use berries when catch", Position = 13)]
        [DefaultValue(90)]
        [Range(0, 100)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 13)]
        public float UseBerriesMinIv = 90;

        [ExcelConfig(Description = "Specify max catch chance  will be use berries when catch", Position = 14)]
        [DefaultValue(0.20)]
        [Range(0, 1)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 14)]
        public double UseBerriesBelowCatchProbability = 0.20;

        [ExcelConfig(Description = "The operator logic for berry use", Position = 15)]
        [DefaultValue("or")]
        [EnumDataType(typeof(Operator))]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 15)]
        public string UseBerriesOperator = "or";

        [ExcelConfig(Description = "Number of berries can be used for 1 pokemon", Position = 16)]
        [DefaultValue(30)]
        [Range(0, 999)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 16)]
        public int MaxBerriesToUsePerPokemon = 1;

        /*Transfer*/
        [ExcelConfig(Description = "Allow bot transfer weeak/low cp pokemon", Position = 17)]
        [DefaultValue(false)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 17)]
        public bool TransferWeakPokemon;

        [ExcelConfig(Description = "Alow bot transfer all duplicate pokemon", Position = 18)]
        [DefaultValue(true)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 18)]
        public bool TransferDuplicatePokemon = true;

        [ExcelConfig(Description = "Allow bo transfer duplicated pokemon right after catch", Position = 19)]
        [DefaultValue(true)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 19)]
        public bool TransferDuplicatePokemonOnCapture = true;

        /*Rename*/
        [ExcelConfig(Description = "Allow bot rename pokemon after catch", Position = 20)]
        [DefaultValue(false)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 20)]
        public bool RenamePokemon;

        [ExcelConfig(Description = "Set Min IV for rename , bot only rename pokemon has IV higher then this value", Position = 21)]
        [DefaultValue(false)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 21)]
        public bool RenameOnlyAboveIv;

        [ExcelConfig(Description = "The template for pokemon rename", Position = 22)]
        [DefaultValue("{1}_{0}")]
        [MinLength(0)]
        [MaxLength(32)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 22)]
        public string RenameTemplate = "{1}_{0}";

        /*Favorite*/
        [ExcelConfig(Description = "Set min IV for auto favorite pokemon", Position = 23)]
        [DefaultValue(95)]
        [Range(0, 100)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 23)]
        public float FavoriteMinIvPercentage = 95;

        [ExcelConfig(Description = "Allow bot auto favorite pokemon after catch", Position = 24)]
        [DefaultValue(false)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 24)]
        public bool AutoFavoritePokemon;

        /*PokeBalls*/
        [ExcelConfig(Description = "Number of balls will be use for catch a pokemon", Position = 25)]
        [DefaultValue(6)]
        [Range(0, 999)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 25)]
        public int MaxPokeballsPerPokemon = 6;

        [ExcelConfig(Description = "Define min CP for use greate ball instead of poke ball", Position = 26)]
        [DefaultValue(1000)]
        [Range(0, 9999)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 26)]
        public int UseGreatBallAboveCp = 1000;

        [ExcelConfig(Description = "Define min CP for use ultra ball instead of greate ball", Position = 27)]
        [DefaultValue(1250)]
        [Range(0, 9999)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 27)]
        public int UseUltraBallAboveCp = 1250;

        [ExcelConfig(Description = "Define min CP for use master ball instead of ultra ball", Position = 28)]
        [DefaultValue(1500)]
        [Range(0, 9999)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 28)]
        public int UseMasterBallAboveCp = 1500;

        [ExcelConfig(Description = "Define min IV for use greate ball instead of poke ball", Position = 29)]
        [DefaultValue(85.0)]
        [Range(0, 100)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 29)]
        public double UseGreatBallAboveIv = 85.0;

        [ExcelConfig(Description = "Define min CP for use ultra ball instead of greate ball", Position = 30)]
        [DefaultValue(95.0)]
        [Range(0, 100)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 30)]
        public double UseUltraBallAboveIv = 95.0;

        [ExcelConfig(Description = "Define min catch probability for use greate ball instead of pokemon ball", Position = 31)]
        [DefaultValue(0.2)]
        [Range(0, 1)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 31)]
        public double UseGreatBallBelowCatchProbability = 0.2;

        [ExcelConfig(Description = "Define min catch probability for use ultra ball instead of greate ball", Position = 32)]
        [DefaultValue(0.1)]
        [Range(0, 1)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 32)]
        public double UseUltraBallBelowCatchProbability = 0.1;

        [ExcelConfig(Description = "Define min catch probability for use master ball instead of ultra ball", Position = 33)]
        [DefaultValue(0.05)]
        [Range(0, 999)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 33)]
        public double UseMasterBallBelowCatchProbability = 0.05;

        /*PoweUp*/
        [ExcelConfig(Description = "Allow bot power up pokemon ", Position = 34)]
        [DefaultValue(false)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 34)]
        public bool AutomaticallyLevelUpPokemon;

        [ExcelConfig(Description = "Only allow bot upgrade favorited pokemon", Position = 35)]
        [DefaultValue(true)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 35)]
        public bool OnlyUpgradeFavorites = true;

        [ExcelConfig(Description = "Use level up list pokemon", Position = 36)]
        [DefaultValue(true)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 36)]
        public bool UseLevelUpList = true;

        [ExcelConfig(Description = "Number of time upgrade 1 pokemon", Position = 37)]
        [DefaultValue(5)]
        [Range(0, 99)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 37)]
        public int AmountOfTimesToUpgradeLoop = 5;

        [ExcelConfig(Description = "Min startdust keep for auto power up", Position = 38)]
        [DefaultValue(5000)]
        [Range(0, 999999)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 38)]
        public int GetMinStarDustForLevelUp = 5000;

        [ExcelConfig(Description = "Select pokemon to powerup by IV or CP", Position = 39)]
        [DefaultValue("iv")]
        [EnumDataType(typeof(CpIv))]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 39)]
        public string LevelUpByCPorIv = "iv";

        [ExcelConfig(Description = "MIn CP for pokemon upgrade", Position = 40)]
        [DefaultValue(1000)]
        [Range(0, 9999)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 40)]
        public float UpgradePokemonCpMinimum = 1000;

        [ExcelConfig(Description = "MIn IV for pokemon upgrade", Position = 41)]
        [DefaultValue(95)]
        [Range(0, 100)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 41)]
        public float UpgradePokemonIvMinimum = 95;

        [ExcelConfig(Description = "Logic operator for select pokemon for upgrade", Position = 42)]
        [DefaultValue("and")]
        [EnumDataType(typeof(Operator))]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 42)]
        public string UpgradePokemonMinimumStatsOperator = "and";

        /*Evolve*/
        [ExcelConfig(Description = "Specify min IV for evolve pokemon", Position = 43)]
        [DefaultValue(95)]
        [Range(0, 100)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 43)]
        public float EvolveAboveIvValue = 95;

        [ExcelConfig(Description = "Allow bot evolve all pokemon above this IV", Position = 44)]
        [DefaultValue(false)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 44)]
        public bool EvolveAllPokemonAboveIv;

        [ExcelConfig(Description = "When turn on, bot will evolve pokemon when has enought candy", Position = 45)]
        [DefaultValue(true)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 45)]
        public bool EvolveAllPokemonWithEnoughCandy = true;

        [ExcelConfig(Description = "Specify the max storage pokemon bag for trigger evolve", Position = 46)]
        [DefaultValue(90)]
        [Range(0, 100)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 46)]
        public double EvolveKeptPokemonsAtStorageUsagePercentage = 90.0;

        [ExcelConfig(Description = "Specify the pokemon to keep for mass evolve", Position = 47)]
        [DefaultValue(120)]
        [Range(0, 350)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 47)]
        public int EvolveKeptPokemonsOverrideStartIfThisManyReady = 120;
        
        /*Keep*/
        [ExcelConfig(Description = "Allow bot keep low candy pokemon for evolve", Position = 47)]
        [DefaultValue(false)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 48)]
        public bool KeepPokemonsThatCanEvolve;

        [ExcelConfig(Description = "Specify min CP to not transfer pokemon", Position = 48)]
        [DefaultValue(1250)]
        [Range(0, 9999)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 49)]
        public int KeepMinCp = 1250;

        [ExcelConfig(Description = "Specify min IV to not transfer pokemon", Position = 49)]
        [DefaultValue(90)]
        [Range(0, 100)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 50)]
        public float KeepMinIvPercentage = 90;

        [ExcelConfig(Description = "Specify min LV to not transfer pokemon", Position = 50)]
        [DefaultValue(6)]
        [Range(0, 100)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 51)]
        public int KeepMinLvl = 6;

        [ExcelConfig(Description = "Logic operator for keep pokemon check", Position = 51)]
        [DefaultValue("or")]
        [EnumDataType(typeof(Operator))]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 52)]
        public string KeepMinOperator = "or";

        [ExcelConfig(Description = "Tell bot to check level before transfer", Position = 52)]
        [DefaultValue(false)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 53)]
        public bool UseKeepMinLvl;

        [ExcelConfig(Description = "Keep pokemon has higher IV then CP to not transfer pokemon", Position = 53)]
        [DefaultValue(true)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 53)]
        public bool PrioritizeIvOverCp = true;

        [ExcelConfig(Description = "Number of duplicated pokemon to keep", Position = 54)]
        [DefaultValue(1)]
        [Range(0, 999)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 54)]
        public int KeepMinDuplicatePokemon = 1;

        /*NotCatch*/
        [ExcelConfig(Description = "Use list pokemon not catch filter", Position = 55)]
        [DefaultValue(true)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 55)]
        public bool UsePokemonToNotCatchFilter = true;

        [ExcelConfig(Description = "UsePokemonSniperFilterOnly", Position = 56)]
        [DefaultValue(false)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 56)]
        public bool UsePokemonSniperFilterOnly = false;

        /*Dump Stats*/
        [ExcelConfig(Description = "Allow bot dump list pokemon to csv file", Position = 57)]
        [DefaultValue(false)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 57)]
        public bool DumpPokemonStats;

        [DefaultValue(10000)]
        [ExcelConfig(Description = "Delay time between pokemon upgrade", Position = 58)]
        [Range(0, 99999)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 58)]
        public int DelayBetweenPokemonUpgrade = 10000;

        [DefaultValue(5)]
        [ExcelConfig(Description = "Temporary disable catch pokemon for certain minutes if bot run out of balls", Position = 59)]
        [Range(0, 120)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 59)]
        public int OutOfBallCatchBlockTime = 5;

        [DefaultValue(50)]
        [ExcelConfig(Description = "Number of balls you want to save for snipe or manual play - it mean if total ball less than this value, catch pokemon will be deactive", Position = 60)]
        [Range(0, 999)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 60)]
        public int PokeballToKeepForSnipe = 50;
    }
}
