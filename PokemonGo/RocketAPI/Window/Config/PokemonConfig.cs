using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Forms;

namespace PokemonGo.RocketAPI.Window.Config
{
    public class PokemonConfig
    {
        // pokemon rare list
        public static readonly List<String> Everywhere = new List<string>
    (new List<string> { "Rattata", "Pidgey", "Weedle", "Caterpie", "Drowzee" });
        public static readonly List<String> VirtuallyEverywhere = new List<string>
    (new List<string> { "Eevee", "Zubat", "Venonat", "Oddish", "Magikarp" });
        public static readonly List<String> VeryCommon = new List<string>
    (new List<string> { "Meowth", "Spearow", "Bellsprout", "Paras", "Krabby" });
        public static readonly List<String> Common = new List<string>
    (new List<string> { "Clefairy", "Nidoran", "Nidoran", "Ekans", "Cubone", "Goldeen", "Poliwag", "Shellder" });
        public static readonly List<String> Uncommon = new List<string>
    (new List<string> { "Jigglypuff", "Growlithe", "Gastly", "Geodude", "Exeggcute", "Slowpoke", "Psyduck", "Jynx", "Onyx" });
        public static readonly List<String> Rare = new List<string>
    (new List<string> { "Ponyta", "Vulpix", "Koffing", "Sandshrew", "Staryu", "Tentacool", "Horsea", "Magmar", "Tangela" });
        public static readonly List<String> VeryRare = new List<string>
    (new List<string> { "Abra", "Machop", "Grimer", "Rhyhorn", "Voltorb", "Lickitung", "Scyther", "Pinsir" });
        public static readonly List<String> Epic = new List<string>
    (new List<string> { "Dratini", "Magnemite", "Electabuzz", "Hitmonlee", "Hitmonchan", "Chansey" });
        public static readonly List<String> Mythical = new List<string>
    (new List<string> { "Lapras", "Snorlax", "Porygon" });
        public static readonly List<String> Special = new List<string>
    (new List<string> { "Bulbasaur", "Charmander", "Squirtle", "Pikachu", "Omanyte", "Kabuto", "Aerodactyl" });
        public static readonly List<String> RegionExclusive = new List<string>
    (new List<string> { "Farfetchd", "Mr. Mime", "Tauros", "Kangaskhan" });
        public static readonly List<String> NotCurrentlyAvailable = new List<string>
    (new List<string> { "Ditto", "Moltres", "Zapdos", "Articuno", "Mewtwo", "Mew" });

        // the rare level of pokemon go
        public static readonly List<string> RareLevel = new List<string>(new List<string> { "Everywhere", "Virtually Everywhere", "Very Common", "Common", "Uncommon", "Rare", "Very Rare", "Epic", "Mythical", "Special", "Region Exclusive", "Not Currently Available" });

        public static List<List<string>> PokemonRareList = new List<List<string>>();

        public PokemonConfig()
        {
            InitialisePokemonRareList();
            CachePokemonImage();
        }

        /// <summary>
        /// Initialise PokemonRareList
        /// </summary>
        private void InitialisePokemonRareList()
        {
            PokemonRareList.Add(Everywhere);
            PokemonRareList.Add(VirtuallyEverywhere);
            PokemonRareList.Add(VeryCommon);
            PokemonRareList.Add(Common);
            PokemonRareList.Add(Uncommon);
            PokemonRareList.Add(Rare);
            PokemonRareList.Add(VeryRare);
            PokemonRareList.Add(Epic);
            PokemonRareList.Add(Mythical);
            PokemonRareList.Add(Special);
            PokemonRareList.Add(RegionExclusive);
            PokemonRareList.Add(NotCurrentlyAvailable);
        }

        /// <summary>
        /// get the pokemon list by the given rare number
        /// </summary>
        /// <param name="rare"> rare ranking ranges from 0 to 11. 0 means the most common, while 11 means the rarest</param>
        /// <returns></returns>
        public List<string> GetPokemonListByRare(int rare)
        {
            if (rare < 0 || rare > 12)
                return null;

            return PokemonRareList.ElementAt(rare);

        }

        /// <summary>
        /// cache the pokemon image when the app starts, to reduce the loading time of getting pokemon list.
        /// </summary>
        private void CachePokemonImage()
        {
            var Sprites = AppDomain.CurrentDomain.BaseDirectory + "Sprites\\";

            if (!Directory.Exists(Sprites))
                Directory.CreateDirectory(Sprites);

            for(int i = 1; i <= 151; i++)
            {
                string location = Sprites + i + ".png";
                if (!File.Exists(location))
                {
                    WebClient wc = new WebClient();
                    wc.DownloadFile("http://pokeapi.co/media/sprites/pokemon/" + i + ".png", @location);
                }
            }
        }


    }
}
