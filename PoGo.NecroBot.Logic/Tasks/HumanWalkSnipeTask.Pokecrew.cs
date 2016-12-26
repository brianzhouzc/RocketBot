using Newtonsoft.Json;
using PoGo.NecroBot.Logic.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace PoGo.NecroBot.Logic.Tasks
{
    public partial class HumanWalkSnipeTask
    {
        public class PokecrewWrap
        {
            public class PokecrewItem
            {
                public double latitude { get; set; }
                public double longitude { get; set; }
                public int pokemon_id { get; set; }
                public DateTime expires_at { get; set; }
            }
            public List<PokecrewItem> seens { get; set; }
        }

        private static SnipePokemonInfo Map(PokecrewWrap.PokecrewItem result)
        {
            return new SnipePokemonInfo()
            {
                Latitude = result.latitude,
                Longitude = result.longitude,
                Id = result.pokemon_id,
                ExpiredTime = result.expires_at.ToLocalTime(),
                Source = "Pokecrew"
            };
        }
         private static async Task<List<SnipePokemonInfo>> FetchFromPokecrew(double lat, double lng)
        {
            List<SnipePokemonInfo> results = new List<SnipePokemonInfo>();
            if (!_setting.HumanWalkingSnipeUsePokecrew) return results;

            //var startFetchTime = DateTime.Now;

            try
            {
                HttpClient client = new HttpClient();
                double offset = _setting.HumanWalkingSnipeSnipingScanOffset; //0.015 
                string url = $"https://api.pokecrew.com/api/v1/seens?center_latitude={lat}&center_longitude={lng}&live=true&minimal=false&northeast_latitude={lat + offset}&northeast_longitude={lng + offset}&pokemon_id=&southwest_latitude={lat - offset}&southwest_longitude={lng - offset}";

                var task = await client.GetStringAsync(url);

                var data = JsonConvert.DeserializeObject<PokecrewWrap>(task);
                foreach (var item in data.seens)
                {
                    var pItem = Map(item);
                    if (pItem != null)
                    {
                        results.Add(pItem);
                    }
                }
            }
            catch (Exception)
            {
                Logger.Write("Error loading data from Pokecrew", LogLevel.Error, ConsoleColor.DarkRed);
            }

            //var endFetchTime = DateTime.Now;
            //Logger.Write($"FetchFromPokecrew spent {(endFetchTime - startFetchTime).TotalSeconds} seconds", LogLevel.Info, ConsoleColor.White);
            return results;
        }

    }
}
