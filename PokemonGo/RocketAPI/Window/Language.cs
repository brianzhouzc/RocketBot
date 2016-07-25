using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PokemonGo.RocketAPI.Window
{
    class Language
    {
        public static Dictionary<string, string> langPokemons = new Dictionary<string, string>();
        public static Dictionary<string, string> langPhrases = new Dictionary<string, string>();

        private static Dictionary<string, string> GetLanguageDictionary(string lang, string type)
        {
            XDocument doc = XDocument.Load(lang + ".xml");
            var dic = doc.Root.Elements(type)
                   .ToDictionary(c => (string)c.Attribute("key").Value,
                                 c => (string)c.Attribute("value").Value);

            return dic;
        }

        public static void LoadLanguageFile(string lang)
        {
            try
            {
                langPokemons = GetLanguageDictionary(lang, "pokemon");
                langPhrases = GetLanguageDictionary(lang, "phrase");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static Dictionary<string, string> GetPokemons()
        {
            return langPokemons;
        }

        public static Dictionary<string, string> GetPhrases()
        {
            return langPhrases;
        }
    }
}
