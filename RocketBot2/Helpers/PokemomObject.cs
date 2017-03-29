using PoGo.NecroBot.Logic.PoGoUtils;
using PoGo.NecroBot.Logic.State;
using POGOProtos.Data;
using POGOProtos.Enums;
using POGOProtos.Inventory.Item;
using POGOProtos.Settings.Master;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace RocketBot2.Helpers
{
    public class EvolutionToPokemon
    {
        public int CandyNeed { get; set; }
        public ulong OriginPokemonId { get; set; }
        public PokemonId Pokemon { get; set; }
        public bool AllowEvolve { get; set; }
        public ItemId ItemNeed { get; set; }
    }

    public class PokemonEvoleTo 
    {
        private static ISession Session;
        private PokemonSettings setting;
        public PokemonData PokemonData { get; set; }
        public bool Displayed { get; set; }
        public List<EvolutionToPokemon> EvolutionBranchs { get; set; }
        public PokemonEvoleTo(ISession session, PokemonData pokemon)
        {
            Session = session;
            PokemonData = pokemon;
            this.Displayed = true;
            var pkmSettings = session.Inventory.GetPokemonSettings().Result;
            setting = pkmSettings.FirstOrDefault(x => x.PokemonId == pokemon.PokemonId);

            this.EvolutionBranchs = new List<EvolutionToPokemon>();

            //TODO - implement the candy count for enable evolution
            foreach (var item in setting.EvolutionBranch)
            {
                this.EvolutionBranchs.Add(new EvolutionToPokemon()
                {
                    CandyNeed = item.CandyCost,
                    ItemNeed = item.EvolutionItemRequirement,
                    Pokemon = item.Evolution,
                    AllowEvolve = session.Inventory.CanEvolvePokemon(pokemon).Result,
                    OriginPokemonId = pokemon.Id
                });
            }
        }
    }

    public class PokemonObject
    {
        private static ISession Session;
        public PokemonData PokemonData { get; }
        public static bool _initialized { get; set; }
        public static Dictionary<PokemonId, int> CandyToEvolveDict = new Dictionary<PokemonId, int>();

        public PokemonObject(ISession session, PokemonData pokemonData)
        {
            Session = session;
            this.PokemonData = pokemonData;
        }

        public ulong Id
        {           
            get { return this.PokemonData.Id; }
        }

        public PokemonId PokemonId
        {
            get { return this.PokemonData.PokemonId; }
        }

        public int Cp
        {
            get { return this.PokemonData.Cp; }
        }

        public int IndividualAttack
        {
            get { return this.PokemonData.IndividualAttack; }
        }

        public int IndividualDefense
        {
            get { return this.PokemonData.IndividualDefense; }
        }

        public int IndividualStamina
        {
            get { return this.PokemonData.IndividualStamina; }
        }

        public double GetIV
        {
            get { return PokemonInfo.CalculatePokemonPerfection(this.PokemonData)/100; }
        }

        public double GetLv
        {
            get { return PokemonInfo.GetLevel(this.PokemonData); }
        }

        public string Nickname
        {
            get { return this.PokemonData.Nickname; }
        }

        public string Move1
        {
            get { return Session.Translation.GetPokemonMovesetTranslation(this.PokemonData.Move1); }
        }

        public string Move2
        {
            get { return Session.Translation.GetPokemonMovesetTranslation(this.PokemonData.Move2); }
        }

        public int Candy
        {
            get
            {
                return Session.Inventory.GetCandyCount(this.PokemonData.PokemonId).Result;
            }
        }

        public int CandyToEvolve
        {
            get
            {
                if (CandyToEvolveDict.ContainsKey(this.PokemonData.PokemonId))
                {
                    return CandyToEvolveDict[this.PokemonData.PokemonId];
                }
                return 0;
            }
        }

        public int EvolveTimes
        {
            get
            {
                if (CandyToEvolve > 0)
                {
                    return Candy/CandyToEvolve;
                }
                return 0;
            }
        }

        public bool Favorited
        {
            get
            {
                return this.PokemonData.Favorite == 1;
            }
        }

        public bool AllowPowerup
        {
            get
            {
                return Session.Inventory.CanUpgradePokemon(this.PokemonData).Result;
            }
        }

        public bool AllowEvolve
        {
            get
            {
                return Session.Inventory.CanEvolvePokemon(this.PokemonData).Result;
            }
        }

        public bool AllowTransfer
        {
            get
            {
                return Session.Inventory.CanTransferPokemon(this.PokemonData);
            }
        }

        public Image Icon
        {
            get
            {
                return ResourceHelper.GetPokemonImage((int)this.PokemonData.PokemonId);
            }
        }
               
        public static void Initilize(ISession session, List<PokemonSettings> templates)
        {
            Session = session;
            if (!_initialized)
            {
                _initialized = true;

                foreach (var t in templates)
                {
                    if (t != null)
                    {
                        CandyToEvolveDict.Add(t.PokemonId, t.CandyToEvolve);
                    }
                }
            }
        }
    }
}