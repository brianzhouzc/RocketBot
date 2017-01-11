using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace PoGo.NecroBot.Logic.Model.Settings
{
    [JsonObject(Title = "Pokemon Config", Description = "Set your pokemon settings.", ItemRequired = Required.DisallowNull)]
    public class PokemonConfig  :BaseConfig
    {
        public PokemonConfig() :base()
        {

        }

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
        public bool CatchPokemon { get; set; }

        [ExcelConfig(Description = "Delay time between 2 time catch pokemon ", Position = 2)]
        [DefaultValue(2000)]
        [Range(0, 99999)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 2)]
        public int DelayBetweenPokemonCatch{ get; set; }

        /*CatchLimit*/
        [ExcelConfig(Description = "Check for daily limit catch rate - 1000/24h", Position = 3)]
        [DefaultValue(true)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 3)]
        public bool UseCatchLimit { get; set; }

        [ExcelConfig(Description = "Number of pokemon allow for catch duration", Position = 4)]
        [DefaultValue(998)]
        [Range(0, 9999)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 4)]
        public int CatchPokemonLimit{ get; set; }

        [ExcelConfig(Description = "Catch duration apply for catch limit  & number", Position = 5)]
        [DefaultValue(60 * 24 + 30)]
        [Range(0, 9999)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 5)]
        public int CatchPokemonLimitMinutes{ get; set; }

        /*Incense*/
        [ExcelConfig(Description = "Allow bot use Incense ", Position = 6)]
        [DefaultValue(false)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 6)]
        public bool UseIncenseConstantly;

        /*Egg*/
        [ExcelConfig(Description = "Allow bot put egg in Incubator for hatching", Position = 7)]
        [DefaultValue(true)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 7)]
        public bool UseEggIncubators { get; set; }

        [ExcelConfig(Description = "TUrn this on bot only put 10km egg in to non infinity incubator", Position = 8)]
        [DefaultValue(true)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 8)]
        public bool UseLimitedEggIncubators { get; set; }

        [ExcelConfig(Description = "Turn on to allow bot always use lucky egg when they are available in bag", Position = 9)]
        [DefaultValue(false)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 9)]
        public bool UseLuckyEggConstantly;

        [ExcelConfig(Description = "Number of pokemon ready for evolve that can use lucky egg", Position = 10)]
        [DefaultValue(30)]
        [Range(0, 999)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 10)]
        public int UseLuckyEggsMinPokemonAmount{ get; set; }

        [ExcelConfig(Description = "Allow bot use lucky egg when evolve pokemon", Position = 11)]
        [DefaultValue(false)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 11)]
        public bool UseLuckyEggsWhileEvolving;

        /*Berries*/
        [ExcelConfig(Description = "Specify min CP will be use berries when catch", Position = 12)]
        [DefaultValue(1000)]
        [Range(0, 9999)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 12)]
        public int UseBerriesMinCp{ get; set; }

        [ExcelConfig(Description = "Specify min IV will be use berries when catch", Position = 13)]
        [DefaultValue(90)]
        [Range(0, 100)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 13)]
        public float UseBerriesMinIv{ get; set; }

        [ExcelConfig(Description = "Specify max catch chance  will be use berries when catch", Position = 14)]
        [DefaultValue(0.20)]
        [Range(0, 1)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 14)]
        public double UseBerriesBelowCatchProbability{ get; set; }

        [ExcelConfig(Description = "The operator logic for berry use", Position = 15)]
        [DefaultValue("or")]
        [EnumDataType(typeof(Operator))]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 15)]
        public string UseBerriesOperator{ get; set; }

        [ExcelConfig(Description = "Number of berries can be used for 1 pokemon", Position = 16)]
        [DefaultValue(30)]
        [Range(0, 999)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 16)]
        public int MaxBerriesToUsePerPokemon{ get; set; }

        /*Transfer*/
        [ExcelConfig(Description = "Allow bot transfer weeak/low cp pokemon", Position = 17)]
        [DefaultValue(false)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 17)]
        public bool TransferWeakPokemon;

        [ExcelConfig(Description = "Alow bot transfer all duplicate pokemon", Position = 18)]
        [DefaultValue(true)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 18)]
        public bool TransferDuplicatePokemon { get; set; }

        [ExcelConfig(Description = "Allow bo transfer duplicated pokemon right after catch", Position = 19)]
        [DefaultValue(true)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 19)]
        public bool TransferDuplicatePokemonOnCapture { get; set; }

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
        public string RenameTemplate{ get; set; }

        /*Favorite*/
        [ExcelConfig(Description = "Set min IV for auto favorite pokemon", Position = 23)]
        [DefaultValue(95)]
        [Range(0, 100)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 23)]
        public float FavoriteMinIvPercentage{ get; set; }

        [ExcelConfig(Description = "Allow bot auto favorite pokemon after catch", Position = 24)]
        [DefaultValue(false)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 24)]
        public bool AutoFavoritePokemon;

        /*PokeBalls*/
        [ExcelConfig(Description = "Number of balls will be use for catch a pokemon", Position = 25)]
        [DefaultValue(6)]
        [Range(0, 999)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 25)]
        public int MaxPokeballsPerPokemon{ get; set; }

        [ExcelConfig(Description = "Define min CP for use greate ball instead of poke ball", Position = 26)]
        [DefaultValue(1000)]
        [Range(0, 9999)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 26)]
        public int UseGreatBallAboveCp{ get; set; }

        [ExcelConfig(Description = "Define min CP for use ultra ball instead of greate ball", Position = 27)]
        [DefaultValue(1250)]
        [Range(0, 9999)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 27)]
        public int UseUltraBallAboveCp{ get; set; }

        [ExcelConfig(Description = "Define min CP for use master ball instead of ultra ball", Position = 28)]
        [DefaultValue(1500)]
        [Range(0, 9999)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 28)]
        public int UseMasterBallAboveCp{ get; set; }

        [ExcelConfig(Description = "Define min IV for use greate ball instead of poke ball", Position = 29)]
        [DefaultValue(85.0)]
        [Range(0, 100)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 29)]
        public double UseGreatBallAboveIv{ get; set; }

        [ExcelConfig(Description = "Define min CP for use ultra ball instead of greate ball", Position = 30)]
        [DefaultValue(95.0)]
        [Range(0, 100)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 30)]
        public double UseUltraBallAboveIv{ get; set; }

        [ExcelConfig(Description = "Define min catch probability for use greate ball instead of pokemon ball", Position = 31)]
        [DefaultValue(0.2)]
        [Range(0, 1)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 31)]
        public double UseGreatBallBelowCatchProbability{ get; set; }

        [ExcelConfig(Description = "Define min catch probability for use ultra ball instead of greate ball", Position = 32)]
        [DefaultValue(0.1)]
        [Range(0, 1)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 32)]
        public double UseUltraBallBelowCatchProbability{ get; set; }

        [ExcelConfig(Description = "Define min catch probability for use master ball instead of ultra ball", Position = 33)]
        [DefaultValue(0.05)]
        [Range(0, 999)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 33)]
        public double UseMasterBallBelowCatchProbability{ get; set; }

        /*PoweUp*/
        [ExcelConfig(Description = "Allow bot power up pokemon ", Position = 34)]
        [DefaultValue(false)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 34)]
        public bool AutomaticallyLevelUpPokemon;

        [ExcelConfig(Description = "Only allow bot upgrade favorited pokemon", Position = 35)]
        [DefaultValue(true)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 35)]
        public bool OnlyUpgradeFavorites { get; set; }

        [ExcelConfig(Description = "Use level up list pokemon", Position = 36)]
        [DefaultValue(true)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 36)]
        public bool UseLevelUpList { get; set; }

        [ExcelConfig(Description = "Number of time upgrade 1 pokemon", Position = 37)]
        [DefaultValue(5)]
        [Range(0, 99)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 37)]
        public int AmountOfTimesToUpgradeLoop{ get; set; }

        [ExcelConfig(Description = "Min startdust keep for auto power up", Position = 38)]
        [DefaultValue(5000)]
        [Range(0, 999999)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 38)]
        public int GetMinStarDustForLevelUp{ get; set; }

        [ExcelConfig(Description = "Select pokemon to powerup by IV or CP", Position = 39)]
        [DefaultValue("iv")]
        [EnumDataType(typeof(CpIv))]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 39)]
        public string LevelUpByCPorIv{ get; set; }

        [ExcelConfig(Description = "MIn CP for pokemon upgrade", Position = 40)]
        [DefaultValue(1000)]
        [Range(0, 9999)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 40)]
        public float UpgradePokemonCpMinimum{ get; set; }

        [ExcelConfig(Description = "MIn IV for pokemon upgrade", Position = 41)]
        [DefaultValue(95)]
        [Range(0, 100)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 41)]
        public float UpgradePokemonIvMinimum{ get; set; }

        [ExcelConfig(Description = "Logic operator for select pokemon for upgrade", Position = 42)]
        [DefaultValue("and")]
        [EnumDataType(typeof(Operator))]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 42)]
        public string UpgradePokemonMinimumStatsOperator{ get; set; }

        /*Evolve*/
        [ExcelConfig(Description = "Specify min IV for evolve pokemon", Position = 43)]
        [DefaultValue(95)]
        [Range(0, 100)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 43)]
        public float EvolveAboveIvValue{ get; set; }

        [ExcelConfig(Description = "Allow bot evolve all pokemon above this IV", Position = 44)]
        [DefaultValue(false)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 44)]
        public bool EvolveAllPokemonAboveIv;

        [ExcelConfig(Description = "When turn on, bot will evolve pokemon when has enought candy", Position = 45)]
        [DefaultValue(true)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 45)]
        public bool EvolveAllPokemonWithEnoughCandy { get; set; }

        [ExcelConfig(Description = "Specify the max storage pokemon bag for trigger evolve", Position = 46)]
        [DefaultValue(90)]
        [Range(0, 100)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 46)]
        public double EvolveKeptPokemonsAtStorageUsagePercentage{ get; set; }

        [ExcelConfig(Description = "Specify the pokemon to keep for mass evolve", Position = 47)]
        [DefaultValue(120)]
        [Range(0, 999)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 47)]
        public int EvolveKeptPokemonIfBagHasOverThisManyPokemon = 120;
        
        /*Keep*/
        [ExcelConfig(Description = "Allow bot keep low candy pokemon for evolve", Position = 47)]
        [DefaultValue(false)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 48)]
        public bool KeepPokemonsThatCanEvolve;

        [ExcelConfig(Description = "Specify min CP to not transfer pokemon", Position = 48)]
        [DefaultValue(1250)]
        [Range(0, 9999)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 49)]
        public int KeepMinCp{ get; set; }

        [ExcelConfig(Description = "Specify min IV to not transfer pokemon", Position = 49)]
        [DefaultValue(90)]
        [Range(0, 100)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 50)]
        public float KeepMinIvPercentage{ get; set; }

        [ExcelConfig(Description = "Specify min LV to not transfer pokemon", Position = 50)]
        [DefaultValue(6)]
        [Range(0, 100)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 51)]
        public int KeepMinLvl{ get; set; }

        [ExcelConfig(Description = "Logic operator for keep pokemon check", Position = 51)]
        [DefaultValue("or")]
        [EnumDataType(typeof(Operator))]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 52)]
        public string KeepMinOperator{ get; set; }

        [ExcelConfig(Description = "Tell bot to check level before transfer", Position = 52)]
        [DefaultValue(false)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 53)]
        public bool UseKeepMinLvl;

        [ExcelConfig(Description = "Keep pokemon has higher IV then CP to not transfer pokemon", Position = 53)]
        [DefaultValue(true)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 53)]
        public bool PrioritizeIvOverCp { get; set; }

        [ExcelConfig(Description = "Number of duplicated pokemon to keep", Position = 54)]
        [DefaultValue(1)]
        [Range(0, 999)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 54)]
        public int KeepMinDuplicatePokemon{ get; set; }

        /*NotCatch*/
        [ExcelConfig(Description = "Use list pokemon not catch filter", Position = 55)]
        [DefaultValue(true)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 55)]
        public bool UsePokemonToNotCatchFilter { get; set; }

        [ExcelConfig(Description = "UsePokemonSniperFilterOnly", Position = 56)]
        [DefaultValue(false)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 56)]
        public bool UsePokemonSniperFilterOnly{ get; set; }

        /*Dump Stats*/
        [ExcelConfig(Description = "Allow bot dump list pokemon to csv file", Position = 57)]
        [DefaultValue(false)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 57)]
        public bool DumpPokemonStats;

        [DefaultValue(10000)]
        [ExcelConfig(Description = "Delay time between pokemon upgrade", Position = 58)]
        [Range(0, 99999)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 58)]
        public int DelayBetweenPokemonUpgrade{ get; set; }

        [DefaultValue(5)]
        [ExcelConfig(Description = "Temporary disable catch pokemon for certain minutes if bot run out of balls", Position = 59)]
        [Range(0, 120)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 59)]
        public int OutOfBallCatchBlockTime{ get; set; }

        [DefaultValue(50)]
        [ExcelConfig(Description = "Number of balls you want to save for snipe or manual play - it mean if total ball less than this value, catch pokemon will be deactive", Position = 60)]
        [Range(0, 999)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 60)]
        public int PokeballToKeepForSnipe{ get; set; }

        [DefaultValue(true)]
        [ExcelConfig(Description = "Transfer multiple pokemon at 1 time - that will increase bot speed and reduce api call", Position = 61)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 61)]
        public bool UseBulkTransferPokemon { get;  set; }

        [DefaultValue(10)]
        [ExcelConfig(Description = "Bot will transfer pokemons only when MaxStogare < pokemon + buffer", Position = 62)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 62)]
        public int BulkTransferStogareBuffer { get;  set; }

        [DefaultValue(100)]
        [ExcelConfig(Description = "Maximun number of pokemon in 1 transfer", Position = 63)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 63)]
        [Range(1,100)]
        public int BulkTransferSize { get;  set; }
    }
}
