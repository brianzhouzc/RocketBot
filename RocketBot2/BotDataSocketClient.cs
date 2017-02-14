using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PoGo.NecroBot.Logic.Event;
using PoGo.NecroBot.Logic.Event.Snipe;
using PoGo.NecroBot.Logic.State;
using PoGo.NecroBot.Logic.Tasks;
using POGOProtos.Enums;
using WebSocketSharp;
using Logger = PoGo.NecroBot.Logic.Logging.Logger;
using System.Runtime.Caching;
using System.Reflection;

namespace RocketBot2
{
    public class BotDataSocketClient
    {
        public class SocketMessage
        {
            public string Header { get; set; }
            public string Body { get; set; }
            public long TimeTimestamp { get; set; }
            public string Hash { get; set; }
        }
        public class SocketClientUpdate
        {
            public List<EncounteredEvent> Pokemons { get; set; }

            public List<SnipeFailedEvent> SnipeFailedPokemons { get; set; }
            public List<string> ExpiredPokemons { get; set; }
            public string ClientVersion { get; set; }
            public SocketClientUpdate()
            {
                this.Pokemons = new List<EncounteredEvent>();
                this.SnipeFailedPokemons = new List<SnipeFailedEvent>();
                this.ExpiredPokemons = new List<string>();
                this.ClientVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();

            }

            public bool HasData()
            {
                return Pokemons.Count > 0 || SnipeFailedPokemons.Count > 0 || ExpiredPokemons.Count > 0;
            }
        }
        private static SocketClientUpdate clientData = new SocketClientUpdate();

        private const int POLLING_INTERVAL = 5000;

        public static void HandleEvent(IEvent evt, ISession session)
        {
        }
        public static void HandleEvent(SnipeFailedEvent e, ISession sesion)
        {
            lock (clientData)
            {
                clientData.SnipeFailedPokemons.Add(e);
            }
        }

        public static void Listen(IEvent evt, ISession session)
        {
            dynamic eve = evt;

            try
            {
                HandleEvent(eve, session);
            }
            catch
            {
            }
        }

        private static void HandleEvent(EncounteredEvent eve, ISession session)
        {
            lock (clientData)
            {
                if (eve.IsRecievedFromSocket || cache.Get(eve.EncounterId) != null) return;
                clientData.Pokemons.Add(eve);
            }
        }
        private static SnipePokemonUpdateEvent lastEncouteredEvent;
        private static void HandleEvent(SnipePokemonUpdateEvent eve, ISession session)
        {
            lock(clientData)
            {
                clientData.ExpiredPokemons.Add(eve.EncounterId);
            }
        }
        private static string Serialize(dynamic evt)
        {
            var jsonSerializerSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };

            // Add custom seriaizer to convert uong to string (ulong shoud not appear to json according to json specs)
            jsonSerializerSettings.Converters.Add(new IdToStringConverter());

            string json = JsonConvert.SerializeObject(evt, Formatting.None, jsonSerializerSettings);
            //json = Regex.Replace(json, @"\\\\|\\(""|')|(""|')", match => {
            //    if (match.Groups[1].Value == "\"") return "\""; // Unescape \"
            //    if (match.Groups[2].Value == "\"") return "'";  // Replace " with '
            //    if (match.Groups[2].Value == "'") return "\\'"; // Escape '
            //    return match.Value;                             // Leave \\ and \' unchanged
            //});
            return json;
        }

        static List<EncounteredEvent> processing = new List<EncounteredEvent>();

        public static String SHA256Hash(String value)
        {
            StringBuilder Sb = new StringBuilder();
            using (SHA256 hash = SHA256Managed.Create())
            {
                Encoding enc = Encoding.UTF8;
                Byte[] result = hash.ComputeHash(enc.GetBytes(value));

                foreach (Byte b in result)
                    Sb.Append(b.ToString("x2"));
            }

            return Sb.ToString();
        }

