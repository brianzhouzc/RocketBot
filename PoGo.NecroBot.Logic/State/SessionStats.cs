using System;
using System.Collections.Generic;
using System.IO;
using LiteDB;
using PoGo.NecroBot.Logic.Common;
using PoGo.NecroBot.Logic.Event;
using PokemonGo.RocketAPI.Enums;

namespace PoGo.NecroBot.Logic.State
{
    public class SessionStats
    {
        const string DB_NAME = @"SessionStats.db";
        const string POKESTOP_STATS_COLLECTION = "PokeStopTimestamps";
        const string POKEMON_STATS_COLLECTION = "PokemonTimestamps";

        public int SnipeCount { get; set; }
        public DateTime LastSnipeTime { get; set; }
        public DateTime StartTime { get; set; }
        public bool IsSnipping { get; internal set; }

        private ISession ownerSession;
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

        DateTime lastPrintPokestopMessage = DateTime.Now;

        public bool SearchThresholdExceeds(ISession session, bool printMessage)
        {
            if (!session.LogicSettings.UsePokeStopLimit) return false;
            //if (_pokestopLimitReached || _pokestopTimerReached) return true;

            this.CleanOutExpiredStats();

            // Check if user defined max Pokestops reached
            var timeDiff = (DateTime.Now - session.Stats.StartTime);

            if (this.GetNumPokestopsInLast24Hours() >= session.LogicSettings.PokeStopLimit)
            {
                if (printMessage && lastPrintPokestopMessage.AddSeconds(60) < DateTime.Now)
                {
                    lastPrintPokestopMessage = DateTime.Now;
                    session.EventDispatcher.Send(new ErrorEvent
                    {
                        Message = session.Translation.GetTranslation(TranslationString.PokestopLimitReached)
                    });
                }
                //_pokestopLimitReached = true;
                return true;
            }

            // Check if user defined time since start reached
            else if (timeDiff.TotalSeconds >= session.LogicSettings.PokeStopLimitMinutes * 60)
            {
                session.EventDispatcher.Send(new ErrorEvent
                {
                    Message = session.Translation.GetTranslation(TranslationString.PokestopTimerReached)
                });

                //_pokestopTimerReached = true;
                return true;
            }

            return false; // Continue running
        }


        DateTime lastPrintCatchMessage = DateTime.Now;

        public bool CatchThresholdExceeds(ISession session, bool printMessage = true)
        {
            if (!session.LogicSettings.UseCatchLimit) return false;

            this.CleanOutExpiredStats();

            var timeDiff = (DateTime.Now - session.Stats.StartTime);

            // Check if user defined max AMOUNT of Catches reached
            if (GetNumPokemonsInLast24Hours() >= session.LogicSettings.CatchPokemonLimit)
            {
                if (printMessage && lastPrintCatchMessage.AddSeconds(60) < DateTime.Now)
                {
                    lastPrintCatchMessage = DateTime.Now;
                    session.EventDispatcher.Send(new ErrorEvent
                    {
                        Message = session.Translation.GetTranslation(TranslationString.CatchLimitReached)
                    });
                }
                // _catchPokemonLimitReached = true;
                return true;
            }

            // Check if user defined TIME since start reached
            else if (timeDiff.TotalSeconds >= session.LogicSettings.CatchPokemonLimitMinutes * 60)
            {
                session.EventDispatcher.Send(new ErrorEvent
                {
                    Message = session.Translation.GetTranslation(TranslationString.CatchTimerReached)
                });

                //_catchPokemonTimerReached = true;
                return true;
            }

            return false;
        }

        public bool IsPokestopLimit(ISession session)
        {
            if (!session.LogicSettings.UsePokeStopLimit) return false;

            this.CleanOutExpiredStats();

            if (GetNumPokestopsInLast24Hours() >= session.LogicSettings.PokeStopLimitMinutes)
                return true;
            //TODO - Other logic should come here, but I don't think we need
            return false;
        }

        public SessionStats(ISession session)
        {
            StartTime = DateTime.Now;
            ownerSession = session;
            InitializeDatabase(session);
            LoadLegacyData(session);
        }

        public void InitializeDatabase(ISession session)
        {
            string username = session.Settings.AuthType == AuthType.Ptc
                ? session.Settings.PtcUsername
                : session.Settings.GoogleUsername;
            if (string.IsNullOrEmpty(username))
            {
                //firsttime setup , don't need to initial database
                return;
            }
            var path = Path.Combine(session.LogicSettings.ProfileConfigPath, username);

            if (!Directory.Exists(path))
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
                var stat = new PokeStopTimestamp {Timestamp = ts};
                pokestopTimestampCollection.Insert(stat);
            }
        }

        public void AddPokemonTimestamp(Int64 ts)
        {
            if (!pokemonTimestampCollection.Exists(s => s.Timestamp == ts))
            {
                var stat = new PokemonTimestamp {Timestamp = ts};
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