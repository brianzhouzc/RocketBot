#region using directives

#region using directives

using System;
using System.Linq;
using System.Threading.Tasks;
using Google.Protobuf.Collections;
using PoGo.NecroBot.Logic.Exceptions;
using PoGo.NecroBot.Logic.Logging;
using PoGo.NecroBot.Logic.Model.Settings;
using PoGo.NecroBot.Logic.State;
using POGOProtos.Inventory.Item;
using POGOProtos.Networking.Responses;

#endregion

// ReSharper disable CyclomaticComplexity

#endregion

namespace PoGo.NecroBot.Logic.Utils
{
    public delegate void StatisticsDirtyDelegate();

    public class Statistics
    {
        private DateTime _initSessionDateTime = DateTime.Now;

        private StatsExport _exportStats;
        private string _playerName;
        public int TotalExperience;
        public int TotalItemsRemoved;
        public int TotalPokemons = 0;
        public int TotalPokemonEvolved = 0;
        public int TotalPokestops = 0;
        public int TotalPokemonTransferred;
        public int TotalStardust;
        public int LevelForRewards = -1;

        public StatsExport StatsExport => _exportStats;

        public void Dirty(Inventory inventory, ISession session)
        {
            _exportStats = GetCurrentInfo(inventory);
            DirtyEvent?.Invoke();
            OnStatisticChanged(session);
        }

        public void OnStatisticChanged(ISession session)
        {
            if (MultipleBotConfig.IsMultiBotActive(session.LogicSettings))
            {
                var config = session.LogicSettings.MultipleBotConfig;

                if (config.PokestopSwitch > 0 && config.PokestopSwitch <= this.TotalPokestops)
                {
                    session.CancellationTokenSource.Cancel();

                    //Activate switcher by pokestop
                    throw new ActiveSwitchByRuleException()
                    {
                        MatchedRule = SwitchRules.Pokestop,
                        ReachedValue = this.TotalPokestops
                    };
                }

                if (config.PokemonSwitch > 0 && config.PokemonSwitch <= this.TotalPokemons)
                {
                    session.CancellationTokenSource.Cancel();
                    //Activate switcher by pokestop
                    throw new ActiveSwitchByRuleException()
                    {
                        MatchedRule = SwitchRules.Pokemon,
                        ReachedValue = this.TotalPokemons
                    };
                }

                if (config.EXPSwitch > 0 && config.EXPSwitch <= this.TotalExperience)
                {
                    session.CancellationTokenSource.Cancel();
                    //Activate switcher by pokestop
                    throw new ActiveSwitchByRuleException()
                    {
                        MatchedRule = SwitchRules.EXP,
                        ReachedValue = this.TotalExperience
                    };
                }

                var totalMin = (DateTime.Now - _initSessionDateTime).TotalMinutes;
                if (config.RuntimeSwitch > 0 && config.RuntimeSwitch <= totalMin)
                {
                    session.CancellationTokenSource.Cancel();
                    //Activate switcher by pokestop
                    throw new ActiveSwitchByRuleException()
                    {
                        MatchedRule = SwitchRules.Runtime,
                        ReachedValue = Math.Round(totalMin, 1)
                    };
                }
            }
        }

        public event StatisticsDirtyDelegate DirtyEvent;

        public string FormatRuntime()
        {
            return (DateTime.Now - _initSessionDateTime).ToString(@"dd\.hh\:mm\:ss");
        }