        public static async Task Start(Session session, CancellationToken cancellationToken)
        {

            //Disable autosniper service until finger out how to make it work with API change

            return;
            await Task.Delay(30000, cancellationToken); //delay running 30s

            ServicePointManager.Expect100Continue = false;

            cancellationToken.ThrowIfCancellationRequested();

            while (true)
            {
                var socketURL = servers.Dequeue();
                Logger.Write($"Connecting to {socketURL} ....");
                await ConnectToServer(session, socketURL);
                servers.Enqueue(socketURL);
                File.WriteAllLines("debug.log", new string[] { $"server queue {servers.Count}" });  
            }

        }
        public static async Task ConnectToServer(ISession session, string socketURL)
        {
            File.WriteAllLines("debug.log", new string[] { $"new connect to {socketURL}" });

            if (!string.IsNullOrEmpty(session.LogicSettings.DataSharingConfig.SnipeDataAccessKey))
            {
                socketURL += "&access_key=" + session.LogicSettings.DataSharingConfig.SnipeDataAccessKey;
            }

            int retries = 0;
            using (var ws = new WebSocket(socketURL))
            {
                ws.Log.Level = LogLevel.Fatal; ;
                ws.Log.Output = (logData, message) =>
                {
                    //silenly, no log exception message to screen that scare people :)
                };

                ws.OnMessage += (sender, e) => { onSocketMessageRecieved(session, sender, e); };

                ws.Connect();
                while (true)
                {
                    try
                    {
                        if (retries == 3)
                        {
                            File.WriteAllLines("debug.log", new string[] { $"Couldn't establish the connection to necro socket server : {socketURL}" });
                            //failed to make connection to server  times contiuing, temporary stop for 10 mins.
                            session.EventDispatcher.Send(new WarnEvent()
                            {
                                Message = $"Couldn't establish the connection to necro socket server : {socketURL}"
                            });
                            if (session.LogicSettings.DataSharingConfig.EnableFailoverDataServers && servers.Count > 1)
                            {
                                break;
                            }
                            await Task.Delay(1 * 60 * 1000);
                            retries = 0;
                        }

                        if (ws.ReadyState != WebSocketState.Open)
                        {
                            retries++;
                            ws.Connect();
                        }

                        while (ws.ReadyState == WebSocketState.Open)
                        {
                            //Logger.Write("Connected to necrobot data service.");
                            retries = 0;

                            if (ws.IsAlive && clientData.HasData())
                            {
                                var data = JsonConvert.SerializeObject(clientData);// Serialize(processing);
                                clientData = new SocketClientUpdate();

                                var message = Encrypt(data);
                                var actualMessage = JsonConvert.SerializeObject(message);
                                ws.Send($"42[\"client-update\",{actualMessage}]");
                            }

                            await Task.Delay(POLLING_INTERVAL);
                            ws.Ping();
                        }
                    }
                    catch (IOException)
                    {
                        session.EventDispatcher.Send(new WarnEvent
                        {
                            Message = "Disconnect to necro socket. New connection will be established when service available..."
                        });
                        File.WriteAllLines("debug.log", new string[] { $"Disconnect to necro socket. New connection will be established when service available." });

                    }
                    catch (Exception)
                    {
                    }
                    finally
                    {
                        //everytime disconnected with server bot wil reconnect after 15 sec
                        await Task.Delay(POLLING_INTERVAL);
                    }
                }
            }
        }

        private static void onSocketMessageRecieved(ISession session, object sender, MessageEventArgs e)
        {
            try
            {
                OnPokemonRemoved(session, e.Data);
                OnPokemonUpdateData(session, e.Data);
                OnPokemonData(session, e.Data);
                OnSnipePokemon(session, e.Data);
                OnServerMessage(session, e.Data);
                //ONFPMBridgeData(session, e.Data); //Nolonger use
            }

#pragma warning disable 0168 // Comment Suppress compiler warning - ex is used in DEBUG section
            catch (Exception ex)
#pragma warning restore 0168
            {
                // Comment Suppress compiler warning - ex is used in DEBUG section
#if DEBUG
                Logger.Write("ERROR TO ADD SNIPE< DEBUG ONLY " + ex.Message + "\r\n " + ex.StackTrace,
                    PoGo.NecroBot.Logic.Logging.LogLevel.Info, ConsoleColor.Yellow);
#endif
            }
        }

        private static void OnServerMessage(ISession session, string message)
        {
            var match = Regex.Match(message, "42\\[\"server-message\",(.*)]");
            if (match != null && !string.IsNullOrEmpty(match.Groups[1].Value))
            {
                session.EventDispatcher.Send(new NoticeEvent()
                {
                    Message = "(SERVER) " + match.Groups[1].Value
                });
            }
        }

