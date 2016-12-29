using Newtonsoft.Json;
using PoGo.NecroBot.Logic.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace PoGo.NecroBot.Logic.Tasks
{
    public class PokeWatcherItem
    {
        public double timeend { get; set; }
        public string cords { get; set; }
        public int pid { get; set; }
        public string pokemon { get; set; }
    }
   
    //need refactor this class, move list snipping pokemon to session and split function out to smaller class.
    public partial class HumanWalkSnipeTask
    {
        private static async Task<List<SnipePokemonInfo>> FetchFromPokeWatcher(double lat, double lng)
        {
            List<SnipePokemonInfo> results = new List<SnipePokemonInfo>();
            if (!_setting.HumanWalkingSnipeUsePokeWatcher) return results;

            //var startFetchTime = DateTime.Now;

            string url = $"https://pokewatchers.com/grab/";

            try
            {
                HttpClient client = new HttpClient();

                client.DefaultRequestHeaders.Accept.TryParseAdd("text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
                client.DefaultRequestHeaders.AcceptEncoding.ParseAdd("gzip, deflate, sdch, br");
                client.DefaultRequestHeaders.Host = "pokewatchers.com";
                client.DefaultRequestHeaders.UserAgent.TryParseAdd("Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/52.0.2743.116 Safari/537.36");

                var json = await client.GetStringAsync(url);

                var list = JsonConvert.DeserializeObject<List<PokeWatcherItem>>(json);
                results = list.Select(p => Map(p)).ToList();
            }
            catch (Exception)
            {
                Logger.Write("Error loading data from PokeWatcher", LogLevel.Error, ConsoleColor.DarkRed);
            }

            //var endFetchTime = DateTime.Now;
            //Logger.Write($"FetchFromPokeWatcher spent {(endFetchTime - startFetchTime).TotalSeconds} seconds", LogLevel.Info, ConsoleColor.White);
            return results;
        }

        public static async Task AddFastPokemapItem(dynamic data)
        {
            var list = JsonConvert.DeserializeObject<List<FastPokemapItem>>(data.ToString());

            List<SnipePokemonInfo> result = new List<SnipePokemonInfo>();
            foreach (var item in list)
            {
                var snipeItem = Map(item);
                if(snipeItem != null)
                {
                    result.Add(snipeItem);
                }
            }
            await PostProcessDataFetched(result, false);
            
        }

        private static SnipePokemonInfo Map(PokeWatcherItem result)
        {
            string[] arr = result.cords.Split(',');
            return new SnipePokemonInfo()
            {
                Latitude = Convert.ToDouble(arr[0]),
                Longitude = Convert.ToDouble(arr[1]),
                Id = result.pid,
                ExpiredTime = UnixTimeStampToDateTime(result.timeend),
                Source = "Pokewatchers"
            };
        }
    }
}
