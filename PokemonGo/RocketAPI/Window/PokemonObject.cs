using AllEnum;
using PokemonGo.RocketAPI.GeneratedCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokemonGo.RocketAPI.Window {
    public class PokemonObject {

        private static bool initialized = false;
        public static Dictionary<PokemonId, int> candyToEvolveDict = new Dictionary<PokemonId, int>(); 

        private readonly PokemonData pokemonData_;

        private int candy_ = 0;

        public static async void Initilize(Client c) {
            if (!initialized) {
                var itemtemplates = await c.GetItemTemplates();
                foreach (var t in itemtemplates.ItemTemplates) {
                    if (t != null) {
                        if (t.PokemonSettings != null) {
                            PokemonObject.candyToEvolveDict.Add(t.PokemonSettings.PokemonId, t.PokemonSettings.CandyToEvolve);
                        }
                    }
                }
                initialized = true;
            }
        }

        public PokemonObject(PokemonData pokemonData) {
            pokemonData_ = pokemonData;
        }

        public PokemonData PokemonData {
            get { return pokemonData_; }
        }

        public ulong Id {
            get { return pokemonData_.Id; }
        }

        public PokemonId PokemonId {
            get { return pokemonData_.PokemonId; }
        }

        public int Cp {
            get { return pokemonData_.Cp; }
        }

        public int IndividualAttack {
            get { return pokemonData_.IndividualAttack; }
        }

        public int IndividualDefense {
            get { return pokemonData_.IndividualDefense; }
        }

        public int IndividualStamina {
            get { return pokemonData_.IndividualStamina; }
        }

        public float GetIV {
            get { return pokemonData_.GetIV(); }
        }

        public int Candy {
            get { return candy_; }
            set { candy_ = value; }
        }

        public int CandyToEvolve {
            get {
                if (candyToEvolveDict.ContainsKey(pokemonData_.PokemonId)) {
                    return candyToEvolveDict[pokemonData_.PokemonId];
                }
                return 0;
            }
        }

        public int EvolveTimes {
            get {
                if (CandyToEvolve > 0) {
                    return Candy / CandyToEvolve;
                }
                return 0;
            }
        }

        public bool CanEvolve {
            get { return EvolveTimes > 0; }
        }
    }
}