        private static void ONFPMBridgeData(ISession session, string message)
        {
            var match = Regex.Match(message, "42\\[\"fpm\",(.*)]");
            if (match != null && !string.IsNullOrEmpty(match.Groups[1].Value))
            {
                //var data = JsonConvert.DeserializeObject<List<Logic.Tasks.HumanWalkSnipeTask.FastPokemapItem>>(match.Groups[1].Value);

                // jjskuld - Ignore CS4014 warning for now.
#pragma warning disable 4014
                HumanWalkSnipeTask.AddFastPokemapItem(match.Groups[1].Value);
#pragma warning restore 4014
            }
        }

        public static bool CheckIfPokemonBeenCaught(double lat, double lng, PokemonId id, ulong encounterId,
            ISession session)
        {
            string uniqueCacheKey =
                $"{session.Settings.PtcUsername}{session.Settings.GoogleUsername}{Math.Round(lat, 6)}{id}{Math.Round(lng, 6)}";
            if (session.Cache.Get(uniqueCacheKey) != null) return true;
            if (encounterId > 0 && session.Cache[encounterId.ToString()] != null) return true;

            return false;
        }
        private static void OnPokemonUpdateData(ISession session, string message)
        {
            var match = Regex.Match(message, "42\\[\"pokemon-update\",(.*)]");
            if (match != null && !string.IsNullOrEmpty(match.Groups[1].Value))
            {
                var data = JsonConvert.DeserializeObject<EncounteredEvent>(match.Groups[1].Value);
                MSniperServiceTask.RemoveExpiredSnipeData(session, data.EncounterId);
            }
        }

        private static void OnPokemonRemoved(ISession session, string message)
        {
            var match = Regex.Match(message, "42\\[\"pokemon-remove\",(.*)]");
            if (match != null && !string.IsNullOrEmpty(match.Groups[1].Value))
            {
                var data = JsonConvert.DeserializeObject<EncounteredEvent>(match.Groups[1].Value);
                MSniperServiceTask.RemoveExpiredSnipeData(session, data.EncounterId);
            }
        }

        private static MemoryCache cache = new MemoryCache("dump");

        static int count = 0;
        private static void OnPokemonData(ISession session, string message)
        {
            var match = Regex.Match(message, "42\\[\"pokemon\",(.*)]");
            if (match != null && !string.IsNullOrEmpty(match.Groups[1].Value))
            {
                var data = JsonConvert.DeserializeObject<EncounteredEvent>(match.Groups[1].Value);
                data.IsRecievedFromSocket = true;
                ulong encounterid = 0;
                if (Math.Abs(data.Latitude) > 90 || Math.Abs(data.Longitude) > 180)
                {
                    return;
                };

                ulong.TryParse(data.EncounterId, out encounterid);
                if (encounterid > 0 && cache.Get(encounterid.ToString()) == null)
                {
                    cache.Add(encounterid.ToString(), DateTime.Now, DateTime.Now.AddMinutes(15));
                }

                session.EventDispatcher.Send(data);
                if (session.LogicSettings.DataSharingConfig.AutoSnipe)
                {
                    var move1 = PokemonMove.MoveUnset;
                    var move2 = PokemonMove.MoveUnset;
                    Enum.TryParse<PokemonMove>(data.Move1, true, out move1);
                    Enum.TryParse<PokemonMove>(data.Move2, true, out move2);

                    bool caught = CheckIfPokemonBeenCaught(data.Latitude, data.Longitude,
                        data.PokemonId, encounterid, session);
                    if (!caught)
                    {
                        var added = MSniperServiceTask.AddSnipeItem(session, new MSniperServiceTask.MSniperInfo2()
                        {
                            UniqueIdentifier = data.EncounterId,
                            Latitude = data.Latitude,
                            Longitude = data.Longitude,
                            EncounterId = encounterid,
                            SpawnPointId = data.SpawnPointId,
                            PokemonId = (short)data.PokemonId,
                            Level = data.Level,
                            Iv = data.IV,
                            Move1 = move1,
                            Move2 = move2,
                            ExpiredTime = data.ExpireTimestamp
                        });
                        if (added)
                        {
                            session.EventDispatcher.Send(new AutoSnipePokemonAddedEvent(data));
                        }
                    }
                }
            }
        }

