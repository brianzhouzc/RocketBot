#region using directives

using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using POGOProtos.Enums;

#endregion

namespace PoGo.NecroBot.Logic.Model.Settings
{
    public class UpgradeFilter
    {
        internal enum Operator
        {
            or,
            and
        }

        internal enum CPorIv
        {
            cp,
            iv
        }

        public UpgradeFilter()
        {
        }

        public UpgradeFilter(string levelUpByCPorIv, double upgradePokemonCpMinimum, double upgradePokemonIvMinimum,
            string upgradePokemonMinimumStatsOperator, bool onlyUpgradeFavorites)
        {
            LevelUpByCPorIv = levelUpByCPorIv;
            UpgradePokemonCpMinimum = upgradePokemonCpMinimum;
            UpgradePokemonIvMinimum = upgradePokemonIvMinimum;
            UpgradePokemonMinimumStatsOperator = upgradePokemonMinimumStatsOperator;
            OnlyUpgradeFavorites = onlyUpgradeFavorites;
            AllowTransfer = true;
        }

        [JsonIgnore]
        [ExcelConfig(IsPrimaryKey = true, Key = "Allow Upgrade", Position = 1, Description = "TRUE is allow custom filter for level up")]
        public bool AllowTransfer { get; set; }

        [ExcelConfig(Key = "LevelUpByCPorIv", Position = 2, Description ="Upgrade by IV or CP")]
        [DefaultValue("iv")]
        [EnumDataType(typeof(CPorIv))]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 2)]
        public string LevelUpByCPorIv { get; set; } = "iv";

        [ExcelConfig(Key = "MIN CP", Position = 3, Description = "Upgrade by IV or CP")]
        [DefaultValue(1250)]
        [Range(0, 9999)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 2)]
        public double UpgradePokemonCpMinimum { get; set; } = 1250;

        [ExcelConfig(Key = "MIN IV ", Position = 4, Description = "Define Min IV to upgrade")]
        [DefaultValue(90)]
        [Range(0, 100)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 3)]
        public double UpgradePokemonIvMinimum { get; set; } = 90;

        [ExcelConfig(Key = "Operator", Position = 5, Description = "Operator logic to check pokemon for upgrade")]
        [DefaultValue("or")]
        [EnumDataType(typeof(Operator))]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 4)]
        public string UpgradePokemonMinimumStatsOperator { get; set; } = "or";

        [ExcelConfig(Key = "Only Farovite", Position = 6, Description = "TRUE-> ONLy upgrade pokemon that you marked favorite or auto mark as favorite")]
        [DefaultValue(false)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 5)]
        public bool OnlyUpgradeFavorites { get; set; }

        internal static Dictionary<PokemonId, UpgradeFilter> Default()
        {
            return new Dictionary<PokemonId, UpgradeFilter>
            {
                {PokemonId.Dratini, new UpgradeFilter("iv", 600, 99, "or", false)}
            };
        }
    }
}