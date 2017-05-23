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
            PokemonData = pokemon;
            var pkmSettings = session.Inventory.GetPokemonSettings().Result;
            setting = pkmSettings.FirstOrDefault(x => x.PokemonId == pokemon.PokemonId);

            EvolutionBranchs = new List<EvolutionToPokemon>();

            //TODO - implement the candy count for enable evolution
            foreach (var item in setting.EvolutionBranch)
            {
                EvolutionBranchs.Add(new EvolutionToPokemon()
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
            settings = pkmSettings.FirstOrDefault(x => x.PokemonId == pokemonData.PokemonId);
        }
 
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
            get { return PokemonInfo.CalculatePokemonPerfection(PokemonData) / 100; }
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

        public float HeightM
        {
            get { return (float)Math.Round(PokemonData.HeightM,2); }
        }

        public string Move2
        {
            get { return Session.Translation.GetPokemonMovesetTranslation(PokemonData.Move2); }
        }

        public float WeightKg
        {
            get { return (float)Math.Round(PokemonData.WeightKg,2); }
        }

        public int Candy
        {
            get
            {
                return Session.Inventory.GetCandyCount(PokemonData.PokemonId).Result;
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
                    return Candy / CandyToEvolve;
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
                return Session.Inventory.CanUpgradePokemon(PokemonData).Result;
            }
        }

        public bool AllowEvolve
        {
            get
            {
                return Session.Inventory.CanEvolvePokemon(PokemonData).Result;
            }
        }

        public bool AllowTransfer
        {
            get
            {
                return Session.Inventory.CanTransferPokemon(PokemonData);
            }
        }

        public Image Icon
        {
            get
            {
                return ResourceHelper.GetPokemonImage(PokemonData);
            }
        }

        public string Form => PokemonData.PokemonDisplay.Form.ToString().Replace("Unown", "").Replace("Unset", "Normal");
        public string Costume => PokemonData.PokemonDisplay.Costume.ToString().Replace("Unset", "Regular");
        public string Sex => PokemonData.PokemonDisplay.Gender.ToString().Replace("Less", "Genderless");
        public bool Shiny => PokemonData.PokemonDisplay.Shiny ? true : false;

        public string Types
        {
            get
            {
                return settings.Type.ToString() + ((settings.Type2 != PokemonType.None) ? "," + settings.Type2.ToString() : "");
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

        public DateTime CaughtTime => TimeUtil.GetDateTimeFromMilliseconds((long)PokemonData.CreationTimeMs).ToLocalTime();

        private GeoLocation geoLocation;
        public GeoLocation GeoLocation
        {
            get
            {
                return GeoLocation.FindOrUpdateInDatabase(PokemonData.CapturedCellId).Result;
                //return geoLocation;
            }
            set
            {
                geoLocation = value;
            }
        }

        public string CaughtLocation
        {
            get
            {
                if (geoLocation == null)
                {
                    // Just return latitude, longitude string
                    return new GeoLocation(PokemonData.CapturedCellId).ToString();
                }

                return geoLocation.ToString();
            }
        }

        public int HP
        {
            get
            {
                return PokemonData.Stamina;
            }
        }

        public int MaxHP
        {
            get
            {
                return PokemonData.StaminaMax;
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