        private static void OnSnipePokemon(ISession session, string message)
        {
            var match = Regex.Match(message, "42\\[\"snipe-pokemon\",(.*)]");
            if (match != null && !string.IsNullOrEmpty(match.Value) && !string.IsNullOrEmpty(match.Groups[1].Value))
            {
                var data = JsonConvert.DeserializeObject<EncounteredEvent>(match.Groups[1].Value);

                //not your snipe item, return need more encrypt here and configuration to allow catch others item
                if (string.IsNullOrEmpty(session.LogicSettings.DataSharingConfig.DataServiceIdentification) ||
                    string.IsNullOrEmpty(data.RecieverId) ||
                    data.RecieverId.ToLower() != session.LogicSettings.DataSharingConfig.DataServiceIdentification.ToLower()) return;

                var move1 = PokemonMove.Absorb;
                var move2 = PokemonMove.Absorb;
                Enum.TryParse<PokemonMove>(data.Move1, true, out move1);
                Enum.TryParse<PokemonMove>(data.Move1, true, out move2);
                ulong encounterid = 0;
                ulong.TryParse(data.EncounterId, out encounterid);

                bool caught = CheckIfPokemonBeenCaught(data.Latitude, data.Longitude, data.PokemonId, encounterid,
                    session);
                if (caught)
                {
                    Logger.Write("[SNIPE IGNORED] - Your snipe pokemon has already been cautgh by bot",
                        PoGo.NecroBot.Logic.Logging.LogLevel.Sniper);
                    return;
                }

                MSniperServiceTask.AddSnipeItem(session, new MSniperServiceTask.MSniperInfo2()
                {
                    UniqueIdentifier = data.EncounterId,
                    Latitude = data.Latitude,
                    Longitude = data.Longitude,
                    EncounterId = encounterid,
                    SpawnPointId = data.SpawnPointId,
                    Level = data.Level,
                    PokemonId = (short)data.PokemonId,
                    Iv = data.IV,
                    Move1 = move1,
                    ExpiredTime = data.ExpireTimestamp,
                    Move2 = move2
                }, true);
            }
        }
        private static Queue<string> servers = new Queue<string>();
        internal static Task StartAsync(Session session,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var config = session.LogicSettings.DataSharingConfig;

            if (config.EnableSyncData)
            {
                servers.Enqueue(config.DataRecieverURL);

                if (config.EnableFailoverDataServers)
                {
                    foreach (var item in config.FailoverDataServers.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                    {
                        servers.Enqueue(item);
                    }
                }
            }

            return Task.Run(() => Start(session, cancellationToken), cancellationToken);
        }

        public static SocketMessage Encrypt(string message)
        {
            var encryptedtulp = Encrypt(message, RocketBot2.Properties.Resources.EncryptKey, false);

            var socketMessage = new SocketMessage()
            {
                TimeTimestamp = (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds,
                Header = encryptedtulp.Item1,
                Body = encryptedtulp.Item2
            };
            socketMessage.Hash = CalculateMD5Hash($"{socketMessage.TimeTimestamp}{socketMessage.Body}{socketMessage.Header}");

            return socketMessage;
        }
        public static string CalculateMD5Hash(string input)
        {
            // step 1, calculate MD5 hash from input

            MD5 md5 = System.Security.Cryptography.MD5.Create();

            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);

            byte[] hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < hash.Length; i++)

            {

                sb.Append(hash[i].ToString("X2"));

            }

            return sb.ToString();

        }


        public static Tuple<string, string> Encrypt(string toEncrypt, string key, bool useHashing)
        {
            byte[] keyArray;
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

            if (useHashing)
            {
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
            }
            else
                keyArray = UTF8Encoding.UTF8.GetBytes(key);

            var tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            // tdes.Mode = CipherMode.CBC;  // which is default     
            // tdes.Padding = PaddingMode.PKCS7;  // which is default

            //Console.WriteLine("iv: {0}", Convert.ToBase64String(tdes.IV));

            ICryptoTransform cTransform = tdes.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0,
                toEncryptArray.Length);
            string iv = Convert.ToBase64String(tdes.IV);
            string message = Convert.ToBase64String(resultArray, 0, resultArray.Length);
            return new Tuple<string, string>(iv, message);
        }

    }
}