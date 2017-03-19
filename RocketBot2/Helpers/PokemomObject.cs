using System;
using System.Collections.Generic;
using PoGo.NecroBot.Logic.PoGoUtils;
using POGOProtos.Data;
using POGOProtos.Enums;
using POGOProtos.Networking.Responses;
using PoGo.NecroBot.Logic.State;
using RocketBot2.Forms;
using System.Linq;
using POGOProtos.Settings.Master;
using POGOProtos.Inventory.Item;

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
        private static bool _initialized;
        private static Session Session;
        public static Dictionary<PokemonId, int> CandyToEvolveDict = new Dictionary<PokemonId, int>();

        public PokemonObject(PokemonData pokemonData)
        {
            PokemonData = pokemonData;
        }

        public PokemonData PokemonData { get; }

        public ulong Id
        {
            get { return PokemonData.Id; }
        }

        public PokemonId PokemonId
        {
            get { return PokemonData.PokemonId; }
        }

        public int Cp
        {
            get { return PokemonData.Cp; }
        }

        public int IndividualAttack
        {
            get { return PokemonData.IndividualAttack; }
        }

        public int IndividualDefense
        {
            get { return PokemonData.IndividualDefense; }
        }

        public int IndividualStamina
        {
            get { return PokemonData.IndividualStamina; }
        }

        public double GetIV
        {
            get { return PokemonInfo.CalculatePokemonPerfection(PokemonData)/100; }
        }

        public double GetLv
        {
            get { return PokemonInfo.GetLevel(PokemonData); }
        }

        public string Nickname
        {
            get { return PokemonData.Nickname; }
        }

        public string Move1
        {
            get { return Session.Translation.GetPokemonMovesetTranslation(PokemonData.Move1); }
        }

        public string Move2
        {
            get { return Session.Translation.GetPokemonMovesetTranslation(PokemonData.Move2); }
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
                if (CandyToEvolveDict.ContainsKey(PokemonData.PokemonId))
                {
                    return CandyToEvolveDict[PokemonData.PokemonId];
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
                return PokemonData.Favorite == 1;
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
               
        public static void Initilize(Session session, List<PokemonSettings> templates)
        {
            if (!_initialized)
            {
                _initialized = true;
                Session = session;

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