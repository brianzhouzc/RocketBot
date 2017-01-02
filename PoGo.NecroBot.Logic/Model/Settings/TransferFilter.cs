using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using POGOProtos.Enums;

namespace PoGo.NecroBot.Logic.Model.Settings
{
    public enum Operator
    {
        or,
        and
    }

    [JsonObject(Description = "", ItemRequired = Required.DisallowNull)] //Dont set Title
    public class TransferFilter 
    {
        public TransferFilter()
        {
            MovesOperator = "and";
            Moves = new List<List<PokemonMove>>();
            DeprecatedMoves = new List<PokemonMove>();
            
        }

        public TransferFilter(int keepMinCp, int keepMinLvl, bool useKeepMinLvl, float keepMinIvPercentage, string keepMinOperator, int keepMinDuplicatePokemon,
            List<List<PokemonMove>> moves = null, List<PokemonMove> deprecatedMoves = null, string movesOperator = "or", bool catchOnlyPokemonMeetTransferCriteria = false)
        {
            DoNotTransfer = false;
            AllowTransfer = true;
            KeepMinCp = keepMinCp;
            KeepMinLvl = keepMinLvl;
            UseKeepMinLvl = useKeepMinLvl;
            KeepMinIvPercentage = keepMinIvPercentage;
            KeepMinDuplicatePokemon = keepMinDuplicatePokemon;
            KeepMinOperator = keepMinOperator;
            Moves = (moves == null && deprecatedMoves != null)
                ? new List<List<PokemonMove>> { deprecatedMoves }
                : moves ?? new List<List<PokemonMove>>();
            MovesOperator = movesOperator;
            CatchOnlyPokemonMeetTransferCriteria = catchOnlyPokemonMeetTransferCriteria;
        }
        
        [JsonIgnore]
        [ExcelConfig(IsPrimaryKey =true, Key = "Allow Transfer", Position = 1, Description = "If TRUE bot will transfer this type of pokemon when match with filter condition.")]
        public bool AllowTransfer { get; set; }

        [JsonIgnore]
        [ExcelConfig(Key = "Do Not Transfer", Position = 2, Description = "TRUE, Bot won't transfer this pokemon, FALSE , Bot will use other parameter to check.")]
        public bool DoNotTransfer { get; set; }

        [ExcelConfig(Key = "KeepMinCp", Position =3 , Description = "Pokemon with CP lower than this value will be transfered")]
        [DefaultValue(1250)]
        [Range(0, 9999)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 1)]
        public int KeepMinCp { get; set; }