        public StatsExport GetCurrentInfo(Inventory inventory)
        {
            var stats = inventory.GetPlayerStats().Result;
            StatsExport output = null;
            var stat = stats.FirstOrDefault();
            if (stat != null)
            {
                var ep = stat.NextLevelXp - stat.PrevLevelXp - (stat.Experience - stat.PrevLevelXp);
                var time = Math.Round(ep / (TotalExperience / GetRuntime()), 2);
                var hours = 0.00;
                var minutes = 0.00;
                if (double.IsInfinity(time) == false && time > 0)
                {
                    hours = Math.Truncate(TimeSpan.FromHours(time).TotalHours);
                    minutes = TimeSpan.FromHours(time).Minutes;
                }

                if (LevelForRewards == -1 || stat.Level >= LevelForRewards)
                {
                    LevelUpRewardsResponse Result = Execute(inventory).Result;

                    if (Result.ToString().ToLower().Contains("awarded_already"))
                        LevelForRewards = stat.Level + 1;

                    if (Result.ToString().ToLower().Contains("success"))
                    {
                        Logger.Write("Leveled up: " + stat.Level, LogLevel.Info);

                        RepeatedField<ItemAward> items = Result.ItemsAwarded;

                        if (items.Any<ItemAward>())
                        {
                            Logger.Write("- Received Items -", LogLevel.Info);
                            foreach (ItemAward item in items)
                            {
                                Logger.Write($"[ITEM] {item.ItemId} x {item.ItemCount} ", LogLevel.Info);
                            }
                        }
                    }
                }
                var Result2 = Execute(inventory).Result;
                LevelForRewards = stat.Level;
                if (Result2.ToString().ToLower().Contains("success"))
                {
                    string[] tokens = Result2.Result.ToString().Split(new[] {"itemId"}, StringSplitOptions.None);
                    Logger.Write("Items Awarded:" + Result2.ItemsAwarded.ToString());
                }
                output = new StatsExport
                {
                    Level = stat.Level,
                    HoursUntilLvl = hours,
                    MinutesUntilLevel = minutes,
                    CurrentXp = stat.Experience - stat.PrevLevelXp - GetXpDiff(stat.Level),
                    LevelupXp = stat.NextLevelXp - stat.PrevLevelXp - GetXpDiff(stat.Level)
                };
            }
            return output;
        }

        internal void Reset()
        {
            this.TotalExperience = 0;
            this.TotalItemsRemoved = 0;
            this.TotalPokemons = 0;
            this.TotalPokemonEvolved = 0;
            this.TotalPokestops = 0;
            this.TotalStardust = 0;
            this.TotalPokemonTransferred = 0;
            this._initSessionDateTime = DateTime.Now;
        }

        public async Task<LevelUpRewardsResponse> Execute(ISession ctx)
        {
            var Result = await ctx.Inventory.GetLevelUpRewards(LevelForRewards);
            return Result;
        }

        public async Task<LevelUpRewardsResponse> Execute(Inventory inventory)
        {
            var Result = await inventory.GetLevelUpRewards(inventory);
            return Result;
        }

        public double GetRuntime()
        {
            return (DateTime.Now - _initSessionDateTime).TotalSeconds / 3600;
        }

        public string GetTemplatedStats(string template, string xpTemplate)
        {
            var xpStats = string.Format(xpTemplate, _exportStats.Level, _exportStats.HoursUntilLvl,
                _exportStats.MinutesUntilLevel, _exportStats.CurrentXp, _exportStats.LevelupXp);

            return string.Format(template, _playerName, FormatRuntime(), xpStats, TotalExperience / GetRuntime(),
                TotalPokestops / GetRuntime(),
                TotalStardust, TotalPokemonTransferred, TotalItemsRemoved);
        }

        public static int GetXpDiff(int level)
        {
            if (level > 0 && level <= 40)
            {
                int[] xpTable =
                {
                    0, 1000, 2000, 3000, 4000, 5000, 6000, 7000, 8000, 9000,
                    10000, 10000, 10000, 10000, 15000, 20000, 20000, 20000, 25000, 25000,
                    50000, 75000, 100000, 125000, 150000, 190000, 200000, 250000, 300000, 350000,
                    500000, 500000, 750000, 1000000, 1250000, 1500000, 2000000, 2500000, 3000000, 5000000
                };
                return xpTable[level - 1];
            }
            return 0;
        }

        public void SetUsername(GetPlayerResponse profile)
        {
            _playerName = profile.PlayerData.Username ?? "";
        }
    }

    public class StatsExport
    {
        public long CurrentXp;
        public double HoursUntilLvl;
        public int Level;
        public long LevelupXp;
        public double MinutesUntilLevel;
    }
}