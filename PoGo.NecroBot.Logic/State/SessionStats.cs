using LiteDB;
using PoGo.NecroBot.Logic.Event;
using System;
using System.Collections.Generic;
using System.IO;

namespace PoGo.NecroBot.Logic.State
{
    public class SessionStats
    {
        const string DB_NAME = @"SessionStats.db";
        const string POKESTOP_STATS_COLLECTION = "PokeStopTimestamps";
        const string POKEMON_STATS_COLLECTION  = "PokemonTimestamps";

        public int SnipeCount { get; set; }
        public DateTime LastSnipeTime { get; set; }
        public DateTime StartTime { get; set; }
        
        private LiteDatabase db;
        private LiteCollection<PokeStopTimestamp> pokestopTimestampCollection;
        private LiteCollection<PokemonTimestamp> pokemonTimestampCollection;

        class PokeStopTimestamp
        {
            public Int64 Timestamp { get; set; }
        }

        class PokemonTimestamp
        {
            public Int64 Timestamp { get; set; }
        }

        public SessionStats(ISession session)
        {                                                                                                                           
            StartTime = DateTime.Now;

            InitializeDatabase(session);
            LoadLegacyData(session);
        }

        public void InitializeDatabase(ISession session)
        {
            string username = session.Settings.AuthType == PokemonGo.RocketAPI.Enums.AuthType.Ptc ? session.Settings.PtcUsername : session.Settings.GoogleUsername;
            if (string.IsNullOrEmpty(username))
            {
                //firsttime setup , don't need to initial database
                return;
            }
            var path = Path.Combine(session.LogicSettings.ProfileConfigPath, username);
            
            if(!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            path = Path.Combine(path, DB_NAME);

            db = new LiteDatabase(path);
            pokestopTimestampCollection = db.GetCollection<PokeStopTimestamp>(POKESTOP_STATS_COLLECTION);
            pokemonTimestampCollection = db.GetCollection<PokemonTimestamp>(POKEMON_STATS_COLLECTION);

            // Add index
            pokestopTimestampCollection.EnsureIndex(s => s.Timestamp);
            pokemonTimestampCollection.EnsureIndex(s => s.Timestamp);
        }

        public void AddPokestopTimestamp(Int64 ts)
        {
            if (!pokestopTimestampCollection.Exists(s => s.Timestamp == ts))
            {
                var stat = new PokeStopTimestamp { Timestamp = ts };
                pokestopTimestampCollection.Insert(stat);
            }
        }

        public void AddPokemonTimestamp(Int64 ts)
        {
            if (!pokemonTimestampCollection.Exists(s => s.Timestamp == ts))
            {
                var stat = new PokemonTimestamp { Timestamp = ts };
                pokemonTimestampCollection.Insert(stat);
            }
        }

        public void CleanOutExpiredStats()
        {
            var TSminus24h = DateTime.Now.AddHours(-24).Ticks;
            pokestopTimestampCollection.Delete(s => s.Timestamp < TSminus24h);
            pokemonTimestampCollection.Delete(s => s.Timestamp < TSminus24h);
        }

        public int GetNumPokestopsInLast24Hours()
        {
            var TSminus24h = DateTime.Now.AddHours(-24).Ticks;
            return pokestopTimestampCollection.Count(s => s.Timestamp >= TSminus24h);
        }

        public int GetNumPokemonsInLast24Hours()
        {
            var TSminus24h = DateTime.Now.AddHours(-24).Ticks;
            return pokemonTimestampCollection.Count(s => s.Timestamp >= TSminus24h);
        }

        public void LoadLegacyData(ISession session)
        {
            List<Int64> list = new List<Int64>();
            // for pokestops
            try
            {
                var path = Path.Combine(session.LogicSettings.ProfileConfigPath, "PokestopTS.txt");
                if (File.Exists(path))
                {
                    var content = File.ReadLines(path);
                    foreach (var c in content)
                    {
                        if (c.Length > 0)
                        {
                            list.Add(Convert.ToInt64(c));
                        }
                    }
                    File.Delete(path);
                }
            }
            catch (Exception)
            {
                session.EventDispatcher.Send(new ErrorEvent
                {
                    Message = "Garbage information in PokestopTS.txt"
                });
            }

            foreach (var l in list)
            {
                AddPokestopTimestamp(l);
            }

            // for pokemons
            list = new List<Int64>();
            try
            {
                var path = Path.Combine(session.LogicSettings.ProfileConfigPath, "PokemonTS.txt");
                if (File.Exists(path))
                {
                    var content = File.ReadLines(path);
                    foreach (var c in content)
                    {
                        if (c.Length > 0)
                        {
                            list.Add(Convert.ToInt64(c));
                        }
                    }
                    File.Delete(path);
                }
            }
            catch (Exception)
            {
                session.EventDispatcher.Send(new ErrorEvent
                {
                    Message = "Garbage information in PokemonTS.txt"
                });
            }
            foreach (var l in list)
            {
                AddPokemonTimestamp(l);
            }
        }
    }
}