        [ExcelConfig (Key = "KeepMinIvPercentage", Position =4, Description = "Pokemon with IV lower than this value will be transfered")]
        [DefaultValue(90)]
        [Range(0, 100)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 2)]
        public float KeepMinIvPercentage { get; set; }

        [ExcelConfig(Key = "KeepMinLvl", Position = 5, Description = "Pokemon with LV lower than this value will be transfered")]
        [DefaultValue(6)]
        [Range(0, 999)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 3)]
        public int KeepMinLvl { get; set; }

        [ExcelConfig(Key = "UseKeepMinLvl", Position = 6, Description = "Use Min Level for transfer")]
        [DefaultValue(false)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 4)]
        public bool UseKeepMinLvl { get; set; }

        [ExcelConfig(Key = "KeepMinOperator", Position = 7, Description ="The operator logic use to check for transfer ")]
        [DefaultValue("or")]
        [EnumDataType(typeof(Operator))]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 5)]
        public string KeepMinOperator { get; set; }

        [ExcelConfig(Key = "KeepMinDuplicatePokemon", Position = 8 , Description = "Number of duplication pokemon to keep")]
        [DefaultValue(1)]
        [Range(0, 999)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 6)]
        public int KeepMinDuplicatePokemon { get; set; }

        [ExcelConfig(Key = "Moves", Position = 9, Description = "Defined unwanted moves , pokemon has this move will be transfered")]
        [DefaultValue(null)]
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Populate, Order = 7)]
        public List<List<PokemonMove>> Moves { get; set; }

        [DefaultValue(null)]
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Populate, Order = 8)]
        public List<PokemonMove> DeprecatedMoves { get; set; }

        [ExcelConfig(Key = "MovesOperator", Position = 10)]
        [DefaultValue("and")]
        [EnumDataType(typeof(Operator))]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 9)]
        public string MovesOperator { get; set; }

        [ExcelConfig(Key = "CatchOnlyPokemonMeetTransferCriteria", Position = 11, Description ="Turn on this option to set bot catch only good pokemon with not meet transfer condition.")]
        [DefaultValue(false)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 10)]
        public bool CatchOnlyPokemonMeetTransferCriteria { get; set; }


        internal static Dictionary<PokemonId, TransferFilter> TransferFilterDefault()
        {
            return new Dictionary<PokemonId, TransferFilter>
            {
				//criteria: based on NY Central Park and Tokyo variety + sniping optimization
				{PokemonId.Golduck, new TransferFilter(1800, 6, false, 95, "or", 1,new List<List<PokemonMove>>() { new List<PokemonMove>() { PokemonMove.WaterGunFast,PokemonMove.HydroPump }},null,"and")},
                {PokemonId.Aerodactyl, new TransferFilter(1250, 6, false, 80, "or", 1,new List<List<PokemonMove>>() { new List<PokemonMove>() { PokemonMove.BiteFast,PokemonMove.HyperBeam }},null,"and")},
                {PokemonId.Venusaur, new TransferFilter(1800, 6, false, 95, "or", 1,new List<List<PokemonMove>>() { new List<PokemonMove>() { PokemonMove.VineWhipFast,PokemonMove.SolarBeam }},null,"and")},
                {PokemonId.Farfetchd, new TransferFilter(1250, 6, false, 80, "or", 1)},
                {PokemonId.Krabby, new TransferFilter(1250, 6, false, 95, "or", 1)},
                {PokemonId.Kangaskhan, new TransferFilter(1500, 6, false, 60, "or", 1)},
                {PokemonId.Horsea, new TransferFilter(1250, 6, false, 95, "or", 1)},
                {PokemonId.Staryu, new TransferFilter(1250, 6, false, 95, "or", 1)},
                {PokemonId.MrMime, new TransferFilter(1250, 6, false, 40, "or", 1)},
                {PokemonId.Scyther, new TransferFilter(1800, 6, false, 80, "or", 1)},
                {PokemonId.Jynx, new TransferFilter(1250, 6, false, 95, "or", 1)},
                {PokemonId.Charizard, new TransferFilter(1250, 6, false, 80, "or", 1,new List<List<PokemonMove>>() { new List<PokemonMove>() { PokemonMove.WingAttackFast,PokemonMove.FireBlast }},null,"and")},
                {PokemonId.Electabuzz, new TransferFilter(1250, 6, false, 80, "or", 1,new List<List<PokemonMove>>() { new List<PokemonMove>() { PokemonMove.ThunderShockFast,PokemonMove.Thunder }},null,"and")},
                {PokemonId.Magmar, new TransferFilter(1500, 6, false, 80, "or", 1)},
                {PokemonId.Pinsir, new TransferFilter(1800, 6, false, 95, "or", 1,new List<List<PokemonMove>>() { new List<PokemonMove>() { PokemonMove.RockSmashFast,PokemonMove.XScissor }},null,"and")},
                {PokemonId.Tauros, new TransferFilter(1250, 6, false, 90, "or", 1)},
                {PokemonId.Magikarp, new TransferFilter(200, 6, false, 95, "or", 1)},
                {PokemonId.Exeggutor, new TransferFilter(1800, 6, false, 90, "or", 1,new List<List<PokemonMove>>() { new List<PokemonMove>() { PokemonMove.ZenHeadbuttFast,PokemonMove.SolarBeam }},null,"and")},
                {PokemonId.Gyarados, new TransferFilter(1250, 6, false, 90, "or", 1,new List<List<PokemonMove>>() { new List<PokemonMove>() { PokemonMove.DragonBreath,PokemonMove.HydroPump }},null,"and")},
                {PokemonId.Lapras, new TransferFilter(1800, 6, false, 80, "or", 1,new List<List<PokemonMove>>() { new List<PokemonMove>() { PokemonMove.FrostBreathFast,PokemonMove.Blizzard }},null,"and")},
                {PokemonId.Eevee, new TransferFilter(1250, 6, false, 95, "or", 1)},
                {PokemonId.Vaporeon, new TransferFilter(1500, 6, false, 90, "or", 1,new List<List<PokemonMove>>() { new List<PokemonMove>() { PokemonMove.WaterGun,PokemonMove.HydroPump }},null,"and")},
                {PokemonId.Jolteon, new TransferFilter(1500, 6, false, 90, "or", 1)},
                {PokemonId.Flareon, new TransferFilter(1500, 6, false, 90, "or", 1,new List<List<PokemonMove>>() { new List<PokemonMove>() { PokemonMove.Ember,PokemonMove.FireBlast }},null,"and")},
                {PokemonId.Porygon, new TransferFilter(1250, 6, false, 60, "or", 1)},
                {PokemonId.Arcanine, new TransferFilter(1800, 6, false, 80, "or", 1,new List<List<PokemonMove>>() { new List<PokemonMove>() { PokemonMove.FireFangFast,PokemonMove.FireBlast }},null,"and")},
                {PokemonId.Snorlax, new TransferFilter(2600, 6, false, 90, "or", 1,new List<List<PokemonMove>>() { new List<PokemonMove>() { PokemonMove.ZenHeadbuttFast,PokemonMove.HyperBeam }},null,"and")},
                {PokemonId.Dragonite, new TransferFilter(2600, 6, false, 90, "or", 1,new List<List<PokemonMove>>() { new List<PokemonMove>() { PokemonMove.DragonBreath,PokemonMove.DragonClaw }},null,"and")},
            };
        }
    }
}