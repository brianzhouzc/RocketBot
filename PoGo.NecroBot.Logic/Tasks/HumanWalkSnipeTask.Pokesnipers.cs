using Newtonsoft.Json;
using PoGo.NecroBot.Logic.Logging;
using POGOProtos.Enums;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace PoGo.NecroBot.Logic.Tasks
{
    public partial class HumanWalkSnipeTask
    {
        public class PokesniperWrap
        {
            public class PokesniperItem
            {
                public string coords { get; set; }
                public string name { get; set; }
                public DateTime until { get; set; }
            }
            public List<PokesniperItem> results { get; set; }
        }

        private static SnipePokemonInfo Map(PokesniperWrap.PokesniperItem result)
        {
            var arr = result.coords.Split(',');
            return new SnipePokemonInfo()
            {
                Latitude = Convert.ToDouble(arr[0]),
                Longitude = Convert.ToDouble(arr[1]),
                Id = (int)Enum.Parse(typeof(PokemonId), result.name),
                ExpiredTime = result.until.ToLocalTime()   ,
                Source = "Pokesnipers"
            };
        }

        private static async Task<List<SnipePokemonInfo>> FetchFromPokesnipers(double lat, double lng)
        {
            List<SnipePokemonInfo> results = new List<SnipePokemonInfo>();
            if (!_setting.HumanWalkingSnipeUsePokesnipers) return results;

            //var startFetchTime = DateTime.Now;

            try
            {
                HttpClient client = new HttpClient();
                string url = $"http://pokesnipers.com/api/v1/pokemon.json";

                var task = await client.GetStringAsync(url);

                var data = JsonConvert.DeserializeObject<PokesniperWrap>(task);
                foreach (var item in data.results)
                {
                    try
                    {
                        var pItem = Map(item);
                        if (pItem != null)
                        {
                            results.Add(pItem);
                        }
                    }
                    catch (Exception)
                    {
                        //ignore if any data failed.
                    }
                }
            }
            catch (Exception)
            {
                Logger.Write("Error loading data from pokesnipers", LogLevel.Error, ConsoleColor.DarkRed);
            }

            //var endFetchTime = DateTime.Now;
            //Logger.Write($"FetchFromPokesnipers spent {(endFetchTime - startFetchTime).TotalSeconds} seconds", LogLevel.Info, ConsoleColor.White);
            return results;
        }
    }
}
