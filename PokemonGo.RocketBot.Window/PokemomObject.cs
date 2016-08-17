using System;
using System.Collections.Generic;
using PokemonGo.RocketBot.Logic.PoGoUtils;
using POGOProtos.Data;
using POGOProtos.Enums;
using POGOProtos.Networking.Responses;

namespace PokemonGo.RocketBot.Window
{
    public class PokemonObject
    {
        private static bool _initialized;
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
            get { return Math.Round(PokemonInfo.CalculatePokemonPerfection(PokemonData) / 100, 2); }
        }

        public double GetLv
        {
            get { return PokemonInfo.GetLevel(PokemonData); }
        }

        public string Nickname
        {
            get { return PokemonData.Nickname; }
        }

        public int Candy { get; set; } = 0;

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

        public bool CanEvolve
        {
            get { return EvolveTimes > 0; }
        }

        public String Move1
        {
            get { return PokemonData.Move1.ToString();  }
        }

        public String Move2
        {
            get { return PokemonData.Move2.ToString();  }
        }

        public static void Initilize(DownloadItemTemplatesResponse itemtemplates)
        {
            if (!_initialized)
            {
                foreach (var t in itemtemplates.ItemTemplates)
                {
                    if (t != null)
                    {
                        if (t.PokemonSettings != null)
                        {
                            CandyToEvolveDict.Add(t.PokemonSettings.PokemonId, t.PokemonSettings.CandyToEvolve);
                        }
                    }
                }
                _initialized = true;
            }
        }
    }
}