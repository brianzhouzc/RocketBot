using PoGo.NecroBot.Logic.Model;
using PoGo.NecroBot.Logic.PoGoUtils;
using PoGo.NecroBot.Logic.State;
using POGOProtos.Data;
using POGOProtos.Enums;
using POGOProtos.Inventory.Item;
using POGOProtos.Settings.Master;
using PokemonGo.RocketAPI.Util;
using System;
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
        public List<EvolutionToPokemon> EvolutionBranchs { get; set; }
        public PokemonEvoleTo(ISession session, PokemonData pokemon)
        {
            Session = session;
            this.PokemonData = pokemon;
            var pkmSettings = session.Inventory.GetPokemonSettings().Result;
            this.setting = pkmSettings.FirstOrDefault(x => x.PokemonId == pokemon.PokemonId);

            this.EvolutionBranchs = new List<EvolutionToPokemon>();

            //TODO - implement the candy count for enable evolution
            foreach (var item in this.setting.EvolutionBranch)
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
        private PokemonSettings settings;
        public PokemonData PokemonData { get; set; }
        public static bool _initialized { get; set; }
        public static Dictionary<PokemonId, int> CandyToEvolveDict = new Dictionary<PokemonId, int>();

        public PokemonObject(ISession session, PokemonData pokemonData)
        {
            Session = session;
            PokemonData = pokemonData;
            var pkmSettings = session.Inventory.GetPokemonSettings().Result;
            this.settings = pkmSettings.FirstOrDefault(x => x.PokemonId == pokemonData.PokemonId);
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
            get { return PokemonInfo.CalculatePokemonPerfection(this.PokemonData) / 100; }
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
                    return Candy / CandyToEvolve;
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
                return ResourceHelper.GetPokemonImage(this.PokemonData);
            }
        }

        public bool Shiny => this.PokemonData.PokemonDisplay.Shiny ? true : false;

        public string Sex => this.PokemonData.PokemonDisplay.Gender.ToString();

        public string Types
        {
            get
            {
                return this.settings.Type.ToString() + ((this.settings.Type2 != PokemonType.None) ? "," + this.settings.Type2.ToString() : "");
            }
        }

        public PokemonFamilyId FamilyId
        {
            get
            {
                return PokemonSettings.FamilyId;
            }
        }

        public PokemonSettings PokemonSettings
        {
            get
            {
                return Session.Inventory.GetPokemonSettings().Result.FirstOrDefault(x => x.PokemonId == PokemonId);
            }
        }

        public DateTime CaughtTime => TimeUtil.GetDateTimeFromMilliseconds((long)this.PokemonData.CreationTimeMs).ToLocalTime();

#pragma warning disable CS0649 // Le champ 'PokemonObject.geoLocation' n'est jamais assigné et aura toujours sa valeur par défaut null
        private GeoLocation geoLocation;
#pragma warning restore CS0649 // Le champ 'PokemonObject.geoLocation' n'est jamais assigné et aura toujours sa valeur par défaut null
        public GeoLocation GeoLocation
        {
            get
            {
                return geoLocation;
            }
        }

        public string CaughtLocation
        {
            get
            {
                if (geoLocation == null)
                {
                    // Just return latitude, longitude string
                    return new GeoLocation(this.PokemonData.CapturedCellId).ToString();
                }

                return geoLocation.ToString();
            }
        }

        public int HP
        {
            get
            {
                return this.PokemonData.Stamina;
            }
        }

        public int MaxHP
        {
            get
            {
                return this.PokemonData.StaminaMax;
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